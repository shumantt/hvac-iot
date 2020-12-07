using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace DataTools
{
    class Program
    {
        private static Dictionary<int, string> parseNames = new Dictionary<int, string>()
        {
            [0] = "Date",
            [9] = "TempInside",
            [10] = "Humidity",
            [21] = "TempOutside"
        };
        
        private static DateTime april1 = new DateTime(2016, 4, 1);
        private static DateTime april30 = new DateTime(2016, 4, 30, 23, 59, 59);
        private static Random random = new Random();

        static void Main(string[] args)
        {
            Console.WriteLine("Temp no auto");
            var (goodNums, total, percent) = Process(@"C:\Projects\iot\HVAC\Decisions Layer\data\temprature-data\temp-no-auto.txt", 20, 24);
            Console.WriteLine($"{goodNums} {total} {percent}");
            
            Console.WriteLine("Temp auto");
            (goodNums, total, percent) = Process(@"C:\Projects\iot\HVAC\Decisions Layer\data\temprature-data\temp-auto.txt", 20, 24);
            Console.WriteLine($"{goodNums} {total} {percent}");
            
            Console.WriteLine("Humidity no auto");
            (goodNums, total, percent) = Process(@"C:\Projects\iot\HVAC\Decisions Layer\data\temprature-data\humidity-no-auto.txt", 40, 60);
            Console.WriteLine($"{goodNums} {total} {percent}");
            
            Console.WriteLine("Humidity auto");
            (goodNums, total, percent) = Process(@"C:\Projects\iot\HVAC\Decisions Layer\data\temprature-data\humidity-auto.txt", 40, 60);
            Console.WriteLine($"{goodNums} {total} {percent}");
        }

        static (int goodNums, int total, double percent) Process(string fileName, double minComfort, double maxComfort)
        {
            var lines = File.ReadLines(fileName);
            var nums = lines.Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => double.Parse(x))
                .ToList();
            var goodNums = nums.Count(x => x >= minComfort && x <= maxComfort);
            var total = nums.Count;
            var percent = (double)goodNums / (double)total * 100;
            return (goodNums, total, percent);
        }
        
        static void GenerateNoActionsFile()
        {
            var lines = File.ReadLines(@"C:\Projects\iot\HVAC\Decisions Layer\data\temprature-data\testing.csv");
            using (var sr = new StreamWriter("no_actions_data.csv"))
            {
                sr.WriteLine("DateTime;TempInside;Humidity;People;Co2;TempOutside");
                foreach (var line in lines.Skip(1))
                {
                    var values = line.Replace("\"", "").Split(',');
                    var date = values[0];
                    var dateParsed = DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    if(dateParsed < april1)
                        continue;
                    if(dateParsed > april30)
                        break;
                    var tempInside = ParseDouble(values[9]);
                    var humidity = ParseDouble(values[10]);
                    var tempOutside = ParseDouble(values[21]);
                    var people = GetPeopleCount(dateParsed);
                    var co2 = GenerateCo2Level(dateParsed);
                    sr.WriteLine(
                        string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4};{5}", date, tempInside, humidity, people, co2, tempOutside));
                }
            }
        }

        private static int GenerateCo2Level(DateTime dateParsed)
        {
            var deviation = random.Next(-50, 50);
            var hour = dateParsed.Hour;
            if (hour < 9 || hour > 20)
                return 450 + deviation;
            if (hour <= 10)
                return 550 + deviation;
            if (hour >= 18)
                return 550 + deviation;
            return 700 + deviation;
        }

        private static int GetPeopleCount(DateTime dateParsed)
        {
            var hour = dateParsed.Hour;
            if (hour < 9 || hour > 20)
                return 0;
            if (hour <= 10)
                return 1;
            if (hour >= 18)
                return 1;
            return 2;
        }
        
        static double ParseDouble(string value)
        {
            try
            {
                return Math.Round(double.Parse(value, CultureInfo.InvariantCulture), 2);
            }
            catch (Exception e)
            {
                Console.WriteLine(value);
                throw;
            }
        }
    }
}