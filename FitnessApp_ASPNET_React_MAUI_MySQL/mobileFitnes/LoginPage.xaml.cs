using mobileFitnes.ApiService;

namespace mobileFitnes;
public partial class LoginPage : ContentPage
{
    private bool _isSigningUp = false;
    private readonly IApiEndpoints _api;

    public LoginPage(IApiEndpoints api)
    {
        InitializeComponent();
        _api = api;
    }
    override protected void OnAppearing()
    {
        base.OnAppearing();

        EmailEntry.Text = "";
        PasswordEntry.Text = "";
        RepeatPasswordEntry.Text = "";
        UsernameEntry.Text = "";

        var refreshToken = SecureStorage.Default.GetAsync("refresh").Result;
        if (refreshToken == null)
            return;

        // Try to refresh token
        _api.Refresh(new(refreshToken)).ContinueWith(async res =>
        {
            if (res.Result.IsSuccessStatusCode)
            {
                await SecureStorage.Default.SetAsync("jwt", res.Result.Content!.JwtToken);
                await SecureStorage.Default.SetAsync("refresh", res.Result.Content!.RefreshToken);
                await Shell.Current.GoToAsync("//main/all_classes");
            }
            else
            {
                SecureStorage.Default.RemoveAll();
            }
        });
    }

    private void OnToggleModeTapped(object? sender, EventArgs? e)
    {
        _isSigningUp = !_isSigningUp;
        TitleLabel.Text = _isSigningUp ? "Zarejestruj siê" : "Zaloguj siê";
        SubmitBtn.Text = _isSigningUp ? "Zarejestruj siê" : "Zaloguj siê";
        ToggleText.Text = _isSigningUp ? "Masz ju¿ konto?" : "Nie masz jeszcze konta?";
        ToggleBtnLabel.Text = _isSigningUp ? "Zaloguj siê" : "Zarejestruj siê";

        RepeatPasswordEntry.IsVisible = _isSigningUp;
        UsernameEntry.IsVisible = _isSigningUp;
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        ShowAlert(null); 
        SubmitBtn.IsEnabled = false;
        await (_isSigningUp ? Register() : Login());
        SubmitBtn.IsEnabled = true;
    }
    private async Task<bool> Register()
    {
        try
        {
            if (PasswordEntry.Text != RepeatPasswordEntry.Text)
            {
                ShowAlert("Has³a musz¹ byæ takie same");
                return false;
            }

            var res = await _api.Register(new RegisterReqDto(
                Email: EmailEntry.Text,
                Password: PasswordEntry.Text,
                Username: UsernameEntry.Text
            ));
            if (!res.IsSuccessStatusCode)
            {
                switch (res.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        ShowAlert("Nieprawid³owe dane rejestracji");
                        break;
                    case System.Net.HttpStatusCode.Conflict:
                        ShowAlert("U¿ytkownik o podanym emailu ju¿ istnieje");
                        break;
                    default:
                        ShowAlert("Coœ posz³o nie tak.");
                        break;
                }
                return false;
            }

            ShowAlert("Zarejestrowano pomyœlnie!", false);
            return true;
        }
        catch (Exception ex)
        {
            ShowAlert("B³¹d po³¹czenia z serwerem: " + ex.Message);
            return false;
        }
    }
    private async Task<bool> Login()
    {
        try
        {
            var res = await _api.Login(new LoginReqDto(
                Email: EmailEntry.Text,
                Password: PasswordEntry.Text
            ));

            if (!res.IsSuccessStatusCode)
            {
                switch (res.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        ShowAlert("Nieprawid³owe email lub has³o");
                        break;
                    default:
                        ShowAlert("Coœ posz³o nie tak.");
                        break;
                }
                return false;
            }

            await SecureStorage.Default.SetAsync("jwt", res.Content!.JwtToken);
            await SecureStorage.Default.SetAsync("refresh", res.Content!.RefreshToken);
            await Shell.Current.GoToAsync("//main/all_classes?login=true");
            return true;
        }
        catch (Exception ex)
        {
            ShowAlert("B³¹d po³¹czenia z serwerem: " + ex.Message);
            return false;
        }

    }

    private void ShowAlert(string? message, bool isError = true)
    {
        if (string.IsNullOrEmpty(message))
        {
            AlertBox.IsVisible = false;
            return;
        }
        AlertBox.IsVisible = true;
        AlertBox.BackgroundColor = isError ? Color.FromArgb("#f8d7da") : Color.FromArgb("#d1e7dd");
        AlertLabel.Text = message;
        AlertLabel.TextColor = isError ? Color.FromArgb("#842029") : Color.FromArgb("#0f5132");
    }
}