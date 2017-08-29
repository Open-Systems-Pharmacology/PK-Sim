using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Utils;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.TeX.Items;
using PKSim.Presentation;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class PopulationPKAnalysesTeXBuilder : PresentableTeXBuilder<PopulationPKAnalyses, DefaultPresentationSettings>
   {
      private readonly IPKAnalysesTask _pkAnalysisTask;
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IPopulationPKAnalysisToDataTableMapper _pkAnalysisToDataTableMapper;

      public PopulationPKAnalysesTeXBuilder(IPKAnalysesTask pkAnalysisTask, ITeXBuilderRepository builderRepository,
         IPopulationPKAnalysisToDataTableMapper pkAnalysisToDataTableMapper, IPresentationSettingsTask presentationSettingsTask, IDisplayUnitRetriever displayUnitRetriever) : base(presentationSettingsTask, displayUnitRetriever)
      {
         _pkAnalysisTask = pkAnalysisTask;
         _builderRepository = builderRepository;
         _pkAnalysisToDataTableMapper = pkAnalysisToDataTableMapper;
      }

      public override void Build(PopulationPKAnalyses pkAnalyses, OSPSuiteTracker buildTracker)
      {
         var objectsToReport = pkAnalysisFor(pkAnalyses, buildTracker);
         _builderRepository.Report(objectsToReport, buildTracker);
      }

      private IEnumerable<object> pkAnalysisFor(PopulationPKAnalyses populationPKAnalyses, OSPSuiteTracker buildTracker)
      {
         var report = new List<object>();
         var pkAnalyses = _pkAnalysisTask.CalculateFor(populationPKAnalyses.DataCollector, populationPKAnalyses.ChartData).ToList();
         updateDisplayUnits(pkAnalyses, PresentationSettingsFor(populationPKAnalyses.PopulationAnalysisChart, PresenterConstants.PresenterKeys.PopulationPKAnalysisPresenter));

         var pkAnalysesTable = _pkAnalysisToDataTableMapper.MapFrom(pkAnalyses);
         var pkAnalysesForReport = new PKAnalysesTable(pkAnalysesTable);

         report.Add(buildTracker.GetStructureElementRelativeToLast(PKSimConstants.UI.PKAnalyses, 1));
         report.Add(pkAnalysesForReport);
         return report;
      }

      private void updateDisplayUnits(IReadOnlyList<PopulationPKAnalysis> pkAnalyses, IPresentationSettings unitSettings)
      {
         pkAnalyses.Each(p => UpdateParameterDisplayUnit(p.PKAnalysis.AllPKParameters, unitSettings));
      }
   }
}