using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Infrastructure.Reporting.Markdown.Builders;

namespace PKSim.Infrastructure.Reporting.Markdown
{
   public interface IMarkdownBuilderRepository : IBuilderRepository<IMarkdownBuilder>
   {
      void Report<T>(T objectToReport, MarkdownTracker tracker, int identationLevel = 0);
   }

   public class MarkdownBuilderRepository : BuilderRepository<IMarkdownBuilder>, IMarkdownBuilderRepository
   {
      public MarkdownBuilderRepository(IContainer container) : base(container, typeof(IMarkdownBuilder<>))
      {
      }

      public void Report<T>(T objectToReport, MarkdownTracker tracker, int identationLevel = 0)
      {
         var builder = BuilderFor(objectToReport);
         builder.Report(objectToReport, tracker, identationLevel);
      }
   }
}