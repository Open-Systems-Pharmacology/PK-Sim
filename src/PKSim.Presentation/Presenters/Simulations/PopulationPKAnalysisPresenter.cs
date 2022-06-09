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
using System;
using OSPSuite.Utility.Data;
using OSPSuite.Core.Domain;

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
      private IEnumerable<PopulationPKAnalysis> _allPKAnalysesOnCurves;
      private IEnumerable<PopulationPKAnalysis> _allPKAnalysesOnIndividuals;
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
         _allAnalysesOnCurves.Clear();
         _populationDataCollector = populationDataCollector;
         _allPKAnalysesOnCurves = _pkAnalysesTask.CalculateFor(populationDataCollector, timeProfileChartData);
         _allAnalysesOnCurves.AddRange(_allPKAnalysesOnCurves);
         LoadPreferredUnitsForPKAnalysis();
         BindToPKAnalysis();
         _globalPKAnalysisPresenter.CalculatePKAnalysis(new Simulation[] { populationDataCollector as Simulation });
      }

      public void CalculatePKAnalysisOnIndividuals(IPopulationDataCollector populationDataCollector, IEnumerable<PopulationPKAnalysis> pks)
      {
         _allAnalysesOnIndividuals.Clear();
         _populationDataCollector = populationDataCollector;
         _allPKAnalysesOnIndividuals = pks;
         _allAnalysesOnIndividuals.AddRange(_allPKAnalysesOnIndividuals);
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
         get { return (View.IsOnCurvesSelected() ? _allPKAnalysesOnCurves : _allPKAnalysesOnIndividuals).Select(x => x.PKAnalysis); }
      }

      public override void ExportToExcel()
      {
         _exportTask.ExportToExcel(_view.GetSummaryData(), _populationDataCollector.Name);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.PopulationPKAnalysisPresenter;
   }
}