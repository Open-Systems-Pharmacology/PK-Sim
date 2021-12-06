using System.Linq;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public interface IDiseaseStateImplementationFactory
   {
      IDiseaseStateImplementation CreateFor(DiseaseState diseaseState);
   }

   public class DiseaseStateImplementationFactory : IDiseaseStateImplementationFactory
   {
      private readonly IRepository<IDiseaseStateImplementation> _diseaseStateImplementations;

      public DiseaseStateImplementationFactory(IRepository<IDiseaseStateImplementation> diseaseStateImplementations)
      {
         _diseaseStateImplementations = diseaseStateImplementations;
      }

      public IDiseaseStateImplementation CreateFor(DiseaseState diseaseState)
      {
         //We use all here to ensure that we get new implementations everytime
         var implementation = _diseaseStateImplementations.All().SingleOrDefault(x => x.IsSatisfiedBy(diseaseState));
         return implementation ?? new HealthyDiseaseStateImplementation();
      }

      public IDiseaseStateImplementation CreateFor(Individual individual)
      {
         return CreateFor(individual.OriginData.DiseaseState);
      }
   }
}