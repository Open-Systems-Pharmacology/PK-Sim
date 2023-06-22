using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Maths.Interpolations;

namespace PKSim.Core.Model
{
   public class LogNormalDistributionFormulaSpecificationFactory : IDistributionFormulaSpecificationFactory
   {
      private readonly IInterpolation _interpolation;
      private readonly OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory _distributionFormulaFactory;

      public LogNormalDistributionFormulaSpecificationFactory(IInterpolation interpolation,
         OSPSuite.Core.Domain.Formulas.IDistributionFormulaFactory distributionFormulaFactory)
      {
         _interpolation = interpolation;
         _distributionFormulaFactory = distributionFormulaFactory;
      }

      public DistributionFormula CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, OriginData originData)
      {
         UpdateDistributionBasedOn(distributions, parameter, null, originData);
         return _distributionFormulaFactory.CreateLogNormalDistributionFormulaFor(parameter, parameter.MeanParameter, parameter.DeviationParameter);
      }

      public DistributionFormula CreateFor(IDistributionMetaData distribution, IDistributedParameter parameter)
      {
         parameter.MeanParameter.Value = distribution.Mean;
         parameter.DeviationParameter.Value = distribution.Deviation;
         return _distributionFormulaFactory.CreateLogNormalDistributionFormulaFor(parameter, parameter.MeanParameter, parameter.DeviationParameter);
      }

      public void UpdateDistributionBasedOn(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, IDistributedParameter baseParameter, OriginData originData)
      {
         var knownSamples = from distribution in distributions
            select new
            {
               Mean = new Sample(distribution.Age, distribution.Mean),
               Std = new Sample(distribution.Age, distribution.Deviation)
            };

         knownSamples = knownSamples.ToList();
         parameter.MeanParameter.Value = _interpolation.Interpolate(knownSamples.Select(item => item.Mean), originData.Age.Value);
         parameter.DeviationParameter.Value = _interpolation.Interpolate(knownSamples.Select(item => item.Std), originData.Age.Value);
         parameter.ScaleDistributionBasedOn(baseParameter);
      }

      public bool IsSatisfiedBy(IEnumerable<ParameterDistributionMetaData> distributions) => distributions.All(IsSatisfiedBy);

      public bool IsSatisfiedBy(IDistributionMetaData distribution) => distribution.Distribution == DistributionType.LogNormal;
   }
}