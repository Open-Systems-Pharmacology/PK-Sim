namespace PKSim.Core.Model
{
   public class ParameterValue : RandomValue
   {
      public string ParameterPath { get; set; }

      public ParameterValue(string parameterPath, double value, double percentile)
      {
         ParameterPath = parameterPath;
         Value = value;
         Percentile = percentile;
      }

      public ParameterValue Clone()
      {
         return new ParameterValue(ParameterPath, Value, Percentile);
      }
   }

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