using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

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

    public enum Genre
    {
        [Display(Name = "Non Fiction")]
        NonFiction = 1,
        Romance = 2,
        Action = 3,
        [Display(Name = "Science Fiction")]
        ScienceFiction = 4
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Zip_Test()
        {
            var enumType = typeof(Genre);
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType).Cast<int>();

            var items = names.Zip(values, (name, value) =>
                new KeyValuePair<string, int>(GetName(enumType, name), value)
            );
        }

        private string GetName(Type enumType, string name)
        {
            var result = name;

            var attribute = enumType
                .GetField(name)
                .GetCustomAttributes(inherit: false)
                .OfType<DisplayAttribute>()
                .FirstOrDefault();

            if (attribute != null)
                result = attribute.GetName();
            
            return result;
        }

        public class PetOwner
        {
            public string Name { get; set; }

            public List<string> Pets { get; set; }
        }

        [TestMethod]
        public void SelectMany_Test()
        {
            string[] array = { "dot", "net", "perls" };

            // Convert each string in the string array to a character array.
            // ... Then combine those character arrays into one.
            var result1 = array.SelectMany(element => element.ToCharArray());

            // Display letters.
            foreach (char letter in result1)
                Console.WriteLine(letter);

            // use like cross join in SQL
            List<int> number = new List<int>() { 10, 20 };
            List<string> animals = new List<string>() { "cat", "dog", "donkey" };
            
            var result2 = number.SelectMany(num => animals, (n, a) => new { n, a });
            foreach (var resultItem in result2)
                Console.WriteLine("{0}{1}", resultItem.n, resultItem.a);
        }

        [TestMethod]
        public void Select_SelectMany_Difference_Test()
        {
            // combine multiple collections into one
            PetOwner[] petOwners =
                   { new PetOwner { Name="Higa, Sidney",
                          Pets = new List<string>{ "Scruffy", "Sam" } },
                      new PetOwner { Name="Ashkenazi, Ronen",
                          Pets = new List<string>{ "Walker", "Sugar" } },
                      new PetOwner { Name="Price, Vernette",
                          Pets = new List<string>{ "Scratches", "Diesel" } } };

            // use Select
            IEnumerable<List<String>> result1 = petOwners.Select(petOwner => petOwner.Pets);
            
            // Notice that two foreach loops are required to 
            // iterate through the results
            // because the query returns a collection of arrays.
            foreach (List<String> petList in result1)
                foreach (string pet in petList)
                    Console.WriteLine(pet);
            
            Console.WriteLine();

            // use SelectMany
            IEnumerable<string> result2 = petOwners.SelectMany(petOwner => petOwner.Pets);

            // Only one foreach loop is required to iterate 
            // through the results since it is a
            // one-dimensional collection.
            foreach (string pet in result2)
                Console.WriteLine(pet);
        }

        [TestMethod]
        public void Aggregate_Test()
        {
            int[] array = { 1, 2, 3, 4, 5 };
            int result = array.Aggregate((a, b) => b + a);
            // 1 + 2 = 3
            // 3 + 3 = 6
            // 6 + 4 = 10
            // 10 + 5 = 15
            Console.WriteLine(result);

            int result1 = array.Aggregate((a, b) => b * a);
            // 1 * 2 = 2
            // 2 * 3 = 6
            // 6 * 4 = 24
            // 24 * 5 = 120
            Console.WriteLine(result1);

            string sentence = "the quick brown fox jumps over the lazy dog";

            // Split the string into individual words.
            string[] words = sentence.Split(' ');

            // Prepend each word to the beginning of the 
            // new sentence to reverse the word order.
            string result3 = words.Aggregate((workingSentence, next) => next + " " + workingSentence);

            Console.WriteLine(result3);
        }
        
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

        [TestMethod]
        public void Group_By_Age_From_Birthdate_Test()
        {
            string namesAndAges = "Phillip Do, 10/10/1978; Mony Taing, 11/10/1979; Mason Do, 03/23/2009; Emma Do, 11/09/2010";
            var result = namesAndAges
                .Split(';')
                .Select(n => n.Split(','))
                .Select(n => new { Name = n[0].Trim(), DateOfBirth = DateTime.ParseExact(n[1].Trim(), "M/d/yyyy", CultureInfo.InvariantCulture) })
                .OrderByDescending(n => n.DateOfBirth)
                .Select(n => 
                {
                    DateTime today = DateTime.Today;
                    int age = today.Year - n.DateOfBirth.Year;
                    if (n.DateOfBirth > today.AddYears(-age)) age--;
                    return new { Name = n.Name, Age = age };
                });
        }

        [TestMethod]
        public void Group_By_Age_From_Birthdate_Clean_Test()
        {
            Func<string, DateTime> parseDob = dob => DateTime.ParseExact(dob.Trim(), "M/d/yyyy", CultureInfo.InvariantCulture);
            Func<DateTime, int> getAge = dateOfBirth => {
                DateTime today = DateTime.Today;
                int age = today.Year - dateOfBirth.Year;
                if (dateOfBirth > today.AddYears(-age)) age--;
                return age;
            };

            string namesAndAges = "Phillip Do, 10/10/1978; Mony Taing, 11/10/1979; Mason Do, 03/23/2009; Emma Do, 11/09/2010";
            var result = namesAndAges
                .Split(';')
                .Select(n => n.Split(','))
                .Select(n => new { Name = n[0].Trim(), DateOfBirth = parseDob(n[1].Trim()) })
                .OrderByDescending(n => n.DateOfBirth)
                .Select(n => new { Name = n.Name, Age = getAge(n.DateOfBirth) });
        }

        private DateTime ParseDob(string dob)
        {
            return DateTime.ParseExact(dob.Trim(), "M/d/yyyy", CultureInfo.InvariantCulture);
        }

        private int GetAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;
            return age;
        }

        [TestMethod]
        public void Chess_Bishop_Move_Test()
        {
            // this will not be a very high performing test
            // we start with a Bishop on c6
            // what positions can it reach in one move?
            // output should include b5, a4, b7, a8
            var result1 = Enumerable.Range('a', 8)
                .SelectMany(x => Enumerable.Range('1', 8),
                    (f, r) => new { File = (char)f, Rank = (char)r })
                .Where(x => Math.Abs(x.File - 'c') == Math.Abs(x.Rank - '6'))
                .Where(x => x.File != 'c')
                .Select(x => String.Format("{0}{1}", x.File, x.Rank));

            var result2 =
                from row in Enumerable.Range('a', 8)
                from col in Enumerable.Range('1', 8)
                let dx = Math.Abs(row - 'c')
                let dy = Math.Abs(col - '6')
                where dx == dy && dx != 0
                select String.Format("{0}{1}", (char)row, (char)col);
        }

        [TestMethod]
        public void Chess_Bishop_Move_Clean_Test()
        {
            var result = GetBoardPositions().Where(p => BishopCanMoveTo(p, "c6"));
        }

        private IEnumerable<string> GetBoardPositions()
        {
            return Enumerable.Range('a', 8).SelectMany(
                x => Enumerable.Range('1', 8), (f, r) => 
                    String.Format("{0}{1}", (char)f, (char)r));
        }

        private bool BishopCanMoveTo(string startPos, string targetPos)
        {
            var dx = Math.Abs(startPos[0] - targetPos[0]);
            var dy = Math.Abs(startPos[1] - targetPos[1]);
            return dx == dy && dx != 0;
        }

        [TestMethod]
        public void Title_Of_Longest_Book_Test()
        {
            var books = new[]
            {
                new { Author = "Robert Martin", Title = "Clean Code", Pages = 464 },
                new { Author = "Oliver Sturm", Title = "Functional Programming in C#", Pages = 270 },
                new { Author = "Martin Fowler", Title = "Patterns of Enterprise Application Architecture", Pages = 533 },
                new { Author = "Bill Wagner", Title = "Effective C#", Pages = 328 }
            };

            var mostPages = books.Max(x => x.Pages);
            var result1 = books.First(b => b.Pages == mostPages);
            var result2 = books.OrderByDescending(b => b.Pages).First();

            // best performance
            var result3 = books.Aggregate((agg, next) => next.Pages > agg.Pages ? next : agg);     
        }
    }
}
