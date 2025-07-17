using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Reporting;
using OSPSuite.Core.Domain;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SchemaItemReportBuilder : ReportBuilder<ISchemaItem>
   {
      private readonly IReportGenerator _reportGenerator;

      public SchemaItemReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(ISchemaItem schemaItem, ReportPart reportPart)
      {
         reportPart.Title = schemaItem.Name;
         reportPart.AddToContent(schemaItem.ApplicationType.DisplayName);

         if (schemaItem.ApplicationType.NeedsFormulation)
            reportPart.AddToContent(PKSimConstants.UI.ReportIs(PKSimConstants.UI.PlaceholderFormulation, schemaItem.FormulationKey));

         schemaItem.AllParameters().Each(x => reportPart.AddToContent(_reportGenerator.ReportFor(x)));
      }
   }
}