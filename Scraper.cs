namespace TWscraper;
using HtmlAgilityPack;
using PuppeteerSharp;

public class Scraper
{
    public string Url { get; set; }
    public string XpathName { get; set; }
    public string XpathValue { get; set; }

    public HtmlDocument doc { get; set; }
    public HtmlNodeCollection value { get; set; }
    public HtmlNodeCollection name { get; set; }


    public Scraper(string dataType, string ticker)
    {
        switch (dataType)
        {
            case "income":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-income-statement/";
                XpathName = "//div[contains(@class, 'container-YOfamMRP')]//@data-name";
                XpathValue = "//div[contains(@class, 'value-pg2GO866')]";
                break;
            case "balance":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-balance-sheet/";
                XpathName = "";
                XpathValue = "";
                break;
            case "flow":
                Url = $"https://www.tradingview.com/symbols/{ticker}/financials-financials-cash-flow/";
                XpathName = "";
                XpathValue = "";
                break;
        }
    }

    public async Task LoadHtmlAsync()
    {
        var htmlAsTask = await LoadAndWaitForSelector(Url, ".value-pg2GO866");
        doc.Load(htmlAsTask);
    }

    public static async Task<string> LoadAndWaitForSelector(string url, string selector)
    {
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = @"c:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
        });
        using (Page page = (Page)await browser.NewPageAsync())
        {
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync(selector);
            return await page.GetContentAsync();
        }
    }

    public void ScrapeDataOnce()
    {
        name = doc.DocumentNode.SelectNodes(XpathName);
        value = doc.DocumentNode.SelectNodes(XpathValue);
    }

    public void PrintNodes()
    {

        foreach (var item in name)
        {
            System.Console.WriteLine(item.InnerText);
        }

        foreach (var item in value)
        {
            System.Console.WriteLine(item.InnerText);
        }
    }


}
