using System.Collections.Generic;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public abstract class BuildingBlockTeXBuilder<TBuildingBlock> : OSPSuiteTeXBuilder<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly ITeXBuilderRepository _builderRepository;
      protected readonly IReportGenerator _reportGenerator;
      private readonly ILazyLoadTask _lazyLoadTask;

      protected BuildingBlockTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask)
      {
         _builderRepository = builderRepository;
         _reportGenerator = reportGenerator;
         _lazyLoadTask = lazyLoadTask;
      }

      protected abstract IEnumerable<object> BuildingBlockReport(TBuildingBlock buildingBlock, OSPSuiteTracker tracker);

      protected string TextReportFor<T>(T objectToReport)
      {
         return _reportGenerator.StringReportFor(objectToReport);
      }

      public override void Build(TBuildingBlock buildingBlock, OSPSuiteTracker tracker)
      {
         _lazyLoadTask.Load(buildingBlock);
         var section = new Section(buildingBlock.Name);
         tracker.AddReference(buildingBlock, section);

         _builderRepository.Report(section, tracker);
         _builderRepository.Report(this.ReportDescription(buildingBlock, tracker), tracker);
         _builderRepository.Report(BuildingBlockReport(buildingBlock, tracker), tracker);
      }
   }

   public class BuildingBlockTeXBuilder : BuildingBlockTeXBuilder<IPKSimBuildingBlock>
   {
      public BuildingBlockTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask) : base(builderRepository, reportGenerator, lazyLoadTask)
      {
      }

      protected override IEnumerable<object> BuildingBlockReport(IPKSimBuildingBlock buildingBlock, OSPSuiteTracker tracker)
      {
         return new List<object>
         {
            TextReportFor(buildingBlock)
         };
      }
   }
}