namespace StockTracker;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(Watchlist), typeof(Watchlist));
		Routing.RegisterRoute(nameof(Settings), typeof(Settings));
	}
}
