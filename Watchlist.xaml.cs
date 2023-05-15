using StockTracker.Model;
using Plugin.LocalNotification;

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

        Refresh(true);

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
                if (stock.Length != 0)
                {
                    List<Stock> watchlist_before = await App.StockRepo.Get_Stock_Watchlist(true);

                    stock = stock.ToUpper();
                    await App.StockRepo.Add_Stock(stock);

                    List<Stock> watchlist_after = await App.StockRepo.Get_Stock_Watchlist(true);

                    if (watchlist_before.Count == watchlist_after.Count)
                    {
                        await DisplayAlert("Add Stock", $"No stock with ticker {stock} found on stock market or already on watchlist", "ok");
                    }
                }
            }
        } else if (btn.Text == "Remove") /* else if remove button was clicked */
        {
            string stock = await DisplayPromptAsync("Remove Stock", "Enter stock ticker", "Remove", keyboard: Keyboard.Text, maxLength: 5);
            
            if (stock != null) /* if not cancelled remove */
            {
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

        Refresh(true);
    }

    /* handles removing a stock from watchlist with swipeview */
    private async void Swipe_Remove(object sender, EventArgs e)
    {
        SwipeItem remove_stock = (SwipeItem) sender;
        string stock_ticker = remove_stock.Text;

        await App.StockRepo.Remove_Stock(stock_ticker);
        Refresh(false);
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
                Refresh(false);
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

        Refresh(false);
    }

    /*
     * updates all the stocks on the database within watchlist
     * - this code is executed at notification alert times given within app settings
     */
    private async void Refresh(bool call_api)
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

            if (call_api) /* only calls api when desired */
            {
                await App.StockRepo.Update_Watchlist(sort_alpha);
                Gather_Threshold_Stocks();
            }
        }

        watchlist = await App.StockRepo.Get_Stock_Watchlist(sort_alpha);
        watchlist_items_display.ItemsSource = watchlist;
    }

    /* finds the stocks from watchlist that meet the requirements for a local push notification */
    private async void Gather_Threshold_Stocks()
    {
        /* access settings variables */
        Settings local_settings = new Settings();

        bool notifications = local_settings.notifications; /* notifications on/off */
        double value_percent_change_threshold = local_settings.value_percent_change; /* percent change of a stock to receive a push notification */
        
        if (notifications) /* if notifications are turned on */
        {
            /* gathering list of stocks from watchlist that meet change threshold */
            List<Stock> watchlist = await App.StockRepo.Get_Stock_Watchlist(true);
            List<Stock> stock_positive_threshold = new List<Stock>();
            List<Stock> stock_negative_threshold = new List<Stock>();

            for (int i = 0; i < watchlist.Count; i++)
            {
                Stock temp_stock = watchlist[i];
                
                double stock_percent_change = temp_stock.ticker_percent_day_change;
                double stock_percent_change_abs = Math.Abs(stock_percent_change);

                if (stock_percent_change_abs > value_percent_change_threshold) /* if percent change threshold reached either positive or negative */
                {
                    if (stock_percent_change > 0) /* if positive threshold reached */
                    {
                        stock_positive_threshold.Add(temp_stock);
                    } 
                    else /* else negative threshold reached */
                    {
                        stock_negative_threshold.Add(temp_stock);
                    }
                }
            }

            if (stock_positive_threshold.Count > 0) /* if any positive stocks past threshold */
            {
                Create_Notification(stock_positive_threshold, true);
            }

            if (stock_negative_threshold.Count > 0) /* if any negative stocks past threshold */
            {
                Create_Notification(stock_negative_threshold, false);
            }
        }
    }

    /* creates and sends local push notifications */
    private void Create_Notification(List<Stock> threshold_list, bool is_positive_list)
    {
        if (is_positive_list) /* if list of positive stocks past threshold */
        {
            string notification_description = "";

            /* fill lists for notification display */
            for (int i = 0; i < threshold_list.Count; i++)
            {
                if (i == threshold_list.Count - 1) /* if last stock on the threshold list */
                {
                    notification_description = $"{notification_description}{threshold_list[i].ticker_name} is up ${threshold_list[i].ticker_dollar_day_change} ({threshold_list[i].ticker_percent_day_change}%)";
                    break;
                }

                notification_description = $"{notification_description}{threshold_list[i].ticker_name} is up ${threshold_list[i].ticker_dollar_day_change} ({threshold_list[i].ticker_percent_day_change}%)\n";
            }

            var notification_alert = new NotificationRequest
            {
                NotificationId = 1,
                Title = "Bull Stocks",
                Subtitle = "Stock Threshold Alert",
                Description = notification_description,
                BadgeNumber = 1
            };

            LocalNotificationCenter.Current.Show(notification_alert);
        }
        else /* else list of negative stocks past threshold */
        {
            string notification_description = "";

            /* fill lists for notification display */
            for (int i = 0; i < threshold_list.Count; i++)
            {
                double abs_dollar_change = Math.Abs(threshold_list[i].ticker_dollar_day_change);
                double abs_percent_change = Math.Abs(threshold_list[i].ticker_percent_day_change);

                if (i == threshold_list.Count - 1) /* if last stock on the threshold list */
                {
                    notification_description = $"{notification_description}{threshold_list[i].ticker_name} is down -${abs_dollar_change} (-{abs_percent_change}%)";
                    break;
                }

                notification_description = $"{notification_description}{threshold_list[i].ticker_name} is down -${abs_dollar_change} (-{abs_percent_change}%)\n";
            }

            var notification_alert = new NotificationRequest
            {
                NotificationId = 2,
                Title = "Bear Stocks",
                Subtitle = "Stock Threshold Alert",
                Description = notification_description,
                BadgeNumber = 2
            };

            LocalNotificationCenter.Current.Show(notification_alert);
        }
    }
}