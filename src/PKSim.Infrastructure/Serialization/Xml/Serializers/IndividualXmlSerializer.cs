using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class IndividualXmlSerializer : BuildingBlockXmlSerializer<Individual>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.OriginData);
         Map(x => x.Seed);

      }

      protected override XElement TypedSerialize(Individual individual, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(individual, serializationContext);
         //Expression profile is saved as reference so we have to deal with it separately
         element.Add(SerializerRepository.CreateExpressionProfileReferenceListElement(individual));
         return element;
      }

      protected override void TypedDeserialize(Individual individual, XElement individualElement, SerializationContext serializationContext)
      {
         //We need to retrieve the expression profile from the project (e.g they are registered in the global WithIdRepository)
         var lazyLoadTask = serializationContext.Resolve<ILazyLoadTask>();
         var withIdRepository = serializationContext.Resolve<IWithIdRepository>();

         individualElement.AddReferencedExpressionProfiles(individual, withIdRepository, lazyLoadTask);
         base.TypedDeserialize(individual, individualElement, serializationContext);
      }
   }
}