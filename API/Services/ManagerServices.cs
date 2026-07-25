namespace API.Services;

public class ManagerService
{
    private readonly StockService _stockService;
    private readonly PriceService _priceService;
    private readonly OrderService _orderService;

    public ManagerService(
        StockService stockService,
        PriceService priceService,
        OrderService orderService)
    {
        _stockService = stockService;
        _priceService = priceService;
        _orderService = orderService;
    }

    public async Task<object> ExecuteAsync(
        string action,
        int productId,
        int value)
    {
        switch (action.ToLower())
        {
            case "change_stock":
            {
                var quantity = await _stockService.ChangeStockAsync(
                    productId,
                    value
                );

                return new
                {
                    action,
                    productId,
                    quantity
                };
            }

            case "change_price":
            {
                var price = await _priceService.ChangePriceAsync(
                    productId,
                    value
                );

                return new
                {
                    action,
                    productId,
                    price
                };
            }

            case "create_order":
            {
                var order = await _orderService.CreateOrderAsync(
                    productId,
                    value
                );

                return new
                {
                    action,
                    order
                };
            }

            default:
                throw new InvalidOperationException(
                    $"Неизвестная команда: {action}"
                );
        }
    }
}