namespace MyApi.ViewModels.Shop;

using MyApi.Database.Tables;

public class ShopVM : BaseViewModel
{
    public List<Product> Products { get; set; } = new();
}