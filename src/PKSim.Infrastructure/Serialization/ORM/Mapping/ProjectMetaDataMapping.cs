using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class ProjectMetaDataMapping : ClassMap<ProjectMetaData>
   {
      public ProjectMetaDataMapping()
      {
         Table("PROJECTS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();

         Map(x => x.Name).Not.Nullable();
         Map(x => x.Description);
         Map(x => x.Version);

         References(x => x.Content)
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_Project_Content");

         HasMany(x => x.BuildingBlocks)
            .Not.LazyLoad()
            .Fetch.Select()
            .Cascade.AllDeleteOrphan()
            .ForeignKeyConstraintName("fk_Project_BuildingBlocks")
            .KeyColumn("ProjectId")
            .AsSet();

         HasMany(x => x.SimulationComparisons)
            .Not.LazyLoad()
            .Fetch.Select()
            .Cascade.AllDeleteOrphan()
            .ForeignKeyConstraintName("fk_Project_SummaryCharts")
            .KeyColumn("ProjectId")
            .AsSet();

         HasMany(x => x.AllObservedData)
            .Not.LazyLoad()
            .Fetch.Select()
            .Cascade.AllDeleteOrphan()
            .ForeignKeyConstraintName("fk_Project_ObservedData")
            .KeyColumn("ProjectId")
            .AsSet();

         HasMany(x => x.ParameterIdentifications)
            .Not.LazyLoad()
            .Fetch.Select()
            .Cascade.AllDeleteOrphan()
            .ForeignKeyConstraintName("fk_Project_ParameterIdentifications")
            .KeyColumn("ProjectId")
            .AsSet();

         HasMany(x => x.SensitivityAnalyses)
           .Not.LazyLoad()
           .Fetch.Select()
           .Cascade.AllDeleteOrphan()
           .ForeignKeyConstraintName("fk_Project_SensitivityAnalyses")
           .KeyColumn("ProjectId")
           .AsSet();
      }
   }
}