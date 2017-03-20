using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.Services
{
   public class ChartTask : IChartTask
   {
      private readonly IProjectChangedNotifier _projectChangedNotifier;
      private readonly IProjectRetriever _projectRetriever;
      private readonly ExportChartToPDFCommand _exportChartToPDFCommand;

      public ChartTask(IProjectChangedNotifier projectChangedNotifier, IProjectRetriever projectRetriever, ExportChartToPDFCommand exportChartToPDFCommand)
      {
         _projectChangedNotifier = projectChangedNotifier;
         _projectRetriever = projectRetriever;
         _exportChartToPDFCommand = exportChartToPDFCommand;
      }

      public void ProjectChanged()
      {
         _projectChangedNotifier.Changed();
      }

      public void ExportToPDF(ICurveChart chart)
      {
         _exportChartToPDFCommand.Subject = chart;
         _exportChartToPDFCommand.Execute();
      }

      public void SetOriginTextFor(string simulationName, IChart chart)
      {
         chart.SetOriginTextFor(_projectRetriever.CurrentProject.Name, simulationName);
      }

      public void UpdateObservedDataInChartsFor(Simulation simulation)
      {
         simulation.ChartWithObservedData.Each(c => UpdateObservedDataInChartFor(simulation, c));
      }

      public void UpdateObservedDataInChartFor(Simulation simulation, IChartWithObservedData chartWithObservedData)
      {
         foreach (var usedObservedData in simulation.UsedObservedData)
         {
            chartWithObservedData.AddObservedData(_projectRetriever.CurrentProject.ObservedDataBy(usedObservedData.Id));
         }
      }

      public bool IsColumnVisibleInDataBrowser(DataColumn dataColumn)
      {
         return !dataColumn.IsBaseGrid();
      }
   }
}