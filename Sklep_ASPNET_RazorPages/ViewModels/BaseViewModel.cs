namespace MyApi.ViewModels;

public class BaseViewModel
{
    public string Title { get; set; } = "Document";

    public List<string> Css { get; set; } = new()
    {
        "/static/css/main.css"
    };

    public List<string> JsScripts { get; set; } = new()
    {
        "/static/js/main.js"
    };
    public BaseViewModel() { }
    public BaseViewModel(string Title) => this.Title = Title;
}