using Android.OS;
using AndroidX.Activity;
using Plugin.LocalNotification;
using StockTracker.Model;

namespace StockTracker;

public class Notification_Timers
{
    private Timer timer1;
    private Timer timer2;
    private Timer timer3;

    public Notification_Timers()
	{
        timer1 = new Timer(Refresh); 
        timer2 = new Timer(Refresh);
        timer3 = new Timer(Refresh);
    }

    /* prepares timers */
    public void Create_Timers()
    {
        Settings settings = new Settings();

        bool notifications = settings.notifications;

        if (notifications) /* if notifications are turned on */
        {
            int num_notifications = settings.num_notification_times;

            string notification_time1 = settings.value_tod1;
            string notification_time2 = settings.value_tod2;
            string notification_time3 = settings.value_tod3;

            /* timer 1 info */
            string[] updated_view_array = notification_time1.Split(':');
            string hours_string = updated_view_array[0];
            string mins_string = updated_view_array[1];
            int hours = int.Parse(hours_string);
            int mins = int.Parse(mins_string);

            /* timer 2 info */
            string[] updated_view_array2 = notification_time2.Split(':');
            string hours_string2 = updated_view_array2[0];
            string mins_string2 = updated_view_array2[1];
            int hours2 = int.Parse(hours_string2);
            int mins2 = int.Parse(mins_string2);

            /* timer 3 info */
            string[] updated_view_array3 = notification_time3.Split(':');
            string hours_string3 = updated_view_array3[0];
            string mins_string3 = updated_view_array3[1];
            int hours3 = int.Parse(hours_string3);
            int mins3 = int.Parse(mins_string3);

            DateTime current_time = DateTime.Now;

            if (num_notifications == 0) /* if 1 timer set */
            {
                /* timer 1 setup */
                DateTime notification_time_hours = DateTime.Today.AddHours(hours);
                DateTime notification_time_total = notification_time_hours.AddMinutes(mins);

                if (current_time > notification_time_total) /* if already past notification time for that day */
                {
                    notification_time_total = notification_time_total.AddDays(1.0);
                }

                int ms_until_notification_time = (int)((notification_time_total - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time, Timeout.Infinite);
                timer2.Change(Timeout.Infinite, Timeout.Infinite);
                timer3.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else if (num_notifications == 1) /* else 2 timers set */
            {                                
                /* timer 1 setup */
                DateTime notification_time_hours1 = DateTime.Today.AddHours(hours);
                DateTime notification_time_total1 = notification_time_hours1.AddMinutes(mins);

                if (current_time > notification_time_total1) /* if already past notification time for that day */
                {
                    notification_time_total1 = notification_time_total1.AddDays(1.0);
                }

                int ms_until_notification_time1 = (int)((notification_time_total1 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time1, Timeout.Infinite);

                /* timer 2 setup */
                DateTime notification_time_hours2 = DateTime.Today.AddHours(hours2);
                DateTime notification_time_total2 = notification_time_hours2.AddMinutes(mins2);

                if (current_time > notification_time_total2) /* if already past notification time for that day */
                {
                    notification_time_total2 = notification_time_total2.AddDays(1.0);
                }

                int ms_until_notification_time2 = (int)((notification_time_total2 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time1, Timeout.Infinite);
                timer2.Change(ms_until_notification_time2, Timeout.Infinite);
                timer3.Change(Timeout.Infinite, Timeout.Infinite);
            }
            else /* else 3 timers set */
            {
                /* timer 1 setup */
                DateTime notification_time_hours1 = DateTime.Today.AddHours(hours);
                DateTime notification_time_total1 = notification_time_hours1.AddMinutes(mins);

                if (current_time > notification_time_total1) /* if already past notification time for that day */
                {
                    notification_time_total1 = notification_time_total1.AddDays(1.0);
                }

                int ms_until_notification_time1 = (int)((notification_time_total1 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time1, Timeout.Infinite);

                /* timer 2 setup */
                DateTime notification_time_hours2 = DateTime.Today.AddHours(hours2);
                DateTime notification_time_total2 = notification_time_hours2.AddMinutes(mins2);

                if (current_time > notification_time_total2) /* if already past notification time for that day */
                {
                    notification_time_total2 = notification_time_total2.AddDays(1.0);
                }

                int ms_until_notification_time2 = (int)((notification_time_total2 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer2.Change(ms_until_notification_time2, Timeout.Infinite);

                /* timer 3 setup */
                DateTime notification_time_hours3 = DateTime.Today.AddHours(hours3);
                DateTime notification_time_total3 = notification_time_hours3.AddMinutes(mins3);

                if (current_time > notification_time_total3) /* if already past notification time for that day */
                {
                    notification_time_total3 = notification_time_total3.AddDays(1.0);
                }

                int ms_until_notification_time3 = (int)((notification_time_total3 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time1, Timeout.Infinite);
                timer2.Change(ms_until_notification_time2, Timeout.Infinite);
                timer3.Change(ms_until_notification_time3, Timeout.Infinite);
            }
        }
    }

    /*
     * updates all the stocks on the database within watchlist
     * - this code is executed at notification alert times given within app settings
     */
    private async void Refresh(object state)
    {
        List<Stock> watchlist = await App.StockRepo.Get_Stock_Watchlist(true);

        if (watchlist.Count != 0) /* if watchlist is not empty */
        {
            await App.StockRepo.Update_Watchlist(true);
        }

        List<Stock> watchlist_updated = await App.StockRepo.Get_Stock_Watchlist(true);

        if (watchlist_updated.Count != 0) /* if local push notification needs to be created */
        {
            bool market_open = false;
            for (int i = 0; i < watchlist_updated.Count; i++) /* loop through the watchlist */
            {
                if (watchlist[i].ticker_price != watchlist_updated[i].ticker_price) /* if ticker prices are the same in previous watchlist and updated watchlist; then stockmarket is closed and send no alert */
                {
                    market_open = true;
                    break;
                }
            }

            Gather_Threshold_Stocks();

            if (market_open) /* if the stock market is open; send local push notification */
            {
                Gather_Threshold_Stocks();
            }
        }
    }

    /* finds the stocks from watchlist that meet the requirements for a local push notification */
    private async void Gather_Threshold_Stocks()
    {
        /* access settings variables */
        Settings local_settings = new Settings();
        double value_percent_change_threshold = local_settings.value_percent_change; /* percent change of a stock to receive a push notification */

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