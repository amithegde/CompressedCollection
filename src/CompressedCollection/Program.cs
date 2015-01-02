using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CompressedCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            var sampleSize = 1000000;
            Console.WriteLine("Sample Size: " + sampleSize.ToString("#,###"));
            Console.WriteLine("generating data");
            var data = GetFakeData(sampleSize);

            CsvHelper.WriteToCsv<Person>(data.ToList(), @"sourceData.csv");

            Console.WriteLine("\nstarting to compress...");
            var stopWatch = Stopwatch.StartNew();
            var compressed = new ZippedCollection<Person>().CreateateInstance(data);
            stopWatch.Stop();

            Console.WriteLine("Time to compress: " + TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds));

            Console.WriteLine("\nstarting to decompress...");
            stopWatch.Restart();
            var decompressed = compressed.ToDecompressedList();
            stopWatch.Stop();

            Console.WriteLine("Time to decompress: " + TimeSpan.FromMilliseconds(stopWatch.ElapsedMilliseconds));

            CsvHelper.WriteToCsv<Person>(decompressed.ToList(), @"targetData.csv");

            Console.Read();
        }

        static IList<Person> GetFakeData(int length)
        {
            var list = new List<Person>();

            for (int i = 0; i < length; i++)
            {
                list.Add(new Person { 
                    Name = Faker.NameFaker.FirstName(), 
                    Age = Faker.NumberFaker.Number(),
                    Address = Faker.StringFaker.AlphaNumeric(50),
                    Email = Faker.StringFaker.AlphaNumeric(50),
                    DateOfBirth = DateTimeOffset.UtcNow.AddDays(-10) 
                });

            }

            return list;
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
    }
}