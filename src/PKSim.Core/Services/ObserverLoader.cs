using System;
using System.Xml.Linq;
using OSPSuite.Core.Converters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Core.Services
{
   public interface IObserverLoader
   {
      ObserverBuilder Load(string pkmlFileFullPath);
   }

   public class ObserverLoader : IObserverLoader
   {
      private readonly IObjectConverterFinder _objectConverterFinder;
      private readonly IDimensionFactory _dimensionFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ICloneManagerForModel _cloneManagerForModel;
      private readonly IXmlSerializer<SerializationContext> _containerObserverSerializer;
      private readonly IXmlSerializer<SerializationContext> _amountObserverSerializer;
      private readonly IOSPSuiteXmlSerializerRepository _modelingXmlSerializerRepository;
      private readonly IContainer _container;

      public ObserverLoader(
         IOSPSuiteXmlSerializerRepository modelingXmlSerializerRepository,
         IObjectConverterFinder objectConverterFinder,
         IDimensionFactory dimensionFactory,
         IObjectBaseFactory objectBaseFactory,
         ICloneManagerForModel cloneManagerForModel,
         IContainer container
      )
      {
         _objectConverterFinder = objectConverterFinder;
         _dimensionFactory = dimensionFactory;
         _objectBaseFactory = objectBaseFactory;
         _cloneManagerForModel = cloneManagerForModel;
         _modelingXmlSerializerRepository = modelingXmlSerializerRepository;
         _containerObserverSerializer = modelingXmlSerializerRepository.SerializerFor<ContainerObserverBuilder>();
         _amountObserverSerializer = modelingXmlSerializerRepository.SerializerFor<AmountObserverBuilder>();
         _container = container;
      }

      private void convertXml(XElement sourceElement, int version)
      {
         if (sourceElement == null) return;
         //set version to avoid double conversion in the case of multiple load
         convert(sourceElement, version, x => x.ConvertXml);
         sourceElement.SetAttributeValue(Constants.Serialization.Attribute.VERSION, Constants.PKML_VERSION);
      }

      private void convert<T>(T objectToConvert, int objectVersion, Func<IObjectConverter, Func<T, (int, bool)>> converterAction)
      {
         int version = objectVersion;
         if (version <= PKMLVersion.NON_CONVERTABLE_VERSION)
            throw new OSPSuiteException(Constants.TOO_OLD_PKML);

         while (version != Constants.PKML_VERSION)
         {
            var converter = _objectConverterFinder.FindConverterFor(version);
            var (convertedVersion, _) = converterAction(converter).Invoke(objectToConvert);
            version = convertedVersion;
         }
      }

      public ObserverBuilder Load(string pkmlFileFullPath)
      {
         ObserverBuilder observerBuilder;
         int version;
         using (var serializationContext = SerializationTransaction.Create(_container, _dimensionFactory, _objectBaseFactory, new WithIdRepository(), _cloneManagerForModel))
         {
            var element = XElementSerializer.PermissiveLoad(pkmlFileFullPath);
            version = element.GetPKMLVersion();
            var elementName = element.Name.LocalName;
            convertXml(element, version);

            var serializer = findObserverSerializerFor(elementName);
            if (serializer == null)
               throw new OSPSuiteException(PKSimConstants.Error.CouldNotLoadObserverFromFile(pkmlFileFullPath, elementName));

            _modelingXmlSerializerRepository.DeserializeFormulaCacheIn(element, serializationContext);
            observerBuilder = serializer.Deserialize<ObserverBuilder>(element, serializationContext);
         }

         convert(observerBuilder, version, x => x.Convert);

         return observerBuilder;
      }

      private IXmlSerializer<SerializationContext> findObserverSerializerFor(string elementName)
      {
         if (string.Equals(_amountObserverSerializer.ElementName, elementName))
            return _amountObserverSerializer;

         if (string.Equals(_containerObserverSerializer.ElementName, elementName))
            return _containerObserverSerializer;

         return null;
      }
   }
}