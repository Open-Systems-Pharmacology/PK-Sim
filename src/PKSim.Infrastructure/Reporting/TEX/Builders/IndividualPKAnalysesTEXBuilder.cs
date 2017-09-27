using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;
using PKSim.Presentation;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class IndividualPKAnalysesTeXBuilder : PresentableTeXBuilder<IndividualPKAnalyses, DefaultPresentationSettings>
   {
      private readonly IGlobalPKAnalysisTask _globalPKAnalysisTask;
      private readonly IPKAnalysesTask _pkAnalysisTask;
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IGlobalPKAnalysisToDataTableMapper _globalPKAnalysisToDataTableMapper;
      private readonly IIndividualPKAnalysisToDataTableMapper _pkAnalysisToDataTableMapper;

      public IndividualPKAnalysesTeXBuilder(IGlobalPKAnalysisTask globalPKAnalysisTask,
         IPKAnalysesTask pkAnalysisTask, ITeXBuilderRepository builderRepository, IGlobalPKAnalysisToDataTableMapper globalPKAnalysisToDataTableMapper,
         IIndividualPKAnalysisToDataTableMapper pkAnalysisToDataTableMapper, IPresentationSettingsTask presentationSettingsTask, IDisplayUnitRetriever displayUnitRetriever) : base(presentationSettingsTask, displayUnitRetriever)
      {
         _globalPKAnalysisTask = globalPKAnalysisTask;
         _pkAnalysisTask = pkAnalysisTask;
         _builderRepository = builderRepository;
         _globalPKAnalysisToDataTableMapper = globalPKAnalysisToDataTableMapper;
         _pkAnalysisToDataTableMapper = pkAnalysisToDataTableMapper;
      }

      public override void Build(IndividualPKAnalyses pkAnalyses, OSPSuiteTracker buildTracker)
      {
         var objectsToReport = pkAnalysisFor(pkAnalyses.Simulation, pkAnalyses.SimulationChart, buildTracker);
         _builderRepository.Report(objectsToReport, buildTracker);
      }

      private IEnumerable<object> pkAnalysisFor(Simulation simulation, CurveChart chart, OSPSuiteTracker buildTracker)
      {
         var report = new List<object>();

         var globalPKAnalysis = _globalPKAnalysisTask.CalculateGlobalPKAnalysisFor(new[] {simulation});
         updateDisplayUnits(globalPKAnalysis, PresentationSettingsFor(chart, PresenterConstants.PresenterKeys.GlobalPKAnalysisPresenter));

         var globalPKAnalysisTable = _globalPKAnalysisToDataTableMapper.MapFrom(globalPKAnalysis);

         if (globalPKAnalysisTable.Rows.Count > 0)
         {
            report.Add(buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.GlobalPKAnalyses, 1));
            report.Add(globalPKAnalysisTable);
         }

         var curvesToDisplay = chart.Curves.ForPKAnalysis();
         var pkAnalyses = _pkAnalysisTask.CalculateFor(new[] {simulation}, curvesToDisplay.Select(c => c.yData)).ToList();
         updateDisplayUnits(pkAnalyses, PresentationSettingsFor(chart, PresenterConstants.PresenterKeys.IndividualPKParametersPresenter));
         var pkAnalysesTable = _pkAnalysisToDataTableMapper.MapFrom(pkAnalyses, curvesToDisplay);
         var pkAnalysesForReport = new PKAnalysesTable(pkAnalysesTable);

         report.Add(buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.PKAnalyses, 1));
         report.Add(pkAnalysesForReport);

         return report;
      }

      private void updateDisplayUnits(IReadOnlyList<IndividualPKAnalysis> pkAnalyses, IPresentationSettings presentationSettings)
      {
         pkAnalyses.Each(x => UpdateParameterDisplayUnit(x.PKAnalysis.AllPKParameters, presentationSettings));
      }

      private void updateDisplayUnits(GlobalPKAnalysis globalPKAnalysis, IPresentationSettings presentationSettings)
      {
         UpdateParameterDisplayUnit(globalPKAnalysis.AllPKParameters, presentationSettings);
      }
   }
}