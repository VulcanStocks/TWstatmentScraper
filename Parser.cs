using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;

namespace TWscraper
{

    public class DataModel
    {
        public string titleText { get; set; }
        public List<string> columns { get; set; }
    }
    internal class Parser
    {
        public HtmlNodeCollection values { get; set; }
        public HtmlNodeCollection titles { get; set; }
        public List<DataModel> dataRows { get; set; }
        public bool UsePrefix { get; set; }


        public Parser(HtmlNodeCollection values, HtmlNodeCollection titles, bool UsePrefix)
        {
            this.UsePrefix = UsePrefix;
            this.values = values;
            this.titles = titles;

            dataRows = new List<DataModel>();
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
                        dataRows.Add(new DataModel { titleText = titleText, columns = columns });
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

}
