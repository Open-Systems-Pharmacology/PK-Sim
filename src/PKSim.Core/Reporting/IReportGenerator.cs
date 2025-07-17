namespace PKSim.Core.Reporting
{
   /// <summary>
   ///    Generates a report that can be displayed for instance as tool tip
   /// </summary>
   public interface IReportGenerator
   {
      ReportPart ReportFor<TObject>(TObject objectToReport);
      string StringReportFor<TObject>(TObject objectToReport);
   }
}