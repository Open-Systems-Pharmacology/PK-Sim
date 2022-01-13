using OSPSuite.Core.Domain;
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


      /// <summary>
      /// Ensures that some parameters that might have been overwritten by the algorithm are reset (distributions or formula)
      /// </summary>
      void ResetParametersAfterPopulationIteration(Individual individual);

      /// <summary>
      /// Validates that the parameters are compatible with the underlying disease state (age constraints etc..).
      /// Throws an exception if the origin data is not valid
      /// </summary>
      void Validate(OriginData originData);

      /// <summary>
      /// Returns <c>true</c> if the parameters are compatible with th underlying disease state otherwise <c>false</c>
      /// If the <paramref name="originData"/> is not valid, the return value will contain the reason in error
      /// </summary>
      (bool isValid, string error) IsValid(OriginData originData);
      
      /// <summary>
      /// Apply any change required to the disease factor parameter associated with the molecule
      /// </summary>
      void ApplyTo(IndividualMolecule individualMolecule);

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

      public void Validate(OriginData originData)
      {
         //nothing to do here
      }

      public (bool isValid, string error) IsValid(OriginData originData)
      {
         return (true, string.Empty);
      }

      public void ApplyTo(IndividualMolecule individualMolecule)
      {
         //nothing to do here
      }

      public bool IsSatisfiedBy(DiseaseState item) => false;
   }
}