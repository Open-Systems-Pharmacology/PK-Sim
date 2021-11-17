using System.Linq;
using OSPSuite.Core.Services;
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
      private readonly IDialogCreator _dialogCreator;

      public IndividualExpressionsUpdater(IMoleculeExpressionTask<Individual> moleculeExpressionTask, IDialogCreator dialogCreator)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _dialogCreator = dialogCreator;
      }

      public void Update(Individual sourceIndividual, Individual targetIndividual)
      {
         var sourceSpecies = sourceIndividual.Species;
         var targetSpecies = targetIndividual.Species;

         //Uses expression profile but not the same species. We show a warning
         if (sourceIndividual.AllExpressionProfiles().Any() && !Equals(sourceSpecies, targetSpecies))
         {
            _dialogCreator.MessageBoxInfo(PKSimConstants.Warning.CannotUseExpressionProfilesDefinedForAnotherSpecies(sourceSpecies.DisplayName, targetSpecies.DisplayName));
            return;
         }
            

         sourceIndividual.AllExpressionProfiles().Each(x => _moleculeExpressionTask.AddExpressionProfile(targetIndividual, x));
      }
   }
}