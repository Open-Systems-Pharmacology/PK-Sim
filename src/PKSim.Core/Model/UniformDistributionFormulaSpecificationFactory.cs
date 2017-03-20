using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Maths.Interpolations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public class UniformDistributionFormulaSpecificationFactory : IDistributionFormulaSpecificationFactory
   {
      private readonly IInterpolation _interpolation;
      private readonly OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory _distributionFormulaFactory;

      public UniformDistributionFormulaSpecificationFactory(IInterpolation interpolation,
                                                            OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory distributionFormulaFactory)
      {
         _interpolation = interpolation;
         _distributionFormulaFactory = distributionFormulaFactory;
      }

      public IDistributionFormula CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, OriginData originData)
      {
         UpdateDistributionBasedOn(distributions, parameter, null, originData);
         return _distributionFormulaFactory.CreateUniformDistributionFormulaFor(parameter, parameter.Parameter(Constants.Distribution.MINIMUM), parameter.Parameter(Constants.Distribution.MAXIMUM));
      }

      public IDistributionFormula CreateFor(IDistributionMetaData distribution, IDistributedParameter parameter)
      {
         var minParameter = parameter.Parameter(Constants.Distribution.MINIMUM);
         var maxParameter = parameter.Parameter(Constants.Distribution.MAXIMUM);
         var distributionMetaData = distribution as ParameterDistributionMetaData;
         if (distributionMetaData != null)
         {
            minParameter.Value = distributionMetaData.MinValue.Value;
            maxParameter.Value = distributionMetaData.MaxValue.Value;
         }
         return _distributionFormulaFactory.CreateUniformDistributionFormulaFor(parameter, minParameter, maxParameter);
      }

      public void UpdateDistributionBasedOn(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, IDistributedParameter baseParameter, OriginData originData)
      {
         var knownSamples = from distribution in distributions
                            select new
                               {
                                  Min = new Sample(distribution.Age, distribution.MinValue.Value),
                                  Max = new Sample(distribution.Age, distribution.MaxValue.Value),
                               };

         knownSamples = knownSamples.ToList();
         parameter.Parameter(Constants.Distribution.MINIMUM).Value = _interpolation.Interpolate(knownSamples.Select(item => item.Min), originData.Age.Value);
         parameter.Parameter(Constants.Distribution.MAXIMUM).Value = _interpolation.Interpolate(knownSamples.Select(item => item.Max), originData.Age.Value);
         parameter.IsFixedValue = false;
      }

      public bool IsSatisfiedBy(IEnumerable<ParameterDistributionMetaData> distributions)
      {
         return distributions.All(IsSatisfiedBy);
      }

      public bool IsSatisfiedBy(IDistributionMetaData distribution)
      {
         return distribution.Distribution == DistributionTypes.Uniform;
      }
   }
}