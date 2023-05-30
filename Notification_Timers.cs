using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using StockTracker.Model;

namespace StockTracker;

[Service]
public class Notification_Timers : Service
{
    private Timer timer1;
    private Timer timer2;
    private Timer timer3;

    private Intent timer_service;

    public Notification_Timers()
	{
        timer1 = new Timer(Refresh); 
        timer2 = new Timer(Refresh);
        timer3 = new Timer(Refresh);

        timer_service = new Intent(MainActivity.ActivityCurrent, typeof(Notification_Timers));
    }

    public override IBinder OnBind(Intent intent)
    {
        throw new NotImplementedException();
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        if (intent.Action == "START_SERVICE")
        {
            RegisterNotification();//Proceed to notify
            Create_Timers(); /* create timers from settings */
        }
        else if (intent.Action == "STOP_SERVICE")
        {
            Create_Timers(); /* turns off timers */
            StopForeground(true);//Stop the service
            StopSelfResult(startId);
        }

        return StartCommandResult.NotSticky;
    }

    /* start the foreground service */
    public void Start()
    {
        timer_service.SetAction("START_SERVICE");
        MainActivity.ActivityCurrent.StartService(timer_service);
    }

    /* ends the foreground service */
    public void Stop()
    {
        timer_service.SetAction("STOP_SERVICE");
        MainActivity.ActivityCurrent.StartService(timer_service);
    }

    /* creates notification for the foreground service */
    private void RegisterNotification()
    {
        NotificationChannel channel = new NotificationChannel("ForegroundService", "ServiceNotification", NotificationImportance.None);
        NotificationManager manager = (NotificationManager)MainActivity.ActivityCurrent.GetSystemService(Context.NotificationService);
        manager.CreateNotificationChannel(channel);

        Notification notification = new Notification.Builder(this, "ForegroundService")
           .SetSmallIcon(Resource.Drawable.man_on_graph_light)
           .SetOngoing(false)
           .Build();

        StartForeground(1, notification);
    }

