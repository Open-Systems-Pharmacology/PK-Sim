using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;

namespace PKSim.Core.Services
{
   public class KeepScalingMethod : ScalingMethod
   {
      public KeepScalingMethod() : base(PKSimConstants.UI.KeepTarget)
      {
      }

      public override double ScaledValueFor(ParameterScaling parameterScaling)
      {
         return parameterScaling.TargetValue;
      }

      protected override ICommand PerformScaling(ParameterScaling parameterScaling)
      {
         //nothing to do in the case of override. Return an empty command
         return new PKSimEmptyCommand();
      }
   }
}