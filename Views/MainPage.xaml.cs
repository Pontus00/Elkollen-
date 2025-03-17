using Elkollen.ViewModels;

namespace Elkollen.Pages
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private readonly LoginViewModel _loginViewModel;
        public MainPage(LoginViewModel loginViewModel)
        {
            InitializeComponent();
            _loginViewModel = loginViewModel;
        }

        private void LoginBtn_Clicked(object sender, EventArgs e)
        {
            //Navigate to LoginPage
            Navigation.PushAsync(new LoginPage(_loginViewModel));
        }
    }

}
