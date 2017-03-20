using NHibernate;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   public class UsedBuildingBlockMetaData : MetaData<string>, IUpdatableFrom<UsedBuildingBlockMetaData>
   {
      public virtual string TemplateId { get; set; }
      public virtual int Version { get; set; }
      public virtual int StructureVersion { get; set; }
      public virtual bool Altered { get; set; }
      public virtual PKSimBuildingBlockType BuildingBlockType { get; set; }
      public virtual string Name { get; set; }

      public virtual void UpdateFrom(UsedBuildingBlockMetaData usedBuildingBlockMetaData, ISession session)
      {
         TemplateId = usedBuildingBlockMetaData.TemplateId;
         Version = usedBuildingBlockMetaData.Version;
         StructureVersion = usedBuildingBlockMetaData.StructureVersion;
         Name = usedBuildingBlockMetaData.Name;
         Altered = usedBuildingBlockMetaData.Altered;
         BuildingBlockType = usedBuildingBlockMetaData.BuildingBlockType;
      }
   }
}