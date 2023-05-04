using SQLite;
using StockTracker.Model;

namespace StockTracker;

public partial class Watchlist : ContentPage
{
    public Watchlist()
	{
		InitializeComponent();
        Refresh();
    }

    /* handles the adding and removing buttons on watchlist page */
    public async void Create_Stock_Entity(object sender, EventArgs e)
    {
        Button btn = (Button) sender; /* identifies the button that directed to this function */

        if (btn.Text == "Add") /* if add button was clicked on */
        {
            string stock = await DisplayPromptAsync("Add Stock", "Enter stock ticker", "Add", keyboard: Keyboard.Text, maxLength: 5);

            await App.StockRepo.Add_Stock(stock);
        } else if (btn.Text == "Remove") /* else if remove button was clicked */
        {
            string stock = await DisplayPromptAsync("Remove Stock", "Enter stock ticker", "Remove", keyboard: Keyboard.Text, maxLength: 5);

            //await App.StockRepo.Remove_Stock(stock);
        }

        Refresh();
    }

    /* gets all the stocks on the database and displays on UI */
    public async void Refresh()
    {
        List<Stock> watchlist = await App.StockRepo.Get_Stock_Watchlist();

        if (watchlist.Count == 0) /* if watchlist is empty */
        {
            watchlist_items_display.VerticalOptions = LayoutOptions.Center;
        }
        else /* else watchlist is not empty */
        {
            watchlist_items_display.VerticalOptions = LayoutOptions.Start;
            watchlist_items_display.ItemsSource = watchlist;
        }
    }
}