namespace StockTracker;

public partial class App : Application
{
	public static StockRepository StockRepo { get; private set; }

    public App(StockRepository repo)
	{
		InitializeComponent();

		MainPage = new AppShell();

        StockRepo = repo;
    }
}