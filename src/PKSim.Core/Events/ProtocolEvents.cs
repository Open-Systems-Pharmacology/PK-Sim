using PKSim.Core.Model;

namespace PKSim.Core.Events
{
   public interface IProtocolEvent
   {
      Protocol Protocol { get; }
   }

   public class ProtocolModeChangedEvent : IProtocolEvent
   {
      public Protocol Protocol { get; private set; }
      public ProtocolMode ProtocolMode { get; private set; }

      public ProtocolModeChangedEvent(Protocol protocol, ProtocolMode newProtocolMode)
      {
         Protocol = protocol;
         ProtocolMode = newProtocolMode;
      }
   }

   public class RemoveSchemaItemFromSchemaEvent : RemoveEntityEvent<ISchemaItem, Schema>
   {
   }

   public class AddSchemaItemToSchemaEvent : AddEntityEvent<ISchemaItem, Schema>
   {
   }

   public class AddSchemaToProtocolEvent : AddEntityEvent<Schema, AdvancedProtocol>
   {
   }

   public class RemoveSchemaFromProtocolEvent : RemoveEntityEvent<Schema, AdvancedProtocol>
   {
   }
}