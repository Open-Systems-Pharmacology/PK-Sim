using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.UICommands
{
   public class ExportPopulationSimulationForClusterCommand : ActiveObjectUICommand<PopulationSimulation>
   {
      private readonly IPopulationSimulationExportTask _populationSimulationExportTask;
      private readonly IApplicationController _applicationController;

      public ExportPopulationSimulationForClusterCommand(IPopulationSimulationExportTask populationSimulationExportTask, IApplicationController applicationController, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _populationSimulationExportTask = populationSimulationExportTask;
         _applicationController = applicationController;
      }

      protected override void PerformExecute()
      {
         using (var presenter = _applicationController.Start<ISelectFilePresenter>())
         {
            var folder = presenter.SelectDirectory(PKSimConstants.UI.ExportForClusterSimulationTitle, Constants.DirectoryKey.SIM_MODEL_XML);
            _populationSimulationExportTask.ExportForCluster(Subject, folder);
         }
      }
   }
}