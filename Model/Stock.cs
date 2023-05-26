using System.ComponentModel.DataAnnotations;
using SQLite;

namespace StockTracker.Model;

[Table("Stocks")]
public class Stock
{
    [PrimaryKey, Unique]
    public string ticker_name { get; set; }

    [Unique]
    public string company_name {  get; set; }

    public double ticker_price { get; set; }

    public double ticker_dollar_day_change { get; set; }

    public double ticker_percent_day_change { get; set; }

    public bool was_market_open { get; set; }
}