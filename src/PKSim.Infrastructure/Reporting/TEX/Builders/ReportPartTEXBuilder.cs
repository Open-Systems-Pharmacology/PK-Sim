using System.Data;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ReportPartTeXBuilder : OSPSuiteTeXBuilder<ReportPart>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public ReportPartTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(ReportPart reportPart, OSPSuiteTracker buildTracker)
      {
         if (reportPart == null) return;

         var content = reportPart.Content;
         if (!string.IsNullOrEmpty(content))
         {
            if (!string.IsNullOrEmpty(reportPart.Title))
               _builderRepository.Report(new Paragraph(reportPart.Title), buildTracker);

            _builderRepository.Report(contentFrom(reportPart), buildTracker);
         }

         foreach (var subPart in reportPart.SubParts)
         {
            Build(subPart, buildTracker);
         }
      }

      private object contentFrom(ReportPart reportPart)
      {
         var tablePart = reportPart as TablePart;
         if (tablePart == null)
            return reportPart.Content;

         var table = new DataTable(tablePart.Caption);
         table.AddColumn(tablePart.KeyName);
         foreach (var valueName in tablePart.ValueNames)
         {
            table.AddColumn(valueName, tablePart.Types[valueName]);
         }

         foreach (var row in tablePart.Rows)
         {
            var dataRow = table.NewRow();
            dataRow[0] = row.Key;
            for (int i = 0; i < row.Value.Count; i++)
            {
               dataRow[i + 1] = row.Value[i];
            }
            table.Rows.Add(dataRow);
         }

         if (string.IsNullOrEmpty(reportPart.Title))
            return table;

         return new Table(table.DefaultView, reportPart.Title);
      }
   }
}