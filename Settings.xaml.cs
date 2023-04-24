namespace StockTracker;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
	}

    /* turns on and off push notifications */
    private async void Notification_Toggle(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "Notification_Toggle", "OK");
    }

    /* changes value of the percent change a stock has to make for a push notification */
    private async void Stock_Change_Notification_Setting(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "Stock_Change_Notification_Setting", "OK");
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
        await DisplayAlert("Pop up", "Time_Of_Day_Change1", "OK");
    }
}