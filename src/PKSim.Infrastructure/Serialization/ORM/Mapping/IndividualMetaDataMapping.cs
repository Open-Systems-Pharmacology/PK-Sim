using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class IndividualMetaDataMapping : SubclassMap<IndividualMetaData>
   {
      public IndividualMetaDataMapping()
      {
         Table("INDIVIDUALS");
         KeyColumn("IndividualId");
         Map(x => x.ExpressionProfileIds);
      }
   }
}