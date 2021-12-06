using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IDiseaseStateImplementation : ISpecification<DiseaseState>
   {
      void ApplyTo(Individual individual);
   }

   public class HealthyDiseaseStateImplementation : IDiseaseStateImplementation
   {
      public void ApplyTo(Individual individual)
      {
         //nothing to do here
      }

      public bool IsSatisfiedBy(DiseaseState item) => false;
   }
}