using StockTracker.Model;
using SQLite;
using System.Diagnostics;

namespace StockTracker;

public class StockRepository
{
	string _dbpath;
	
	public string StatusMessage { get; set; }

	private static SQLiteAsyncConnection conn;

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

        try
        {
            await Init_Database();

            Stock stock = new Stock
            {
                ticker_name = stock_ticker,
                company_name = "The " + stock_ticker + " company",
                ticker_price = -1,
                ticker_dollar_day_change = -1,
                ticker_percent_day_change = -1
            };

            int result = await conn.InsertAsync(stock);

            StatusMessage = string.Format("{0} stock added (Ticker: {1})", result, stock_ticker);
        } catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to add {0}. Error: {1}", stock_ticker, ex.Message);
        }        
    }

    /* removes a stock from the database */
    public async Task Remove_Stock(string stock_ticker)
    {
        ArgumentNullException.ThrowIfNull(stock_ticker, nameof(stock_ticker));

        try
        {
            await Init_Database();

            Stock removing_stock = await conn.FindAsync<Stock>(stock_ticker);

            await conn.DeleteAsync(removing_stock);
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to remove {0}. Error: {1}", stock_ticker, ex.Message);
        }
    }

    /* returns a list of all the stocks within the database */
    public async Task<List<Stock>> Get_Stock_Watchlist()
    {
        try
        {
            await Init_Database();

            return await conn.Table<Stock>().ToListAsync();
        } catch(Exception ex) 
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Stock>();
    }
}