using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ExportPopulationSimulationForClusterCommand : ActiveObjectUICommand<PopulationSimulation>
   {
      private readonly IPopulationExportTask _populationExportTask;

      public ExportPopulationSimulationForClusterCommand(IPopulationExportTask populationExportTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _populationExportTask = populationExportTask;
      }

      protected override void PerformExecute()
      {
         _populationExportTask.ExportForCluster(Subject);
      }
   }
}