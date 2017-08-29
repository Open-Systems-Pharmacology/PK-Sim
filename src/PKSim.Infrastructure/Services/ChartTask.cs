using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Infrastructure.Services
{
   public class ChartTask : IChartTask
   {
      private readonly IProjectRetriever _projectRetriever;
      private readonly IContainer _executionContext;

      public ChartTask(IProjectRetriever projectRetriever, IContainer container)
      {
         _projectRetriever = projectRetriever;
         _executionContext = container;
      }

      public void ExportToPDF(CurveChart chart)
      {
         var exportToPDFCommand = _executionContext.Resolve<ExportChartToPDFCommand>();
         exportToPDFCommand.Subject = chart;
         exportToPDFCommand.Execute();
      }

      public void SetOriginTextFor(string simulationName, IChart chart)
      {
         chart.SetOriginTextFor(_projectRetriever.CurrentProject.Name, simulationName);
      }

      public void UpdateObservedDataInChartsFor(Simulation simulation)
      {
         simulation.ChartWithObservedData.Each(c => UpdateObservedDataInChartFor(simulation, c));
      }

      public void UpdateObservedDataInChartFor(Simulation simulation, ChartWithObservedData chartWithObservedData)
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