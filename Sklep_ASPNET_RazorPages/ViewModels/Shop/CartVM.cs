using MyApi.DataObjects;

namespace MyApi.ViewModels.Shop;

public class CartVM : BaseViewModel
{
    public List<CartItem> CartItems { get; set; } = new();
}
