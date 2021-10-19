using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public abstract class SwapBuildingBlockCommand<TBuildingBlock> : BuildingBlockChangeCommand where TBuildingBlock : class, IPKSimBuildingBlock
   {
      private readonly string _newBuildingBlockId;
      private readonly bool _raiseRemoveEvent;
      protected TBuildingBlock _newBuildingBlock;
      protected TBuildingBlock _oldBuildingBlock;
      private byte[] _serializationStream;

      protected SwapBuildingBlockCommand(TBuildingBlock oldBuildingBlock, TBuildingBlock newBuildingBlock, IExecutionContext context) :
         this(oldBuildingBlock, newBuildingBlock, context, raiseRemoveEvent: true)
      {
      }

      protected SwapBuildingBlockCommand(TBuildingBlock oldBuildingBlock, TBuildingBlock newBuildingBlock, IExecutionContext context, bool raiseRemoveEvent)
      {
         _oldBuildingBlock = oldBuildingBlock;
         _newBuildingBlock = newBuildingBlock;
         BuildingBlockId = _newBuildingBlock.Id;
         _raiseRemoveEvent = raiseRemoveEvent;
         _newBuildingBlockId = newBuildingBlock.Id;
         CommandType = PKSimConstants.Command.CommandTypeUpdate;
         ObjectType = context.TypeFor(oldBuildingBlock);
         Description = PKSimConstants.Command.SwapBuildingCommandDescription(ObjectType, _oldBuildingBlock.Name);
         context.UpdateBuildingBlockPropertiesInCommand(this, _newBuildingBlock);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //Remove old building block
         var removeCommand = PerformRemoveCommand(context);
         _serializationStream = removeCommand.SerializationStream;

         //Add new building block
         PerformAddCommand(context);

         //Update properties
         _newBuildingBlock.Version = _oldBuildingBlock.Version;
         _newBuildingBlock.StructureVersion = _oldBuildingBlock.StructureVersion;
         _newBuildingBlock.Name = _oldBuildingBlock.Name;
         _newBuildingBlock.Description = _oldBuildingBlock.Description;
      }

      protected virtual void PerformAddCommand(IExecutionContext context)
      {
         var addCommand = new AddBuildingBlockToProjectCommand(_newBuildingBlock, context);
         addCommand.Execute(context);
      }

      protected virtual RemoveBuildingBlockFromProjectCommand PerformRemoveCommand(IExecutionContext context)
      {
         var removeCommand = new RemoveBuildingBlockFromProjectCommand(_oldBuildingBlock, context, _raiseRemoveEvent, dueToSwap: true);
         removeCommand.Execute(context);
         return removeCommand;
      }

      protected override void ClearReferences()
      {
         _oldBuildingBlock = null;
         _newBuildingBlock = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _oldBuildingBlock = context.Deserialize<TBuildingBlock>(_serializationStream);
         _newBuildingBlock = context.Get<TBuildingBlock>(_newBuildingBlockId);
      }
   }

   public class SwapBuildingBlockCommand : SwapBuildingBlockCommand<IPKSimBuildingBlock>
   {
      public SwapBuildingBlockCommand(IPKSimBuildingBlock oldBuildingBlock, IPKSimBuildingBlock newBuildingBlock, IExecutionContext context) : base(oldBuildingBlock, newBuildingBlock, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SwapBuildingBlockCommand(_oldBuildingBlock, _newBuildingBlock, context).AsInverseFor(this);
      }
   }
}