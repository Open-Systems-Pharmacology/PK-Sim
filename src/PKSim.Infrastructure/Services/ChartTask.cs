using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Infrastructure.Services
{
   public class ChartTask : IChartTask
   {
      private readonly IProjectRetriever _projectRetriever;

      public ChartTask(IProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }

      public void SetOriginTextFor(string simulationName, IChart chart)
      {
         chart.SetOriginTextFor(_projectRetriever.CurrentProject.Name, simulationName);
      }

      public void UpdateObservedDataInChartsFor(Simulation simulation, IProject project = null)
      {
         simulation.ChartWithObservedData.Each(c => UpdateObservedDataInChartFor(simulation, c, project));
      }

      public void UpdateObservedDataInChartFor(Simulation simulation, ChartWithObservedData chartWithObservedData, IProject project = null)
      {
         var projectToUse = project ?? _projectRetriever.CurrentProject;
         foreach (var usedObservedData in simulation.UsedObservedData)
         {
            chartWithObservedData.AddObservedData(projectToUse.ObservedDataBy(usedObservedData.Id));
         }
      }

      public bool IsColumnVisibleInDataBrowser(DataColumn dataColumn) => !dataColumn.IsBaseGrid();
   }
}