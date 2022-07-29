using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Presentation.Services;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPopulationPKAnalysisPresenter : IPKAnalysisPresenter
   {
      void CalculatePKAnalysisOnCurves(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData);
      bool PKAnalysisOnIndividualsEnabled { get; set; }
      void CalculatePKAnalysisOnIndividuals(IPopulationDataCollector populationDataCollector, IEnumerable<PopulationPKAnalysis> pks);
      void HandleTabChanged();
      bool SupportsDifferentAggregations();
   }

   public class PopulationPKAnalysisPresenter : PKAnalysisPresenter<IPopulationPKAnalysisView, IPopulationPKAnalysisPresenter>, IPopulationPKAnalysisPresenter
   {
      private readonly IPKAnalysesTask _pkAnalysesTask;
      private readonly IPKAnalysisExportTask _exportTask;
      private readonly List<PopulationPKAnalysis> _allAnalysesOnCurves;
      private readonly List<PopulationPKAnalysis> _allAnalysesOnIndividuals;
      private readonly List<PopulationPKAnalysis> _allPKAnalysesOnCurves = new List<PopulationPKAnalysis>();
      private readonly List<PopulationPKAnalysis> _allPKAnalysesOnIndividuals = new List<PopulationPKAnalysis>();
      private IPopulationDataCollector _populationDataCollector;
      private readonly IPopulationPKAnalysisToPKAnalysisDTOMapper _populationPKAnalysisToDTOMapper;
      private readonly IGlobalPKAnalysisPresenter _globalPKAnalysisPresenter;
      private bool _pkAnalysisOnIndividualsEnabled = true;

      public PopulationPKAnalysisPresenter(IPopulationPKAnalysisView view, IPKAnalysesTask pkAnalysesTask,
         IPKAnalysisExportTask exportTask, IPopulationPKAnalysisToPKAnalysisDTOMapper populationPKAnalysisToDTOMapper,
         IPKParameterRepository pkParameterRepository, IPresentationSettingsTask presentationSettingsTask,
         IGlobalPKAnalysisPresenter globalPKAnalysisPresenter)
         : base(view, pkParameterRepository, presentationSettingsTask)
      {
         _pkAnalysesTask = pkAnalysesTask;
         _exportTask = exportTask;
         _allAnalysesOnCurves = new List<PopulationPKAnalysis>();
         _allAnalysesOnIndividuals = new List<PopulationPKAnalysis>();
         _populationPKAnalysisToDTOMapper = populationPKAnalysisToDTOMapper;
         _globalPKAnalysisPresenter = globalPKAnalysisPresenter;
         AddSubPresenters(_globalPKAnalysisPresenter);
         _view.AddGlobalPKAnalysisView(_globalPKAnalysisPresenter.View);
      }

      public void CalculatePKAnalysisOnCurves(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData)
      {
         CalculatePKAnalysis(populationDataCollector, _allAnalysesOnCurves, _pkAnalysesTask.CalculateFor(populationDataCollector, timeProfileChartData), _allPKAnalysesOnCurves);
      }

      public void CalculatePKAnalysisOnIndividuals(IPopulationDataCollector populationDataCollector, IEnumerable<PopulationPKAnalysis> pkAnalyses)
      {
         CalculatePKAnalysis(populationDataCollector, _allAnalysesOnIndividuals, pkAnalyses, _allPKAnalysesOnIndividuals);
      }

      public bool PKAnalysisOnIndividualsEnabled
      {
         get { return _pkAnalysisOnIndividualsEnabled; }
         set
         {
            _pkAnalysisOnIndividualsEnabled = value;
            View.EnablePKAnalysisOnIndividualsTab(value);
         }
      }

      private void CalculatePKAnalysis(IPopulationDataCollector populationDataCollector, List<PopulationPKAnalysis> allAnalysis, IEnumerable<PopulationPKAnalysis> sourcePKAnalyses, List<PopulationPKAnalysis> targetPKAnalyses)
      {
         allAnalysis.Clear();
         _populationDataCollector = populationDataCollector;
         targetPKAnalyses.Clear();
         targetPKAnalyses.AddRange(sourcePKAnalyses);
         allAnalysis.AddRange(targetPKAnalyses);
         LoadPreferredUnitsForPKAnalysis();
         BindToPKAnalysis();
         _globalPKAnalysisPresenter.CalculatePKAnalysis(new Simulation[] {populationDataCollector as Simulation});
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
         _view.BindTo(new IntegratedPKAnalysisDTO()
         {
            OnCurves = _populationPKAnalysisToDTOMapper.MapFrom(_allAnalysesOnCurves),
            OnIndividuals = _populationPKAnalysisToDTOMapper.MapFrom(_allAnalysesOnIndividuals)
         });
      }

      protected override IEnumerable<PKAnalysis> AllPKAnalyses
      {
         get { return (View.IsOnCurvesSelected ? _allPKAnalysesOnCurves : _allPKAnalysesOnIndividuals).Select(x => x.PKAnalysis); }
      }

      public override void ExportToExcel()
      {
         _exportTask.ExportToExcel(_view.GetSummaryData(), _populationDataCollector.Name);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.PopulationPKAnalysisPresenter;

      public bool SupportsDifferentAggregations()
      {
         return _pkAnalysesTask?.PopulationDataCollectorSupportsDifferentAggregations(_populationDataCollector) ?? false;
      }
   }
}