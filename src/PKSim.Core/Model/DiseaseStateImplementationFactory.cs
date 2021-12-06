using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public interface IDiseaseStateImplementationFactory
   {
      IDiseaseStateImplementation CreateFor(DiseaseState diseaseState);
      IDiseaseStateImplementation CreateFor(Individual individual);
   }

   public class DiseaseStateImplementationFactory : IDiseaseStateImplementationFactory
   {
      private readonly IReadOnlyList<IDiseaseStateImplementationSpecificationFactory> _diseaseStateImplementationFactories;

      public DiseaseStateImplementationFactory(IRepository<IDiseaseStateImplementationSpecificationFactory> diseaseStateImplementationFactories)
      {
         _diseaseStateImplementationFactories = diseaseStateImplementationFactories.All().ToList();
      }

      public IDiseaseStateImplementation CreateFor(DiseaseState diseaseState)
      {
         var factory = _diseaseStateImplementationFactories.FirstOrDefault(x => x.IsSatisfiedBy(diseaseState));
         return factory?.Create() ?? new HealthyDiseaseStateImplementation();
      }

      public IDiseaseStateImplementation CreateFor(Individual individual)
      {
         return CreateFor(individual.OriginData.DiseaseState);
      }
   }
}