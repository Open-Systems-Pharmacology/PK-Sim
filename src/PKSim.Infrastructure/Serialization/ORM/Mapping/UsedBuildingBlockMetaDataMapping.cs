using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class UsedBuildingBlockMetaDataMapping : ClassMap<UsedBuildingBlockMetaData>
   {
      public UsedBuildingBlockMetaDataMapping()
      {
         Table("USED_BUILDING_BLOCKS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.Name).Not.Nullable();
         Map(x => x.TemplateId).Not.Nullable();
         Map(x => x.Version).Not.Nullable();
         Map(x => x.StructureVersion).Not.Nullable();
         Map(x => x.BuildingBlockType).Not.Nullable();
         Map(x => x.Altered);
      }
   }
}