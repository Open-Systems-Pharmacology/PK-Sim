using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class SaveSimulationToMoBiFileCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly IMoBiExportTask _moBiExportTask;

      public SaveSimulationToMoBiFileCommand(IMoBiExportTask moBiExportTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _moBiExportTask = moBiExportTask;
      }

      protected override void PerformExecute()
      {
         _moBiExportTask.ExportSimulationToPkmlFile(Subject);
      }
   }
}