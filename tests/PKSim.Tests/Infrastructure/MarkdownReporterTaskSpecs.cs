using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.Markdown;
using PKSim.Infrastructure.Reporting.Markdown.Builders;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_MarkdownReporterTask : ContextSpecification<IMarkdownReporterTask>
   {
      private IReportGenerator _reportGenerator;
      private IMarkdownBuilderRepository _markdownReportBuilder;
      protected Individual _individual = new Individual();
      protected Compound _compound = new Compound();

      protected override void Context()
      {
         _reportGenerator = A.Fake<IReportGenerator>();
         _markdownReportBuilder = A.Fake<IMarkdownBuilderRepository>();
         sut = new MarkdownReporterTask(_reportGenerator, _markdownReportBuilder);

         A.CallTo(() => _reportGenerator.StringReportFor((object)_compound)).Returns("COMP");
         var individualMarkdownBuilder = A.Fake<IMarkdownBuilder>();
         A.CallTo(() => _markdownReportBuilder.BuilderFor(_individual)).Returns(individualMarkdownBuilder);
         A.CallTo(() => _markdownReportBuilder.BuilderFor(_compound)).Returns(null);
         A.CallTo(() => individualMarkdownBuilder.Report(_individual, A<MarkdownTracker>._, A<int>._)).Invokes(x =>
         {
            var markdownTracker = x.GetArgument<MarkdownTracker>(1);
            markdownTracker.Add("IND");
         });
      }
   }

   public class When_generating_a_markdown_report_for_an_object_that_has_a_markdown_export : concern_for_MarkdownReporterTask
   {
      [Observation]
      public void should_return_the_expected_export()
      {
         sut.ExportToMarkdownString(_individual).ShouldBeEqualTo("IND");
      }
   }

   public class When_generating_a_markdown_report_for_an_object_that_has_no_markdown_export : concern_for_MarkdownReporterTask
   {
      [Observation]
      public void should_return_the_expected_default_export()
      {
         sut.ExportToMarkdownString(_compound).ShouldBeEqualTo("COMP");
      }
   }

   public class IndividualEqualityComparer : GenericEqualityComparer<Individual>
   {

   }

   public class CompoundEqualityComparer : GenericEqualityComparer<Compound>
   {
   }
}