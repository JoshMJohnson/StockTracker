using StockTracker.Model;
using SQLite;
using System.Diagnostics;

namespace StockTracker;

public class StockRepository
{
	string _dbpath;
	
	public string StatusMessage { get; set; }

	private SQLiteAsyncConnection conn;

    /* initializes the database */
    private async Task Init_Database()
    {
        if (conn != null) { return; }

        /* connect to database */
        conn = new SQLiteAsyncConnection(_dbpath);

        await conn.CreateTableAsync<Stock>();
    }

    public StockRepository(string dbPath)
    {
        _dbpath = dbPath;
    }

    /* adds a stock to the database */
    public async Task Add_Stock(string stock_ticker)
    {
        ArgumentNullException.ThrowIfNull(stock_ticker, nameof(stock_ticker));

        int result = 0;

        try
        {
            await Init_Database();

            var stock = new Stock
            {
                ticker_name = stock_ticker,
                company_name = "",
                ticker_price = -1,
                ticker_dollar_day_change = -1,
                ticker_percent_day_change = -1
            };

            Debug.WriteLine("****************ticker added***: " + stock.ticker_name);

            result = await conn.InsertAsync(stock);

            Debug.WriteLine("****************ticker added***22: " + stock.ticker_name);

            StatusMessage = string.Format("{0} stock added (Ticker: {1})", result, stock_ticker);
        } catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", stock_ticker, ex.Message);
        }        
    }

    /* removes a stock from the database *//*
    public int Remove_Stock(Stock stock)
    {
        int result = conn.Delete(stock);
        return result;
    }*/

    /* returns a list of all the stocks within the database */
    public async Task<List<Stock>> Get_Stock_Watchlist()
    {
        try
        {
            await Init_Database();

            Debug.WriteLine("****database location: " + _dbpath);

            return await conn.Table<Stock>().ToListAsync();
        } catch(Exception ex) 
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Stock>();
    }
}