using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IIndividualPKAnalysisPresenter : IPresenter<IIndividualPKAnalysisView>, IPKAnalysisPresenter
   {
      void ShowPKAnalysis(IEnumerable<Simulation> simulations, IEnumerable<Curve> curves);
   }

   public class IndividualPKAnalysisPresenter : PKAnalysisPresenter<IIndividualPKAnalysisView, IIndividualPKAnalysisPresenter>, IIndividualPKAnalysisPresenter
   {
      private readonly IPKAnalysesTask _pkAnalysesTask;
      private readonly IPKAnalysisExportTask _exportTask;
      private List<IndividualPKAnalysis> _allPKAnalysis;
      private IReadOnlyList<Simulation> _simulations;
      private readonly ICache<DataColumn, Curve> _curveCache;
      private List<DataColumn> _allColumns;
      private readonly IGlobalPKAnalysisPresenter _globalPKAnalysisPresenter;
      private readonly IIndividualPKAnalysisToPKAnalysisDTOMapper _pKAnalysisToDTOMapper;

      public IndividualPKAnalysisPresenter(IIndividualPKAnalysisView view, IPKAnalysesTask pkAnalysesTask, IPKAnalysisExportTask exportTask,
         IGlobalPKAnalysisPresenter globalPKAnalysisPresenter, IIndividualPKAnalysisToPKAnalysisDTOMapper pKAnalysisToDTOMapper,
         IPKParameterRepository pkParameterRepository, IPresentationSettingsTask presentationSettingsTask) : base(view, pkParameterRepository, presentationSettingsTask)
      {
         _pkAnalysesTask = pkAnalysesTask;
         _globalPKAnalysisPresenter = globalPKAnalysisPresenter;
         _exportTask = exportTask;
         _view.ShowControls = false;
         _curveCache = new Cache<DataColumn, Curve>(onMissingKey: x => null);
         AddSubPresenters(_globalPKAnalysisPresenter);
         _view.AddGlobalPKAnalysisView(_globalPKAnalysisPresenter.View);
         _pKAnalysisToDTOMapper = pKAnalysisToDTOMapper;
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.IndividualPKParametersPresenter;

      public void ShowPKAnalysis(IEnumerable<Simulation> simulations, IEnumerable<Curve> curves)
      {
         _simulations = simulations.ToList();
         _globalPKAnalysisPresenter.CalculatePKAnalysis(_simulations);

         var globalPKAnalysis = _globalPKAnalysisPresenter.GlobalPKAnalysis;

         var curveList = curves.ToList();

         createColumnsWithPKAnalysesFrom(curveList);
         _allColumns = _curveCache.Keys.ToList();
         _allPKAnalysis = _pkAnalysesTask.CalculateFor(_simulations, _allColumns, globalPKAnalysis).ToList();
         
         _view.GlobalPKVisible = _globalPKAnalysisPresenter.HasParameters();

         _view.ShowControls = true;

         LoadPreferredUnitsForPKAnalysis();
         BindToPKAnalysis();
      }

      private void createColumnsWithPKAnalysesFrom(IEnumerable<Curve> curves)
      {
         //should be done before cache clear as the curves might well be the cache itselfs
         var curvesToDisplay = curves.ForPKAnalysis();
         _curveCache.Clear();
         curvesToDisplay.Each(curve => _curveCache[curve.yData] = curve);
      }

      public override void LoadSettingsForSubject(IWithId subject)
      {
         base.LoadSettingsForSubject(subject);
         _globalPKAnalysisPresenter.LoadSettingsForSubject(subject);
      }

      protected override void BindToPKAnalysis()
      {
         _view.BindTo(_pKAnalysisToDTOMapper.MapFrom(_allPKAnalysis.ToList(), _curveCache.All()));
      }

      protected override IEnumerable<PKAnalysis> AllPKAnalyses
      {
         get { return _allPKAnalysis.Select(x => x.PKAnalysis); }
      }

      public override void ExportToExcel()
      {
         var pivotGridData = _view.GetSummaryData();
         _exportTask.ExportToExcel(_allColumns, _globalPKAnalysisPresenter.GlobalPKAnalysis, pivotGridData, _simulations);
      }
   }
}