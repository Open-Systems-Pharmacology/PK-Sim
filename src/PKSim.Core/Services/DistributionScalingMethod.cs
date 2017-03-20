using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class DistributionScalingMethod : ScalingMethod
   {
      private readonly IParameterTask _parameterTask;

      public DistributionScalingMethod(IParameterTask parameterTask) : base(PKSimConstants.UI.ScalePercentile)
      {
         _parameterTask = parameterTask;
      }

      public override double ScaledValueFor(ParameterScaling parameterScaling)
      {
         var targetParameter = distributedParameterFrom(parameterScaling.TargetParameter);
         return targetParameter.ValueFor(scaledPercentile(parameterScaling));
      }

      protected override ICommand PerformScaling(ParameterScaling parameterScaling)
      {
         return _parameterTask.SetParameterPercentile(parameterScaling.TargetParameter, scaledPercentile(parameterScaling));
      }

      private double scaledPercentile(ParameterScaling parameterScaling)
      {
         return distributedParameterFrom(parameterScaling.SourceParameter).Percentile;
      }

      private IDistributedParameter distributedParameterFrom(IParameter parameter)
      {
         return parameter.DowncastTo<IDistributedParameter>();
      }
   }
}