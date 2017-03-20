using System;
using OSPSuite.Core.Maths.Random;
using OSPSuite.Core.Maths.Statistics;

namespace PKSim.Core.Model
{
   public class OntogenyMetaData
   {
      private LogNormalDistribution _logNormalDistribution;

      public double PostmenstrualAge { get; set; }
      public string DisplayName { get; set; }
      public double Deviation { get; set; }
      public string MoleculeName { get; set; }
      public string SpeciesName { get; set; }
      public string GroupName { get; set; }
      public double OntogenyFactor { get; set; }

      public double RandomizedFactor(RandomGenerator randomGenerator)
      {
         return logNormalDistribution.RandomDeviate(randomGenerator);
      }

      public double RandomizedFactor(double percentile)
      {
         return logNormalDistribution.CalculateValueFromPercentile(percentile);
      }

      public double RandomizedPercentile(RandomGenerator randomGenerator)
      {
         return PercentileFor(RandomizedFactor(randomGenerator));
      }

      public double PercentileFor(double value)
      {
         return logNormalDistribution.CalculatePercentileForValue(value);
      }

      private LogNormalDistribution logNormalDistribution
      {
         get
         {
            if (_logNormalDistribution == null)
            {
               _logNormalDistribution = new LogNormalDistribution(Math.Log(OntogenyFactor), Math.Log(Deviation));
            }

            return _logNormalDistribution;
         }
      }
   }
}