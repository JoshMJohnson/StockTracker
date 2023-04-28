namespace StockTracker;

public class Stock : ContentPage
{
	public string ticker_name { get; set; }
	public string ticker_price { get; set; }

	/* add stock to the watchlist database */
	public void AddStock()
	{
		var stock = new Stock();
		stock.ticker_name = ticker_name;
		stock.ticker_price = ticker_price;
	}
}