namespace StockTracker;

public partial class Watchlist : ContentPage
{
	public Watchlist()
	{
		InitializeComponent();

        /* TODO: if collection is empty, set VerticalOptions="Center" for collectionview */
	}

    private async void Add_Stock(object sender, EventArgs e)
    {
        string stock = await DisplayPromptAsync("Add Stock", "Enter stock ticker", "Add", keyboard: Keyboard.Text, maxLength: 5);
    }

    private async void Remove_Stock(object sender, EventArgs e)
    {
        string stock = await DisplayPromptAsync("Remove Stock", "Enter stock ticker", "Remove", keyboard: Keyboard.Text, maxLength: 5);
    }
}