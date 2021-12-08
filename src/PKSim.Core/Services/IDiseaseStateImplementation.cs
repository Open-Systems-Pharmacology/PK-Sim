using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IDiseaseStateImplementation : ISpecification<DiseaseState>
   {
      /// <summary>
      /// Apply the disease state implementation to an individual in the context of a create individual (it will change the underlying distributions) 
      /// </summary>
      /// <param name="individual">Individual to update</param>
      /// <returns><c>true</c> if the algorithm could be applied otherwise <c>false</c> </returns>
      bool ApplyTo(Individual individual);


      /// <summary>
      /// Apply the disease state implementation to an individual in the context of a population creation (it will not change the underlying distributions or formula) 
      /// </summary>
      /// <param name="individual">Individual to update</param>
      /// <returns><c>true</c> if the algorithm could be applied otherwise <c>false</c> </returns>
      bool ApplyForPopulationTo(Individual individual);

      /// <summary>
      /// Returns an individual that can be used as based when creating a population with disease state
      /// </summary>
      /// <param name="originalIndividual">Original individual selected by the user as based individual</param>
      /// <returns></returns>
      Individual CreateBaseIndividualForPopulation(Individual originalIndividual);

      void ResetParametersAfterPopulationIteration(Individual individual);
   }

   public class HealthyDiseaseStateImplementation : IDiseaseStateImplementation
   {
      public bool ApplyTo(Individual individual)
      {
         //nothing to do here
         return true;
      }

      public bool ApplyForPopulationTo(Individual individual) => ApplyTo(individual);

      public Individual CreateBaseIndividualForPopulation(Individual originalIndividual)
      {
         //for healthy population, we use this individual as is
         return originalIndividual;
      }

      public void ResetParametersAfterPopulationIteration(Individual individual)
      {
         //nothing to do here
      }

      public bool IsSatisfiedBy(DiseaseState item) => false;
   }
}