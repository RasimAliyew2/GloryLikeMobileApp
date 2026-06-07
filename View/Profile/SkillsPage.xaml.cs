using MetanetA_MobileApp.ViewModels.Profile;

namespace MetanetA_MobileApp.View.Profile;

public partial class SkillsPage : ContentPage
{
    public SkillsPage(SkillsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}