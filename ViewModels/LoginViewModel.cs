using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Text.Json;
using Elkollen.Services;

namespace Elkollen.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }
        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isLoading;

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Ange både Användarnamn och Lösenord", "OK");
                return;
            }

            IsLoading = true;
            try
            {
                var token = await AuthenticateUser(Username, Password);
                if (!string.IsNullOrEmpty(token))
                {
                    // håller på token
                    await _authService.LoginAsync(token);

                    await Shell.Current.DisplayAlert("Perfekt", "Inloggning lyckad", "OK");
                    // navigering till MainPage
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Okänt Användarnamn eller Lösenord", "OK");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task Logout() // la den bara här för att testa
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task<string> AuthenticateUser(string username, string password)
        {
            var client = HttpClientFactory.CreateHttpClient();

            var loginData = new { Username = username, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/Auth/Login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var token = JsonSerializer.Deserialize<TokenResponse>(responseContent)?.token;
                return token;
            }

            return null;
        }

        private class TokenResponse
        {
            public string token { get; set; }
        }
    }
}