using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationSimulationComparisonItemPresenter : ISubPresenter
   {
      void EditComparison(PopulationSimulationComparison comparison);
   }

   public interface IPopulationSimulationComparisonPresenter : IEditAnalyzablePresenter<PopulationSimulationComparison>
   {
   }

   public class PopulationSimulationComparisonPresenter : EditAnalyzablePresenter<IPopulationSimulationComparisonView, IPopulationSimulationComparisonPresenter, PopulationSimulationComparison, IPopulationSimulationComparisonItemPresenter>, IPopulationSimulationComparisonPresenter
   {
      public PopulationSimulationComparisonPresenter(IPopulationSimulationComparisonView view, ISubPresenterItemManager<IPopulationSimulationComparisonItemPresenter> subPresenterItemManager, ISimulationAnalysisPresenterFactory simulationAnalysisPresenterFactory,
         ISimulationAnalysisPresenterContextMenuFactory contextMenuFactory, IPresentationSettingsTask presentationSettingsTask, ISimulationAnalysisCreator simulationAnalysisCreator)
         : base(view, subPresenterItemManager, PopulationSimulationComparisonItems.All, simulationAnalysisPresenterFactory, contextMenuFactory, presentationSettingsTask, simulationAnalysisCreator)
      {
      }

      protected override void UpdateCaption()
      {
         _view.Caption = Analyzable.Name;
      }

      protected override void InitializeSubPresentersWith(PopulationSimulationComparison comparison)
      {
         _subPresenterItemManager.AllSubPresenters.Each(x => x.EditComparison(comparison));
      }

      protected override bool CanHandle(AnalysableEvent analysableEvent)
      {
         var populationSimulation = analysableEvent.Analysable as PopulationSimulation;
         return populationSimulation != null && Analyzable.HasSimulation(populationSimulation);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.EditAnalyzablePresenter;
   }
}