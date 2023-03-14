using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IChartTask
   {
      /// <summary>
      /// updates all observed data defined in the simulation to the underlying charts of this simulation
      /// </summary>
      /// <param name="simulation">Simulation used to retrieve the charts to update</param>
      /// <param name="project">Optional project. If undefined, it will be retrieved from the project retriever</param>
      void UpdateObservedDataInChartsFor(Simulation simulation, IProject project = null);

      /// <summary>
      /// adds all observed data defined in the simulation to<paramref name="chartWithObservedData"/>
      /// </summary>
      /// <param name="simulation">Simulation used to retrieve the charts to update</param>>>
      /// <param name="chartWithObservedData">Chart where observed data should be updated</param>
      /// <param name="project">Optional project. If undefined, it will be retrieved from the project retriever</param>
      void UpdateObservedDataInChartFor(Simulation simulation, ChartWithObservedData chartWithObservedData, IProject project = null);

      /// <summary>
      ///    Returns if the column should be displayed or not
      /// </summary>
      bool IsColumnVisibleInDataBrowser(DataColumn dataColumn);

      /// <summary>
      ///    Sets origin data for the <paramref name="chart" /> to indicate project name , <paramref name="simulationName" /> and
      ///    current date/time
      /// </summary>
      void SetOriginTextFor(string simulationName, IChart chart);
   }
}