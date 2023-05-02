using SQLite;

namespace StockTracker;

public partial class Watchlist : ContentPage
{
	public Watchlist()
	{
		InitializeComponent();

        string path = FileSystem.Current.AppDataDirectory;

        string filename = Path.Combine(path, "Stock_Watchlist.db");
        SQLiteConnection conn = new SQLiteConnection(filename);

        /* TODO: if collection is empty, set VerticalOptions="Center" for collectionview */
	}

    /* adds a stock to the database */
    /*
    public int AddStock(string ticker)
    {

    }
    */

    /* removes a stock from the database */ 
    /*
    public int RemoveStock(string ticker)
    {

    }
    */
}