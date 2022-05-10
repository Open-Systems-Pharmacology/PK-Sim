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
      void CalculatePKAnalysis(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData);
   }

   public class PopulationPKAnalysisPresenter : PKAnalysisPresenter<IPopulationPKAnalysisView, IPopulationPKAnalysisPresenter>, IPopulationPKAnalysisPresenter
   {
      private readonly IPKAnalysesTask _pkAnalysesTask;
      private readonly IPKAnalysisExportTask _exportTask;
      private readonly List<PopulationPKAnalysis> _allAnalyses;
      private IEnumerable<PopulationPKAnalysis> _allPKAnalyses;
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
         _allAnalyses = new List<PopulationPKAnalysis>();
         _populationPKAnalysisToDTOMapper = populationPKAnalysisToDTOMapper;
         _globalPKAnalysisPresenter = globalPKAnalysisPresenter;
         AddSubPresenters(_globalPKAnalysisPresenter);
         _view.AddGlobalPKAnalysisView(_globalPKAnalysisPresenter.View);
      }

      public void CalculatePKAnalysis(IPopulationDataCollector populationDataCollector, ChartData<TimeProfileXValue, TimeProfileYValue> timeProfileChartData)
      {
         _allAnalyses.Clear();
         _populationDataCollector = populationDataCollector;
         _allPKAnalyses = _pkAnalysesTask.CalculateFor(populationDataCollector, timeProfileChartData);
         _allAnalyses.AddRange(_allPKAnalyses);
         LoadPreferredUnitsForPKAnalysis();
         BindToPKAnalysis();

         _globalPKAnalysisPresenter.CalculatePKAnalysis(new Simulation[] { populationDataCollector as Simulation });
      }

      protected override void BindToPKAnalysis()
      {
         _view.BindTo(_populationPKAnalysisToDTOMapper.MapFrom(_allAnalyses));
      }

      protected override IEnumerable<PKAnalysis> AllPKAnalyses
      {
         get { return _allPKAnalyses.Select(x => x.PKAnalysis); }
      }

      public override void ExportToExcel()
      {
         _exportTask.ExportToExcel(_view.GetSummaryData(), _populationDataCollector.Name);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.PopulationPKAnalysisPresenter;
   }
}