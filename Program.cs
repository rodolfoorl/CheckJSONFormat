using CheckJSONFormat.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CheckJSONFormat
{
    internal class Program
    {
        static AppSettings appSettings;

        static void Main(string[] args)
        {
            ConfigurationHelper.CreateDefaultConfig(ref appSettings);
            InitCommand();
        }

        static void InitCommand()
        {
            //Get files JSON
            var jsonFiles = Directory
                .GetFiles(appSettings.PathFolderJSON, "*.json", SearchOption.AllDirectories)
                .ToList();

            jsonFiles.ForEach(x =>
            {
                //Get lines of JSON
                var json = File.ReadAllLines(x)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

                var count = 0;
                foreach (var s in json)
                {
                    count++;
                    
                    //Check if JSON contain breakline
                    if (!s.Contains("{") &&
                        !s.Contains("}") &&
                        s.Trim().LastOrDefault() != ',' &&
                        count + 1 != json.Count)
                    {
                        //Recreate JSON file
                        CleanJSON(x);
                        break;
                    }
                }
            });
        }

        static void CleanJSON(string pathJson)
        {
            try
            {
                //Get folder path
                var pathFolder = $"{AppContext.BaseDirectory}{Directory.GetParent(pathJson).FullName.Replace(appSettings.PathFolderJSON, string.Empty)}";

                //Create folder if not exists
                if (!Directory.Exists(pathFolder))
                    Directory.CreateDirectory(pathFolder);

                //Read and create JSON Object
                var json = File.ReadAllText(pathJson, Encoding.UTF8);
                var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                foreach (var obj in jsonObj)
                {
                    //Clean breakline 
                    var text = obj.Value
                        .Replace("\r\n", " ")
                        .Replace("\t\t", " ");

                    //Clean white space
                    jsonObj[obj.Key] = Regex.Replace(text, @"\s+", " ");
                }

                //White file JSON Indented
                var jsonString = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                File.WriteAllText($"{pathFolder}\\{pathJson.Split("\\").LastOrDefault()}", jsonString, Encoding.UTF8);
            }
            catch
            {
                Console.WriteLine($"File: {pathJson.Split("\\").LastOrDefault()}");
                Console.WriteLine($"Path: {pathJson}");
                Console.WriteLine($"{Environment.NewLine}");
            }
        }
    }
}
