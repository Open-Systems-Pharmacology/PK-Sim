using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Core;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ObjectPathXmlAttributeMapper : ContextSpecification<ObjectPathXmlAttributeMapper>
   {
      protected SerializationContext _serializationContext;
      private IContainer _container;

      protected override void Context()
      {
         sut = new ObjectPathXmlAttributeMapper();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.Resolve<ObjectPathFactory>()).Returns(new ObjectPathFactoryForSpecs());
         _serializationContext = SerializationTransaction.Create(_container);
      }
   }

   public class When_mapping_an_entity_path_to_a_string : concern_for_ObjectPathXmlAttributeMapper
   {
      private ObjectPath _containerPath;
      private object _result;

      protected override void Context()
      {
         base.Context();
         _containerPath = new ObjectPath(new[] {"TOTO", "Organism"});
      }

      protected override void Because()
      {
         _result = sut.ConvertFrom(sut.Convert(_containerPath, _serializationContext), _serializationContext);
      }

      [Observation]
      public void should_return_a_string_that_can_be_used_to_create_an_identical_entity_path()
      {
         _result.ShouldBeEqualTo(_containerPath);
      }
   }

   public class When_mapping_an_empty_entity_path_ : concern_for_ObjectPathXmlAttributeMapper
   {
      private ObjectPath _entityPath;
      private object _result;

      protected override void Context()
      {
         base.Context();
         _entityPath = new ObjectPath();
      }

      protected override void Because()
      {
         _result = sut.ConvertFrom(sut.Convert(_entityPath, _serializationContext), _serializationContext);
      }

      [Observation]
      public void should_not_crash()
      {
         _result.ShouldBeEqualTo(_entityPath);
      }
   }

   public class When_the_entity_path_xml_attribute_mapper_is_asked_if_a_type_is_a_match : concern_for_ObjectPathXmlAttributeMapper
   {
      [Observation]
      public void should_return_true_for_any_implementation_of_container_path()
      {
         sut.IsMatch(typeof(ObjectPath)).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_otherwise()
      {
         sut.IsMatch(typeof(IEntity)).ShouldBeFalse();
      }
   }
}