using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace Tests
{
    static class MyExtensions
    {
        public static IEnumerable<string> FindCSharpFiles(
            this IEnumerable<string> projectPaths)
        {
            string xmlNamespace = "{http://schemas.microsoft.com/developer/msbuild/2003}";

            return from projectPath in projectPaths
                   let xml = XDocument.Load(projectPath)
                   let dir = Path.GetDirectoryName(projectPath)
                   from c in xml.Descendants(xmlNamespace + "Compile")
                   let inc = c.Attribute("Include").Value
                   where inc.EndsWith(".cs")
                   select Path.Combine(dir, c.Attribute("Include").Value);
        }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void How_Long_Entire_Album_Is_Test()
        {
            var trackTimes = "2:54,3:48,4:51,3:32,6:15,4:08,5:17,3:13,4:16,3:55,4:53,5:35,4:24";
            var totalTrackTimesInSeconds = trackTimes
                .Split(',')
                .Select(t => "0:" + t)
                .Select(t => TimeSpan.Parse(t))
                .Select(t => t.TotalSeconds)
                .Sum();

            var totalTrackTimes = TimeSpan.FromSeconds(totalTrackTimesInSeconds);
            Console.WriteLine(totalTrackTimes);

            Console.WriteLine(
                trackTimes
                    .Split(',')
                    .Select(t => TimeSpan.Parse("0:" + t))
                    .Aggregate((t1, t2) => t1 + t2)
            );
        }

        [TestMethod]
        public void Expand_Range_Test()
        {
            // Expand the range
            // e.g., "2,3-5,7" should expand to 2,3,4,5,7
            var data = "2,5,7-10,11,17-18";
            var result = data
                .Split(',')
                .Select(x => x.Split('-'))
                .Select(p => new { First = int.Parse(p[0]), Last = int.Parse(p.Last()) })
                .SelectMany(r => Enumerable.Range(r.First, r.Last - r.First + 1));

            Console.WriteLine(result);
        }

        [TestMethod]
        public void Expand_Range_In_Order_No_Dups_Test()
        {
            // Expand the range
            // e.g., "2,3-5,7" should expand to 2,3,4,5,7
            // "6,1-3,2-4" should expand to 1,2,3,4,6
            var data = "6,1-3,2-4";
            var result = data
                .Split(',')
                .Select(x => x.Split('-'))
                .Select(p => new { First = int.Parse(p[0]), Last = int.Parse(p.Last()) })
                .SelectMany(r => Enumerable.Range(r.First, r.Last - r.First + 1))
                .OrderBy(r => r)
                .Distinct();            
        }

        [TestMethod]
        public void Search_Log_Files_Test()
        {
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var diagsFolder = Path.Combine(myDocs, "SkypeVoiceChanger", "diags");
            var fileType = "*.csv";
            var searchTerm = "User Submitted";

            var result = Directory.EnumerateFiles(diagsFolder, fileType)
                .SelectMany(file => File.ReadAllLines(file)
                    .Select((line, index) => new
                    {
                        File = file,
                        Text = line,
                        LineNumber = index + 1
                    })
                )
                .Where(line => Regex.IsMatch(line.Text, searchTerm));
        }

        [TestMethod]
        public void Search_Log_Files_2_Test()
        {
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var diagsFolder = Path.Combine(myDocs, "SkypeVoiceChanger", "diags");
            var fileType = "*.csv";

            var result = Directory.EnumerateFiles(diagsFolder, fileType)
                .SelectMany(file => File.ReadAllLines(file)
                    .Skip(1)
                    .Select(line => line.Split(','))
                    .Where(parts => parts.Length > 8))
                .Where(parts => Regex.IsMatch(parts[8], "Checking license for"))
                .Select(parts => new
                {
                    Date = DateTime.Parse(parts[0]),
                    License = Regex.Match(parts[8], @"\d+").Value,
                    FirstTime = Regex.Match(parts[9], @"True|False").Value
                })
                .Where(x => x.License.Length >= 6)
                .Select(e => e.License)
                .Distinct();
        }

        [TestMethod]
        public void Orphaned_Project_Files_Test()
        {
            var sourceFolder = @"C:\Users\Mark\code\github\NAudio";

            var allCSharpFiles = Directory.EnumerateFiles(sourceFolder, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.Contains(@"\obj\"))
                .Where(p => !p.Contains(@"\bin\"))
                .ToList();

            var allCSharpFilesInProjects = Directory.EnumerateFiles(sourceFolder, "*.csproj", SearchOption.AllDirectories)
                .FindCSharpFiles()
                .ToList();

            var result = allCSharpFiles.Except(allCSharpFilesInProjects);
        }
        
        [TestMethod]
        public void Sum_All_Scores_Remove_Three_Lowest_Test()
        {
            var scores = "10,5,0,8,10,1,4,0,10,1";
            var desiredScoreSum = scores
                .Split(',')
                .Select(s => int.Parse(s))
                .OrderBy(s => s)
                .Skip(3)
                .Sum();
            Console.WriteLine(desiredScoreSum);
        }

        [TestMethod]
        public void Get_List_File_Sizes_Test()
        {
            var paths = new[] { "Bill.pdf", "Tutorial.pdf", "W9.pdf" };
            var fileSizes = paths.Select(p => new FileInfo(p).Length).ToList();
            fileSizes.ForEach(fs => Console.WriteLine(fs));

            var nameAndFileSizes = paths.Select(p => new FileInfo(p)).ToDictionary(p => p.Name, p => p.Length);
            foreach (var nameAndFileSize in nameAndFileSizes)
                Console.WriteLine("Key: {0}, Value: {1}", nameAndFileSize.Key, nameAndFileSize.Value);
        }

        [TestMethod]
        public void Grouping_Test()
        {
            var orders = new List<Order>()
            {
                new Order { Id = 123, Amount = 29.95m, CustomerId = "Mark", Status = "Delivered" },
                new Order { Id = 456, Amount = 45.00m, CustomerId = "Steph", Status = "Refunded" },
                new Order { Id = 768, Amount = 32.50m, CustomerId = "Claire", Status = "Delivered" },
                new Order { Id = 222, Amount = 300.00m, CustomerId = "Mark", Status = "Delivered" },
                new Order { Id = 333, Amount = 465.00m, CustomerId = "Steph", Status = "Awaiting Stock" },
            };

            Dictionary<string, List<Order>> ordersByCustomer = OrdersByCustomer(orders);
            foreach (var kvp in ordersByCustomer)
                Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value.Count);

            Dictionary<string, List<Order>> ordersByCustomer2 = orders.GroupBy(o => o.CustomerId)
                .ToDictionary(g => g.Key, g => g.ToList());
            foreach (var kvp in ordersByCustomer2)
                Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value.Count);
        }

        private Dictionary<string, List<Order>> OrdersByCustomer(List<Order> orders)
        {
            var dict = new Dictionary<string, List<Order>>();
            foreach (var order in orders)
            {
                if (!dict.ContainsKey(order.CustomerId))
                    dict[order.CustomerId] = new List<Order>();
                dict[order.CustomerId].Add(order);
            }
            return dict;
        }

        public class Order
        {
            public int Id { get; set; }
            public decimal Amount { get; set; }
            public string CustomerId { get; set; }
            public string Status { get; set; }
        }
    }
}
