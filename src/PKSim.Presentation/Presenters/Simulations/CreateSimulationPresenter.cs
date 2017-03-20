using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ICreateSimulationPresenter : ICreateBuildingBlockPresenter<Simulation>, ISimulationWizardPresenter, IPresenter<ICreateSimulationView>
   {
   }

   public class CreateSimulationPresenter : SimulationWizardPresenter<ICreateSimulationView>, ICreateSimulationPresenter
   {
      private readonly IBuildingBlockPropertiesMapper _simulationPropertiesMapper;
      private readonly IObjectBaseDTOFactory _buildingBlockDTOFactory;
      private ObjectBaseDTO _simulationPropertiesDTO;

      public CreateSimulationPresenter(ICreateSimulationView view, ISubPresenterItemManager<ISimulationItemPresenter> subPresenterItemManager, ISimulationModelCreator simulationModelCreator, IHeavyWorkManager heavyWorkManager, IBuildingBlockPropertiesMapper simulationPropertiesMapper, IObjectBaseDTOFactory buildingBlockDTOFactory, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager, simulationModelCreator, heavyWorkManager, dialogCreator)
      {
         _simulationPropertiesMapper = simulationPropertiesMapper;
         _buildingBlockDTOFactory = buildingBlockDTOFactory;
      }

      public IPKSimCommand Create()
      {
         _simulationPropertiesDTO = _buildingBlockDTOFactory.CreateFor<Simulation>();
         _view.BindToProperties(_simulationPropertiesDTO);

         _view.ActivateControl(SimulationItems.Model);
         _view.EnableControl(SimulationItems.Model);
         AllSimulationItemsAfterModel.Each(x => _view.DisableControl(x));
         SetWizardButtonEnabled(SimulationItems.Model);
         _view.Display();

         if (_view.Canceled)
            return new PKSimEmptyCommand();

         return new PKSimMacroCommand();
      }

      public Simulation BuildingBlock
      {
         get { return Simulation; }
      }

      protected override void UpdateSimulationProperties()
      {
         _simulationPropertiesMapper.MapProperties(_simulationPropertiesDTO, Simulation);
         SaveBuildingBlocksConfiguration();
         _simulationModelCreator.CreateModelFor(Simulation);
      }

      public override void ModelConfigurationDone()
      {
         if (PresenterAt(SimulationItems.Model).SimulationCreated) return;

         SaveBuildingBlocksConfiguration();

         PresenterAt(SimulationItems.Model).CreateSimulation();
         AllSimulationItemsAfterModel.Each(x => PresenterAt(x).EditSimulation(Simulation, CreationMode.New));
      }

      protected override string HeayWorkCaption
      {
         get { return PKSimConstants.UI.CreatingSimulation; }
      }
   }
}