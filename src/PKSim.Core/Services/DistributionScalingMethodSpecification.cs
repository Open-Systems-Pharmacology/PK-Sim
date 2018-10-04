namespace PKSim.Core.Services
{
   public class DistributionScalingMethodSpecification : IScalingMethodSpecification
   {
      private readonly IParameterTask _parameterTask;

      public DistributionScalingMethodSpecification(IParameterTask parameterTask)
      {
         _parameterTask = parameterTask;
      }

      public bool IsSatisfiedBy(ParameterScaling parameterScaling)
      {
         return parameterScaling.IsDistributedScaling;
      }

      public ScalingMethod Method => new DistributionScalingMethod(_parameterTask);

      public bool IsDefaultFor(ParameterScaling parameterScaling)
      {
         //default value for all distribution
         return parameterScaling.IsDistributedScaling;
      }
   }
}