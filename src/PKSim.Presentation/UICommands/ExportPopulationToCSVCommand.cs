using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ExportPopulationToCSVCommand : ObjectUICommand<Population>
   {
      private readonly IPopulationExportTask _populationExportTask;

      public ExportPopulationToCSVCommand(IPopulationExportTask populationExportTask)
      {
         _populationExportTask = populationExportTask;
      }

      protected override void PerformExecute()
      {
         _populationExportTask.ExportToCSV(Subject);
      }
   }
}