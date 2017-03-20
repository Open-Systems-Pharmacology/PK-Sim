using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class EventProtocolMetaDataMapping : SubclassMap<EventProtocolMetaData>
   {
      public EventProtocolMetaDataMapping()
      {
         Table("EVENT_PROTOCOLS");
         KeyColumn("EventProtocolId");
      }
   }
}