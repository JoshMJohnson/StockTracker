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

	private async void Add_Stock_Clicked(object sender, EventArgs e)
	{
		string stock = await DisplayPromptAsync("Add Stock", "Enter stock ticker", "Add", keyboard: Keyboard.Text);
	}
}

