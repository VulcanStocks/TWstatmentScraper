namespace VSscraper;

using HtmlAgilityPack;
using PuppeteerSharp;
using System.IO;
using TWscraper.Models;
using System.Net;
using System.Text.RegularExpressions;



public class ScraperService
{
    private string Url { get; set; }
    private string xpath { get; set; }
    private string xpathTitles { get; set; }
    private HtmlDocument doc { get; set; }
    private HtmlNodeCollection values { get; set; }
    private HtmlNodeCollection titles { get; set; }
    private bool annual { get; set; }
    public List<StamentModel> dataRows { get; set; }
    public bool UsePrefix { get; set; }
    private IBrowser browser  { get; set; }


    public async Task<string> ScrapeWiki(string param)
    {
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe"
        });
        string wikicontentXpath = "//*[@id=\"mw-content-text\"]/div[1]/p[not(@*)]";
        string wikiXPath = "//*[@id=\"kp-wp-tab-overview\"]/div[1]/div/div/div/div/div/div[1]/div[2]/div/div/div/span[2]/a/@href";
        string name = await GetFullName(param);
        var html = await LoadPage($"https://www.google.com/search?q={name}&hl=en");
        doc = new HtmlDocument();
        doc.LoadHtml(html);

        try
        {
            var WikiLink = doc.DocumentNode
            .SelectSingleNode(wikiXPath).Attributes["href"].Value;

            html = await LoadPage(WikiLink);
            doc = new HtmlDocument();
            doc.LoadHtml(html);

            var value = doc.DocumentNode
            .SelectSingleNode(wikicontentXpath).InnerText;
            return value;

        }
        catch (NullReferenceException)
        {
            return "No data available";
        }
        
    }

    public async Task<string> GetFullName(string ticker){

        string NameXPath = "//*[@id=\"anchor-page-1\"]/div/div[2]/div[1]/div[1]/h1";

        var html = await LoadPage($"https://www.tradingview.com/symbols/{ticker}/");
        doc = new HtmlDocument();
        doc.LoadHtml(html);

        try
        {
            var value = doc.DocumentNode
            .SelectSingleNode(NameXPath).InnerText;
            string[] substrings = value.Split(' ');

            return substrings[0];
        }
        catch (NullReferenceException)
        {
            return "No data available";
        }
    } 
    private async Task<string> LoadPage(string url)
    {
        
        using (Page page = (Page)await browser.NewPageAsync())
        {
            await page.GoToAsync(url);

            return await page.GetContentAsync();
        }
    }

    public void InitializeTW(string dataType, string ticker, bool annual, bool UsePrefix)
    {

        xpath = "//div[contains(@class, 'value-pg2GO866')]";
        xpathTitles = "//span[@class='titleText-_PBNXQ7k']";
        this.UsePrefix = UsePrefix;
        dataRows = new List<StamentModel>();


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

    public async Task LoadHtmlAsync()
    {
        var htmlAsTask = await LoadTWAndWaitForSelector(Url, ".value-pg2GO866");
        doc = new HtmlDocument();

        doc.LoadHtml(htmlAsTask);
    }

    private async Task<string> LoadTWAndWaitForSelector(string url, string selector)
    {
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
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

    public Task ScrapeTWDataAsync()
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
        await ParseIncomeAsync();
    }

    public async Task SaveIncomeAsyncToCsv(string path)
    {
        await SaveIncomeAsync(path);
    }

    public async Task<string[][]> GetStatmentArrayAsync()
    {
        var statment = await GetStatmentAsync();
        string[][] statmentArray = new string[statment.Count][];
        for (int i = 0; i < statment.Count(); i++)
        {
            statmentArray[i] = new string[statment[i].columns.Count() + 1];
            statmentArray[i][0] = statment[i].titleText;
            for (int j = 0; j < statment[i].columns.Count(); j++)
            {
                statmentArray[i][j + 1] = statment[i].columns[j];
            }
        }
        return statmentArray;
    }

    public Task<List<StamentModel>> GetStatmentAsync()
    {
        return Task.FromResult(dataRows);
    }

    public Task ParseIncomeAsync()
    {
        List<string> columns = new List<string>();

        int count = 0;
        int titleCount = 0;
        bool start = false;

        foreach (var item in values)
        {
            string nodeText = WebUtility.HtmlDecode(item.InnerText.ToString());


            if (UsePrefix)
            {
                nodeText = Regex.Replace(nodeText, "[^0-9.KMB]", "");
            }
            else
            {
                nodeText = Regex.Replace(nodeText, "[^0-9.-]", "");
            }
            if (start)
            {
                columns.Add(nodeText);
            }

            count++;
            if (count == 8)
            {

                string titleText = "";
                titleText = WebUtility.HtmlDecode(titles[titleCount].InnerText.ToString());


                if (start)
                {
                    dataRows.Add(new StamentModel { titleText = titleText, columns = columns });
                    columns = new List<string>();
                    titleCount++;
                }
                else
                {
                    start = true;
                }


                count = 0;
            }

        }



        return Task.CompletedTask;
    }


    public Task SaveIncomeAsync(string path)
    {
        try
        {
            StreamWriter writer = new StreamWriter(path);

            foreach (var row in dataRows)
            {
                writer.Write(row.titleText + ",");

                for (int i = 0; i < row.columns.Count; i++)
                {
                    writer.Write(row.columns[i] + ",");
                }

                writer.WriteLine();

            }
            writer.Close();
            Console.WriteLine($"Saved to {path}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return Task.CompletedTask;

    }

}
