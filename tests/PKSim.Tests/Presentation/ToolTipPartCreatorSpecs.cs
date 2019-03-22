using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;


namespace PKSim.Presentation
{
   public abstract class concern_for_ToolTipPartCreator : ContextSpecification<IToolTipPartCreator>
   {
      protected IReportPartToToolTipPartsMapper _toolTipMapper;
      protected IReportGenerator _reportGenerator;

      protected override void Context()
      {
         _toolTipMapper= A.Fake<IReportPartToToolTipPartsMapper>();
         _reportGenerator= A.Fake<IReportGenerator>();
         sut = new ToolTipPartCreator(_reportGenerator,_toolTipMapper);
      }
   }

   public class When_generating_a_tool_tip_for_a_wrappable_object : concern_for_ToolTipPartCreator
   {
      private IList<ToolTipPart> _toolTips;
      private object _classifiableWrapper;

      protected override void Context()
      {
         base.Context();
         _toolTips=new List<ToolTipPart>();
         var simulation= A.Fake<Simulation>();
         var reportPart=new ReportPart();
         A.CallTo(() => _reportGenerator.ReportFor(simulation)).Returns(reportPart);
         A.CallTo(() => _toolTipMapper.MapFrom(reportPart)).Returns(_toolTips);
         _classifiableWrapper = new ClassifiableSimulation {Subject = simulation};
      }

      [Observation]
      public void should_return_the_tool_tip_for_the_wrapper_object()
      {
         sut.ToolTipFor(_classifiableWrapper).ShouldOnlyContain(_toolTips);
      }
   }
}	