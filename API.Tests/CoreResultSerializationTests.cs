using System.Text.Json;
using API.Cores;
using Xunit;

namespace API.Tests;

public class CoreResultSerializationTests
{
    private static readonly string[] RequiredProperties =
    [
        "moduleId",
        "operationId",
        "operation",
        "status",
        "data",
        "errors",
        "missingData",
        "nextModuleId",
        "databaseChecked",
        "databaseOperation",
        "recordIds"
    ];

    public static TheoryData<CoreResult, string> ResultsByStatus => new()
    {
        {
            CoreResultFactory.Success(
                "CORE-PRODUCT-001",
                "operation-success",
                "read_product",
                new { product = new { id = 42 } },
                databaseChecked: true,
                databaseOperation: "Completed product lookup.",
                recordIds: [42]),
            CoreStatuses.Success
        },
        {
            CoreResultFactory.NotFound(
                "CORE-PRODUCT-001",
                "operation-not-found",
                "read_product",
                "Product was not found.",
                databaseChecked: true,
                databaseOperation: "Completed product lookup."),
            CoreStatuses.NotFound
        },
        {
            CoreResultFactory.NeedsData(
                "CORE-PRODUCT-001",
                "operation-needs-data",
                "read_product",
                ["productId"]),
            CoreStatuses.NeedsData
        },
        {
            CoreResultFactory.Rejected(
                "CORE-PRODUCT-001",
                "operation-rejected",
                "read_product",
                "Product ID is invalid."),
            CoreStatuses.Rejected
        },
        {
            CoreResultFactory.Error(
                "CORE-PRODUCT-001",
                "operation-error",
                "read_product",
                "Unable to read product data.",
                databaseChecked: false,
                databaseOperation: "Product lookup failed."),
            CoreStatuses.Error
        }
    };

    [Theory]
    [MemberData(nameof(ResultsByStatus))]
    public void Serialize_ContainsOnlySchemaProperties(
        CoreResult result,
        string expectedStatus)
    {
        var json = JsonSerializer.SerializeToElement(result);
        var actualProperties = json.EnumerateObject()
            .Select(property => property.Name)
            .Order()
            .ToArray();

        Assert.Equal(RequiredProperties.Order(), actualProperties);
        Assert.Equal(expectedStatus, json.GetProperty("status").GetString());
        Assert.False(json.TryGetProperty("isSuccess", out _));
        Assert.False(json.TryGetProperty("module", out _));
        Assert.False(json.TryGetProperty("nextModule", out _));
    }

    [Theory]
    [MemberData(nameof(ResultsByStatus))]
    public void Serialize_NeverWritesNullForDataOrCollections(
        CoreResult result,
        string _)
    {
        var json = JsonSerializer.SerializeToElement(result);

        Assert.Equal(JsonValueKind.Object, json.GetProperty("data").ValueKind);
        Assert.Equal(JsonValueKind.Array, json.GetProperty("errors").ValueKind);
        Assert.Equal(
            JsonValueKind.Array,
            json.GetProperty("missingData").ValueKind);
        Assert.Equal(
            JsonValueKind.Array,
            json.GetProperty("recordIds").ValueKind);
    }

    [Fact]
    public void Serialize_UsesSchemaNamesAndSupportsStringRecordIds()
    {
        var result = CoreResultFactory.Success(
            "CORE-PRODUCT-001",
            "operation-string-record-id",
            "read_product",
            recordIds: ["external-product-id"]);

        var json = JsonSerializer.SerializeToElement(result);

        Assert.Equal(
            "CORE-PRODUCT-001",
            json.GetProperty("moduleId").GetString());
        Assert.Equal(
            "operation-string-record-id",
            json.GetProperty("operationId").GetString());
        Assert.Equal(
            JsonValueKind.String,
            json.GetProperty("recordIds")[0].ValueKind);
        Assert.Equal(
            string.Empty,
            json.GetProperty("nextModuleId").GetString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Factory_RejectsEmptyOperationId(string? operationId)
    {
        Assert.ThrowsAny<ArgumentException>(() =>
            CoreResultFactory.Success(
                "CORE-PRODUCT-001",
                operationId!,
                "read_product"));
    }

    [Fact]
    public void Deserialize_RequiresOperationId()
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<CoreResult>("{}"));
    }

    [Fact]
    public void Factory_RejectsDataThatIsNotAJsonObject()
    {
        Assert.Throws<ArgumentException>(() =>
            CoreResultFactory.Success(
                "CORE-PRODUCT-001",
                "operation-invalid-data",
                "read_product",
                new[] { 1, 2, 3 }));
    }

    [Fact]
    public void FactoryDefaults_DoNotClaimDatabaseWasChecked()
    {
        var result = CoreResultFactory.NotFound(
            "CORE-PRODUCT-001",
            "operation-defaults",
            "read_product",
            "Product was not found.");

        Assert.False(result.DatabaseChecked);
        Assert.Equal(string.Empty, result.DatabaseOperation);
    }

    [Fact]
    public void NullAssignments_AreSerializedAsEmptyDataAndCollections()
    {
        var result = CoreResultFactory.Success(
            "CORE-PRODUCT-001",
            "operation-null-normalization",
            "read_product");

        result.Data = null!;
        result.Errors = null!;
        result.MissingData = null!;
        result.RecordIds = null!;

        var json = JsonSerializer.SerializeToElement(result);

        Assert.Equal(JsonValueKind.Object, json.GetProperty("data").ValueKind);
        Assert.Empty(json.GetProperty("errors").EnumerateArray());
        Assert.Empty(json.GetProperty("missingData").EnumerateArray());
        Assert.Empty(json.GetProperty("recordIds").EnumerateArray());
    }
}
