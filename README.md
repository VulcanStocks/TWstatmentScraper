# TWstatmentScraper
Welcome to the TWstatmentScraper C# web scraper! This scraper is designed to scrape financial statements from tradingview.com. Here is a brief overview of how to use it:

Getting Started
To use the scraper, you will need to first import the TWscraper namespace:

Copy code
using TWscraper;
Next, you will need to create a new Scraper object, passing in the type of financial statement you want to scrape (e.g. "balance", "income", "cash flow") and the ticker symbol for the company you are interested in (e.g. "NASDAQ-AAPL" for Apple).

Copy code
var incomeScraper = new Scraper("income", "NASDAQ-AAPL");
Scraping the Data
To scrape the data from the website, you will first need to load the HTML of the page you want to scrape. You can do this using the LoadHtmlAsync method:

Copy code
await incomeScraper.LoadHtmlAsync();
Once the HTML has been loaded, you can scrape the data using the ScrapeDataOnce method:

Copy code
incomeScraper.ScrapeDataOnce();
This will extract the relevant financial data from the HTML and store it in the Scraper object.

Saving the Data to a CSV File
To save the scraped data to a CSV file, you can use the ParseAndSaveToCsvAsync method, passing in the path to the file you want to create:

Copy code
await incomeScraper.ParseAndSaveToCsvAsync(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "data.csv");
This will save the scraped data to a CSV file in the parent directory of the current working directory, with the file name "data.csv".

Additional Notes
The Scraper object can be used to scrape multiple pages of data by calling the LoadHtmlAsync and ScrapeDataOnce methods multiple times.
The Scraper object also provides several other methods for accessing and manipulating the scraped data, such as GetData, ClearData, and SortData. Refer to the documentation for more information on these methods.
I hope this helps get you started with the TWscraper C# web scraper. Happy scraping!
