using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Infrastructure.Reporting.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class SimpleProtocolReportBuilder : ReportBuilder<SimpleProtocol>
   {
      private readonly IReportGenerator _reportGenerator;

      public SimpleProtocolReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected override void FillUpReport(SimpleProtocol simpleProtocol, ReportPart reportPart)
      {
         reportPart.Title = PKSimConstants.UI.SimpleProtocolMode;
         reportPart.AddToContent(simpleProtocol.ApplicationType?.DisplayName);
         reportPart.AddToContent(PKSimConstants.UI.ReportIs(PKSimConstants.UI.DosingInterval, simpleProtocol.DosingInterval?.DisplayName));

         simpleProtocol.AllParameters().Where(simpleProtocol.ParameterShouldBeExported)
            .Each(x => reportPart.AddToContent(_reportGenerator.ReportFor(x)));
      }
   }
}