using PKSim.Core.Model;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Presentation.Services
{
   public interface IChartTask
   {
      void UpdateObservedDataInChartsFor(Simulation simulation);
      void UpdateObservedDataInChartFor(Simulation simulation, ChartWithObservedData chartWithObservedData);

      /// <summary>
      ///    Returns if the column should be displayed or not
      /// </summary>
      bool IsColumnVisibleInDataBrowser(DataColumn dataColumn);

      void ExportToPDF(CurveChart chart);

      /// <summary>
      /// Sets origin data for the <paramref name="chart"/> to indicate project name , <paramref name="simulationName"/> and current date/time
      /// </summary>
      void SetOriginTextFor(string simulationName, IChart chart);
   }
}