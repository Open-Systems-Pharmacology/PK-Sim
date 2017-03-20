using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public class RatioScalingMethodSpecification : IScalingMethodSpecification
   {
      private readonly IParameterTask _parameterTask;

      public RatioScalingMethodSpecification(IParameterTask parameterTask)
      {
         _parameterTask = parameterTask;
      }

      public bool IsSatisfiedBy(ParameterScaling parameterScaling)
      {
         //changed by user!
         var originParameter = parameterScaling.SourceParameter;

         if (originParameter.IsFixedValue) return true;

         var defaultValue = originParameter.DefaultValue;
         if (!defaultValue.HasValue) return false;

         //same value and default, no need for ratio
         if (ValueComparer.AreValuesEqual(originParameter.Value, defaultValue.Value)) return false;

         //same target and default, not need for ratio (equivalent to keep)
         if (ValueComparer.AreValuesEqual(parameterScaling.TargetParameter.Value, defaultValue.Value)) return false;

         //only scake with ration if default !=0
         return defaultValue.Value != 0;
      }

      public bool IsDefaultFor(ParameterScaling parameterSatifyingSpecification)
      {
         return !parameterSatifyingSpecification.IsDistributedScaling;
      }

      public ScalingMethod Method
      {
         get { return new RatioScalingMethod(_parameterTask); }
      }
   }
}