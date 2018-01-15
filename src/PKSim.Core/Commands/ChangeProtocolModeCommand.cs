using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public abstract class ChangeProtocolModeCommand : PKSimReversibleCommand
   {
      protected readonly ProtocolMode _newProtocolMode;
      protected readonly ProtocolMode _oldProtocolMode;
      private readonly string _protocolId;
      protected Protocol _protocol;

      protected ChangeProtocolModeCommand(Protocol protocol, ProtocolMode oldProtocolMode, ProtocolMode newProtocolMode, IExecutionContext context)
      {
         _protocol = protocol;
         _protocolId = protocol.Id;
         _oldProtocolMode = oldProtocolMode;
         _newProtocolMode = newProtocolMode;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         context.UpdateBuildinBlockPropertiesInCommand(this, _protocol);

         Visible = false;
      }

      protected override void ClearReferences()
      {
         _protocol = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         _protocol = context.Get<Protocol>(_protocolId);
      }
   }

   public class StartingChangeProtocolModeCommand : ChangeProtocolModeCommand
   {
      public StartingChangeProtocolModeCommand(Protocol protocol, ProtocolMode oldProtocolMode, ProtocolMode newProtocolMode, IExecutionContext context) : base(protocol, oldProtocolMode, newProtocolMode, context)
      {
         Description = PKSimConstants.Command.ProtocolModeChangingFrom(oldProtocolMode.ToString(), newProtocolMode.ToString());
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         //nothing to do
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new FinishingChangeProtocolModeCommand(_protocol, _newProtocolMode, _oldProtocolMode, context).AsInverseFor(this);
      }
   }

   public class FinishingChangeProtocolModeCommand : ChangeProtocolModeCommand
   {
      public FinishingChangeProtocolModeCommand(Protocol protocol, ProtocolMode oldProtocolMode, ProtocolMode newProtocolMode, IExecutionContext context) : base(protocol, oldProtocolMode, newProtocolMode, context)
      {
         Description = PKSimConstants.Command.ProtocolModeChangedFrom(oldProtocolMode.ToString(), newProtocolMode.ToString());
      }

      protected override void ExecuteWith(IExecutionContext context)
      {
         context.PublishEvent(new ProtocolModeChangedEvent(_protocol, _newProtocolMode));
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new StartingChangeProtocolModeCommand(_protocol, _newProtocolMode, _oldProtocolMode, context).AsInverseFor(this);
      }
   }
}