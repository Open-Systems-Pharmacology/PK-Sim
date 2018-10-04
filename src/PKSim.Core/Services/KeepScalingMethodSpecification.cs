namespace PKSim.Core.Services
{
   public class KeepScalingMethodSpecification : IScalingMethodSpecification
   {
      public bool IsSatisfiedBy(ParameterScaling parameterScaling)
      {
         //Satisfied by all parameter scaling
         return true;
      }

      public bool IsDefaultFor(ParameterScaling parameterScaling)
      {
         //never the default value
         return false;
      }

      public ScalingMethod Method => new KeepScalingMethod();
   }
}