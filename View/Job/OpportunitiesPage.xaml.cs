using MetanetA_MobileApp.ViewModels.Job;

namespace MetanetA_MobileApp.View.Job;

public partial class OpportunitiesPage : ContentPage
{
    private readonly OpportunitiesViewModel _viewModel;

    public OpportunitiesPage(OpportunitiesViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.LoadCommand.CanExecute(null))
            _viewModel.LoadCommand.Execute(null);
    }
}
