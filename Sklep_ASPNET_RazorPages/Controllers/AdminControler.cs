namespace MyApi.Controllers;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Database;
using MyApi.Database.Tables;
using MyApi.ViewModels;
using MyApi.ViewModels.Admin;
using MyApi.ViewModels.Shop;

[ApiController]
[Route("admin")]
public class AdminControler : Controller
{
    private readonly AppDbContext _db;
    public AdminControler(AppDbContext db) { _db = db; }

    public record FormRequest(string title, string imageUrl, string price, string description);
    [HttpPost("add-product")]
    public async Task<ActionResult> AddProductToDb([FromForm] FormRequest form)
    {
        Product newProduct = new Product();
        newProduct.Title = form.title;
        newProduct.ImageUrl = form.imageUrl;
        newProduct.Price = double.Parse(form.price, System.Globalization.CultureInfo.InvariantCulture);
        newProduct.Description = form.description;
        _db.Products.Add(newProduct);
        await _db.SaveChangesAsync();

        return Redirect("/");
    }

    [HttpGet("add-product")]
    public ActionResult AddProductView()
    {
        var vm = new EditProductVM();
        vm.Title = "Add Product";
        vm.Editing = false;
        vm.Css.Add("/static/css/product.css");
        vm.Css.Add("/static/css/forms.css");

        return View("~/Views/Admin/EditProduct.cshtml", vm);
    }

    [HttpGet("products")]
    public ActionResult GetProductsView()
    {
        var vm = new ShopVM();  
        vm.Css.Add("/static/css/product.css");
        vm.Css.Add("/static/css/myAdditionalStyles.css");

        vm.Title = "Admin Products";
        vm.Products = _db.Products.ToList();
        return View("~/Views/Admin/Products.cshtml", vm);
    }

    [HttpGet("edit-product/{productId?}")]
    public async Task<ActionResult> EditProductView([FromRoute] int? productId, [FromQuery] bool editing)
    {
        if(productId == null) return Redirect("/");
        Product? product = await _db.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();
        if(product == null) return Redirect("/");

        var vm = new EditProductVM();
        vm.Title = "Edit Product";
        vm.Css.Add("/static/css/product.css");
        vm.Css.Add("/static/css/forms.css");
        vm.Editing = editing;
        
        vm.p_name = product.Title;
        vm.p_imgurl = product.ImageUrl;
        vm.p_description = product.Description;
        vm.p_price = product.Price;
        vm.p_id = product.Id;

        return View("~/Views/Admin/EditProduct.cshtml", vm);
    }

    [HttpPost("edit-product/{productId?}")]
    public async Task<ActionResult> EditProductPost([FromRoute] int? productId, [FromForm] FormRequest form)
    {
        Product? product;
        if(productId == null) return Redirect("/");
        
        product = await _db.Products.Where(p => p.Id == productId).FirstOrDefaultAsync();

        if(product == null) return Redirect("/");
        product.Title = form.title;
        product.ImageUrl = form.imageUrl;
        product.Price = double.Parse(form.price, System.Globalization.CultureInfo.InvariantCulture);
        product.Description = form.description;

        await _db.SaveChangesAsync();

        return Redirect("/admin/products");
    }

    [HttpPost("delete-product/{productId?}")]
    public async Task<ActionResult> RemoveProductPost([FromRoute] int? productId)
    {
        if (productId == null)
            return Redirect("/");

        var productToRemove = await _db.Products.Where(p => p.Id == productId).FirstAsync();
        if (productToRemove == null)
            return Redirect("/");

        _db.Products.Remove(productToRemove);
        await _db.SaveChangesAsync();

        return Redirect("/admin/products");
    }
}