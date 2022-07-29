using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPopulationPKAnalysisPresenter : IPKAnalysisPresenter
   {
      void CalculatePKAnalyses(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, PopulationStatisticalAnalysis populationAnalysis);
      void HandleTabChanged();
   }

   public class PopulationPKAnalysisPresenter : PKAnalysisPresenter<IPopulationPKAnalysisView, IPopulationPKAnalysisPresenter>, IPopulationPKAnalysisPresenter
   {
      private readonly IPKAnalysesTask _pkAnalysesTask;
      private readonly IPKAnalysisExportTask _exportTask;
      private readonly List<PopulationPKAnalysis> _allPKAnalysesOnCurves = new List<PopulationPKAnalysis>();
      private readonly List<PopulationPKAnalysis> _allPKAnalysesOnIndividuals = new List<PopulationPKAnalysis>();
      private IPopulationDataCollector _populationDataCollector;
      private readonly IPopulationPKAnalysisToPKAnalysisDTOMapper _populationPKAnalysisToDTOMapper;
      private readonly IGlobalPKAnalysisPresenter _globalPKAnalysisPresenter;

      public PopulationPKAnalysisPresenter(IPopulationPKAnalysisView view, IPKAnalysesTask pkAnalysesTask,
         IPKAnalysisExportTask exportTask, IPopulationPKAnalysisToPKAnalysisDTOMapper populationPKAnalysisToDTOMapper,
         IPKParameterRepository pkParameterRepository, IPresentationSettingsTask presentationSettingsTask,
         IGlobalPKAnalysisPresenter globalPKAnalysisPresenter)
         : base(view, pkParameterRepository, presentationSettingsTask)
      {
         _pkAnalysesTask = pkAnalysesTask;
         _exportTask = exportTask;
         _populationPKAnalysisToDTOMapper = populationPKAnalysisToDTOMapper;
         _globalPKAnalysisPresenter = globalPKAnalysisPresenter;
         AddSubPresenters(_globalPKAnalysisPresenter);
         _view.AddGlobalPKAnalysisView(_globalPKAnalysisPresenter.View);
      }

      public void CalculatePKAnalyses(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, PopulationStatisticalAnalysis populationAnalysis)
      {
         _populationDataCollector = populationDataCollector;
         _allPKAnalysesOnCurves.Clear();
         _allPKAnalysesOnIndividuals.Clear();
         var supportsPKAnalysisOnIndividual = _populationDataCollector.SupportsMultipleAggregations;
         View.ShowPKAnalysisOnIndividuals(supportsPKAnalysisOnIndividual);

         //Calculate based on curves
         _allPKAnalysesOnCurves.AddRange(_pkAnalysesTask.CalculateFor(populationDataCollector, timeProfileChartData));

         if (!supportsPKAnalysisOnIndividual)
         {
            updateView();
            return;
         }

         //We downcast here as this should only be made available for simulations and not comparisons
         var simulation = populationDataCollector.DowncastTo<PopulationSimulation>();

         //Calculate first global PK that should always be updated
         _globalPKAnalysisPresenter.CalculatePKAnalysis(new[] { simulation });

         var pkParameters = extractPKParameters(populationAnalysis, simulation);
         if (!pkParameters.Any())
            return;

         var captionPrefix = populationAnalysis.AllFieldNamesOn(PivotArea.DataArea);

         var pkAnalyses = _pkAnalysesTask.AggregatePKAnalysis(simulation, pkParameters, populationAnalysis.SelectedStatistics, captionPrefix[0]);
         _allPKAnalysesOnIndividuals.AddRange(pkAnalyses);

         updateView();
      }


      private IReadOnlyList<QuantityPKParameter> extractPKParameters(PopulationStatisticalAnalysis populationAnalysis, PopulationSimulation populationSimulation)
      {
         var fields = populationAnalysis.AllFields.OfType<PopulationAnalysisOutputField>().Select(x => x.QuantityPath);
         return fields.SelectMany(x => populationSimulation.PKAnalyses.AllPKParametersFor(x)).ToList();
      }

      public bool PKAnalysisOnIndividualsVisible
      {
         set => View.ShowPKAnalysisOnIndividuals(value);
      }

      private void updateView()
      {
         LoadPreferredUnitsForPKAnalysis();
         BindToPKAnalysis();
      }

      public void HandleTabChanged()
      {
         //When changing tabs, the units are not properly shown for those rows in the lower table that are not present in the old tab,
         //so if change from first tab to the second, all those rows for normalized values will not show the unit because they were
         //not shown for the first tab but for the second
         BindToPKAnalysis();
      }

      protected override void BindToPKAnalysis()
      {
         _view.BindTo(new IntegratedPKAnalysisDTO
         {
            OnCurves = _populationPKAnalysisToDTOMapper.MapFrom(_allPKAnalysesOnCurves),
            OnIndividuals = _populationPKAnalysisToDTOMapper.MapFrom(_allPKAnalysesOnIndividuals)
         });
      }

      protected override IEnumerable<PKAnalysis> AllPKAnalyses => (View.IsOnCurvesSelected ? _allPKAnalysesOnCurves : _allPKAnalysesOnIndividuals).Select(x => x.PKAnalysis);

      public override void ExportToExcel()
      {
         _exportTask.ExportToExcel(_view.GetSummaryData(), _populationDataCollector.Name);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.PopulationPKAnalysisPresenter;
   }
}