using MathNet.Numerics.Statistics;
using System;

namespace DataStatistics
{
    public class DataProcess
    {
        public double GetMinium(double[] data)
        {
            return ArrayStatistics.Minimum(data);
        }
        public double GetMax(double[] data)
        {
            return ArrayStatistics.Maximum(data);
        }
        public double GetMean(double[] data)
        {
            return ArrayStatistics.Mean(data);
        }
    }

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
