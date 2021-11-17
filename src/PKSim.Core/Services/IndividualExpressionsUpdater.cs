using System.Linq;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IIndividualExpressionsUpdater
   {
      void Update(Individual sourceIndividual, Individual targetIndividual);
   }

   public class IndividualExpressionsUpdater : IIndividualExpressionsUpdater
   {
      private readonly IMoleculeExpressionTask<Individual> _moleculeExpressionTask;

      public IndividualExpressionsUpdater(IMoleculeExpressionTask<Individual> moleculeExpressionTask)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
      }

      public void Update(Individual sourceIndividual, Individual targetIndividual)
      {
         var sourceSpecies = sourceIndividual.Species;
         var targetSpecies = sourceIndividual.Species;

         //Uses expression profile but not the same species. We show a warning
         if (sourceIndividual.AllExpressionProfiles().Any() && !Equals(sourceSpecies, targetSpecies))
            throw new OSPSuiteException(PKSimConstants.Warning.CannotUseExpressionProfilesDefinedForAnotherSpecies(sourceSpecies.DisplayName, targetSpecies.DisplayName));


         sourceIndividual.AllExpressionProfiles().Each(x => _moleculeExpressionTask.AddExpressionProfile(targetIndividual, x));
      }
   }
}