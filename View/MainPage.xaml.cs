using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshScore();
    }
}
