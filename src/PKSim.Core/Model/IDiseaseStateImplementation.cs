using OSPSuite.Utility;

namespace PKSim.Core.Model
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