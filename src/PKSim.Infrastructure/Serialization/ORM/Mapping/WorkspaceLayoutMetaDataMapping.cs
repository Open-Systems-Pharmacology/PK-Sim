using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class WorkspaceLayoutMetaDataMapping : ClassMap<WorkspaceLayoutMetaData>
   {
      public WorkspaceLayoutMetaDataMapping()
      {
         Table("WORKSPACE_LAYOUT");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();
         References(x => x.Content)
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_WorkspaceLayout_Content");

      }
   }
}