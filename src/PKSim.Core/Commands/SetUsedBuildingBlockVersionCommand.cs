using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   public class SetUsedBuildingBlockVersionCommand : PKSimReversibleCommand
   {
      private readonly string _simulationId;
      private readonly string _usedBuildingBlockId;
      private readonly int _version;
      private int _oldVersion;
      private Simulation _simulation;
      private UsedBuildingBlock _usedBuildingBlock;

      public SetUsedBuildingBlockVersionCommand(Simulation simulation, UsedBuildingBlock usedBuildingBlock, int version, IExecutionContext context)
      {
         _simulation = simulation;
         _usedBuildingBlock = usedBuildingBlock;
         _simulationId = simulation.Id;
         _usedBuildingBlockId = usedBuildingBlock.Id;
         _version = version;
         IPKSimBuildingBlock buildingBlock = usedBuildingBlock.BuildingBlock;
         Visible = false;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = string.Format(PKSimConstants.Command.SetUsedBuildingBlockVersionCommandDescription,
            context.TypeFor(buildingBlock), usedBuildingBlock.Name, _version, _simulation.Name);
         context.UpdateBuildingBlockPropertiesInCommand(this, _simulation);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         _oldVersion = _usedBuildingBlock.Version;
         _usedBuildingBlock.Version = _version;
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
         return new SetUsedBuildingBlockVersionCommand(_simulation, _usedBuildingBlock, _oldVersion, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _simulation = context.Get<Simulation>(_simulationId);
         _usedBuildingBlock = _simulation.UsedBuildingBlockById(_usedBuildingBlockId);
      }
   }
}