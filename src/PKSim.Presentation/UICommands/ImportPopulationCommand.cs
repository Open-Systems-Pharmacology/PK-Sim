using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.UICommands
{
   /// <summary>
   ///    Starts the import workflow from files. A new population building block will be created based on the selected
   ///    population files.
   ///    If the import works, the simulation will be added to the project
   /// </summary>
   public class ImportPopulationCommand : IUICommand
   {
      private readonly IApplicationController _applicationController;
      private readonly IBuildingBlockTask _buildingBlockTask;

      public ImportPopulationCommand(IApplicationController applicationController, IBuildingBlockTask buildingBlockTask)
      {
         _applicationController = applicationController;
         _buildingBlockTask = buildingBlockTask;
      }

      public void Execute()
      {
         using (var presenter = _applicationController.Start<IImportPopulationPresenter>())
         {
            var importCommand = presenter.Create();

            //User cancel action. return
            if (importCommand.IsEmpty()) return;

            var population = presenter.BuildingBlock;

            _buildingBlockTask.AddToProject(population, editBuildingBlock: true);
         }
      }
   }
}