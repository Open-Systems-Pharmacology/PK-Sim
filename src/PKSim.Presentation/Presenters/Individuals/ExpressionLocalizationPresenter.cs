using System;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IExpressionLocalizationPresenter : IPresenter<IExpressionLocalizationView>
   {
      void UpdateLocalization(Localization localization, bool selected);
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
      private IndividualProtein _individualProtein;
      private TSimulationSubject _simulationSubject;
      public event EventHandler LocalizationChanged = delegate { };

      public ExpressionLocalizationPresenter(IExpressionLocalizationView view, IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask) :
         base(view)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
      }

      public void UpdateLocalization(Localization localization, bool selected)
      {
         AddCommand(_moleculeExpressionTask.SetExpressionLocalizationFor(_individualProtein, localization, _simulationSubject));
         LocalizationChanged(this, EventArgs.Empty);
      }

      public void Edit(IndividualProtein individualProtein, TSimulationSubject simulationSubject)
      {
         _individualProtein = individualProtein;
         _simulationSubject = simulationSubject;
         _view.BindTo(individualProtein);
      }

   }
}