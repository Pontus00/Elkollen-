using Elkollen.ViewModels;

namespace Elkollen.Pages;

public partial class PricesPage : ContentPage
{
    public PricesPage(PricesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}