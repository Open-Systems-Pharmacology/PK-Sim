using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ICloneSimulationPresenter : ISimulationWizardPresenter
   {
      /// <summary>
      ///    Starts the clone workflow which is simply a configuration with different view title and icons
      /// </summary>
      /// <param name="simulationToClone"> Simulation to clone </param>
      IPKSimCommand CloneSimulation(Simulation simulationToClone);
   }

   public class CloneSimulationPresenter : ConfigureSimulationPresenterBase<ICloneSimulationView>, ICloneSimulationPresenter
   {
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private readonly IRenameBuildingBlockTask _renameBuildingBlockTask;

      private ObjectBaseDTO _simulationPropertiesDTO;

      public CloneSimulationPresenter(ICloneSimulationView view, ISubPresenterItemManager<ISimulationItemPresenter> subPresenterItemManager, ISimulationModelCreator simulationModelCreator,
         IHeavyWorkManager heavyWorkManager, ICloner cloner, IDialogCreator dialogCreator, IObjectBaseDTOFactory buildingBlockDTOFactory,
         IRenameBuildingBlockTask renameBuildingBlockTask, ISimulationParametersUpdater simulationParametersUpdater, IFullPathDisplayResolver fullPathDisplayResolver, IBuildingBlockInSimulationSynchronizer buildingBlockInSimulationSynchronizer)
         : base(view, subPresenterItemManager, simulationModelCreator, heavyWorkManager, cloner, dialogCreator, simulationParametersUpdater, fullPathDisplayResolver, buildingBlockInSimulationSynchronizer, CreationMode.Clone)
      {
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
         _renameBuildingBlockTask = renameBuildingBlockTask;
      }

      public IPKSimCommand CloneSimulation(Simulation simulationToClone)
      {
         _simulationPropertiesDTO = _buildingBlockDTOFactory.CreateFor<Simulation>();
         _simulationPropertiesDTO.Name = simulationToClone.Name;
         _view.BindToProperties(_simulationPropertiesDTO);

         return ConfigureSimulation(simulationToClone);
      }

      protected override string HeayWorkCaption => PKSimConstants.UI.PerformingSimulationClone;

      protected override string ViewCaption(Simulation simulation)
      {
         return PKSimConstants.UI.CloningSimulation(simulation.Name);
      }

      protected override void UpdateSimulationProperties()
      {
         var oldName = Simulation.Name;
         try
         {
            Simulation.Name = _simulationPropertiesDTO.Name;
            base.UpdateSimulationProperties();
         }
         finally
         {
            //reset name to ensure that rename simulation will work
            Simulation.Name = oldName;
         }
         //here we need to rename the full simulation in order to have a consistent naming throughout the model
         _renameBuildingBlockTask.RenameSimulation(Simulation, _simulationPropertiesDTO.Name);
      }
   }
}