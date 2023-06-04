# VSscraper

VSscraper is a .NET library for scraping financial data from TradingView and Wikipedia. It uses the HtmlAgilityPack and PuppeteerSharp libraries to parse HTML and interact with web pages.

## Features

- Scrape real-time price of a ticker from TradingView.
- Scrape full name of a ticker from TradingView.
- Scrape financial statements (income, balance, cash flow, ratios) from TradingView.
- Scrape introductory paragraph from Wikipedia.
- Save scraped data to CSV.

## Usage

First, initialize the `ScraperService` with the desired data type, ticker, and other options:

```csharp
ScraperService scraper = new ScraperService();
scraper.InitializeTW("income", "AAPL", true, true);
```

Then, load the HTML of the page:

```csharp
await scraper.LoadHtmlAsync();
```

Next, scrape the data:

```csharp
await scraper.ScrapeTWDataAsync();
```

Finally, parse the scraped data:

```csharp
await scraper.ParseStatmentAsync();
```

You can also save the scraped data to a CSV file:

```csharp
await scraper.SaveIncomeAsyncToCsv("path/to/your/file.csv");
```

## Dependencies

- HtmlAgilityPack
- PuppeteerSharp

## Installation

To use this library, you need to have .NET installed on your machine. Then, you can clone this repository and reference the project in your solution.

