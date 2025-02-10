using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public class DiscreteDistributionFormulaSpecificationFactory : IDistributionFormulaSpecificationFactory
   {
      private readonly IInterpolation _interpolation;
      private readonly OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory _distributionFormulaFactory;

      public DiscreteDistributionFormulaSpecificationFactory(IInterpolation interpolation,
                                                             OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory distributionFormulaFactory)
      {
         _interpolation = interpolation;
         _distributionFormulaFactory = distributionFormulaFactory;
      }

      public DistributionFormula CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, OriginData originData)
      {
         UpdateDistributionBasedOn(distributions, parameter, null, originData);
         return _distributionFormulaFactory.CreateDiscreteDistributionFormulaFor(parameter, parameter.MeanParameter);
      }

      public void UpdateDistributionBasedOn(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, IDistributedParameter baseParameter, OriginData originData)
      {
         var knownSamples = from distribution in distributions
                            select new Sample(distribution.Age, distribution.Mean);

         parameter.MeanParameter.Value = _interpolation.Interpolate(knownSamples, originData.Age.Value);
         parameter.ScaleDistributionBasedOn(baseParameter);
      }

      public DistributionFormula CreateFor(IDistributionMetaData distribution, IDistributedParameter parameter)
      {
         parameter.MeanParameter.Value = distribution.Mean;
         return _distributionFormulaFactory.CreateDiscreteDistributionFormulaFor(parameter, parameter.MeanParameter);
      }

      public bool IsSatisfiedBy(IEnumerable<ParameterDistributionMetaData> distributions) => distributions.All(IsSatisfiedBy);

      public bool IsSatisfiedBy(IDistributionMetaData distribution) => distribution.Distribution == DistributionType.Discrete;
   }
}