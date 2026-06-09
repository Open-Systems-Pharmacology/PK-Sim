using OSPSuite.Core.Domain.Formulas;
using OSPSuite.R.Domain;
using PKSim.Core.Model;

namespace PKSim.R.Domain
{
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