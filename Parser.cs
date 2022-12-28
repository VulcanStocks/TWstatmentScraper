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
    internal class Parser
    {
        public HtmlNodeCollection htmlNodes { get; set; }
        public string Path { get; set; }
        public string[][] staments { get; set; }
        public Parser(HtmlNodeCollection htmlNodes, string path)
        {
            Path = path;
            this.htmlNodes = htmlNodes;
        }

        public void ParseIncomeAsync()
        {
            staments = new string[htmlNodes.Count][];

            for (int i = 0; i < htmlNodes.Count-1; i++)
            {
                var nodeText = WebUtility.HtmlDecode(htmlNodes[i].InnerText.ToString());
                byte[] uni = Encoding.Unicode.GetBytes(nodeText);
                string Ascii = Encoding.ASCII.GetString(uni);

                string[] words = Ascii.Split("*");
                staments[i] = words;
            }
        }

        public void SaveIncomeAsync()
        {
            try
            {
                ;
                Console.WriteLine(staments[3][1] + ",");

                StreamWriter writer = new StreamWriter(Path);

                for (int i = 0; i < staments.Length - 1; i++)
                {
                    for (int j = 0; j < staments[i].Length -1; j++)
                    {
                        Console.WriteLine(staments[i][j] + ",");

                        writer.Write(staments[i][j] + ",");
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
        }
    }
}
