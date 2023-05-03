using System.ComponentModel.DataAnnotations;
using SQLite;

namespace StockTracker.Model;

[Table("Stocks")]
public class Stock
{
    [PrimaryKey, Unique]
    public string TickerName { get; set; }

    [Unique]
    public string CompanyName {  get; set; }

    public double TickerPrice { get; set; }

    public double TickerDollarDayChange { get; set; }

    public double TickerPercentDayChange { get; set; }
}