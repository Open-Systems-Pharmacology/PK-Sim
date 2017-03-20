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

      public ScalingMethod Method
      {
         get { return new DistributionScalingMethod(_parameterTask); }
      }

      public bool IsDefaultFor(ParameterScaling parameterSatifyingSpecification)
      {
         //default value for all distribution
         return parameterSatifyingSpecification.IsDistributedScaling;
      }
   }
}