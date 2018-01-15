using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   public class SetUsedBuildingBlockAlteredFlagCommand : PKSimReversibleCommand
   {
      private readonly bool _altered;
      private readonly string _simulationId;
      private readonly string _usedBuildingBlockId;
      private bool _oldAltered;
      private Simulation _simulation;
      private UsedBuildingBlock _usedBuildingBlock;

      public SetUsedBuildingBlockAlteredFlagCommand(Simulation simulation, UsedBuildingBlock usedBuildingBlock, bool altered, IExecutionContext context)
      {
         _simulation = simulation;
         _usedBuildingBlock = usedBuildingBlock;
         _simulationId = simulation.Id;
         _usedBuildingBlockId = usedBuildingBlock.Id;
         _altered = altered;
         IPKSimBuildingBlock buildingBlock = usedBuildingBlock.BuildingBlock;
         Visible = false;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = string.Format(PKSimConstants.Command.SetUsedBuildingBlockAlteredFlagCommandDescription,
            context.TypeFor(buildingBlock), usedBuildingBlock.Name,
            _altered, _simulation.Name);

         context.UpdateBuildinBlockPropertiesInCommand(this, _simulation);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         _oldAltered = _usedBuildingBlock.Altered;
         _usedBuildingBlock.Altered = _altered;
         var buildingBlockInSimulationManager = context.Resolve<IBuildingBlockInSimulationManager>();
         buildingBlockInSimulationManager.UpdateBuildingBlockNamesUsedIn(_simulation);

         context.PublishEvent(new SimulationStatusChangedEvent(_simulation));
      }

      protected override void ClearReferences()
      {
         _simulation = null;
         _usedBuildingBlock = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetUsedBuildingBlockAlteredFlagCommand(_simulation, _usedBuildingBlock, _oldAltered, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _simulation = context.Get<Simulation>(_simulationId);
         _usedBuildingBlock = _simulation.UsedBuildingBlockById(_usedBuildingBlockId);
      }
   }
}