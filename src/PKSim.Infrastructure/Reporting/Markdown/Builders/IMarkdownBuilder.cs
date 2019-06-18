using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure.Reporting.Markdown.Builders
{
   public interface IMarkdownBuilder : ISpecification<Type>
   {
      void Report(object objectToReport, MarkdownTracker tracker, int indentationLevel = 1);
   }

   public interface IMarkdownBuilder<T> : IMarkdownBuilder
   {
      void Report(T objectToReport, MarkdownTracker tracker, int indentationLevel);
   }

   public abstract class MarkdownBuilder<T> : IMarkdownBuilder<T>
   {
      public void Report(object objectToReport, MarkdownTracker tracker, int indentationLevel=1)
      {
         Report(objectToReport.DowncastTo<T>(), tracker, indentationLevel);
      }

      public abstract void Report(T objectToReport, MarkdownTracker tracker, int indentationLevel);

      public bool IsSatisfiedBy(Type type) => type.IsAnImplementationOf<T>();
   }
}