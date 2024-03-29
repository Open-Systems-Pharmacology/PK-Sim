﻿using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Collections;
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
      private readonly List<PopulationPKAnalysis> _allPKAnalysesAggregatedPKValues = new List<PopulationPKAnalysis>();
      private readonly List<PopulationPKAnalysis> _allPKAnalysesIndividualPKValues = new List<PopulationPKAnalysis>();
      private IPopulationDataCollector _populationDataCollector;
      private readonly IPopulationPKAnalysisToPKAnalysisDTOMapper _populationPKAnalysisToDTOMapper;

      public PopulationPKAnalysisPresenter(IPopulationPKAnalysisView view, IPKAnalysesTask pkAnalysesTask,
         IPKAnalysisExportTask exportTask, IPopulationPKAnalysisToPKAnalysisDTOMapper populationPKAnalysisToDTOMapper,
         IPKParameterRepository pkParameterRepository, IPresentationSettingsTask presentationSettingsTask,
         IGlobalPKAnalysisPresenter globalPKAnalysisPresenter)
         : base(view, pkParameterRepository, presentationSettingsTask, globalPKAnalysisPresenter)
      {
         _pkAnalysesTask = pkAnalysesTask;
         _exportTask = exportTask;
         _populationPKAnalysisToDTOMapper = populationPKAnalysisToDTOMapper;
      }

      public void CalculatePKAnalyses(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData, PopulationStatisticalAnalysis populationAnalysis)
      {
         _populationDataCollector = populationDataCollector;
         _allPKAnalysesAggregatedPKValues.Clear();
         _allPKAnalysesIndividualPKValues.Clear();
         var supportsPKAnalysisIndividualPKValues = _populationDataCollector.SupportsMultipleAggregations;
         View.ShowPKAnalysisIndividualPKValues(supportsPKAnalysisIndividualPKValues);

         //Calculate based on curves
         _allPKAnalysesAggregatedPKValues.AddRange(_pkAnalysesTask.CalculateFor(populationDataCollector, timeProfileChartData));

         if (!supportsPKAnalysisIndividualPKValues)
         {
            updateView();
            return;
         }

         //We downcast here as this should only be made available for simulations and not comparisons
         var simulation = populationDataCollector.DowncastTo<PopulationSimulation>();

         //Calculate first global PK that should always be updated
         _globalPKAnalysisPresenter.CalculatePKAnalysis(new[] { simulation });

         _view.GlobalPKVisible = _globalPKAnalysisPresenter.CanCalculateGlobalPK();

         var pkParametersCache = extractPKParameters(populationAnalysis, simulation);
         if (!pkParametersCache.Any() || pkParametersCache.All(x => !x.Any()))
            return;

         pkParametersCache.KeyValues.Each(pkParameters =>
         {
            var pkAnalyses = _pkAnalysesTask.AggregatePKAnalysis(simulation, pkParameters.Value, populationAnalysis.SelectedStatistics, pkParameters.Key);
            _allPKAnalysesIndividualPKValues.AddRange(pkAnalyses);
         });

         updateView();
      }

      private Cache<string, IReadOnlyList<QuantityPKParameter>> extractPKParameters(PopulationStatisticalAnalysis populationAnalysis, PopulationSimulation populationSimulation)
      {
         var cache = new Cache<string, IReadOnlyList<QuantityPKParameter>>();
         var fields = populationAnalysis.AllFields.OfType<PopulationAnalysisOutputField>();
         fields.Each(field => cache.Add(field.Name, populationSimulation.PKAnalyses.AllPKParametersFor(field.QuantityPath)));
         return cache;
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
            AggregatedPKValues = _populationPKAnalysisToDTOMapper.MapFrom(_allPKAnalysesAggregatedPKValues),
            IndividualPKValues = _populationPKAnalysisToDTOMapper.MapFrom(_allPKAnalysesIndividualPKValues)
         });
      }

      protected override IEnumerable<PKAnalysis> AllPKAnalyses => (View.IsAggregatedPKValuesSelected ? _allPKAnalysesAggregatedPKValues : _allPKAnalysesIndividualPKValues).Select(x => x.PKAnalysis);

      public override void ExportToExcel()
      {
         _exportTask.ExportToExcel(_view.GetSummaryData(), _populationDataCollector.Name);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.PopulationPKAnalysisPresenter;
   }
}