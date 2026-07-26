namespace API.Entities;

public class Order
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int UnitPrice { get; set; }

    public int TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; }
}