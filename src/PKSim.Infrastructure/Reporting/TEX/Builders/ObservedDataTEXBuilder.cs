using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Core.Reporting;
using DataColumn = OSPSuite.Core.Domain.Data.DataColumn;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ObservedDataTeXBuilder : OSPSuiteTeXBuilder<DataRepository>
   {
      private readonly IReportGenerator _reportGenerator;
      private readonly IDataRepositoryTask _dataRepositoryTask;
      private readonly ITeXBuilderRepository _builderRepository;

      public ObservedDataTeXBuilder(IReportGenerator reportGenerator, IDataRepositoryTask dataRepositoryTask, ITeXBuilderRepository builderRepository)
      {
         _reportGenerator = reportGenerator;
         _dataRepositoryTask = dataRepositoryTask;
         _builderRepository = builderRepository;
      }

      private DataTable tableFor(IEnumerable<DataColumn> observedData)
      {
         return _dataRepositoryTask.ToDataTable(observedData).First();
      }

      public override void Build(DataRepository observedData, OSPSuiteTracker tracker)
      {
         _builderRepository.Report(new Chapter(observedData.Name), tracker);
         _builderRepository.Report(_reportGenerator.StringReportFor(observedData), tracker);
         _builderRepository.Report(tableFor(observedData), tracker);
      }
   }
}