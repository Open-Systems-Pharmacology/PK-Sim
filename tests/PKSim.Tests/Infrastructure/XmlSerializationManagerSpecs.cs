using System.Text;
using System.Xml.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.Serialization.Xml;
using PKSim.Infrastructure.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_XmlSerializationManager : ContextSpecification<XmlSerializationManager>
   {
      protected Individual _individual;
      protected IXmlReader<Individual> _xmlReaderIndividual;
      protected IXmlWriter<Individual> _xmlWriterIndividual;
      protected IXmlReader<Simulation> _xmlReaderSimulation;
      protected IXmlWriter<Simulation> _xmlWriterSimulation;
      protected IReferencesResolver _referenceResolver;
      private IContainer _container;
      protected IObjectConverterFinder _objectConverterFinder;
      protected ISimulationUpdaterAfterDeserialization _simulationUpdater;
      protected IEventPublisher _eventPublisher;
      protected ISerializationContextFactory _serializationContextFactory;
      protected SerializationContext _serializationContext;

      protected override void Context()
      {
         _individual = new Individual();
         _container = A.Fake<IContainer>();
         _serializationContextFactory = A.Fake<ISerializationContextFactory>();
         _simulationUpdater = A.Fake<ISimulationUpdaterAfterDeserialization>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _xmlReaderIndividual = A.Fake<IXmlReader<Individual>>();
         _xmlWriterIndividual = A.Fake<IXmlWriter<Individual>>();
         _xmlReaderSimulation = A.Fake<IXmlReader<Simulation>>();
         _xmlWriterSimulation = A.Fake<IXmlWriter<Simulation>>();
         _objectConverterFinder = A.Fake<IObjectConverterFinder>();
         _serializationContext = A.Fake<SerializationContext>();
         A.CallTo(() => _container.Resolve<IXmlReader<Individual>>()).Returns(_xmlReaderIndividual);
         A.CallTo(() => _container.Resolve<IXmlWriter<Individual>>()).Returns(_xmlWriterIndividual);
         A.CallTo(() => _container.Resolve<IXmlReader<Simulation>>()).Returns(_xmlReaderSimulation);
         A.CallTo(() => _container.Resolve<IXmlWriter<Simulation>>()).Returns(_xmlWriterSimulation);

         _referenceResolver = A.Fake<IReferencesResolver>();
         sut = new XmlSerializationManager(_referenceResolver, _container, _objectConverterFinder, _simulationUpdater, _eventPublisher, _serializationContextFactory);
         A.CallTo(_serializationContextFactory).WithReturnType<SerializationContext>().Returns(_serializationContext);
      }
   }

   public class When_serializing_an_entity : concern_for_XmlSerializationManager
   {
      private XElement _serializedElement;
      private byte[] _result;

      protected override void Context()
      {
         base.Context();
         _serializedElement = new XElement("Indiviudal");
         A.CallTo(() => _xmlWriterIndividual.WriteFor(_individual, _serializationContext)).Returns(_serializedElement);
      }

      protected override void Because()
      {
         _result = sut.Serialize(_individual);
      }

      [Observation]
      public void should_leverate_the_xml_writer_to_retrieve_the_xml_string()
      {
         _result.Length.ShouldBeGreaterThan(0);
      }
   }

   public class When_deserializing_an_object : concern_for_XmlSerializationManager
   {
      private Individual _result;

      protected override void Context()
      {
         base.Context();
         _result = A.Fake<Individual>();
         A.CallTo(() => _xmlReaderIndividual.ReadFrom(A<XElement>._, _serializationContext)).Returns(_result);
      }

      protected override void Because()
      {
         var element = new XElement("Individual");
         element.AddAttribute(CoreConstants.Serialization.Attribute.XmlVersion, ProjectVersions.CurrentAsString);
         _result = sut.Deserialize<Individual>(Encoding.UTF8.GetBytes(element.ToString()));
      }

      [Observation]
      public void should_leverage_the_xml_reader_to_retrieve_object_from_the_xml_string()
      {
         A.CallTo(() => _xmlReaderIndividual.ReadFrom(A<XElement>._, _serializationContext)).MustHaveHappened();
      }
   }

   public class When_deserializing_a_simulation : concern_for_XmlSerializationManager
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _simulation.Model = A.Fake<IModel>();
         A.CallTo(() => _xmlReaderSimulation.ReadFrom(A<XElement>._, _serializationContext)).Returns(_simulation);
      }

      protected override void Because()
      {
         var element = new XElement("Simulation");
         element.AddAttribute(CoreConstants.Serialization.Attribute.XmlVersion, ProjectVersions.CurrentAsString);
         element.Add(new XElement("FormulaCache"));
         _simulation = sut.Deserialize<Simulation>(Encoding.UTF8.GetBytes(element.ToString()));
      }

      [Observation]
      public void should_create_a_serialization_context_that_does_not_reference_project_simulations()
      {
         A.CallTo(() => _serializationContextFactory.Create(null, null, false, true)).MustHaveHappened();
      }

      [Observation]
      public void should_resolve_the_references_of_formula_defined_in_the_simulation()
      {
         A.CallTo(() => _simulationUpdater.UpdateSimulation(_simulation)).MustHaveHappened();
      }
   }

   public class When_deserializing_an_individual_from_a_previous_version : concern_for_XmlSerializationManager
   {
      private IObjectConverter _converter;
      private XElement _element;

      protected override void Context()
      {
         base.Context();
         _individual = A.Fake<Individual>();
         _converter = A.Fake<IObjectConverter>();
         A.CallTo(() => _xmlReaderIndividual.ReadFrom(A<XElement>._, _serializationContext)).Returns(_individual);
         A.CallTo(() => _objectConverterFinder.FindConverterFor(ProjectVersions.V6_1_2)).Returns(_converter);
         _element = new XElement("Individual");
         _element.AddAttribute(CoreConstants.Serialization.Attribute.XmlVersion, ProjectVersions.V6_1_2.VersionAsString);
         A.CallTo(() => _converter.Convert(_individual, ProjectVersions.V6_1_2)).Returns((ProjectVersions.Current, true));
         A.CallTo(() => _converter.ConvertXml(A<XElement>._, ProjectVersions.V6_1_2)).Returns((ProjectVersions.Current, true));
      }

      protected override void Because()
      {
         _individual = sut.Deserialize<Individual>(Encoding.UTF8.GetBytes(_element.ToString()));
      }

      [Observation]
      public void should_have_retrieved_a_converter_for_the_individual_and_convert_the_new_individual()
      {
         A.CallTo(() => _converter.Convert(_individual, ProjectVersions.V6_1_2)).MustHaveHappened();
      }

      [Observation]
      public void should_have_notified_the_building_block_converted_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ObjectBaseConvertedEvent>._)).MustHaveHappened();
      }
   }
}