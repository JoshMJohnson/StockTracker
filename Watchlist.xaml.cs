namespace StockTracker;

public partial class Watchlist : ContentPage
{
	public Watchlist()
	{
		InitializeComponent();
	}

    private async void Add_Stock(object sender, EventArgs e)
    {
        string stock = await DisplayPromptAsync("Add Stock", "Enter stock ticker", "Add", keyboard: Keyboard.Text);
    }

    private async void Remove_Stock(object sender, EventArgs e)
    {
        string stock = await DisplayPromptAsync("Remove Stock", "Enter stock ticker", "Remove", keyboard: Keyboard.Text);
    }
}