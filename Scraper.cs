namespace TWscraper;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.IO;
using TWscraper.Models;



public class Scraper
{
    private string Url { get; set; }
    private string xpath { get; set; }
    private string xpathTitles { get; set; }
    private HtmlDocument doc { get; set; }
    private HtmlNodeCollection values { get; set; }
    private HtmlNodeCollection titles { get; set; }
   private bool annual { get; set; }
    private Parser parser { get; set; }

    public void Initialize(string dataType, string ticker, bool annual)
    {
        xpath = "//div[contains(@class, 'value-pg2GO866')]";
        xpathTitles = "//span[@class='titleText-_PBNXQ7k']";

        this.annual = annual;

        switch (dataType)
        {
            case "income":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-income-statement/";
                break;
            case "balance":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-balance-sheet/";
                break;
            case "flow":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-cash-flow/";
                break;
            case "ratios":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-statistics-and-ratios/";
                break;
        }
    }

    public void InitializeParser(bool usePrefix)
    {
        try
        {
            parser = new Parser(values, titles, usePrefix);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task LoadHtmlAsync()
    {
        var htmlAsTask = await LoadAndWaitForSelector(Url, ".value-pg2GO866");
        doc = new HtmlDocument();

        doc.LoadHtml(htmlAsTask);
    }

    private async Task<string> LoadAndWaitForSelector(string url, string selector)
    {
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
        });
        using (Page page = (Page)await browser.NewPageAsync())
        {
            await page.GoToAsync(url);

            if (annual)
            {
                var button = await page.WaitForSelectorAsync("#FY");
                // Press the button
                await button.ClickAsync();
            }

            await page.WaitForSelectorAsync(selector);

            return await page.GetContentAsync();
        }
    }

    public Task ScrapeDataAsync()
    {
        values = doc.DocumentNode.SelectNodes(xpath);

        titles = doc.DocumentNode.SelectNodes(xpathTitles);

        return Task.CompletedTask;
    }

    public void PrintNodes()
    {
        foreach (var item in values)
        {
            System.Console.WriteLine(item.InnerText);
        }
    }

    public async Task ParseStatmentAsync()
    {
        await parser.ParseIncomeAsync();
    }

    public async Task SaveIncomeAsyncToCsv(string path)
    {
        await parser.SaveIncomeAsync(path);
    }

    public async Task<string[][]> GetStatmentArrayAsync()
    {
        var statment = await parser.GetStatmentAsync();
        string[][] statmentArray= new string[statment.Count][];
        for (int i = 0; i < statment.Count(); i++)
        {
            statmentArray[i] = new string[statment[i].columns.Count()+1];
            statmentArray[i][0] = statment[i].titleText;
            for (int j = 0; j < statment[j].columns.Count(); j++)
            {
                statmentArray[i][j+1] = statment[i].columns[j];
            }
        }
        return statmentArray;
    }

    public async Task<List<StamentModel>> GetStatmentAsync()
    {
        return await parser.GetStatmentAsync();
    }

}
