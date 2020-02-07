using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Xml.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class DatabaseOntogenyXmlSerializer : BaseXmlSerializer<DatabaseOntogeny>
   {
      public override void PerformMapping()
      {
         //For database ontogeny, name is the only information required to map out which ontogeny was saved
         Map(x => x.Name).WithMappingName(CoreConstants.Serialization.Attribute.Name);
         Map(x => x.SpeciesName).WithMappingName(CoreConstants.Serialization.Attribute.Species);
      }

      public override DatabaseOntogeny CreateObject(XElement element, SerializationContext serializationContext)
      {
         var ontogenyName = element.GetAttribute(CoreConstants.Serialization.Attribute.Name);
         var speciesName = element.GetAttribute(CoreConstants.Serialization.Attribute.Species);
         if (string.IsNullOrEmpty(ontogenyName) || string.IsNullOrEmpty(speciesName))
            return new NullOntogeny();

         var ontogenyRepository = serializationContext.Resolve<IOntogenyRepository>();
         var ontogeny = ontogenyRepository.AllFor(speciesName).FindByName(ontogenyName) as DatabaseOntogeny;
         return ontogeny ?? new NullOntogeny();
      }
   }

   public class UserDefinedOntogenyXmlSerializer : ObjectBaseXmlSerializer<UserDefinedOntogeny>, IPKSimXmlSerializer
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.SpeciesName);
         Map(x => x.DisplayName);
         Map(x => x.Table);
      }
   }
}