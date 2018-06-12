using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class SetProtocolModeCommand : PKSimMacroCommand
   {
      private readonly ProtocolMode _newProtocolMode;
      private readonly ProtocolMode _oldProtocolMode;
      private Protocol _protocol;

      public SetProtocolModeCommand(Protocol protocol, ProtocolMode oldProtocolMode, ProtocolMode newProtocolMode, IExecutionContext context)
      {
         _protocol = protocol;
         _oldProtocolMode = oldProtocolMode;
         _newProtocolMode = newProtocolMode;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.SetProtocolModeCommandDescription(oldProtocolMode.ToString(), newProtocolMode.ToString());
         context.UpdateBuildinBlockPropertiesInCommand(this, _protocol);
      }

      public override void Execute(IExecutionContext context)
      {
         //create new protocol for the given mode
         var protocolFactory = context.Resolve<IProtocolFactory>();
         var protocolUpdater = context.Resolve<IProtocolUpdater>();

         Protocol newProtocol = protocolFactory.Create(_newProtocolMode);
         protocolUpdater.UpdateProtocol(_protocol, newProtocol);

         //do this first so that this command will be executed last during the undo
         Add(new StartingChangeProtocolModeCommand(_protocol, _oldProtocolMode, _newProtocolMode, context));

         //Swap old and new protocol
         Add(new SwapProtocolCommand(_protocol, newProtocol, context));

         //do this first so that this command will be executed last during the undo
         Add(new FinishingChangeProtocolModeCommand(newProtocol, _oldProtocolMode, _newProtocolMode, context));

         base.Execute(context);

         //clear references
         _protocol = null;
      }
   }
}