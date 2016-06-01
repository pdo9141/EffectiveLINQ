using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
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
