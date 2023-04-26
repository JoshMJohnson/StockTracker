using System.Numerics;

namespace StockTracker;

public partial class Settings : ContentPage
{
    /* app setting variables */
    
    /*
    public static string notification_toggle = "On";
    public static double percent_change = 5.0;
    public static int notify_type = 0;
    public static string tod1 = "10 am";
    public static string tod2 = "2 pm";
    */

	public Settings()
	{
		InitializeComponent();
	}

    /* turns on and off push notifications */
    private async void Notification_Toggle(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "Notification_Toggle", "OK");

        /* 
         * if (notification_toggle == "On") notification_toggle = "Off";
         * else notification_toggle = "On";
         */


    }

    /* handles action when percent change threshold for push notification is changed */
    private async void Percent_Notify_Change(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "Percent_Notify_Change", "OK");
    }

    /* toggle between sending push notifications
     *  - option 1: send notifications at a given time (ex: 10am CST) 
     *  - option 2: send notifications at the exact time the stock reaches the change percentage (if overnight then send on open) */
    private async void Notify_Type_Change(object sender, EventArgs e)
    { 
        await DisplayAlert("Pop up", "Notify_Type_Change", "OK");
    }

    /* if send notifications at a given time; then choose that time; else display 'N/A' */
    private async void Time_Of_Day_Change(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "Time_Of_Day_Change", "OK");
    }
}