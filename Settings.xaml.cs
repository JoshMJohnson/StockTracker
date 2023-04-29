using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;
using System.Numerics;

namespace StockTracker;

public partial class Settings : ContentPage
{
    /* app setting variables */
    public int value_toggle_notification = 1; /* 0 = off; 1 = on */
    public double value_percent_change = 5; /* percent change of a stock to receive a push notification */
    public int index_notify_type = 2; /* 0 = threshold; 1 = one T.O.D.; 2 = two T.O.D. */
    public string value_tod1 = ""; /* notification time for T.O.D. #1 */
    public string value_tod2 = ""; /* notification time for T.O.D. #2 */

    public Settings()
	{
		InitializeComponent();

        value_percent_change_label.Text = value_percent_change.ToString(); /* display initial percent change value label */
    }

    /* changes value of notifications when switched */
    private async void Notification_Toggle(object sender, EventArgs e)
    {
        if (notification_on != null && notification_off != null)
        {
            if (notification_on.IsChecked) /* if notifications are turned on */
            {
                value_toggle_notification = 1;
            }
            else if (notification_off.IsChecked) /* else notifications are turned off */
            {
                value_toggle_notification = 0;
            }

            await DisplayAlert("Pop up", value_toggle_notification.ToString(), "OK");
        }
    }

    /* handles action when slider for percent threshold is changed */
    private void Percent_Notify_Change(object sender, EventArgs e)
    {
        if (percent_change_slider != null)
        {
            /* take value from slider and format to two decimal points */
            value_percent_change = percent_change_slider.Value;
            value_percent_change = Math.Round(value_percent_change, 2);

            /* convert to string value for label display */
            value_percent_change_label.Text = value_percent_change.ToString();
        }        
    }

    /* manages notification type displays (threshold/1 t.o.d./2 t.o.d.) */
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

        Time_Of_Day_Change(sender, e);
    }

    /* if notify type involves T.O.D.; then retrieve that data */
    private async void Time_Of_Day_Change(object sender, EventArgs e)
    {
        if (tod1_selector != null && tod2_selector != null)
        {
            if (tod1_selector.IsVisible) /* if tod1 is visible */
            {
                var time1 = tod1_selector.Time;
                value_tod1 = time1.ToString();
            }
            else /* else tod1 is not visible */
            {
                value_tod1 = "N/A";
            }

            if (tod2_selector.IsVisible) /* if tod2 is visible */
            {
                var time2 = tod2_selector.Time;
                value_tod2 = time2.ToString();
            }
            else /* else tod2 is not visible */
            {
                value_tod2 = "N/A";
            }

            await DisplayAlert("Pop up", value_tod1, "OK");
            await DisplayAlert("Pop up", value_tod2, "OK");
        }
    }
}