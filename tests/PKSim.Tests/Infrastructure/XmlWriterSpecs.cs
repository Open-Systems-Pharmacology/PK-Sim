using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Serializer.Xml;
using FakeItEasy;
using PKSim.Infrastructure.Serialization.Xml;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_XmlWriter<T> : ContextSpecification<IXmlWriter<T>>
   {
      protected IPKSimXmlSerializerRepository _serializerRepository;

      protected override void Context()
      {
         _serializerRepository = A.Fake<IPKSimXmlSerializerRepository>();
         sut = new XmlWriter<T>(_serializerRepository);
      }
   }

   public class When_serializing_an_object_to_xml : concern_for_XmlWriter<IEntity>
   {
      private XElement _serializedElement;
      private IEntity _entityToSerialize;
      private IXmlSerializer<SerializationContext> _entitySerializer;
      private XElement _element;
      private SerializationContext _serializationContext;

      protected override void Context()
      {
         base.Context();
         _element = new XElement("TOTO");
         _entityToSerialize = A.Fake<IEntity>();
         _entitySerializer = A.Fake<IXmlSerializer<SerializationContext>>();
         A.CallTo(() => _entitySerializer.ObjectType).Returns(typeof(IEntity));
         _serializationContext = SerializationTransaction.Create();
         A.CallTo(() => _entitySerializer.Serialize(_entityToSerialize, _serializationContext)).Returns(_element);
         A.CallTo(() => _serializerRepository.SerializerFor(_entityToSerialize)).Returns(_entitySerializer);
      }

      protected override void Because()
      {
         _serializedElement = sut.WriteFor(_entityToSerialize, _serializationContext);
      }

      [Observation]
      public void should_retrieve_a_serializer_for_the_given_objec_type_and_return_the_serialized_element()
      {
         _serializedElement.ShouldBeEqualTo(_element);
      }

      [Observation]
      public void should_serialize_the_formula_cache_if_required()
      {
         A.CallTo(() => _serializerRepository.SerializeFormulaCache(_element, _serializationContext, _entityToSerialize.GetType())).MustHaveHappened();
      }
   }
}