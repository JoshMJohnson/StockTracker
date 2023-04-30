using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;
using System.Numerics;

namespace StockTracker;

public partial class Settings : ContentPage
{
    /* app setting variables */
    public bool notifications { get; set; } /* saves value for notifications being on */
    public double value_percent_change { get; set; } /* percent change of a stock to receive a push notification */
    public int index_notify_type { get; set; } /* 0 = threshold; 1 = one T.O.D.; 2 = two T.O.D. */
    public string value_tod1 { get; set; } /* notification time for T.O.D. #1 */
    public string value_tod2 { get; set; } /* notification time for T.O.D. #2 */

    public Settings()
	{
		InitializeComponent();

        /* preference key creation */
        notifications = Preferences.Get("NotificationToggle", true);
        value_percent_change = Preferences.Get("ValuePercentChange", 5.0);
        index_notify_type = Preferences.Get("IndexNotifyType", 2);
        value_tod1 = Preferences.Get("ValueTOD1", "10:00");
        value_tod2 = Preferences.Get("ValueTOD2", "12:30");

        /* notification initial display */
        notification_on.IsChecked = notifications;
        notification_off.IsChecked = !notifications;

        /* percent change initial display */
        percent_change_slider.Value = value_percent_change; /* sets the starting value of the slider for percent change */
        value_percent_change_label.Text = value_percent_change.ToString(); /* display initial percent change value label */

        /* notify type initial display */
        /* tod1 initial display */
        /* tod2 initial display */
    }

    /* changes value of notifications when switched */
    private void Check_Notification_Change()
    {
        bool presave_notification_toggle = notification_on.IsChecked; /* value before save */
        bool saved_notification_toggle = Preferences.Get("NotificationToggle", true);

        if (presave_notification_toggle == saved_notification_toggle)  { return; } /* if not change in value since last save */

        Preferences.Set("NotificationToggle", presave_notification_toggle);

        bool updated_value = Preferences.Get("NotificationToggle", true);
        notification_on.IsChecked = updated_value;
    }

    /* handles action when slider for percent threshold is changed */
    private void Percent_Notify_Change()
    {
        double presave_value_percent_change = percent_change_slider.Value;
        double saved_value_percent_change = Preferences.Get("ValuePercentChange", 5.0);

        if (presave_value_percent_change == saved_value_percent_change) { return; } /* if not change in value since last save */

        presave_value_percent_change = Math.Round(value_percent_change, 2); /* take value from slider and format to two decimal points */
        Preferences.Set("ValuePercentChange", presave_value_percent_change);

        /* convert to string value for label display */
        double updated_value = Preferences.Get("ValuePercentChange", 5.0);
        value_percent_change_label.Text = updated_value.ToString();               
    }

    /* updates display of value for unsaved percent change slider */
    private void Percent_Notify_Change_Display(object sender, EventArgs e)
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

    private async void Save_Button_Clicked(object sender, EventArgs e)
    {
        Check_Notification_Change();
        Percent_Notify_Change();

        await DisplayAlert("Settings", "Settings Saved", "Cool");
    }
}