using StockTracker.Model;
using SQLite;
using System.Diagnostics;
using System.Net;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    /* clears the watchlist; deletes all stocks */
    public async Task Clear_Watchlist()
    {
        try
        {
            await Init_Database();

            await conn.DeleteAllAsync<Stock>();
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }
    }

    /* updates the watchlist data from the stock market */
    public async Task Update_Watchlist(bool sort_alpha)
    {
        /* loop through watchlist database and retrieve all stock tickers */
        List<Stock> watchlist = await Get_Stock_Watchlist(sort_alpha);
        List<string> ticker_list = new List<string>();

        /* build list of stock tickers from watchlist */
        for (int i = 0; i < watchlist.Count; i++)
        {
            string current_ticker = watchlist[i].ticker_name;
            ticker_list.Add(current_ticker);
        }

        /* call api to retrieve stock ticker data */
        string ticker_api_call = string.Join(",", ticker_list);
        ticker_api_call = ticker_api_call.TrimEnd(',');

        string api_key = "1a5736c710a640679295533e0c6a53ca";
        string download_url = $"https://api.twelvedata.com/price?symbol={ticker_api_call}&apikey={api_key}";

        WebClient wc = new WebClient();
        var response = wc.DownloadString(download_url);
        string[] request_array = response.Split(',');

        /* update watchlist database with data from api */
        try
        {
            await Init_Database();

            /* update database for each stock on watchlist */
            for (int i = 0; i < watchlist.Count; i++)
            {
                string current_stock_ticker = watchlist[i].ticker_name;

                /* update stock price - convert to double with two decimal points */
                string current_request = request_array[i];

                var fetched_data = current_request.Split(":");
                var fetched_price_string = fetched_data[2];
                fetched_price_string = fetched_price_string.Replace("\"", "");
                fetched_price_string = fetched_price_string.Replace("}", "");
                double fetched_price = double.Parse(fetched_price_string, System.Globalization.CultureInfo.InvariantCulture);
                fetched_price = Math.Truncate(fetched_price * 100) / 100;

                Debug.WriteLine("stock ticker: " + current_stock_ticker);
                Debug.WriteLine("fetched price: " + fetched_price);
                
                Stock stock = new Stock
                {
                    ticker_name = current_stock_ticker,
                    company_name = "The " + current_stock_ticker + " company",
                    ticker_price = fetched_price,
                    ticker_dollar_day_change = -1,
                    ticker_percent_day_change = -1
                };

                await conn.UpdateAsync(stock);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = string.Format("Failed to update. Error: {0}", ex.Message);
        }
    }

    /* returns a list of all the stocks within the database */
    public async Task<List<Stock>> Get_Stock_Watchlist(bool sort_alpha)
    {
        try
        {
            await Init_Database();

            List<Stock> watchlist = await conn.Table<Stock>().ToListAsync();

            if (sort_alpha) /* set sort of watchlist to be alphabetical */
            {
                watchlist = watchlist.OrderBy(stock => stock.ticker_name).ToList();
            }
            else /* set sort of watchlist to be by stock price */
            {
                watchlist = watchlist.OrderBy(stock => stock.ticker_price).Reverse().ToList();
            }

            return watchlist;
        } catch(Exception ex) 
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Stock>();
    }
}