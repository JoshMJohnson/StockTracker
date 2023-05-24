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
            string api_key = "1a5736c710a640679295533e0c6a53ca";
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
                string current_stock_open_price_string = current_stock_data_array[29]; /* get stock price */
                string current_stock_percent_change_string = current_stock_data_array[57]; /* get stock percent change */

                /* stock price dollar change round to two decimal points */
                double current_stock_price_change = double.Parse(current_stock_price_change_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price_change_rounded = Math.Truncate(current_stock_price_change * 100) / 100;

                /* stock price round to two decimal points */
                double current_stock_open_price = double.Parse(current_stock_open_price_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price = current_stock_open_price + current_stock_price_change;
                current_stock_price = Math.Truncate(current_stock_price * 100) / 100;

                /* percent change format to two decimal points */
                double current_stock_percent_change = double.Parse(current_stock_percent_change_string, System.Globalization.CultureInfo.InvariantCulture);
                current_stock_percent_change = Math.Truncate(current_stock_percent_change * 100) / 100;

                Stock stock = new Stock
                {
                    ticker_name = current_stock_ticker,
                    company_name = current_company_name,
                    ticker_price = current_stock_price,
                    ticker_dollar_day_change = current_stock_price_change_rounded,
                    ticker_percent_day_change = current_stock_percent_change
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
        string download_url = $"https://api.twelvedata.com/quote?symbol={ticker_api_call}&apikey={api_key}";

        WebClient wc = new WebClient();
        var response = wc.DownloadString(download_url);
        response = response.Remove(0, 1);
        string[] request_array = response.Split("},");

        /* update watchlist database with data from api */
        try
        {
            await Init_Database();

            /* update database for each stock on watchlist */
            for (int i = 0; i < watchlist.Count; i++)
            {
                /* transform api request to array of data */
                string current_request = request_array[i];
                string[] current_stock_data_array = current_request.Split("\"");

                string current_stock_ticker;
                string current_stock_price_change_string;
                string current_stock_open_price_string;
                string current_stock_percent_change_string;

                if (watchlist.Count == 1)
                {
                    current_stock_ticker = current_stock_data_array[3]; /* get ticker symbol */
                    current_stock_price_change_string = current_stock_data_array[53]; /* get stock price change */
                    current_stock_open_price_string = current_stock_data_array[29]; /* get stock price */
                    current_stock_percent_change_string = current_stock_data_array[57]; /* get stock percent change */
                }
                else
                {
                    current_stock_ticker = current_stock_data_array[1]; /* get ticker symbol */
                    current_stock_price_change_string = current_stock_data_array[55]; /* get stock price change */
                    current_stock_open_price_string = current_stock_data_array[31]; /* get stock price */
                    current_stock_percent_change_string = current_stock_data_array[59]; /* get stock percent change */
                }

                /* stock price dollar change round to two decimal points */
                double current_stock_price_change = double.Parse(current_stock_price_change_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price_change_rounded = Math.Truncate(current_stock_price_change * 100) / 100;

                /* stock price round to two decimal points */
                double current_stock_open_price = double.Parse(current_stock_open_price_string, System.Globalization.CultureInfo.InvariantCulture);
                double current_stock_price = current_stock_open_price + current_stock_price_change;
                current_stock_price = Math.Truncate(current_stock_price * 100) / 100;

                /* percent change format to two decimal points */
                double current_stock_percent_change = double.Parse(current_stock_percent_change_string, System.Globalization.CultureInfo.InvariantCulture);
                current_stock_percent_change = Math.Truncate(current_stock_percent_change * 100) / 100;

                /* locate and update stock info */
                Stock updating_stock = await conn.FindAsync<Stock>(current_stock_ticker);
                updating_stock.ticker_price = current_stock_price;
                updating_stock.ticker_dollar_day_change = current_stock_price_change_rounded;
                updating_stock.ticker_percent_day_change = current_stock_percent_change;

                /* update stock database - remove and add the stock being updated */
                await conn.DeleteAsync(updating_stock);
                await conn.InsertAsync(updating_stock);
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
                watchlist = watchlist.OrderBy(stock => stock.ticker_percent_day_change).Reverse().ToList();
            }

            return watchlist;
        } catch(Exception ex) 
        {
            StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
        }

        return new List<Stock>();
    }
}