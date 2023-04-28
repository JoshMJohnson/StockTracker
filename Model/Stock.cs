namespace StockTracker.Model;

public class Stock
{
    public string company_name {  get; set; }
    public string ticker_name { get; set; }
    public string ticker_price { get; set; }
    public string ticker_dollar_day_change { get; set; }
    public string ticker_percent_day_change { get; set; }
    
}