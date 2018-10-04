using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Commands;

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
         var (sourceParameter, targetParameter) = parameterScaling;

         var updateValueCommand = _parameterTask.SetParameterValue(targetParameter, sourceParameter.Value);
         var updateValueOriginCommand = _parameterTask.SetParameterValueOrigin(targetParameter, sourceParameter.ValueOrigin);

         return new PKSimMacroCommand(new[] {updateValueCommand, updateValueOriginCommand});
      }
   }
}