using System;

namespace DataStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = new DataProcess();
            var result =process.GetMinium(new double[] { 1,2,3,100});
            Console.WriteLine(result);
        }
    }
}
