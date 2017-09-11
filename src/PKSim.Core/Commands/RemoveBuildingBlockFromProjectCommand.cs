using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;

namespace PKSim.Core.Commands
{
   public class RemoveBuildingBlockFromProjectCommand : PKSimReversibleCommand
   {
      private readonly bool _raiseEvent;

      /// <summary>
      ///    indicates that the remove command comes from the swap command
      /// </summary>
      private readonly bool _dueToSwap;

      protected IPKSimBuildingBlock _buildingBlockToRemove;
      public byte[] SerializationStream { get; private set; }

      public RemoveBuildingBlockFromProjectCommand(IPKSimBuildingBlock buildingBlock, IExecutionContext context) : this(buildingBlock, context, true, false)
      {
      }

      public RemoveBuildingBlockFromProjectCommand(IPKSimBuildingBlock buildingBlock, IExecutionContext context, bool raiseEvent, bool dueToSwap)
      {
         _buildingBlockToRemove = buildingBlock;
         _raiseEvent = raiseEvent;
         _dueToSwap = dueToSwap;
         CommandType = PKSimConstants.Command.CommandTypeDelete;
         ObjectType = context.TypeFor(buildingBlock);
         context.UpdateBuildinBlockProperties(this, buildingBlock);
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         var project = context.CurrentProject;

         project.RemoveBuildingBlock(_buildingBlockToRemove);
         Description = PKSimConstants.Command.RemoveEntityFromContainer(ObjectType, _buildingBlockToRemove.Name, context.TypeFor(project), project.Name);

         //remove results for a simulation. This should be done before serialization
         removeResultsForSimulation(_buildingBlockToRemove as Simulation);

         SerializationStream = context.Serialize(_buildingBlockToRemove);
         context.Unregister(_buildingBlockToRemove);

         if (!_raiseEvent) return;

         context.PublishEvent(new BuildingBlockRemovedEvent(_buildingBlockToRemove, context.CurrentProject, _dueToSwap));
         var simulation = _buildingBlockToRemove as ISimulation;
         if(simulation != null)
            context.PublishEvent(new SimulationRemovedEvent(simulation));
      }

      private void removeResultsForSimulation(Simulation simulation)
      {
         simulation?.ClearResults();
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _buildingBlockToRemove = context.Deserialize<IPKSimBuildingBlock>(SerializationStream);
      }

      protected override void ClearReferences()
      {
         _buildingBlockToRemove = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddBuildingBlockToProjectCommand(_buildingBlockToRemove, context).AsInverseFor(this);
      }
   }
}