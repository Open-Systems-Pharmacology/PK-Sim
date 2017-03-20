namespace PKSim.Core.Services
{
   public class OverrideScalingMethodSpecification : IScalingMethodSpecification
   {
      private readonly IParameterTask _parameterTask;

      public OverrideScalingMethodSpecification(IParameterTask parameterTask)
      {
         _parameterTask = parameterTask;
      }

      public bool IsSatisfiedBy(ParameterScaling parameterScaling)
      {
         //Satisfied by all parameter scaling
         return true;
      }

      public bool IsDefaultFor(ParameterScaling parameterSatifyingSpecification)
      {
         //never the default value
         return false;
      }

      public ScalingMethod Method
      {
         get { return new OverrideScalingMethod(_parameterTask); }
      }
   }
}