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
            staments = new string[htmlNodes.Count][];
        }

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

        private string CleanUp(string Ascii)
        {
            Ascii = Regex.Replace(Ascii, @"\p{C}+", string.Empty);
            Ascii = Ascii.Replace(",", string.Empty);
            Ascii = Ascii.Replace(" ", string.Empty); 
            Ascii = Ascii.Replace("YoYgrowth", string.Empty);
            return Ascii;
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
        public Task SaveIncomeAsync()
        {
            try
            {
                StreamWriter writer = new StreamWriter(Path);

                foreach (var statement in staments)
                {
                    foreach (var word in statement)
                    {
                        Console.WriteLine(word + ",");
                        writer.Write(word + ",");
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
