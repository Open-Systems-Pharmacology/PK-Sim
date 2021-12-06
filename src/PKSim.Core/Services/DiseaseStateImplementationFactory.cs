using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class DiseaseStateImplementationFactory : IDiseaseStateImplementationFactory
   {
      private readonly IRepository<IDiseaseStateImplementation> _diseaseStateImplementations;

      public DiseaseStateImplementationFactory(IRepository<IDiseaseStateImplementation> diseaseStateImplementations)
      {
         _diseaseStateImplementations = diseaseStateImplementations;
      }

      public IDiseaseStateImplementation CreateFor(DiseaseState diseaseState)
      {
         //We use all here to ensure that we get new implementations every single time
         var implementation = _diseaseStateImplementations.All().SingleOrDefault(x => x.IsSatisfiedBy(diseaseState));
         return implementation ?? new HealthyDiseaseStateImplementation();
      }
   }
}