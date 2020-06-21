using OSPSuite.Core.Domain.Populations;
using PKSim.Core.Model;

namespace PKSim.R.Domain
{
   public class ParameterValueWithUnit
   {
      private readonly ParameterValue _parameterValue;

      public string Unit { get; }

      public ParameterValueWithUnit(ParameterValue parameterValue, string unit = "")
      {
         _parameterValue = parameterValue;
         Unit = unit;
      }

      public string ParameterPath => _parameterValue.ParameterPath;
      public double Value => _parameterValue.Value;
   }

   public class DistributedParameterValueWithUnit : ParameterValueWithUnit
   {
      private readonly DistributedParameterValue _distributedParameterValue;
 
      public DistributedParameterValueWithUnit(DistributedParameterValue distributedParameterValue, string unit=""):base(distributedParameterValue, unit )
      {
         _distributedParameterValue = distributedParameterValue;
      }

      public double Mean => _distributedParameterValue.Mean;
      public double Std => _distributedParameterValue.Std;
      public DistributionType DistributionType => _distributedParameterValue.DistributionType;

   }
}