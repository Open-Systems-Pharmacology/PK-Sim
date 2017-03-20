using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Reporting;
using PKSim.Presentation.Mappers;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_ReportPartToToolTipPartsMapper : ContextSpecification<IReportPartToToolTipPartsMapper>
   {
      protected override void Context()
      {
         sut = new ReportPartToToolTipPartsMapper();
      }
   }

   
   public class When_converting_a_report_part_with_a_title_and_content : concern_for_ReportPartToToolTipPartsMapper
   {
      private ReportPart _reportPart;
      private IEnumerable<ToolTipPart> _result;

      protected override void Context()
      {
         base.Context();
         _reportPart = new ReportPart().WithTitle("Toto");
         _reportPart.AddToContent("Content");
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_reportPart);
      }
      [Observation]
      public void should_return_only_one_tool_tip_part_with_the_accurate_title_and_content()
      {
         _result.Count().ShouldBeEqualTo(1);
         _result.ElementAt(0).Title.ShouldBeEqualTo(_reportPart.Title);
         _result.ElementAt(0).Content.ShouldBeEqualTo(_reportPart.Content);
      }
   }
}	