using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Infrastructure.Reporting.Markdown.Builders;
using PKSim.Infrastructure.Reporting.Summary;

namespace PKSim.Infrastructure.Reporting.Markdown
{
   public interface IMarkdownBuilderRepository : IBuilderRepository<IMarkdownBuilder>
   {
   }

   public class MarkdownBuilderRepository : BuilderRepository<IMarkdownBuilder>, IMarkdownBuilderRepository
   {
      public MarkdownBuilderRepository(IContainer container) : base(container, typeof(IMarkdownBuilder<>))
      {
      }
   }
}