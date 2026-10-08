using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Events;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    Sets the version of a <see cref="UsedBuildingBlock" /> to the version its template building block has when the
   ///    command executes, so that a building block in sync with its template stays in sync after the template was changed
   ///    earlier in the same macro command.
   /// </summary>
   public class SynchronizeUsedBuildingBlockVersionCommand : PKSimReversibleCommand
   {
      private readonly string _simulationId;
      private readonly string _usedBuildingBlockId;
      private readonly string _templateBuildingBlockId;
      private int _oldVersion;
      private Simulation _simulation;
      private UsedBuildingBlock _usedBuildingBlock;
      private IPKSimBuildingBlock _templateBuildingBlock;

      public SynchronizeUsedBuildingBlockVersionCommand(Simulation simulation, UsedBuildingBlock usedBuildingBlock, IPKSimBuildingBlock templateBuildingBlock, IExecutionContext context)
      {
         _simulation = simulation;
         _usedBuildingBlock = usedBuildingBlock;
         _templateBuildingBlock = templateBuildingBlock;
         _simulationId = simulation.Id;
         _usedBuildingBlockId = usedBuildingBlock.Id;
         _templateBuildingBlockId = templateBuildingBlock.Id;
         Visible = false;
         ObjectType = PKSimConstants.ObjectTypes.Simulation;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SynchronizeUsedBuildingBlockVersionCommandDescription(context.TypeFor(templateBuildingBlock), usedBuildingBlock.Name, simulation.Name);
         context.UpdateBuildingBlockPropertiesInCommand(this, _simulation);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         _oldVersion = _usedBuildingBlock.Version;
         _usedBuildingBlock.Version = _templateBuildingBlock.Version;
         context.Resolve<IBuildingBlockInProjectManager>().UpdateBuildingBlockNamesUsedIn(_simulation);
         context.PublishEvent(new SimulationStatusChangedEvent(_simulation));
      }

      protected override void ClearReferences()
      {
         _simulation = null;
         _usedBuildingBlock = null;
         _templateBuildingBlock = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context) =>
         new SetUsedBuildingBlockVersionCommand(_simulation, _usedBuildingBlock, _oldVersion, context).AsInverseFor(this);

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _simulation = context.Get<Simulation>(_simulationId);
         _usedBuildingBlock = _simulation.UsedBuildingBlockById(_usedBuildingBlockId);
         _templateBuildingBlock = context.Get<IPKSimBuildingBlock>(_templateBuildingBlockId);
      }
   }
}
