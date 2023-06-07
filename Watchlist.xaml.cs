using StockTracker.Model;

namespace StockTracker;

public partial class Watchlist : ContentPage
{
    private VerticalStackLayout vertical_layout_watchlist_empty;

    private bool sort_alpha { get; set; }
    private string sort_display { get; set; }

    public Watchlist()
	{
        InitializeComponent();

        /* sorting the list */
        sort_alpha = Preferences.Get("SortAlphaValue", true);
        sort_display = Preferences.Get("SortDisplay", "Sorted: Alpha");

        sort_button.Text = sort_display;

        Refresh();

        /* creates display for an empty watchlist */
        vertical_layout_watchlist_empty = new VerticalStackLayout();

        vertical_layout_watchlist_empty.VerticalOptions = LayoutOptions.Center;
        vertical_layout_watchlist_empty.HorizontalOptions = LayoutOptions.Center;

        vertical_layout_watchlist_empty.Add(new Label
        {
            Text = "No stocks in watchlist",
            FontSize = 20,
            FontAttributes = FontAttributes.Italic,
            HorizontalOptions = LayoutOptions.Center
        });

        vertical_layout_watchlist_empty.Add(new Image
        {
            Source = "empty_watchlist.png",
            HeightRequest = 200
        });

        Grid watchlist_layout = watchlist_page_layout;
        Grid.SetColumnSpan(vertical_layout_watchlist_empty, 2);
        Grid.SetRow(vertical_layout_watchlist_empty, 0);
        watchlist_layout.Children.Add(vertical_layout_watchlist_empty);
    }

    /* handles the adding and removing buttons on watchlist page */
    private async void Create_Stock_Entity(object sender, EventArgs e)
    {
        Button btn = (Button) sender; /* identifies the button that directed to this function */

        if (btn.Text == "Add") /* if add button was clicked on */
        {
            string stock = await DisplayPromptAsync("Add Stock", "Enter stock ticker", "Add", keyboard: Keyboard.Text, maxLength: 5);
            
            if (stock != null) /* if not cancelled add */
            {
                stock = stock.Trim(); /* removes leading and trailing whitespace */

                if (stock.Length != 0)
                {
                    List<Stock> watchlist_before = await App.StockRepo.Get_Stock_Watchlist(true);

                    stock = stock.ToUpper();
                    await App.StockRepo.Add_Stock(stock);

                    List<Stock> watchlist_after = await App.StockRepo.Get_Stock_Watchlist(true);

                    if (App.StockRepo.api_limit_reached) /* if api limit reached for the day */
                    {
                        await DisplayAlert("Add Stock", "API Get Request limit reached. Wait 1 minute", "ok");
                    }
                    else if (App.StockRepo.stock_not_found) /* if stock does not exist on market */
                    {
                        await DisplayAlert("Add Stock", $"No stock with ticker {stock} found on stock market", "ok");
                    }
                    else if (watchlist_before.Count == watchlist_after.Count) /* if duplicate; stock already on watchlist */
                    {
                        await DisplayAlert("Add Stock", $"Stock with ticker {stock} is already on your watchlist", "ok");
                    }
                }
            }
        } 
        else if (btn.Text == "Remove") /* else if remove button was clicked */
        {
            string stock = await DisplayPromptAsync("Remove Stock", "Enter stock ticker", "Remove", keyboard: Keyboard.Text, maxLength: 5);
            
            if (stock != null) /* if not cancelled remove */
            {
                stock = stock.Trim(); /* removes leading and trailing whitespace */

                if (stock.Length != 0)
                {
                    List<Stock> watchlist_before = await App.StockRepo.Get_Stock_Watchlist(true);

                    stock = stock.ToUpper();
                    await App.StockRepo.Remove_Stock(stock);

                    List<Stock> watchlist_after = await App.StockRepo.Get_Stock_Watchlist(true);

                    if (watchlist_before.Count == watchlist_after.Count)
                    {
                        await DisplayAlert("Remove Stock", $"No stock with ticker {stock} was on watchlist", "ok");
                    }
                }
            }
        }

        Refresh();
    }

    /* handles removing a stock from watchlist with swipeview */
    private async void Swipe_Remove(object sender, EventArgs e)
    {
        SwipeItem remove_stock = (SwipeItem) sender;
        string stock_ticker = remove_stock.Text;

        await App.StockRepo.Remove_Stock(stock_ticker);
        Refresh();
    }

    /* clear button clicked on the watchlist page; deletes all stocks */
    private async void Clear_Watchlist(object sender, EventArgs e)
    {
        List<Stock> watchlist = await App.StockRepo.Get_Stock_Watchlist(true);

        if (watchlist.Count == 0) /* if watchlist was already empty */
        {
            await DisplayAlert("Clear", "Watchlist is already empty", "ok");
        }
        else /* else watchlist is not empty */
        {
            bool confirm = await DisplayAlert("Clear", "Are you sure?", "Confirm", "Cancel");

            if (confirm) /* confirmed clear of watchlist */
            {
                await App.StockRepo.Clear_Watchlist();
                Refresh();
            }
        }
    }

    /* sort button clicked on the watchlist page; toggles between alphabetical and stock percent day change */
    private void Sort_Watchlist(object sender, EventArgs e)
    {
        sort_alpha = Preferences.Get("SortAlphaValue", true);
   
        if (sort_alpha) /* set to sort by percent day change */
        {
            Preferences.Set("SortAlphaValue", false);
            Preferences.Set("SortDisplay", "Sorted: Perc.");
        }
        else /* set to sort alphabetically */
        {
            Preferences.Set("SortAlphaValue", true);
            Preferences.Set("SortDisplay", "Sorted: Alpha");
        }

        sort_display = Preferences.Get("SortDisplay", "Sorted: Alpha");
        sort_button.Text = sort_display;

        Refresh();
    }

    /*
     * updates all the stocks on the database within watchlist
     * - this code is executed at notification alert times given within app settings
     */
    public async void Refresh()
    {
        sort_alpha = Preferences.Get("SortAlphaValue", true);

        List<Stock> watchlist = await App.StockRepo.Get_Stock_Watchlist(sort_alpha);

        if (watchlist.Count == 0) /* if watchlist is empty */
        {
            vertical_layout_watchlist_empty.IsVisible = true;
        }
        else /* else watchlist is not empty */
        {
            vertical_layout_watchlist_empty.IsVisible = false;
        }

        List<Stock>  watchlist_updated = await App.StockRepo.Get_Stock_Watchlist(sort_alpha);
        watchlist_items_display.ItemsSource = watchlist_updated;
    }
}