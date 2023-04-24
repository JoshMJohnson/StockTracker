namespace StockTracker;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private void Settings_Clicked(object sender, EventArgs e)
    {
		DisplayAlert("Settings", "settings clicked", "Save");
    }

	private async void Watchlist_Button_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("Watchlist");
	}
}

