using Elkollen.Services;

namespace Elkollen
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;
        public App(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;

            MainThread.BeginInvokeOnMainThread(async () => {
                await CheckAuthenticationAsync();
            });
        }
        private async Task CheckAuthenticationAsync()
        {
            bool isAuthenticated = await _authService.IsAuthenticatedAsync();

            // om shellet redan finns och användaren är inloggad, gå till huvudsidan
            if (Shell.Current != null)
            {
                if (isAuthenticated)
                    await Shell.Current.GoToAsync("//MainPage");
                else
                    await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            Window window = new Window(new AppShell());

            // storlek på fönstret
            window.Width = 500;  
            window.Height = 1050; 

            // sätter fönstret i mitten
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            window.X = (displayInfo.Width / displayInfo.Density / 2) - (window.Width / 2);
            window.Y = (displayInfo.Height / displayInfo.Density / 2) - (window.Height / 2);

            // förhindra att ändra storleken
            window.MinimumWidth = window.Width;
            window.MinimumHeight = window.Height;
            window.MaximumWidth = window.Width;
            window.MaximumHeight = window.Height;

            return window;
        }
    }
}