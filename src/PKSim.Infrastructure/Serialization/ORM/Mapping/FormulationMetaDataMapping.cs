using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class FormulationMetaDataMapping : SubclassMap<FormulationMetaData>
   {
      public FormulationMetaDataMapping()
      {
         Table("FORMULATIONS");
         KeyColumn("FormulationId");
         Map(x => x.FormulationType);
      }
   }
}