using FluentNHibernate.Mapping;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class SensitivityAnalysisMetaDataMapping : ClassMap<SensitivityAnalysisMetaData>
   {
      public SensitivityAnalysisMetaDataMapping()
      {
         Table("SENSITIVITY_ANALYSES");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.Name).Not.Nullable();
         Map(x => x.Description);

         References(x => x.Content)
            .LazyLoad()
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_SensitivityAnalysis_Content");

         References(x => x.Properties)
            .Not.LazyLoad()
            .Column("PropertiesId")
            .Fetch.Join()
            .Cascade.All()
            .ForeignKey("fk_SensitivityAnalysis_Properties");
      }
   }
}