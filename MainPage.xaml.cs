namespace StockTracker;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void Settings_Button_Clicked(object sender, EventArgs e)
    {
		await Shell.Current.GoToAsync("Settings");
    }

	private async void Watchlist_Button_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("Watchlist");
	}
}