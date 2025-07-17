using PKSim.Assets;
using OSPSuite.Core.Domain;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class ExtendedPropertiesReportBuilder : ReportBuilder<ExtendedProperties>
   {
      protected override void FillUpReport(ExtendedProperties extendedProperties, ReportPart reportPart)
      {
         foreach (var ep in extendedProperties)
         {
            reportPart.AddToContent(PKSimConstants.UI.ReportIs(ep.Name, ep.ValueAsObject));
         }
      }
   }
}