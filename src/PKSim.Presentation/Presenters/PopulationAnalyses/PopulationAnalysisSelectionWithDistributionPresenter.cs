using OSPSuite.Utility.Events;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public abstract class PopulationAnalysisSelectionWithDistributionPresenter<TView, TPresenter> : AbstractSubPresenter<TView, TPresenter>,
      IPopulationAnalysisItemPresenter,
      IListener<FieldUnitChangedInPopulationAnalysisEvent>,
      IListener<FieldRenamedInPopulationAnalysisEvent>
      where TView : IView<TPresenter> where TPresenter : IPresenter
   {
      protected readonly IPopulationAnalysisFieldDistributionPresenter _populationAnalysisFieldDistributionPresenter;
      protected IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysis _populationAnalysis;

      protected PopulationAnalysisSelectionWithDistributionPresenter(TView view, IPopulationAnalysisFieldDistributionPresenter populationAnalysisFieldDistributionPresenter) : base(view)
      {
         _populationAnalysisFieldDistributionPresenter = populationAnalysisFieldDistributionPresenter;
         AddSubPresenters(_populationAnalysisFieldDistributionPresenter);
      }

      private bool canHandle(PopulationAnalysisFieldEvent eventToHandle)
      {
         return Equals(eventToHandle.PopulationAnalysis, _populationAnalysis);
      }

      public void Handle(FieldUnitChangedInPopulationAnalysisEvent eventToHandle)
      {
         if (!canHandle(eventToHandle))
            return;

         RedrawDistribution(eventToHandle);
      }

      public void Handle(FieldRenamedInPopulationAnalysisEvent eventToHandle)
      {
         if (!canHandle(eventToHandle))
            return;

         RedrawDistribution(eventToHandle);
      }

      protected abstract void RedrawDistribution(PopulationAnalysisFieldEvent eventToHandle);

      public virtual void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationDataCollector = populationDataCollector;
         _populationAnalysis = populationAnalysis;
      }

      protected void DrawDistributionFor(PopulationAnalysisDerivedField derivedField)
      {
         _populationAnalysisFieldDistributionPresenter.Plot(_populationDataCollector, derivedField, _populationAnalysis);
      }

      protected void ClearDistribution()
      {
         _populationAnalysisFieldDistributionPresenter.ResetPlot();
      }
   }
}