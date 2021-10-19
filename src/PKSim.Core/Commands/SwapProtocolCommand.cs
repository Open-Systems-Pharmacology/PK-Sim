using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SwapProtocolCommand : SwapBuildingBlockCommand<Protocol>, IBuildingBlockStructureChangeCommand
   {
      //remove event should not be raised for protocol swap since we do not want the edited presenter to close
      public SwapProtocolCommand(Protocol oldProtocol, Protocol newProtocol, IExecutionContext context) : base(oldProtocol, newProtocol, context, raiseRemoveEvent: false)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         context.PublishEvent(new SwapBuildingBlockEvent(_oldBuildingBlock, _newBuildingBlock));
         base.PerformExecuteWith(context);

      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SwapProtocolCommand(_newBuildingBlock, _oldBuildingBlock, context).AsInverseFor(this);
      }
   }
}