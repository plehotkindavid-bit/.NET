using System.Text.Json;
using API.Cores;
using API.Data;
using API.Enetites;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace API.Tests;

public class ProductFlowTests
{
    [Fact]
    public async Task GetById_ReturnsSafeDtoAndDatabaseMetadata()
    {
        await using var database = await TestDatabase.CreateAsync(
            Product(1, "Exact Product", quantity: 25));
        var service = CreateService(database.Context);

        var result = await service.GetByIdAsync("operation-id", 1);
        var json = JsonSerializer.SerializeToElement(result);
        var product = json.GetProperty("data").GetProperty("product");

        Assert.Equal(CoreStatuses.Success, result.Status);
        Assert.Equal("operation-id", result.OperationId);
        Assert.True(result.DatabaseChecked);
        Assert.NotEmpty(result.DatabaseOperation);
        Assert.Equal(1, Assert.IsType<int>(Assert.Single(result.RecordIds)));
        Assert.Equal(1, product.GetProperty("id").GetInt32());
        Assert.False(product.TryGetProperty("quantitiesInStock", out _));
        Assert.Equal(
            ["id", "name", "description", "price", "pictureUrl", "type", "brand"],
            product.EnumerateObject().Select(property => property.Name));
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundAfterCompletedQuery()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = CreateService(database.Context);

        var result = await service.GetByIdAsync("operation-id", 404);

        Assert.Equal(CoreStatuses.NotFound, result.Status);
        Assert.True(result.DatabaseChecked);
        Assert.NotEmpty(result.DatabaseOperation);
        Assert.Empty(result.RecordIds);
    }

    [Fact]
    public async Task ExactNameSearch_DoesNotUsePartialMatching()
    {
        await using var database = await TestDatabase.CreateAsync(
            Product(1, "Exact Product"));
        var service = CreateService(database.Context);

        var exactResult = await service.SearchAsync(
            "operation-exact",
            productId: null,
            exactName: "Exact Product");
        var partialResult = await service.SearchAsync(
            "operation-partial",
            productId: null,
            exactName: "Exact");

        Assert.Equal(CoreStatuses.Success, exactResult.Status);
        Assert.Equal(CoreStatuses.NotFound, partialResult.Status);
        Assert.True(partialResult.DatabaseChecked);
    }

    [Fact]
    public async Task Search_PrioritizesIdAndDoesNotFallbackToName()
    {
        await using var database = await TestDatabase.CreateAsync(
            Product(1, "First"),
            Product(2, "Second"));
        var service = CreateService(database.Context);

        var foundById = await service.SearchAsync(
            "operation-found-id",
            productId: 1,
            exactName: "Second");
        var missingById = await service.SearchAsync(
            "operation-missing-id",
            productId: 404,
            exactName: "Second");
        var foundJson = JsonSerializer.SerializeToElement(foundById);

        Assert.Equal(CoreStatuses.Success, foundById.Status);
        Assert.Equal(
            1,
            foundJson.GetProperty("data")
                .GetProperty("product")
                .GetProperty("id")
                .GetInt32());
        Assert.Equal(CoreStatuses.NotFound, missingById.Status);
    }

    [Fact]
    public async Task SearchWithoutInput_ReturnsNeedsDataWithoutDatabaseClaim()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = CreateService(database.Context);

        var result = await service.SearchAsync(
            "operation-needs-data",
            productId: null,
            exactName: null);

