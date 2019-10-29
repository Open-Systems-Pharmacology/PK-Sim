using OSPSuite.Core.Domain.Populations;

namespace PKSim.Core.Model
{
   public class DistributedParameterValue : ParameterValue
   {
      public double Mean { get; private set; }
      public double Std { get; private set; }
      public DistributionType DistributionType { get; private set; }

      public DistributedParameterValue(string parameterPath, double value, double percentile, double mean, double std, DistributionType distributionType) : base(parameterPath, value, percentile)
      {
         Mean = mean;
         Std = std;
         DistributionType = distributionType;
      }
   }
}