using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;
using System.Numerics;

namespace StockTracker;

public partial class Settings : ContentPage
{
    /* app setting variables */
    public int value_toggle_notification = 1;
    public double value_percent_change = 5;
    public int index_notify_type = 2;
    public string value_tod1 = "";
    public string value_tod2 = "";

    public Settings()
	{
		InitializeComponent();

        value_percent_change_label.Text = value_percent_change.ToString(); /* display initial percent change value label */
    }

    /* turns on and off push notifications */
    private async void Notification_Toggle(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", "e", "OK");        
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

    /* manages notification types (threshold/1 t.o.d./2 t.o.d.) */
    private void Notify_Type_Change(object sender, EventArgs e)
    {
        if (notify_type_picker != null)
        {
            /* get index chosen for notification type dropdown input */
            string value_notify_type_string = notify_type_picker.SelectedIndex.ToString();
            index_notify_type = Int32.Parse(value_notify_type_string);
        }

        if (tod1_label != null && tod2_label != null
                && tod1_selector != null && tod2_selector != null) /* display/hide T.O.D. labels and selectors based on notify type chosen */
        {
            if (index_notify_type == 0) /* if notify type is set to threshold (index 0) */
            {
                tod1_label.IsVisible = false;
                tod1_selector.IsVisible = false;
                tod2_label.IsVisible = false;
                tod2_selector.IsVisible = false;
            }
            else if (index_notify_type == 1) /* else if notify type is set to 1 T.O.D. (index 1) */
            {
                tod1_label.IsVisible = true;
                tod1_selector.IsVisible = true;
                tod2_label.IsVisible = false;
                tod2_selector.IsVisible = false;
            }
            else /* else notify type is set to 2 T.O.D. (index 2) */
            {
                tod1_label.IsVisible = true;
                tod1_selector.IsVisible = true;
                tod2_label.IsVisible = true;
                tod2_selector.IsVisible = true;
            }
        }
    }

    /* if send notifications at a given time; then choose that time; else display 'N/A' */
    private async void Time_Of_Day_Change(object sender, EventArgs e)
    {
        await DisplayAlert("Pop up", value_tod1, "OK");                
    }
}