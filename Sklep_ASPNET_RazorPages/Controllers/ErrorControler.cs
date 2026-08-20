namespace MyApi.Controllers;

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MyApi.ViewModels;
using MyApi.Database.Tables;

[ApiController]
public class ErrorControler : Controller
{
    [Route("Error/{statusCode}")]
    public ActionResult ErrorPage([FromRoute] int statusCode)
    {
        return View("~/Views/Error.cshtml", new BaseViewModel() { Title = "404 Not Found" });
    }
}
