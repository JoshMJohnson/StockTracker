using System.Numerics;

namespace StockTracker;

public partial class Settings : ContentPage
{
    /* app setting variables */
    public int value_toggle_notification = 1;
    public double value_percent_change = 5;
    public int index_notify_type = 2;
    public string value_tod1;
    public string value_tod2;

    public Settings()
	{
		InitializeComponent();

        value_percent_change_label.Text = value_percent_change.ToString(); /* display initial percent change value label */
    }

    /* turns on and off push notifications */
    private async void Notification_Toggle(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "Notification_Toggle", "OK");
    }

    /* handles action when percent change threshold for push notification is changed */
    private void Percent_Notify_Change(object sender, EventArgs e)
    {
        if (percent_change_slider != null)
        {
            string value_percent_change_string = percent_change_slider.Value.ToString(); /* retrieve value from slider */

            /* take value from slider and convert to double value and assign to global variable */
            value_percent_change = double.Parse(value_percent_change_string, System.Globalization.CultureInfo.InvariantCulture);
            value_percent_change = Math.Round(value_percent_change, 2);

            /* convert back to string value for label display after setting value format */
            value_percent_change_string = value_percent_change.ToString();
            value_percent_change_label.Text = value_percent_change_string;
        }        
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