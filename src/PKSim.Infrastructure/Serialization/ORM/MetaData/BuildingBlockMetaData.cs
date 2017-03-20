using NHibernate;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
   /// <summary>
   /// Abtract class for all entities meta data defined in the model
   /// </summary>
   public abstract class BuildingBlockMetaData : ObjectBaseMetaDataWithProperties<BuildingBlockMetaData>
   {
      public virtual int Version { get; set; }
      public virtual int StructureVersion { get; set; }
      public virtual string Icon { get; set; }

      public override void UpdateFrom(BuildingBlockMetaData sourceChild,ISession session)
      {
         base.UpdateFrom(sourceChild,session);
         Icon = sourceChild.Icon;
         Version = sourceChild.Version;
         StructureVersion = sourceChild.StructureVersion;
      }
   }
}