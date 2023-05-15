using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class DiseaseStateImplementationRepository : IDiseaseStateImplementationRepository
   {
      private readonly IRepository<IDiseaseStateImplementation> _diseaseStateImplementations;

      public DiseaseStateImplementationRepository(IRepository<IDiseaseStateImplementation> diseaseStateImplementations)
      {
         _diseaseStateImplementations = diseaseStateImplementations;
      }

      public IDiseaseStateImplementation FindFor(DiseaseState diseaseState)
      {
         //We use all here to ensure that we get new implementations every single time
         var implementation = _diseaseStateImplementations.All().SingleOrDefault(x => x.IsSatisfiedBy(diseaseState));
         return implementation ?? new HealthyDiseaseStateImplementation();
      }

      public IDiseaseStateImplementation FindFor(Individual individual) => FindFor(individual.OriginData.DiseaseState);
   }
}