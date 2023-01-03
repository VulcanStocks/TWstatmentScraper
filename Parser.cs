using CsvHelper;
using HtmlAgilityPack;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWscraper.Models;
using static System.Net.Mime.MediaTypeNames;

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
        public string Path { get; set; }
        public List<DataModel> dataRows { get; set; }
        public bool UsePrefix { get; set; }


        public Parser(HtmlNodeCollection values, HtmlNodeCollection titles, string path, bool UsePrefix)
        {
            this.UsePrefix = UsePrefix;
            Path = path;
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
                    nodeText = Regex.Replace(nodeText, "[^0-9.KMB+\\-%]", "");
                }
                else
                {
                    nodeText = Regex.Replace(nodeText, "[^0-9.+\\-%]", "");
                }
                if (start)
                {
                    columns.Add(nodeText);
                }

                if (count == 7)
                {

                    string titleText = "";
                    try
                    {
                        titleText = WebUtility.HtmlDecode(titles[titleCount].InnerText.ToString());
                    }
                    catch (Exception)
                    {
                        
                    }

                    if (start)
                    {
                        dataRows.Add(new DataModel { titleText = titleText, columns = columns });
                        foreach (var item2 in columns)
                        {
                            Console.WriteLine(item2);
                        }
                        Console.WriteLine("------------------");
                        columns = new List<string>();
                        titleCount++;
                    }
                    else
                    {
                        start = true;
                    }


                    count = 0;
                }

                count++;
            }



            return Task.CompletedTask;
        }







        /*
        public Task ParseIncomeAsync()
        {
            for (int i = 0; i < htmlNodes.Count; i++)
            {
                var nodeText = WebUtility.HtmlDecode(htmlNodes[i].InnerText.ToString());
                byte[] uni = Encoding.Unicode.GetBytes(nodeText);
                string Ascii = Encoding.ASCII.GetString(uni);
                Ascii = CleanUp(Ascii);
                string[] words = Ascii.Split("*");
                words = CleanUpWords(words);
                staments[i] = words;
            }
            return Task.CompletedTask;
        }

        

        private string[] CleanUpWords(string[] Words)
        {
            for (int i = 0; i < Words.Length; i++)
            {
                string[] split = Words[i].Split(new char[] { '+', '"', });

                Words[i] = split[0];

                if (i > 0)
                {
                    Words[i] = Regex.Replace(Words[i], "[^0-9.]", "");
                }
            }

            return Words;
        }
                */

        public Task SaveIncomeAsync()
        {
            try
            {
                StreamWriter writer = new StreamWriter(Path);

                foreach (var row in dataRows)
                {
                    writer.Write(row.titleText+ ",");

                    foreach (var value in row.columns)
                    {
                        Console.WriteLine(value + ",");
                        writer.Write(value + ",");
                    }
                    writer.WriteLine();
                }
                writer.Close();
                Console.WriteLine($"Saved to {Path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Task.CompletedTask;

        }
    }

}
