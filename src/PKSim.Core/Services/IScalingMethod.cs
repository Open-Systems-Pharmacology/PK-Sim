using OSPSuite.Core.Commands.Core;

namespace PKSim.Core.Services
{
   public abstract class ScalingMethod
   {
      protected readonly string _name;

      protected ScalingMethod(string name)
      {
         _name = name;
      }

      public override string ToString()
      {
         return _name;
      }

      public abstract double ScaledValueFor(ParameterScaling parameterScaling);

      public virtual ICommand Scale(ParameterScaling parameterScaling)
      {
         parameterScaling.ResetTargetParameter();
         return PerformScaling(parameterScaling);
      }

      protected abstract ICommand PerformScaling(ParameterScaling parameterScaling);
   }
}