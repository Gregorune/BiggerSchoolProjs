using MyApi.Database.Tables;

namespace MyApi.ViewModels.Shop;

public class ProductDetailVM : BaseViewModel
{
    public Product Product;
    public ProductDetailVM(Product product)
    {
        Product = product;
    }
}
