namespace TWscraper;
using HtmlAgilityPack;
using PuppeteerSharp;

public class Scraper
{
    public string Url { get; set; }
    public string Xpath { get; set; }
    public string XpathTitles { get; set; }
    public HtmlDocument doc { get; set; }
    public HtmlNodeCollection values { get; set; }
    public HtmlNodeCollection titles { get; set; }
    public bool UsePrefix { get; set; }
    public bool annual { get; set; }


    public Scraper(string dataType, string ticker, bool annual, bool UsePrefix)
    {
        Xpath = "//div[contains(@class, 'value-pg2GO866')]";
        XpathTitles = "//span[@class='titleText-_PBNXQ7k']";

        this.UsePrefix = UsePrefix;
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
        }
    }

    public async Task LoadHtmlAsync()
    {
        var htmlAsTask = await LoadAndWaitForSelector(Url, ".value-pg2GO866");
        doc = new HtmlDocument();

        doc.LoadHtml(htmlAsTask);
    }

    public async Task<string> LoadAndWaitForSelector(string url, string selector)
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
        values = doc.DocumentNode.SelectNodes(Xpath);

        titles = doc.DocumentNode.SelectNodes(XpathTitles);

        return Task.CompletedTask;
    }

    public void PrintNodes()
    {
        foreach (var item in values)
        {
            System.Console.WriteLine(item.InnerText);
        }
    }

    public async Task ParseAndSaveToCsvAsync(string path)
    {
        var parser = new Parser(values, titles, path, UsePrefix);
        await parser.ParseIncomeAsync();
        await parser.SaveIncomeAsync();
    }


}
