using PKSim.Assets;
using OSPSuite.Core.Commands.Core;

namespace PKSim.Core.Services
{
   public class OverrideScalingMethod : ScalingMethod
   {
      private readonly IParameterTask _parameterTask;

      public OverrideScalingMethod(IParameterTask parameterTask) : base(PKSimConstants.UI.OverrideWithSource)
      {
         _parameterTask = parameterTask;
      }

      public override double ScaledValueFor(ParameterScaling parameterScaling)
      {
         return parameterScaling.SourceParameter.Value;
      }

      protected override ICommand PerformScaling(ParameterScaling parameterScaling)
      {
         return _parameterTask.SetParameterValue(parameterScaling.TargetParameter, parameterScaling.SourceParameter.Value);
      }
   }
}