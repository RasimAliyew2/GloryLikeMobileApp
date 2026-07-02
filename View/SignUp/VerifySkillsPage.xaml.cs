using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.View.SignUp;

public partial class VerifySkillsPage : ContentPage
{
    private readonly VerifySkillsViewModel _viewModel;

    public VerifySkillsPage(VerifySkillsViewModel vm)
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