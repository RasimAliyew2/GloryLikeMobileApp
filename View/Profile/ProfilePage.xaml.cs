using MetanetA_MobileApp.ViewModels.Profile;

namespace MetanetA_MobileApp.View.Profile;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel? _viewModel;

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.Refresh();
    }
}
