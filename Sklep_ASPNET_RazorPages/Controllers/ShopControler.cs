using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Database;
using MyApi.Database.Tables;
using MyApi.ViewModels;
using MyApi.ViewModels.Shop;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyApi.Controllers;

[ApiController]
[Route("/")]
public class ShopControler : Controller
{
    private readonly AppDbContext _db;
    private readonly RuntimeStorageService _rss;
    public ShopControler(AppDbContext context, RuntimeStorageService rss)
    {
        _db = context;
        _rss = rss;
    }
    [HttpGet]
    public ActionResult GetIndexView()
    {
        var vm = new ShopVM();
        vm.Title = "Shop";
        vm.Css.Add("/static/css/product.css");
        vm.Products = _db.Products.ToList();

        return View("~/Views/Shop/Index.cshtml", vm);
    }

    [HttpGet("products")]
    public ActionResult GetProductsView()
    {
        var vm = new ShopVM();
        vm.Title = "All Products";
        vm.Css.Add("/static/css/product.css");
        vm.Products = _db.Products.ToList();

        return View("~/Views/Shop/ProductList.cshtml", vm);
    }

    [HttpGet("checkout")]
    public ActionResult GetCheckoutView()
    {
        var vm = new BaseViewModel();
        vm.Title = "Checkout";

        return View("~/Views/Shop/Checkout.cshtml", vm);
    }

    [HttpGet("products/{productId?}")]
    public async Task<ActionResult> GetProductDetailsView([FromRoute] int? productId)
    {
        if (productId == null) return Redirect("/");
        Product? product = await _db.Products.Where(p => p.Id == productId).FirstAsync();
        if (product == null) return Redirect("/");

        var vm = new ProductDetailVM(product);

        vm.Css.Add("/static/css/product.css");
        vm.Title = $"{product.Title} details";

        return View("~/Views/Shop/ProductDetail.cshtml", vm);
    }

    [HttpGet("cart")]
    public async Task<ActionResult<object>> GetCartView()
    {
        var vm = new CartVM();
        vm.Title = "Your Cart";
        #region FUN FACT
        /*
        //System.LinQ 1
        var cartItems = await _db.CartItems //SELECT * FROM CartItems ci
            .Join
            (
                _db.Products, //INNER JOIN Products p
                ci => ci.ProductId, //ON ci.ProductId =
                p => p.Id, //p.Id
                (ci, p) => new CartItem
                {
                    Id = ci.Id,
                    UId = ci.UId,
                    Quantity = ci.Quantity,
                    ProductId = ci.ProductId,
                    Product = p
                }
            )
            .ToListAsync();

        //System.LinQ 2
        var cartItems2 = await (
            from ci in _db.CartItems
            join p in _db.Products on ci.ProductId equals p.Id
            select new CartItem
            {
                Id = ci.Id,
                UId = ci.UId,
                Quantity = ci.Quantity,
                ProductId = ci.ProductId,
                Product = p
            }
        ).ToListAsync();
        */
        #endregion

        //Microsoft.EFCore
        //var cartItems = await Cart.Include(ci => ci.Product).ToListAsync();

        var productIds = _rss.Cart
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();

        var products = await _db.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        foreach (var item in _rss.Cart)
        {
            if (products.TryGetValue(item.ProductId, out var product))
            {
                item.Product = product;
            }
        }

        vm.CartItems = _rss.Cart;
        vm.Css.Add("/static/css/myAdditionalStyles.css");

        return View("~/Views/Shop/Cart.cshtml", vm);
    }

    public record RemoveCartItemForm(int removeId);
    [HttpPost("cart/remove")]
    public ActionResult PostRemoveCartItem([FromForm] RemoveCartItemForm form)
    {
        var itemToRemove = _rss.Cart.Where(ci => ci.ProductId == form.removeId).FirstOrDefault();
        if (itemToRemove == null)
            return Redirect("/");

        _rss.Cart.Remove(itemToRemove);

        return Redirect("/cart");
    }

    [HttpGet("orders")]
    public ActionResult GetOrders()
    {
        var vm = new BaseViewModel();
        vm.Title = "Your Orders";

        return View("~/Views/Shop/Orders.cshtml", vm);
    }

    public record AddToCartForm(int? productId);
    [HttpPost("cart")]
    public async Task<ActionResult> PostCart([FromForm] AddToCartForm form)
    {
        if (form.productId == null)
            return Redirect("/");

        if (!await _db.Products.Where(p => p.Id == form.productId).AnyAsync())
            return Redirect("/");

        var cartItem = _rss.Cart.Where(ci => ci.ProductId == form.productId).FirstOrDefault();

        if(cartItem == null)
        {
            cartItem = new();

            cartItem.Quantity = 1;
            cartItem.ProductId = form.productId ?? 0;

            _rss.Cart.Add(cartItem);
        }
        else
        {
            cartItem.Quantity += 1;
        }

        await _db.SaveChangesAsync();

        return Redirect("/cart");
    }
}