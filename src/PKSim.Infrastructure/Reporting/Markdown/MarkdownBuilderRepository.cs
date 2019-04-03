using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Infrastructure.Reporting.Markdown.Builders;

namespace PKSim.Infrastructure.Reporting.Markdown
{
   public interface IMarkdownBuilderRepository : IBuilderRepository<IMarkdownBuilder>
   {
      void Report<T>(T objectToReport, MarkdownTracker tracker, int identationLevel = 0);
      string TypeFor(object objectToReport);
   }

   public class MarkdownBuilderRepository : BuilderRepository<IMarkdownBuilder>, IMarkdownBuilderRepository
   {
      private readonly IObjectTypeResolver _objectTypeResolver;

      public MarkdownBuilderRepository(IContainer container, IObjectTypeResolver objectTypeResolver) : base(container, typeof(IMarkdownBuilder<>))
      {
         _objectTypeResolver = objectTypeResolver;
      }

      public void Report<T>(T objectToReport, MarkdownTracker tracker, int identationLevel = 0)
      {
         var builder = BuilderFor(objectToReport);
         builder.Report(objectToReport, tracker, identationLevel);
      }

      public string TypeFor(object objectToReport) => _objectTypeResolver.TypeFor(objectToReport);
   }
}