using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Serializer;
using OSPSuite.Serializer.Xml;
using FakeItEasy;
using PKSim.Infrastructure.Serialization.Xml;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_XmlReader<T> : ContextSpecification<IXmlReader<T>> where T : class
   {
      protected IPKSimXmlSerializerRepository _serializerRepository;

      protected override void Context()
      {
         _serializerRepository = A.Fake<IPKSimXmlSerializerRepository>();

         sut = new XmlReader<T>(_serializerRepository);
      }
   }

   public class When_deserializing_an_xml_element_to_an_object_for_which_the_type_was_registered : concern_for_XmlReader<IEntity>
   {
      private XElement _element;
      private IEntity _deserializedObject;
      private IXmlSerializer<SerializationContext> _entitySerializer;
      private IEntity _entity;
      private SerializationContext _serializationContext;

      protected override void Context()
      {
         base.Context();
         _entitySerializer = A.Fake<IXmlSerializer<SerializationContext>>();
         _serializationContext = SerializationTransaction.Create();
         _deserializedObject = A.Fake<IEntity>();
         A.CallTo(() => _entitySerializer.ObjectType).Returns(typeof(IEntity));

         _element = new XElement("TUTU");
         A.CallTo(() => _entitySerializer.Deserialize<IEntity>(_element, _serializationContext)).Returns(_deserializedObject);
         //serializers only defined for type but not for element
         A.CallTo(() => _serializerRepository.SerializerFor(typeof (IEntity))).Returns(_entitySerializer);

         A.CallTo(() => _serializerRepository.SerializerFor(_element)).Throws(new SerializerNotFoundException("toto"));
      }

      protected override void Because()
      {
         _entity = sut.ReadFrom(_element, _serializationContext);
      }


      [Observation]
      public void should_deserialize_the_formula_cahce_using_the_repository()
      {
         A.CallTo(() => _serializerRepository.DeserializeFormulaCache(_element, _serializationContext, typeof(IEntity))).MustHaveHappened();
      }

      [Observation]
      public void should_retrieve_a_serializer_for_the_given_object_type_from_the_repository()
      {
         _entity.ShouldBeEqualTo(_deserializedObject);
      }
   }

   public class When_deserializing_an_xml_element_to_an_object_for_which_the_xml_element_was_registered : concern_for_XmlReader<IEntity>
   {
      private XElement _element;
      private IEntity _deserializedObject;
      private IXmlSerializer<SerializationContext> _entitySerializer;
      private IEntity _entity;
      private SerializationContext _serializationContext;

      protected override void Context()
      {
         base.Context();
         _entitySerializer = A.Fake<IXmlSerializer<SerializationContext>>();
         _serializationContext = SerializationTransaction.Create();

         _deserializedObject = A.Fake<IEntity>();
         _element = new XElement("TUTU");
         A.CallTo(() => _entitySerializer.Deserialize<IEntity>(_element, _serializationContext)).Returns(_deserializedObject);
         A.CallTo(() => _serializerRepository.SerializerFor(_element)).Returns(_entitySerializer);
      }

      protected override void Because()
      {
         _entity = sut.ReadFrom(_element, _serializationContext);
      }

      [Observation]
      public void should_retrieve_a_serializer_for_the_given_xml_element_from_the_repository()
      {
         _entity.ShouldBeEqualTo(_deserializedObject);
      }
   }

   public class When_deserializing_an_xml_element_to_an_object_for_which_the_type_was_not_registered_but_a_serializer_was_found_for_the_element : concern_for_XmlReader<IEntity>
   {
      private XElement _element;
      private IEntity _deserializedObject;
      private IXmlSerializer<SerializationContext> _entitySerializer;
      private IEntity _entity;
      private SerializationContext _serializationContext;

      protected override void Context()
      {
         base.Context();
         _entitySerializer = A.Fake<IXmlSerializer<SerializationContext>>();
         _deserializedObject = A.Fake<IEntity>();
         _serializationContext = SerializationTransaction.Create();
         _element = new XElement("TUTU");
         A.CallTo(() => _entitySerializer.Deserialize<IEntity>(_element, _serializationContext)).Returns(_deserializedObject);
         A.CallTo(() => _serializerRepository.SerializerFor(_element)).Returns(_entitySerializer);
         A.CallTo(() => _serializerRepository.SerializerFor(typeof (IEntity))).Throws(new SerializerNotFoundException(typeof (IEntity)));
      }

      protected override void Because()
      {
         _entity = sut.ReadFrom(_element, _serializationContext);
      }

      [Observation]
      public void should_retreive_a_serializer_for_the_given_object_type_from_the_repository()
      {
         _entity.ShouldBeEqualTo(_deserializedObject);
      }
   }
}