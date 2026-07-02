using MetanetA_MobileApp.ViewModels.Sign.SingUp;

namespace MetanetA_MobileApp.View.SignUp;

public partial class SkillEvidencePage : ContentPage
{
    private readonly SkillEvidenceViewModel _viewModel;

    public SkillEvidencePage(SkillEvidenceViewModel viewModel)
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
