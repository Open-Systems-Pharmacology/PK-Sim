using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public interface IMarkdownBuilder : ISpecification<Type>
   {
      void Report(object objectToReport, MarkdownTracker tracker);
   }

   public interface IMarkdownBuilder<T> : IMarkdownBuilder
   {
      void Report(T objectToReport, MarkdownTracker tracker);
   }

   public abstract class MarkdownBuilder<T> : IMarkdownBuilder<T>
   {
      public void Report(object objectToReport, MarkdownTracker tracker)
      {
         Report(objectToReport.DowncastTo<T>(), tracker);
      }

      public abstract void Report(T objectToReport, MarkdownTracker tracker);

      public bool IsSatisfiedBy(Type type) => type.IsAnImplementationOf<T>();
   }
}