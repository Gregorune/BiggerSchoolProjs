using MyApi.Database.Tables;

namespace MyApi.DataObjects;

public class CartItem
{
    public int ProductId { get; set; }

    public int Quantity { get; set; } = 0;
    public Product Product { get; set; } = null!;
}