using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace json_onix_codelists
{
    class Program
    {
        class CodeList
        {
            public string Code { get; set; }

            public string Heading { get; set; }

            public string Note { get; set; }
        }

        static void Main(string[] args)
        {
            string[] filePaths = Directory.GetFiles(@"data\codelists\html");
            for (int i = 0; i < filePaths.Length; i++)
            {
                using (var reader = new StreamReader(filePaths[i]))
                {
                    var html = reader.ReadToEnd();

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    List<List<string>> table = htmlDoc.DocumentNode.SelectSingleNode("//table")
                                .Descendants("tr")
                                .Skip(1)
                                .Where(tr => tr.Elements("td").Count() > 1)
                                .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                                .ToList();

                    var items = new List<CodeList>();
                    foreach (var row in table)
                    {
                        items.Add(new CodeList
                        {
                            Code = row[0],
                            Heading = row[1],
                            Note = row[2]
                        });
                    }

                    var json = JsonConvert.SerializeObject(items);
                    string destPath = Path.Combine(@"data\codelists\json", Path.GetFileName(filePaths[i]).Replace("htm", "json"));
                    using (var writer = new StreamWriter(destPath))
                    {
                        writer.Write(JValue.Parse(json).ToString(Formatting.Indented));
                    }
                }

                Console.WriteLine(Path.GetFileName(filePaths[i]));
            }
        }
    }
}
