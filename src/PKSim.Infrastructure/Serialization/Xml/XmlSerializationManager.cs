using System;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Serialization.Xml;
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
         TObject deserializedObject;

         var (element, originalVersion, conversionHappened) = getConvertedElementFrom(serializationBytes);

         var isSimulation = typeof(TObject).IsAnImplementationOf<Simulation>();

         var context = serializationContext ?? _serializationContextFactory.Create(addProjectSimulations:!isSimulation);

         try
         {
            using (new XElementDisposer(element))
            {
               try
               {
                  var xmlObjectReader = XmlReaderFor<TObject>();
                  deserializedObject = xmlObjectReader.ReadFrom(element, context);
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

         updatePropertiesFor(deserializedObject, originalVersion, conversionHappened);
         return deserializedObject;
      }

      private (XElement element, int originalVersion, bool conversionHappened) getConvertedElementFrom(byte[] serializationBytes)
      {
         var element = ElementFrom(serializationBytes);
         var originalVersion = versionFrom(element);
         if (!ProjectVersions.CanLoadVersion(originalVersion))
            throw new InvalidProjectVersionException(originalVersion);

         var conversionHappened = convertXml(element, originalVersion);
         return (element, originalVersion, conversionHappened);
      }

      public void Deserialize<TObject>(TObject objectToDeserialize, byte[] serializationBytes, SerializationContext serializationContext = null)
      {
         var (element, originalVersion, conversionHappened) = getConvertedElementFrom(serializationBytes);

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

         updatePropertiesFor(objectToDeserialize, originalVersion, conversionHappened);
      }

      private int versionFrom(XElement element)
      {
         string versionString = element.GetAttribute(CoreConstants.Serialization.Attribute.XmlVersion);
         if (string.IsNullOrEmpty(versionString))
            return missingVersionFor(element);

         return versionString.ConvertedTo<int>();
      }

      private int missingVersionFor(XElement element)
      {
         if (element.Name.IsOneOf(Constants.Serialization.DATA_REPOSITORY, CoreConstants.Serialization.Project,
            CoreConstants.Serialization.PopulationSettings, CoreConstants.Serialization.SummaryChart,
            CoreConstants.Serialization.OriginData, CoreConstants.Serialization.WorkspaceLayout))
            return ProjectVersions.UNSUPPORTED;

         return ProjectVersions.Current;
      }

      /// <summary>
      ///    This function converts the XElement prior to deserialization if required
      /// </summary>
      private bool convertXml(XElement sourceElement, int originalVersion)
      {
         return convert(sourceElement, originalVersion, x => x.ConvertXml);
      }

      private bool convert<T>(T objectToConvert, int originalVersion, Func<IObjectConverter, Func<T, int, (int, bool)>> converterAction)
      {
         int version = originalVersion;
         bool conversionHappened = false;
         while (version != ProjectVersions.Current)
         {
            var converter = _objectConverterFinder.FindConverterFor(version);
            var (convertedVersion, converted) = converterAction(converter).Invoke(objectToConvert, originalVersion);
            version = convertedVersion;
            conversionHappened = conversionHappened || converted;
         }

         return originalVersion != ProjectVersions.Current && conversionHappened;
      }

      /// <summary>
      ///    Converts the <paramref name="deserializedObject" /> to convert after the deserialization was made.
      /// </summary>
      /// <returns><c>true</c> if a conversion was performed or <c>false</c> otherwise</returns>
      private bool convert(object deserializedObject, int originalVersion)
      {
         return convert(deserializedObject, originalVersion, x => x.Convert);
      }

      private void updatePropertiesFor<TObject>(TObject deserializedObject, int originalVersion, bool xmlConversionHappened)
      {
         //convert object 
         var conversionHappened = convert(deserializedObject, originalVersion) || xmlConversionHappened;

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

         var buildingBlock = deserializedObject as IPKSimBuildingBlock;
         if (buildingBlock != null)
            buildingBlock.HasChanged = conversionHappened;

         if (conversionHappened && deserializedObject.IsAnImplementationOf<IObjectBase>())
            _eventPublisher.PublishEvent(new ObjectBaseConvertedEvent(deserializedObject.DowncastTo<IObjectBase>(), ProjectVersions.FindBy(originalVersion)));
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