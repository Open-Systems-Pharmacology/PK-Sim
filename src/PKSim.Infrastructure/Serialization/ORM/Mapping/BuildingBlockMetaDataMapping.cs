using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class BuildingBlockMetaDataMapping : ClassMap<BuildingBlockMetaData>
   {
      public BuildingBlockMetaDataMapping()
      {
         Table("BUILDING_BLOCKS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.Name).Not.Nullable();
         Map(x => x.Description);
         Map(x => x.Icon);
         Map(x => x.Version);
         Map(x => x.StructureVersion).Default("0");

         //Content should be lazy loaded for a building block
         References(x => x.Content)
            .LazyLoad()
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_BuildingBlock_Content");

         References(x => x.Properties)
            .Not.LazyLoad()
            .Column("PropertiesId")
            .Fetch.Join()
            .Cascade.All()
            .ForeignKey("fk_BuildingBlock_Properties");
      }
   }
}