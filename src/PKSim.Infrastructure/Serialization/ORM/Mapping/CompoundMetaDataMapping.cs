using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class CompoundMetaDataMapping : SubclassMap<CompoundMetaData>
   {
      public CompoundMetaDataMapping()
      {
         Table("COMPOUNDS");
         KeyColumn("CompoundId");
      }
   }
}