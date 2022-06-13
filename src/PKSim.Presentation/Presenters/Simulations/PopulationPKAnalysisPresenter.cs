using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IPopulationPKAnalysisPresenter : IPKAnalysisPresenter
   {
      void CalculatePKAnalysisOnCurves(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData);

      void CalculatePKAnalysisOnIndividuals(IPopulationDataCollector populationDataCollector, IEnumerable<PopulationPKAnalysis> pks);
   }

   public class PopulationPKAnalysisPresenter : PKAnalysisPresenter<IPopulationPKAnalysisView, IPopulationPKAnalysisPresenter>, IPopulationPKAnalysisPresenter
   {
      private readonly IPKAnalysesTask _pkAnalysesTask;
      private readonly IPKAnalysisExportTask _exportTask;
      private readonly List<PopulationPKAnalysis> _allAnalysesOnCurves;
      private readonly List<PopulationPKAnalysis> _allAnalysesOnIndividuals;
      private List<PopulationPKAnalysis> _allPKAnalysesOnCurves = new List<PopulationPKAnalysis>();
      private List<PopulationPKAnalysis> _allPKAnalysesOnIndividuals = new List<PopulationPKAnalysis>();
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

      public void CalculatePKAnalysisOnIndividuals(IPopulationDataCollector populationDataCollector, IEnumerable<PopulationPKAnalysis> pks)
      {
         CalculatePKAnalysis(populationDataCollector, _allAnalysesOnIndividuals, pks, _allPKAnalysesOnIndividuals);
      }

      private void CalculatePKAnalysis(IPopulationDataCollector populationDataCollector, List<PopulationPKAnalysis> allAnalysis, IEnumerable<PopulationPKAnalysis> sourcePKs, List<PopulationPKAnalysis> targetPKs)
      {
         allAnalysis.Clear();
         _populationDataCollector = populationDataCollector;
         targetPKs.Clear();
         targetPKs.AddRange(sourcePKs);
         allAnalysis.AddRange(targetPKs);
         LoadPreferredUnitsForPKAnalysis();
         BindToPKAnalysis();
         _globalPKAnalysisPresenter.CalculatePKAnalysis(new Simulation[] { populationDataCollector as Simulation });
      }

      protected override void BindToPKAnalysis()
      {
         _view.BindTo(_populationPKAnalysisToDTOMapper.MapFrom(_allAnalysesOnCurves), _populationPKAnalysisToDTOMapper.MapFrom(_allAnalysesOnIndividuals));
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
   }
}