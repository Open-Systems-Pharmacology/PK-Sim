using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class ImportPopulationMetaDataMapping : SubclassMap<ImportPopulationMetaData>
   {
      public ImportPopulationMetaDataMapping()
      {
         Table("IMPORT_POPULATIONS");
         KeyColumn("PopulationId");
         Map(x => x.ExpressionProfileIds);
      }
   }
}