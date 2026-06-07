using MetanetA_MobileApp.View.Map;
using MetanetA_MobileApp.ViewModels;

namespace MetanetA_MobileApp.View;

public partial class OthersPage : ContentPage
{
	public OthersPage(OthersPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    private async void WhatsApp_Tapped(object sender, TappedEventArgs e)
    {
        // linkləri özün istədiyin kimi dəyiş
        await Launcher.Default.OpenAsync(new Uri("https://wa.me/994502983030"));
    }

    private async void Instagram_Tapped(object sender, TappedEventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://www.instagram.com/matanata.company?igsh=Nnlwd20wZWM3Mndl"));
    }

    private async void Facebook_Tapped(object sender, TappedEventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://www.facebook.com/MatanatA.company"));
    }

    private async void LinkedIn_Tapped(object sender, TappedEventArgs e)
    {
        await Launcher.Default.OpenAsync(new Uri("https://www.linkedin.com/company/matanat-a-official/posts/?feedView=all"));
    }
    private async void Map_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(LocationMapPage)}");
    }
    private async void Faq_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync($"//{nameof(FaqPage)}");
    }


    

}