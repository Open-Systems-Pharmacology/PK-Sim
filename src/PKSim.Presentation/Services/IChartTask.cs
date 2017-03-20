using PKSim.Core.Model;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Presentation.Services
{
   public interface IChartTask
   {
      void ProjectChanged();
      void UpdateObservedDataInChartsFor(Simulation simulation);
      void UpdateObservedDataInChartFor(Simulation simulation, IChartWithObservedData chartWithObservedData);

      /// <summary>
      ///    Returns if the column should be displayed or not
      /// </summary>
      bool IsColumnVisibleInDataBrowser(DataColumn dataColumn);

      void ExportToPDF(ICurveChart chart);

      /// <summary>
      /// Sets origin data for the <paramref name="chart"/> to indicate project name , <paramref name="simulationName"/> and current date/time
      /// </summary>
      void SetOriginTextFor(string simulationName, IChart chart);
   }
}