using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Database;
using MyApi.Database.Tables;
using MyApi.ViewModels;

namespace MyApi.Controllers;


[ApiController]
[Route("dev")]
public class DevControler : Controller
{
    private readonly AppDbContext _db;
    public DevControler(AppDbContext db) { _db = db; }

    [HttpGet("db/products/tojson")]
    public async Task<ActionResult> GetProductsAsJson()
    {
        var products = await _db.Products.ToListAsync();
        return Json(products);
    }
    [HttpGet("db/products/fromjson")]
    public ActionResult GetProductsFromJson()
    {
        var vm = new BaseViewModel();
        vm.Title = "Import products from JSON";
        return View("~/Views/Dev/DbProductsFromJson.cshtml", vm);
    }
    [HttpPost("db/products/fromjson")]
    public async Task<ActionResult> AddProductsFromJson([FromBody] List<Product> products)
    {
        _db.Products.AddRange(products);
        await _db.SaveChangesAsync();
        return Redirect("/");
    }
}
