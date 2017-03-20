using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class ProtocolMetaDataMapping : SubclassMap<ProtocolMetaData>
   {
      public ProtocolMetaDataMapping()
      {
         Table("PROTOCOLS");
         KeyColumn("ProtocolId");
         Map(x => x.ProtocolMode);
      }
   }
}