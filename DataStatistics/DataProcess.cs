using MathNet.Numerics.Statistics;

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
}
