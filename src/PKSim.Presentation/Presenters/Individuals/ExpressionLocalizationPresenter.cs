using System;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IExpressionLocalizationPresenter : IPresenter<IExpressionLocalizationView>, ICommandCollectorPresenter
   {
      /// <summary>
      /// Updates the localization for a specific localization flag. 
      /// </summary>
      /// <param name="localization">Localization being turned on or off</param>
      /// <param name="selected"><c>True</c> if the localization is selected otherwise <c>False</c></param>
      /// <returns><c>True</c> if the localization was updated otherwise <c>False</c> </returns>
      bool UpdateLocalization(Localization localization, bool selected);
      event EventHandler LocalizationChanged;
   }

   public interface IExpressionLocalizationPresenter<in TSimulationSubject> : IExpressionLocalizationPresenter
      where TSimulationSubject : ISimulationSubject
   {
      void Edit(IndividualProtein individualProtein, TSimulationSubject simulationSubject);
   }

   public class ExpressionLocalizationPresenter<TSimulationSubject> :
      AbstractCommandCollectorPresenter<IExpressionLocalizationView, IExpressionLocalizationPresenter>,
      IExpressionLocalizationPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IMoleculeExpressionTask<TSimulationSubject> _moleculeExpressionTask;
      private readonly IDialogCreator _dialogCreator;
      private IndividualProtein _individualProtein;
      private TSimulationSubject _simulationSubject;
      public event EventHandler LocalizationChanged = delegate { };

      public ExpressionLocalizationPresenter(
         IExpressionLocalizationView view,
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IDialogCreator dialogCreator) :
         base(view)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _dialogCreator = dialogCreator;
      }

      public bool UpdateLocalization(Localization localization, bool selected)
      {
         //localization value is being deselected. If this action would result in a category being reset
         //for example interstitial is deselected and intracellular is being deselected, we warn the user
         if (!selected)
         {
            var shouldContinue = notifyPossibleResetOfExpressionFor(localization, _individualProtein.Localization);
            if (!shouldContinue)
               return false;
         }

         AddCommand(_moleculeExpressionTask.SetExpressionLocalizationFor(_individualProtein, localization, _simulationSubject));
         LocalizationChanged(this, EventArgs.Empty);
         return true;
      }

      private bool notifyPossibleResetOfExpressionFor(Localization newLocalization, Localization currentLocalization)
      {
         var updatedLocalization = currentLocalization ^ newLocalization;

         return notifyPossibleResetFor(Localization.InTissue) &&
                notifyPossibleResetFor(Localization.InBloodCells) &&
                notifyPossibleResetFor(Localization.InVascularEndothelium);

         bool notifyPossibleResetFor(Localization globalLocalization)
         {
            //This would result in the localization being reset
            if (!newLocalization.Is(globalLocalization) || updatedLocalization.Is(globalLocalization))
               return true;

            var result = _dialogCreator.MessageBoxYesNo(PKSimConstants.Warning.ExpressionParametersWillBeReset);
            return result == ViewResult.Yes;
         }
      }

      public void Edit(IndividualProtein individualProtein, TSimulationSubject simulationSubject)
      {
         _individualProtein = individualProtein;
         _simulationSubject = simulationSubject;
         _view.BindTo(_individualProtein);
      }
   }
}