        Assert.Equal(CoreStatuses.NeedsData, result.Status);
        Assert.False(result.DatabaseChecked);
        Assert.Equal(string.Empty, result.DatabaseOperation);
        Assert.Equal(["productId or name"], result.MissingData);
    }

    [Fact]
    public async Task NonPositiveId_ReturnsRejectedWithoutDatabaseClaim()
    {
        await using var database = await TestDatabase.CreateAsync();
        var service = CreateService(database.Context);

        var result = await service.GetByIdAsync(
            "operation-rejected",
            productId: 0);

        Assert.Equal(CoreStatuses.Rejected, result.Status);
        Assert.False(result.DatabaseChecked);
        Assert.Equal(string.Empty, result.DatabaseOperation);
    }

    [Fact]
    public async Task DatabaseException_ReturnsSafeErrorAndHttp500()
    {
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var context = new TestStoreContext(options);
        var service = CreateService(context);
        var controller = CreateController(context, service);
        controller.HttpContext.Request.Headers["X-Operation-Id"] =
            "operation-error";

        await context.DisposeAsync();

        var actionResult = await controller.GetProduct(1);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        var result = Assert.IsType<CoreResult>(objectResult.Value);
        var responseJson = JsonSerializer.Serialize(result);

        Assert.Equal(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode);
        Assert.Equal(CoreStatuses.Error, result.Status);
        Assert.Equal("operation-error", result.OperationId);
        Assert.False(result.DatabaseChecked);
        Assert.NotEmpty(result.DatabaseOperation);
        Assert.DoesNotContain(
            "disposed",
            responseJson,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "exception",
            responseJson,
            StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(
            "store.db",
            responseJson,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ReadOperations_DoNotCallSaveChangesAsync()
    {
        await using var database = await TestDatabase.CreateAsync(
            Product(1, "Exact Product"));
        var service = CreateService(database.Context);

        await service.GetByIdAsync("operation-id", 1);
        await service.SearchAsync(
            "operation-name",
            productId: null,
            exactName: "Exact Product");
        await service.GetAllAsync("operation-list");

        Assert.Equal(0, database.Context.SaveChangesCallCount);
    }

    [Fact]
    public async Task GetAll_ReturnsProductsWrapperWithoutStock()
    {
        await using var database = await TestDatabase.CreateAsync(
            Product(1, "First", quantity: 10),
            Product(2, "Second", quantity: 20));
        var service = CreateService(database.Context);

        var result = await service.GetAllAsync("operation-list");
        var json = JsonSerializer.SerializeToElement(result);
        var data = json.GetProperty("data");
        var products = data.GetProperty("products");

        Assert.Equal(CoreStatuses.Success, result.Status);
        Assert.True(result.DatabaseChecked);
        Assert.Equal(2, products.GetArrayLength());
        Assert.Single(data.EnumerateObject());
        Assert.All(
            products.EnumerateArray(),
            product => Assert.False(
                product.TryGetProperty("quantitiesInStock", out _)));
    }

    [Fact]
    public async Task ControllerUsesOneSuppliedOperationIdPerRequest()
    {
        await using var database = await TestDatabase.CreateAsync(
            Product(1, "Exact Product"));
        var service = CreateService(database.Context);
        var controller = CreateController(database.Context, service);
        controller.HttpContext.Request.Headers["X-Operation-Id"] =
            "request-operation-id";

        var actionResult = await controller.FindProduct(1, "Other Name");
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<CoreResult>(okResult.Value);

        Assert.Equal("request-operation-id", result.OperationId);
    }

    private static ProductService CreateService(StoreContext context)
    {
        return new ProductService(
            context,
            NullLogger<ProductService>.Instance);
    }

    private static ProductsController CreateController(
        StoreContext context,
        ProductService service)
    {
        return new ProductsController(context, service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static Product Product(
        int id,
        string name,
        int quantity = 0)
    {
        return new Product
        {
            Id = id,
            Name = name,
            Description = "Description",
            Price = 100,
            PictureUrl = "product.png",
            Type = "Type",
            Brand = "Brand",
            QuantitiesInStock = quantity
        };
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private TestDatabase(
            SqliteConnection connection,
            TestStoreContext context)
        {
            Connection = connection;
            Context = context;
        }

        private SqliteConnection Connection { get; }

        public TestStoreContext Context { get; }

        public static async Task<TestDatabase> CreateAsync(
            params Product[] products)
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseSqlite(connection)
                .Options;
            var context = new TestStoreContext(options);
            await context.Database.EnsureCreatedAsync();

            if (products.Length > 0)
            {
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            context.ResetSaveChangesCallCount();

            return new TestDatabase(connection, context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await Connection.DisposeAsync();
        }
    }

    private sealed class TestStoreContext(
        DbContextOptions<StoreContext> options)
        : StoreContext(options)
    {
        public int SaveChangesCallCount { get; private set; }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return base.SaveChangesAsync(cancellationToken);
        }

        public void ResetSaveChangesCallCount()
        {
            SaveChangesCallCount = 0;
        }
    }
}
