using OSPSuite.Utility;

namespace PKSim.Core.Services
{
   public interface IScalingMethodSpecification : ISpecification<ParameterScaling>
   {
      /// <summary>
      ///    Indicates whether the scaling method is the default for the parameter scaling
      /// </summary>
      bool IsDefaultFor(ParameterScaling parameterSatifyingSpecification);

      /// <summary>
      ///    The underlying scaling method
      /// </summary>
      ScalingMethod Method { get; }
   }
}