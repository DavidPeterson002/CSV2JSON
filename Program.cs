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
            string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_dailyactivity_20171001_20171007.csv";
            //string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_dailycalories_20171001_20171007.csv";
            //string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_dailysteps_20171001_20171007.csv";
            //string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_heartrate_15min_20171001_20171007.csv";
            //string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_sleepday_20171001_20171007.csv";
            //string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_dailyrestingheartrate_20171001_20171007.csv";
            //string CSVpath = @"C:\Users\dpeterson003\Documents\id-1003_weightloginfo_20171001_20171007.csv";

            string data = ReadFile(CSVpath);
            var json = JValue.Parse(data).ToString(Formatting.Indented);
            Console.WriteLine(json);
            Console.ReadLine();

            //string[] bullets = { "1.", "2.", "3.", "4." };
            //string[] words = { "Friends", "Family", "CoWorkers" };

            //var bulletedList = bullets.Zip(words, (x, y) => $"{x} {y}");

            //foreach (var item in bulletedList)
            //    Console.WriteLine($"{item}");


            //Console.ReadLine();
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
