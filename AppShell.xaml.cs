namespace StockTracker;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute("Watchlist", typeof(Watchlist));
	}
}
