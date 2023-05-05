using SQLite;
using StockTracker.Model;

namespace StockTracker;

public partial class Watchlist : ContentPage
{
    VerticalStackLayout vertical_layout;

    public Watchlist()
	{
		InitializeComponent();
        Refresh();

        vertical_layout = new VerticalStackLayout();

        vertical_layout.VerticalOptions = LayoutOptions.Center;
        vertical_layout.HorizontalOptions = LayoutOptions.Center;

        vertical_layout.Add(new Label
        {
            Text = "No stocks in watchlist",
            FontSize = 20,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center
        });

        vertical_layout.Add(new Image
        {
            Source = "empty_watchlist.png",
            HeightRequest = 200
        });

        Grid watchlist_layout = watchlist_page_layout;
        Grid.SetColumnSpan(vertical_layout, 2);
        Grid.SetRow(vertical_layout, 0);
        watchlist_layout.Children.Add(vertical_layout);
    }

    /* handles the adding and removing buttons on watchlist page */
    public async void Create_Stock_Entity(object sender, EventArgs e)
    {
        Button btn = (Button) sender; /* identifies the button that directed to this function */

        if (btn.Text == "Add") /* if add button was clicked on */
        {
            string stock = await DisplayPromptAsync("Add Stock", "Enter stock ticker", "Add", keyboard: Keyboard.Text, maxLength: 5);
            
            if (stock != null) /* if not cancelled add */
            {
                stock = stock.ToUpper();
                await App.StockRepo.Add_Stock(stock);
            }
        } else if (btn.Text == "Remove") /* else if remove button was clicked */
        {
            string stock = await DisplayPromptAsync("Remove Stock", "Enter stock ticker", "Remove", keyboard: Keyboard.Text, maxLength: 5);
            
            if (stock != null) /* if not cancelled remove */
            {
                stock = stock.ToUpper();
                await App.StockRepo.Remove_Stock(stock);
            }
        }

        Refresh();
    }

    /* clear button clicked on the watchlist page; deletes all stocks */
    public async void Clear_Watchlist(object sender, EventArgs e)
    {
        await App.StockRepo.Clear_Watchlist();
        Refresh();
    }

    /* gets all the stocks on the database and displays on UI */
    public async void Refresh()
    {
        List<Stock> watchlist = await App.StockRepo.Get_Stock_Watchlist();

        if (watchlist.Count == 0) /* if watchlist is empty */
        {
            vertical_layout.IsVisible = true;
        }
        else /* else watchlist is not empty */
        {
            vertical_layout.IsVisible = false;
            watchlist_items_display.VerticalOptions = LayoutOptions.Start;
        }

        watchlist_items_display.ItemsSource = watchlist;
    }
}