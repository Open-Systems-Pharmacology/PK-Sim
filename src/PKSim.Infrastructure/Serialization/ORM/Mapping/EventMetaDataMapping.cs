using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class EventMetaDataMapping : SubclassMap<EventMetaData>
   {
      public EventMetaDataMapping()
      {
         Table("EVENTS");
         KeyColumn("EventId");
      }  
   }
}