using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Xaml;
using System.Numerics;

namespace StockTracker;

public partial class Settings : ContentPage
{
    /* app setting variables */
    public bool notifications { get; set; } /* saves value for notifications being on */
    public double value_percent_change { get; set; } /* percent change of a stock to receive a push notification */
    public int num_notification_times { get; set; } /* 0 = threshold; 1 = one T.O.D.; 2 = two T.O.D. */
    public string value_tod1 { get; set; } /* notification time for T.O.D. #1 */
    public string value_tod2 { get; set; } /* notification time for T.O.D. #2 */
    public string value_tod3 { get; set; } /* notification time for T.O.D. #3 */

    public Settings()
	{
		InitializeComponent();

        /* preference key creation */
        notifications = Preferences.Get("NotificationToggle", true);
        value_percent_change = Preferences.Get("ValuePercentChange", 5.0);
        num_notification_times = Preferences.Get("NumNotifications", 3);
        value_tod1 = Preferences.Get("ValueTOD1", "8:45");
        value_tod2 = Preferences.Get("ValueTOD2", "12:00");
        value_tod3 = Preferences.Get("ValueTOD3", "2:30");

        /* notification initial display */
        notification_on.IsChecked = notifications;
        notification_off.IsChecked = !notifications;

        /* percent change initial display */
        percent_change_slider.Value = value_percent_change; /* sets the starting value of the slider for percent change */
        value_percent_change_label.Text = value_percent_change.ToString(); /* display initial percent change value label */

        /* notify type initial display */
        notify_type_picker.SelectedIndex = num_notification_times;

        /* tod1 initial display */
        string[] updated_view_array = value_tod1.Split(':');
        string hours_string = updated_view_array[0];
        string mins_string = updated_view_array[1];
        int hours = int.Parse(hours_string);
        int mins = int.Parse(mins_string);

        TimeSpan temp_time = new TimeSpan(hours, mins, 0);
        tod1_selector.Time = temp_time;

        /* tod2 initial display */
        string[] updated_view_array2 = value_tod2.Split(':');
        string hours_string2 = updated_view_array2[0];
        string mins_string2 = updated_view_array2[1];
        int hours2 = int.Parse(hours_string2);
        int mins2 = int.Parse(mins_string2);

        TimeSpan temp_time2 = new TimeSpan(hours2, mins2, 0);
        tod2_selector.Time = temp_time2;

        /* tod3 initial display */
        string[] updated_view_array3 = value_tod3.Split(':');
        string hours_string3 = updated_view_array3[0];
        string mins_string3 = updated_view_array3[1];
        int hours3 = int.Parse(hours_string3);
        int mins3 = int.Parse(mins_string3);

        TimeSpan temp_time3 = new TimeSpan(hours3, mins3, 0);
        tod3_selector.Time = temp_time3;
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
        notification_off.IsChecked = !updated_value;
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
    private void Notify_Type_Change_Display(object sender, EventArgs e)
    {
        if (notify_type_picker != null)
        {
            num_notification_times = notify_type_picker.SelectedIndex;

            if (tod1_label != null && tod2_label != null
                && tod1_selector != null && tod2_selector != null) /* display/hide T.O.D. labels and selectors based on notify type chosen */
            {
                if (num_notification_times == 0) /* if notify type is set to threshold (index 0) */
                {
                    tod1_label.IsVisible = true;
                    tod1_selector.IsVisible = true;
                    tod2_label.IsVisible = false;
                    tod2_selector.IsVisible = false;
                    tod3_label.IsVisible = false;
                    tod3_selector.IsVisible = false;
                }
                else if (num_notification_times == 1) /* else if notify type is set to 1 T.O.D. (index 1) */
                {
                    tod1_label.IsVisible = true;
                    tod1_selector.IsVisible = true;
                    tod2_label.IsVisible = true;
                    tod2_selector.IsVisible = true;
                    tod3_label.IsVisible = false;
                    tod3_selector.IsVisible = false;
                }
                else /* else notify type is set to 2 T.O.D. (index 2) */
                {
                    tod1_label.IsVisible = true;
                    tod1_selector.IsVisible = true;
                    tod2_label.IsVisible = true;
                    tod2_selector.IsVisible = true;
                    tod3_label.IsVisible = true;
                    tod3_selector.IsVisible = true;
                }
            }            
        }
    }

    /* updates display of value for unsaved notify type */
    private void Notify_Type_Change()
    {
        int presave_num_notifications = notify_type_picker.SelectedIndex;
        int saved_num_notifications = Preferences.Get("NumNotifications", 2);

        if (presave_num_notifications == saved_num_notifications) { return; } /* if not change in value since last save */
                
        Preferences.Set("NumNotifications", presave_num_notifications);

        int updated_value = Preferences.Get("NumNotifications", 2);
        notify_type_picker.SelectedIndex = updated_value;
    }

    /* manages TOD1 and TOD2 local storage */
    private void Time_Of_Day_Change()
    {
        var saved_time1_var = tod1_selector.Time;
        string saved_time1_string = saved_time1_var.ToString();
        string prev_time1_string = Preferences.Get("ValueTOD1", null);

        var saved_time2_var = tod2_selector.Time;
        string saved_time2_string = saved_time2_var.ToString();
        string prev_time2_string = Preferences.Get("ValueTOD2", null);

        var saved_time3_var = tod3_selector.Time;
        string saved_time3_string = saved_time3_var.ToString();
        string prev_time3_string = Preferences.Get("ValueTOD3", null);

        if (prev_time1_string == saved_time1_string
                && prev_time2_string == saved_time2_string
                && prev_time3_string == saved_time3_string) { return; } /* if not change in value since last save */

        if (prev_time1_string != saved_time1_string) /* if time1 changes with save */
        {
            /* change display time */
            if (tod1_selector.IsVisible) /* if tod1 is visible */
            {
                Preferences.Set("ValueTOD1", saved_time1_string);
            }
        }

        if (prev_time2_string != saved_time2_string) /* if time2 changes with save */
        {
            /* change display time */
            if (tod2_selector.IsVisible) /* if tod2 is visible */
            {
                Preferences.Set("ValueTOD2", saved_time2_string);
            }
        }

        if (prev_time3_string != saved_time3_string) /* if time3 changes with save */
        {
            /* change display time */
            if (tod3_selector.IsVisible) /* if tod3 is visible */
            {
                Preferences.Set("ValueTOD3", saved_time3_string);
            }
        }
    }

    /* action when the save button is clicked in settings */
    private async void Save_Button_Clicked(object sender, EventArgs e)
    {
        Check_Notification_Change();
        Percent_Notify_Change();
        Notify_Type_Change();
        Time_Of_Day_Change();

        await DisplayAlert("Settings", "Saved", "ok");
    }
}