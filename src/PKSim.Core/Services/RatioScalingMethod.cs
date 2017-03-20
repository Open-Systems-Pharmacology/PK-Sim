using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;

namespace PKSim.Core.Services
{
   public class RatioScalingMethod : ScalingMethod
   {
      private readonly IParameterTask _parameterTask;

      public RatioScalingMethod(IParameterTask parameterTask) : base(PKSimConstants.UI.Ratio)
      {
         _parameterTask = parameterTask;
      }

      public override double ScaledValueFor(ParameterScaling parameterScaling)
      {
         var defaultValue = parameterScaling.SourceParameter.DefaultValue.GetValueOrDefault(parameterScaling.SourceParameter.Value);
         return parameterScaling.TargetValue * parameterScaling.SourceParameter.Value / defaultValue;
      }

      protected override ICommand PerformScaling(ParameterScaling parameterScaling)
      {
         return _parameterTask.SetParameterValue(parameterScaling.TargetParameter, parameterScaling.ScaledValue);
      }
   }
}