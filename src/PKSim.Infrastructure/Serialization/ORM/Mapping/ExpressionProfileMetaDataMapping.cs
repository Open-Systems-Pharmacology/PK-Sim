using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class ExpressionProfileMetaDataMapping : SubclassMap<ExpressionProfileMetaData>
   {
      public ExpressionProfileMetaDataMapping()
      {
         Table("EXPRESSION_PROFILES");
         KeyColumn("ExpressionProfileId");
      }
   }
}