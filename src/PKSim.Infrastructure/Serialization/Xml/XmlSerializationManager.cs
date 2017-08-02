using System;
using System.Xml.Linq;
using OSPSuite.Serializer.Xml;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.Serialization.Xml
{
   public class XmlSerializationManager : ISerializationManager
   {
      private readonly IReferencesResolver _referenceResolver;
      private readonly IObjectConverterFinder _objectConverterFinder;
      private readonly ISimulationUpdaterAfterDeserialization _simulationUpdater;
      private readonly IEventPublisher _eventPublisher;
      private readonly ISerializationContextFactory _serializationContextFactory;
      protected readonly IContainer _container;

      public XmlSerializationManager(IReferencesResolver referenceResolver,
         IContainer container, IObjectConverterFinder objectConverterFinder, ISimulationUpdaterAfterDeserialization simulationUpdater,
         IEventPublisher eventPublisher, ISerializationContextFactory serializationContextFactory)
      {
         _referenceResolver = referenceResolver;
         _objectConverterFinder = objectConverterFinder;
         _simulationUpdater = simulationUpdater;
         _eventPublisher = eventPublisher;
         _serializationContextFactory = serializationContextFactory;
         _container = container;
      }

      public byte[] Serialize<TObject>(TObject objectToSerialize)
      {
         using (var context = _serializationContextFactory.Create())
         using (var element = new XElementDisposer(ElementFrom(objectToSerialize, context)))
         {
            element.Element.AddAttribute(CoreConstants.Serialization.Attribute.XmlVersion, ProjectVersions.CurrentAsString);
            return XmlHelper.XmlContentToByte(element.Element);
         }
      }

      public TObject Deserialize<TObject>(byte[] serializationBytes, SerializationContext serializationContext = null)
      {
         TObject deseserializedObject;
         int version;
         var element = getConvertedElementFrom(serializationBytes, out version);

         var context = serializationContext ?? _serializationContextFactory.Create();
         try
         {
            using (new XElementDisposer(element))
            {
               try
               {
                  var xmlObjectReader = XmlReaderFor<TObject>();
                  deseserializedObject = xmlObjectReader.ReadFrom(element, context);
               }
               catch (Exception)
               {
                  context.SkipResolveStep = true;
                  throw;
               }
            }
         }
         finally
         {
            if (serializationContext == null)
               context.Dispose();
         }

         updatePropertiesFor(deseserializedObject, version);
         return deseserializedObject;
      }

      private XElement getConvertedElementFrom(byte[] serializationBytes, out int version)
      {
         var element = ElementFrom(serializationBytes);
         version = versionFrom(element);
         if (!ProjectVersions.CanLoadVersion(version))
            throw new InvalidProjectVersionException(version);

         convertXml(element, version);
         return element;
      }

      public void Deserialize<TObject>(TObject objectToDeserialize, byte[] serializationBytes, SerializationContext serializationContext = null)
      {
         int version;
         var element = getConvertedElementFrom(serializationBytes, out version);

         var context = serializationContext ?? _serializationContextFactory.Create();
            
         try
         {
            using (new XElementDisposer(element))
            {
               try
               {
                  var xmlReader = XmlReaderFor<TObject>();
                  xmlReader.ReadFrom(objectToDeserialize, element, context);
               }
               catch (Exception)
               {
                  context.SkipResolveStep = true;
                  throw;
               }
            }
         }
         finally
         {
            if (serializationContext == null)
               context.Dispose();
         }

         updatePropertiesFor(objectToDeserialize, version);
      }

      private int versionFrom(XElement element)
      {
         string versionString = element.GetAttribute(CoreConstants.Serialization.Attribute.XmlVersion);
         if (string.IsNullOrEmpty(versionString))
            return mssingVersionFor(element);

         return versionString.ConvertedTo<int>();
      }

      private int mssingVersionFor(XElement element)
      {
         if (element.Name.IsOneOf(Constants.Serialization.DATA_REPOSITORY, CoreConstants.Serialization.Project,
            CoreConstants.Serialization.PopulationSettings, CoreConstants.Serialization.SummaryChart,
            CoreConstants.Serialization.OriginData, CoreConstants.Serialization.WorkspaceLayout))
            return ProjectVersions.V5_0_1;

         return ProjectVersions.Current;
      }

      /// <summary>
      ///    This function converts the XElement prior to deserialization if required
      /// </summary>
      private void convertXml(XElement sourceElement, int originalVersion)
      {
         convert(sourceElement, originalVersion, x => x.ConvertXml);
      }

      private bool convert<T>(T objectToConvert, int originalVersion, Func<IObjectConverter, Func<T, int, int>> converterAction)
      {
         int version = originalVersion;
         while (version != ProjectVersions.Current)
         {
            var converter = _objectConverterFinder.FindConverterFor(version);
            version = converterAction(converter).Invoke(objectToConvert, originalVersion);
         }

         return originalVersion != ProjectVersions.Current;
      }

      /// <summary>
      ///    Converts the <paramref name="deserializedObject" /> to convert after the deserialization was made.
      /// </summary>
      /// <returns><c>true</c> if a conversion was performed or <c>false</c> otherwise</returns>
      private bool convert(object deserializedObject, int originalVersion)
      {
         return convert(deserializedObject, originalVersion, x => x.Convert);
      }

      private void updatePropertiesFor<TObject>(TObject deserializedObject, int version)
      {
         //convert object if required
         var conversionHappened = convert(deserializedObject, version);

         var simulation = deserializedObject as Simulation;
         if (simulation != null)
            _simulationUpdater.UpdateSimulation(simulation);

         var individual = deserializedObject as Individual;
         if (individual != null)
            _referenceResolver.ResolveReferencesIn(individual);

         var population = deserializedObject as Population;
         if (population != null)
            _referenceResolver.ResolveReferencesIn(population.FirstIndividual);

         var lazyLoadable = deserializedObject as ILazyLoadable;
         if (lazyLoadable != null)
            lazyLoadable.IsLoaded = true;

         if (conversionHappened && deserializedObject.IsAnImplementationOf<IObjectBase>())
            _eventPublisher.PublishEvent(new ObjectBaseConvertedEvent(deserializedObject.DowncastTo<IObjectBase>(), ProjectVersions.FindBy(version)));
      }

      protected virtual IXmlReader<TObject> XmlReaderFor<TObject>()
      {
         return _container.Resolve<IXmlReader<TObject>>();
      }

      protected virtual XElement ElementFrom<TObject>(TObject objectToSerialize, SerializationContext context)
      {
         var xmlWriter = _container.Resolve<IXmlWriter<TObject>>();
         return xmlWriter.WriteFor(objectToSerialize, context);
      }

      protected virtual XElement ElementFrom(byte[] serializationBytes)
      {
         return XmlHelper.ElementFromBytes(serializationBytes);
      }
   }
}