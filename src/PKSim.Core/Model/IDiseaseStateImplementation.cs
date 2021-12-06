using OSPSuite.Utility;

namespace PKSim.Core.Model
{
   public interface IDiseaseStateImplementation
   {
      void ApplyTo(Individual individual);
   }

   public interface IDiseaseStateImplementationSpecificationFactory: ISpecification<DiseaseState>
   {
      IDiseaseStateImplementation Create();
   }

   public class HealthyDiseaseStateImplementation : IDiseaseStateImplementation
   {
      public void ApplyTo(Individual individual)
      {
         //nothing to do here
      }
   }
}