    /* prepares timers */
    public void Create_Timers()
    {
        Settings settings = new Settings();

        bool notifications_toggle = settings.notifications;

        if (notifications_toggle)  /* if notifications are toggled on */
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
                timer1.Change(ms_until_notification_time, 86400000); /* 86,400,000 ms = 1 day */
                timer2.Change(Timeout.Infinite, 86400000);
                timer3.Change(Timeout.Infinite, 86400000);                
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

                /* timer 2 setup */
                DateTime notification_time_hours2 = DateTime.Today.AddHours(hours2);
                DateTime notification_time_total2 = notification_time_hours2.AddMinutes(mins2);

                if (current_time > notification_time_total2) /* if already past notification time for that day */
                {
                    notification_time_total2 = notification_time_total2.AddDays(1.0);
                }

                int ms_until_notification_time2 = (int)((notification_time_total2 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time1, 86400000); /* 86,400,000 ms = 1 day */
                timer2.Change(ms_until_notification_time2, 86400000); /* 86,400,000 ms = 1 day */
                timer3.Change(Timeout.Infinite, 86400000); 
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

                /* timer 2 setup */
                DateTime notification_time_hours2 = DateTime.Today.AddHours(hours2);
                DateTime notification_time_total2 = notification_time_hours2.AddMinutes(mins2);

                if (current_time > notification_time_total2) /* if already past notification time for that day */
                {
                    notification_time_total2 = notification_time_total2.AddDays(1.0);
                }

                int ms_until_notification_time2 = (int)((notification_time_total2 - current_time).TotalMilliseconds);

                /* timer 3 setup */
                DateTime notification_time_hours3 = DateTime.Today.AddHours(hours3);
                DateTime notification_time_total3 = notification_time_hours3.AddMinutes(mins3);

                if (current_time > notification_time_total3) /* if already past notification time for that day */
                {
                    notification_time_total3 = notification_time_total3.AddDays(1.0);
                }

                int ms_until_notification_time3 = (int)((notification_time_total3 - current_time).TotalMilliseconds);

                /* set timer to elapse only once at the notification time */
                timer1.Change(ms_until_notification_time1, 86400000); /* 86,400,000 ms = 1 day */
                timer2.Change(ms_until_notification_time2, 86400000); /* 86,400,000 ms = 1 day */
                timer3.Change(ms_until_notification_time3, 86400000); /* 86,400,000 ms = 1 day */
            }
        }
        else /* else notifications are toggled off */
        {
            /* set timers to never go off */
            timer1.Change(Timeout.Infinite, Timeout.Infinite); 
            timer2.Change(Timeout.Infinite, Timeout.Infinite);
            timer3.Change(Timeout.Infinite, Timeout.Infinite); 
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

            List<Stock> watchlist_updated = await App.StockRepo.Get_Stock_Watchlist(true);

            if (watchlist_updated[0].was_market_open) /* prepares notification only if the stock market is open */
            {
                Gather_Threshold_Stocks();
            }

            Gather_Threshold_Stocks(); // USED FOR TESTING; SENDS NOTIFICATIONS EVEN WHEN STOCK MARKET IS CLOSED
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
                double temp_dollar_change = threshold_list[i].ticker_dollar_day_change;
                double temp_percent_change = threshold_list[i].ticker_percent_day_change;
                double temp_price = threshold_list[i].ticker_price;

                if (i == threshold_list.Count - 1) /* if last stock on the threshold list */
                {
                    notification_description = $"{notification_description}{threshold_list[i].ticker_name} is up ${String.Format("{0:0.00}", temp_dollar_change)} ({String.Format("{0:0.##}", temp_percent_change)}%) (${String.Format("{0:0.00}", temp_price)})";
                    break;
                }

                notification_description = $"{notification_description}{threshold_list[i].ticker_name} is up ${String.Format("{0:0.00}", temp_dollar_change)} ({String.Format("{0:0.##}", temp_percent_change)}%) (${String.Format("{0:0.00}", temp_price)})\n";
            }

            var notification_alert = new NotificationRequest
            {
                NotificationId = 2,
                Title = "Bull Stocks",
                Subtitle = "Stock Threshold Alert",
                Description = notification_description,
                BadgeNumber = 2
            };

            LocalNotificationCenter.Current.Show(notification_alert);
        }
        else /* else list of negative stocks past threshold */
        {
            string notification_description = "";

            /* fill lists for notification display */
            for (int i = 0; i < threshold_list.Count; i++)
            {
                double temp_dollar_change = threshold_list[i].ticker_dollar_day_change;
                double temp_percent_change = threshold_list[i].ticker_percent_day_change;
                double temp_price = threshold_list[i].ticker_price;

                double abs_dollar_change = Math.Abs(temp_dollar_change);
                double abs_percent_change = Math.Abs(temp_percent_change);

                if (i == threshold_list.Count - 1) /* if last stock on the threshold list */
                {
                    notification_description = $"{notification_description}{threshold_list[i].ticker_name} is down -${String.Format("{0:0.00}", abs_dollar_change)} (-{String.Format("{0:0.##}", abs_percent_change)}%) (${String.Format("{0:0.00}", temp_price)})";
                    break;
                }

                notification_description = $"{notification_description}{threshold_list[i].ticker_name} is down -${String.Format("{0:0.00}", abs_dollar_change)} (-{String.Format("{0:0.##}", abs_percent_change)}%) (${String.Format("{0:0.00}", temp_price)})\n";
            }

            var notification_alert = new NotificationRequest
            {
                NotificationId = 3,
                Title = "Bear Stocks",
                Subtitle = "Stock Threshold Alert",
                Description = notification_description,
                BadgeNumber = 3
            };

            LocalNotificationCenter.Current.Show(notification_alert);
        }
    }
}