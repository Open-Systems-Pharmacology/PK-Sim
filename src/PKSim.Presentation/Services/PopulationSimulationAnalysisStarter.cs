using System;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class PopulationSimulationAnalysisStarter : IPopulationSimulationAnalysisStarter
   {
      private readonly IApplicationController _applicationController;
      private readonly ICloner _cloner;

      public PopulationSimulationAnalysisStarter(IApplicationController applicationController, ICloner cloner)
      {
         _applicationController = applicationController;
         _cloner = cloner;
      }

      public PopulationAnalysisChart CreateAnalysisForPopulationSimulation(IPopulationDataCollector populationDataCollector, PopulationAnalysisType populationAnalysisType)
      {
         switch (populationAnalysisType)
         {
            case PopulationAnalysisType.TimeProfile:
               return createAnalysis<ICreateTimeProfileAnalysisPresenter>(populationDataCollector);
            case PopulationAnalysisType.BoxWhisker:
               return createAnalysis<ICreateBoxWhiskerAnalysisPresenter>(populationDataCollector);
            case PopulationAnalysisType.Scatter:
               return createAnalysis<ICreateScatterAnalysisPresenter>(populationDataCollector);
            case PopulationAnalysisType.Range:
               return createAnalysis<ICreateRangeAnalysisPresenter>(populationDataCollector);
            default:
               throw new ArgumentOutOfRangeException(nameof(populationAnalysisType));
         }
      }

      public PopulationAnalysisChart EditAnalysisForPopulationSimulation(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart populationAnalysisChart)
      {
         switch (populationAnalysisChart.AnalysisType)
         {
            case PopulationAnalysisType.TimeProfile:
               return editAnalysis<ICreateTimeProfileAnalysisPresenter>(populationDataCollector, populationAnalysisChart);
            case PopulationAnalysisType.BoxWhisker:
               return editAnalysis<ICreateBoxWhiskerAnalysisPresenter>(populationDataCollector, populationAnalysisChart);
            case PopulationAnalysisType.Scatter:
               return editAnalysis<ICreateScatterAnalysisPresenter>(populationDataCollector, populationAnalysisChart);
            case PopulationAnalysisType.Range:
               return editAnalysis<ICreateRangeAnalysisPresenter>(populationDataCollector, populationAnalysisChart);
            default:
               throw new ArgumentOutOfRangeException(nameof(populationAnalysisChart.AnalysisType));
         }
      }

      private PopulationAnalysisChart editAnalysis<TPresenter>(IPopulationDataCollector populationDataCollector, PopulationAnalysisChart populationAnalysisChart) where TPresenter : ICreatePopulationAnalysisPresenter
      {
         using (var presenter = _applicationController.Start<TPresenter>())
         {
            var clone = _cloner.Clone(populationAnalysisChart);
            if (!presenter.Edit(populationDataCollector, clone))
               return populationAnalysisChart;

            //swap out analysis with clone\
            populationDataCollector.RemoveAnalysis(populationAnalysisChart);
            populationDataCollector.AddAnalysis(clone);

            return clone;
         }
      }

      private PopulationAnalysisChart createAnalysis<TPresenter>(IPopulationDataCollector populationDataCollector) where TPresenter : ICreatePopulationAnalysisPresenter
      {
         using (var presenter = _applicationController.Start<TPresenter>())
         {
            return presenter.Create(populationDataCollector);
         }
      }
   }
}