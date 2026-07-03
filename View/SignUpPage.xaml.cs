using MetanetA_MobileApp.View.SignUp;
using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class SignUpPage : ContentPage
{
   
    public SignUpPage(SignUpViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void Continue_Clicked(object sender, EventArgs e)
    {
        //if (string.IsNullOrWhiteSpace(FirstNameEntry.Text) ||
        //    string.IsNullOrWhiteSpace(LastNameEntry.Text) ||
        //    string.IsNullOrWhiteSpace(EmailEntry.Text))
        //{
        //    ValidationLabel.Text = "Please fill first name, last name, and email.";
        //    ValidationPanel.IsVisible = true;
        //    return;
        //}

        //ValidationPanel.IsVisible = false;
       
    }

    private async void Back_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(SignInPage)}");
    }
}
