using OSPSuite.Utility.Events;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Events;

namespace PKSim.Presentation.Core
{
   /// <summary>
   ///    Listens to the ExportToPDF event and export the item if required
   /// </summary>
   public interface IExportToPDFInvoker : IListener<ExportToPDFEvent>
   {
      void Export(object objectToExport);
   }

   internal class ExportToPDFInvoker : IExportToPDFInvoker
   {
      private readonly ExportChartToPDFCommand _exportChartToPDFCommand;

      public ExportToPDFInvoker(ExportChartToPDFCommand exportChartToPDFCommand)
      {
         _exportChartToPDFCommand = exportChartToPDFCommand;
      }

      public void Handle(ExportToPDFEvent eventToHandle)
      {
         Export(eventToHandle.ObjectToExport);
      }

      public void Export(object objectToExport)
      {
         if (objectToExport == null) return;
         tryExport(_exportChartToPDFCommand, objectToExport);
      }

      private void tryExport<T>(ExportToPDFCommand<T> exportToPDFCommand, object objectToReport) where T : class
      {
         var subject = objectToReport as T;
         if (subject == null) return;
         exportToPDFCommand.Subject = subject;
         exportToPDFCommand.Execute();
      }
   }
}