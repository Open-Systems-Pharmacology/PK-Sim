using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Reporting;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IDistributionFormulaFactory
   {
      DistributionFormula CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, OriginData originData);
      DistributionFormula CreateFor(IDistributionMetaData distribution, IDistributedParameter parameter);
      void UpdateDistributionBasedOn(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, IDistributedParameter baseParameter, OriginData originData);
   }

   public interface IDistributionFormulaSpecificationFactory : IDistributionFormulaFactory, ISpecification<IDistributionMetaData>, ISpecification<IEnumerable<ParameterDistributionMetaData>>
   {
   }

   public class DistributionFormulaFactory : IDistributionFormulaFactory
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IEnumerable<IDistributionFormulaSpecificationFactory> _allDistributionFactory;

      public DistributionFormulaFactory(IRepository<IDistributionFormulaSpecificationFactory> repository, IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
         _allDistributionFactory = repository.All();
      }

      public DistributionFormula CreateFor(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, OriginData originData)
      {
         var allDistributions = distributions.ToList();
         foreach (var factory in _allDistributionFactory)
         {
            if (factory.IsSatisfiedBy(allDistributions))
               return factory.CreateFor(allDistributions, parameter, originData);
         }

         throw new DistributionNotFoundException(parameter, _reportGenerator.StringReportFor(originData));
      }

      public DistributionFormula CreateFor(IDistributionMetaData distribution, IDistributedParameter parameter)
      {
         foreach (var factory in _allDistributionFactory)
         {
            if (factory.IsSatisfiedBy(distribution))
               return factory.CreateFor(distribution, parameter);
         }
         throw new DistributionNotFoundException(distribution);
      }

      public void UpdateDistributionBasedOn(IEnumerable<ParameterDistributionMetaData> distributions, IDistributedParameter parameter, IDistributedParameter baseParameter, OriginData originData)
      {
         var allDistributions = distributions.ToList();
         foreach (var factory in _allDistributionFactory)
         {
            if (!factory.IsSatisfiedBy(allDistributions)) continue;
            factory.UpdateDistributionBasedOn(allDistributions, parameter, baseParameter, originData);
            return;
         }
      }
   }
}