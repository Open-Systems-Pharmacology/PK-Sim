using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Reporting;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Mappers
{
   public interface IReportPartToToolTipPartsMapper : IMapper<ReportPart, IList<ToolTipPart>>,
                                                      IMapper<string, IList<ToolTipPart>>
   {

   }

   public class ReportPartToToolTipPartsMapper : IReportPartToToolTipPartsMapper
   {
      public IList<ToolTipPart> MapFrom(ReportPart reportPart)
      {
         var toolTipParts = new List<ToolTipPart>();
         addToolTipPart(toolTipParts, reportPart);
         return toolTipParts;
      }

      private void addToolTipPart(List<ToolTipPart> toolTipParts, ReportPart reportPart)
      {
         if (reportPart == null) return;

         var toolTipPart = new ToolTipPart {Title = reportPart.Title, Content = reportPart.Content};
         if(!string.IsNullOrEmpty(toolTipPart.Content))
            toolTipParts.Add(toolTipPart);

         reportPart.SubParts.Each(x => addToolTipPart(toolTipParts, x));
      }

      public IList<ToolTipPart> MapFrom(string input)
      {
         var report = new ReportPart {Title = PKSimConstants.UI.Description};
         report.AddToContent(input);
         return MapFrom(report);
      }
   }
}