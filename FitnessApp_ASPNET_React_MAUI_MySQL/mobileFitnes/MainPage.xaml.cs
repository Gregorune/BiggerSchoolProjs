using mobileFitnes.ApiService;
namespace mobileFitnes;

[QueryProperty(nameof(afterLogin), "login")]
public partial class MainPage : ContentPage
{
    private readonly ClassesInfo _classesInfo;
    private readonly IApiEndpoints _api;
    
    public bool afterLogin { get; set; }
    
    public MainPage()
    {
        InitializeComponent();
        _classesInfo = MauiProgram.ServiceProvider.GetService<ClassesInfo>()!;
        _api = MauiProgram.ServiceProvider.GetService<IApiEndpoints>()!;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        Dispatcher.Dispatch(async () => {
            await LoadData();
        });
    }

    private async Task LoadData(bool refreshAll = false)
    {
        bool IsMyClasses = Shell.Current.CurrentState.Location.OriginalString.Contains("my");

        if (!_classesInfo.VisibleClasses.Any() || refreshAll || afterLogin)
        {
            await _classesInfo.Load();
            afterLogin = false;
        }

        _classesInfo.SetOnlyMine(IsMyClasses);

        ClassesDisplay.ItemsSource = null;
        ClassesDisplay.ItemsSource = _classesInfo.VisibleClasses;
    }

    private async void LogOut_Clicked(object sender, EventArgs e)
    {
        await _api.Logout();
        _classesInfo.VisibleClasses.Clear();
        SecureStorage.Default.RemoveAll();
        await Shell.Current.GoToAsync("//login");
    }
    private async void Refresh_Clicked(object sender, EventArgs e) => await LoadData(true);
}
