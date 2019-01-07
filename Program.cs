using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace CsvToJson
{
    class Program
    {
        public static void Main(string[] args)
        {
            string CSVpath = @"C:\Your\Documents\Path\here.csv";


            string data = ReadFile(CSVpath);
            var json = JValue.Parse(data).ToString(Formatting.Indented);

            using (StreamWriter file = File.CreateText(@"C:\TEMP\output.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, json);
            }
            Console.WriteLine(json);
            Console.ReadLine();
        }

        private static string ReadFile(string filePath)
        {
            string payload = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath) && Path.GetExtension(filePath).Equals(".csv", StringComparison.InvariantCultureIgnoreCase))
                {
                    string[] lines = File.ReadAllLines(filePath);

                    if (lines != null && lines.Length > 1)
                    {
                        var headers = GetHeaders(lines.First());
                        payload = GetPayload(headers, lines.Skip(1));
                    }
                }
            }
            catch (Exception exp)
            {
            }
            return payload;
        }

        private static IEnumerable<string> GetHeaders(string data)
        {
            IEnumerable<string> headers = null;

            if (!string.IsNullOrWhiteSpace(data) && data.Contains(','))
            {
                headers = GetFields(data).Select(x => x.Replace(" ", ""));
            }
            return headers;
        }

        private static string GetPayload(IEnumerable<string> headers, IEnumerable<string> fields)
        {
            string jsonObject = "";
            try
            {
                var dictionaryList = fields.Select(x => GetField(headers, x));
                jsonObject = JsonConvert.SerializeObject(dictionaryList);
            }
            catch (Exception ex)
            {
            }
            return jsonObject;
        }

        private static Dictionary<string, string> GetField(IEnumerable<string> headers, string fields)
        {
            Dictionary<string, string> dictionary = null;

            if (!string.IsNullOrWhiteSpace(fields))
            {
                var columns = GetFields(fields);

                if (columns != null && headers != null && columns.Count() == headers.Count())
                {
                    dictionary = headers.Zip(columns, (x, y) => new { x, y }).ToDictionary(item => item.x, item => item.y);
                }
            }
            return dictionary;
        }

        public static IEnumerable<string> GetFields(string line)
        {
            IEnumerable<string> fields = null;
            var items = line.Split(",");
            fields = items.ToList();
            return fields;
        }
    }
}
