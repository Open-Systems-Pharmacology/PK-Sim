using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class SetSimpleProtocolEventCommand : BuildingBlockStructureChangeCommand
   {
      private readonly string _newTemplateEventId;
      private readonly string _protocolId;
      private string _oldTemplateEventId;
      private SimpleProtocol _protocol;

      public SetSimpleProtocolEventCommand(SimpleProtocol protocol, string newTemplateEventId, IExecutionContext context)
      {
         _protocol = protocol;
         _newTemplateEventId = newTemplateEventId;
         _protocolId = protocol.Id;
         ObjectType = PKSimConstants.ObjectTypes.AdministrationProtocol;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         BuildingBlockId = protocol.Id;
         context.UpdateBuildingBlockPropertiesInCommand(this, protocol);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         _oldTemplateEventId = _protocol.TemplateEventId;
         _protocol.TemplateEventId = _newTemplateEventId;
         Description = PKSimConstants.Command.SetSimpleProtocolEventDescription(_oldTemplateEventId, _newTemplateEventId);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _protocol = context.Get<SimpleProtocol>(_protocolId);
      }

      protected override void ClearReferences()
      {
         _protocol = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetSimpleProtocolEventCommand(_protocol, _oldTemplateEventId, context).AsInverseFor(this);
      }
   }
}
