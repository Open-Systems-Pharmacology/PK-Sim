using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Reporting;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public interface IReportBuilder : ISpecification<Type>
   {
      ReportPart Report(object objectToReport);
   }

   public interface IReportBuilder<T> : IReportBuilder
   {
      ReportPart Report(T objectToReport);
   }

   public abstract class ReportBuilder<T> : IReportBuilder<T>
   {
      public virtual ReportPart Report(T objectToReport)
      {
         var reportPart = new ReportPart();
         FillUpReport(objectToReport, reportPart);
         //Only one item returned in the report. Then return it instead of wrapping it in yet another report part
         if (reportPart.SubParts.Count == 1)
            return reportPart.SubParts[0];

         return reportPart;
      }

      /// <summary>
      ///    Fill the report. The content of the report is stored in the string builder
      /// </summary>
      /// <param name="objectToReport">Object for which the report should be generated</param>
      /// <param name="reportPart">Report part that will be filled</param>
      protected abstract void FillUpReport(T objectToReport, ReportPart reportPart);

      public bool IsSatisfiedBy(Type type)
      {
         return type.IsAnImplementationOf<T>();
      }

      public ReportPart Report(object objectToReport)
      {
         return Report(objectToReport.DowncastTo<T>());
      }
   }

   /// <summary>
   ///    Represents a report builder whose only task is to return the report parts of another object
   ///    i.e. delegate to another report builder
   /// </summary>
   public abstract class DelegateReportBuilder<T> : ReportBuilder<T>
   {
      private readonly IReportGenerator _reportGenerator;

      protected DelegateReportBuilder(IReportGenerator reportGenerator)
      {
         _reportGenerator = reportGenerator;
      }

      protected ReportPart DelegateReportFor<TSubObject>(TSubObject objectToReport)
      {
         return _reportGenerator.ReportFor(objectToReport);
      }

      protected override void FillUpReport(T objectToReport, ReportPart reportPart)
      {
         /*do nothing here*/
      }
   }
}