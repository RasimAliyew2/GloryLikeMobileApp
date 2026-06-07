using MetanetA_MobileApp.ViewModels.Sign.SingUp;
using MetanetA_MobileApp.ViewModels;



namespace MetanetA_MobileApp.View.SignUp;


    public partial class SkillsSelectionPage : ContentPage
    {
        private readonly SkillsSelectionViewModel _viewModel;

        public SkillsSelectionPage(SkillsSelectionViewModel viewModel)
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
