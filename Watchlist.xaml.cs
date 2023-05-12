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

    /* sort button clicked on the watchlist page; toggles between alphabetical and stock price */
    private void Sort_Watchlist(object sender, EventArgs e)
    {
        sort_alpha = Preferences.Get("SortAlphaValue", true);
   
        if (sort_alpha) /* set to sort by price */
        {
            Preferences.Set("SortAlphaValue", false);
            Preferences.Set("SortDisplay", "Sorted: Price");
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

    /* gets all the stocks on the database and displays on UI */
    private async void Refresh()
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
            await App.StockRepo.Update_Watchlist(sort_alpha);
        }

        watchlist = await App.StockRepo.Get_Stock_Watchlist(sort_alpha);

        watchlist_items_display.ItemsSource = watchlist;
    }
}