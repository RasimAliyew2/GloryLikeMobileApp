using MetanetA_MobileApp.ViewModels.Profile;

namespace MetanetA_MobileApp.View.Profile;

public partial class SkillsPage : ContentPage
{
    private readonly SkillsViewModel _viewModel;

    public SkillsPage(SkillsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LoadCommand.CanExecute(null))
            _viewModel.LoadCommand.Execute(null);
    }
}
