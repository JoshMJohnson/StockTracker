using SQLite;
using StockTracker.Model;

namespace StockTracker;

public partial class Watchlist : ContentPage
{
    SQLiteConnection conn;

    public Watchlist()
	{
		InitializeComponent();

        /* retrieves path to database file */
        string path = FileSystem.Current.AppDataDirectory;
        string filename = Path.Combine(path, "Stock_App_Database.db");

        /* connect to database */
        conn = new SQLiteConnection(filename);



        /* TODO: if collection is empty, set VerticalOptions="Center" for collectionview */
	}

    /* handles the adding and removing buttons on watchlist page */
    private async void Create_Stock(object sender, EventArgs e)
    {
        Button btn = (Button) sender; /* identifies the button that directed to this function */

        if (btn.Text == "Add") /* if add button was clicked on */
        {
            await DisplayAlert("alert", "Add button was clicked", "yayayaya");
        } else if (btn.Text == "Remove") /* else if remove button was clicked */
        {
            await DisplayAlert("alert", "Remove button was clicked", "yayayaya");
        }
    }

    /* adds a stock to the database */
    public int Add_Stock(Stock stock)
    {
        int result = conn.Insert(stock);
        return result;
    }

    /* removes a stock from the database */ 
    public int Remove_Stock(Stock stock)
    {
        int result = conn.Delete(stock);
        return result;
    }

    /* returns a list of all the stocks within the database */
    public List<Stock> Get_Stock_List() 
    {
        List<Stock> watchlist = conn.Table<Stock>().ToList();
        return watchlist;
    }
}