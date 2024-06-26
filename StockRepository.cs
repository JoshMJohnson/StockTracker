using StockTracker.Model;
using SQLite;
using System.Net;

namespace StockTracker;

public class StockRepository
{
	public string _dbpath;
    public bool api_limit_reached;
    public bool stock_not_found;

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

        api_limit_reached = false;
        stock_not_found = false;

        try
        {
            await Init_Database();

            /* call api to retrieve stock ticker data */
            string api_key = ""; /* TODO each user of the app should have a unique api key to stay under the GET Request limits from Twelve Data */
            string download_url = $"https://api.twelvedata.com/quote?symbol={stock_ticker}&apikey={api_key}";

            WebClient wc = new WebClient();
            var response = wc.DownloadString(download_url);
            response = response.Remove(0, 1);

            string[] current_stock_data_array = response.Split("\"");
            string api_error_check = current_stock_data_array[2];

            if (api_error_check == ":429,") /* if api limit reached for the day */
            {
                api_limit_reached = true;
            }
            else if (api_error_check == ":400,") /* if stock does not exist on market */
            {
                stock_not_found = true;
            }
            else /* stock found and api limit not reached */
            {
                /* transform api request to array of data */
                string current_stock_ticker = current_stock_data_array[3]; /* get ticker symbol */
                string current_company_name = current_stock_data_array[7]; /* get company name */
                string current_stock_price_change_string = current_stock_data_array[53]; /* get stock price change */
                string current_stock_prev_close_price_string = current_stock_data_array[49]; /* get stock price */
                string current_stock_percent_change_string = current_stock_data_array[57]; /* get stock percent change */
                string market_open_string = current_stock_data_array[64]; /* if market open = true; else = false */

                /* stock price dollar change round to two decimal points */
                double current_stock_price_change = double.Parse(current_stock_price_change_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price_change_rounded = Math.Truncate(current_stock_price_change * 100) / 100;

                /* stock price round to two decimal points */
                double current_stock_prev_closed_price = double.Parse(current_stock_prev_close_price_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price = current_stock_prev_closed_price + current_stock_price_change;
                current_stock_price = Math.Truncate(current_stock_price * 100) / 100;

                /* percent change format to two decimal points */
                double current_stock_percent_change = double.Parse(current_stock_percent_change_string, System.Globalization.CultureInfo.InvariantCulture);
                current_stock_percent_change = Math.Truncate(current_stock_percent_change * 100) / 100;

                bool market_open = true;
                if (market_open_string.Equals(":false,"))
                {
                    market_open = false;
                }

                Stock stock = new Stock
                {
                    ticker_name = current_stock_ticker,
                    company_name = current_company_name,
                    ticker_price = current_stock_price,
                    ticker_dollar_day_change = current_stock_price_change_rounded,
                    ticker_percent_day_change = current_stock_percent_change,
                    was_market_open = market_open
                };

                int result = await conn.InsertAsync(stock);

                StatusMessage = string.Format("{0} stock added (Ticker: {1})", result, stock_ticker);
            }
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
        ArgumentNullException.ThrowIfNull(sort_alpha, nameof(sort_alpha));

        try
        {
            await Init_Database();

            /* loop through watchlist database and retrieve all stock tickers */
            List<Stock> watchlist = await Get_Stock_Watchlist(sort_alpha);

            /* connect to stock market through Twelve Data API and update watchlist database */
            string api_key = "ef193f533d7c4521ab889fb23307a123"; /* account api key for Twelve Data API */

            for (int i = 0; i < watchlist.Count; i++)
            {
                if (i % 8 == 0 && i != 0) /* if reached api GET Request limit for a minute */
                {
                    await Task.Delay(61000); /* delay 1 minute and 1 second */
                }

                /* call api to retrieve stock ticker data */
                string download_url = $"https://api.twelvedata.com/quote?symbol={watchlist[i].ticker_name}&apikey={api_key}";
                WebClient wc = new WebClient();
                var response = wc.DownloadString(download_url);
                response = response.Remove(0, 1);
                string[] request_array = response.Split("},");

                /* transform api request to array of data */
                string current_request = request_array[0];
                string[] current_stock_data_array = current_request.Split("\"");
                string api_error_check = current_stock_data_array[2];

                if (api_error_check == ":400,") /* if stock symbol removed from stock market */
                {
                    Stock removing_stock = await conn.FindAsync<Stock>(watchlist[i].ticker_name);
                    await conn.DeleteAsync(removing_stock);
                    continue;
                }

                string current_stock_ticker = current_stock_data_array[3]; /* get ticker symbol */
                string current_company_name = current_stock_data_array[7]; /* get company name */
                string current_stock_price_change_string = current_stock_data_array[53]; /* get stock price change */
                string current_stock_prev_close_price_string = current_stock_data_array[49]; /* get stock price */
                string current_stock_percent_change_string = current_stock_data_array[57]; /* get stock percent change */
                string market_open_string = current_stock_data_array[64]; /* if market open = true; else = false */

                /* stock price dollar change round to two decimal points */
                double current_stock_price_change = double.Parse(current_stock_price_change_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price_change_rounded = Math.Truncate(current_stock_price_change * 100) / 100;

                /* stock price round to two decimal points */
                double current_stock_prev_closed_price = double.Parse(current_stock_prev_close_price_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price = current_stock_prev_closed_price + current_stock_price_change;
                current_stock_price = Math.Truncate(current_stock_price * 100) / 100;

                /* percent change format to two decimal points */
                double current_stock_percent_change = double.Parse(current_stock_percent_change_string, System.Globalization.CultureInfo.InvariantCulture);
                current_stock_percent_change = Math.Truncate(current_stock_percent_change * 100) / 100;

                bool market_open = true;
                if (market_open_string.Equals(":false,"))
                {
                    market_open = false;
                }

                /* update stock database */
                Stock updating_stock = await conn.FindAsync<Stock>(current_stock_ticker);
                updating_stock.company_name = current_company_name;
                updating_stock.ticker_price = current_stock_price;
                updating_stock.ticker_dollar_day_change = current_stock_price_change_rounded;
                updating_stock.ticker_percent_day_change = current_stock_percent_change;
                updating_stock.was_market_open = market_open;
                await conn.UpdateAsync(updating_stock);
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
        ArgumentNullException.ThrowIfNull(sort_alpha, nameof(sort_alpha));

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
                watchlist = watchlist.OrderBy(stock => stock.ticker_percent_day_change).Reverse().ToList();
            }

            return watchlist;
        } 
        catch(Exception ex) 
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Stock>();
    }
}
