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

            // If the shell is already loaded, navigate programmatically
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

            // Set a fixed size for the window (adjust these values as needed)
            window.Width = 500;  // phone-like width
            window.Height = 1000; // phone-like height

            // Center the window on the screen
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            window.X = (displayInfo.Width / displayInfo.Density / 2) - (window.Width / 2);
            window.Y = (displayInfo.Height / displayInfo.Density / 2) - (window.Height / 2);

            // Optionally, prevent window resizing
            window.MinimumWidth = window.Width;
            window.MinimumHeight = window.Height;
            window.MaximumWidth = window.Width;
            window.MaximumHeight = window.Height;

            return window;
        }
    }
}