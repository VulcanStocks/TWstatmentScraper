namespace TWscraper;
using HtmlAgilityPack;
using PuppeteerSharp;

public class Scraper
{
    public string Url { get; set; }
    public string Xpath { get; set; }
    public HtmlDocument doc { get; set; }
    public HtmlNodeCollection value { get; set; }


    public Scraper(string dataType, string ticker)
    {
        Xpath = "//div[contains(@class, 'container-_PBNXQ7k')]";

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

    public static async Task<string> LoadAndWaitForSelector(string url, string selector)
    {
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
        });
        using (Page page = (Page)await browser.NewPageAsync())
        {
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync(selector);
            return await page.GetContentAsync();
        }
    }

    public Task ScrapeDataAsync()
    {
        value = doc.DocumentNode.SelectNodes(Xpath);

        return Task.CompletedTask;
    }

    public void PrintNodes()
    {
        foreach (var item in value)
        {
            System.Console.WriteLine(item.InnerText);
        }
    }

    public async Task ParseAndSaveToCsvAsync(string path)
    {
        var parser = new Parser(value, path);
        await parser.ParseIncomeAsync();
        await parser.SaveIncomeAsync();
    }


}
