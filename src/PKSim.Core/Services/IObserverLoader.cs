using System;
using System.Xml.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Converter;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;
using OSPSuite.Serializer.Xml;
using OSPSuite.Utility.Exceptions;

namespace PKSim.Core.Services
{
   public interface IObserverLoader
   {
      IObserverBuilder Load(string pkmlFileFullPath, IDimensionFactory dimensionFactory, IObjectBaseFactory objectBaseFactory, IWithIdRepository withIdRepository, ICloneManagerForModel cloneManagerForModel);
   }

   public class ObserverLoader : IObserverLoader
   {
      private readonly IObjectConverterFinder _objectConverterFinder;
      private readonly IXmlSerializer<SerializationContext> _containerObserverSerializer;
      private readonly IXmlSerializer<SerializationContext> _amountObserverSerializer;
      private readonly IOSPSuiteXmlSerializerRepository _modellingXmlSerializerRepository;

      public ObserverLoader(IOSPSuiteXmlSerializerRepository modellingXmlSerializerRepository, IObjectConverterFinder objectConverterFinder)
      {
         _objectConverterFinder = objectConverterFinder;
         _modellingXmlSerializerRepository = modellingXmlSerializerRepository;
         _containerObserverSerializer = modellingXmlSerializerRepository.SerializerFor<ContainerObserverBuilder>();
         _amountObserverSerializer = modellingXmlSerializerRepository.SerializerFor<AmountObserverBuilder>();
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

      public IObserverBuilder Load(string pkmlFileFullPath, IDimensionFactory dimensionFactory, IObjectBaseFactory objectBaseFactory, IWithIdRepository withIdRepository, ICloneManagerForModel cloneManagerForModel)
      {
         IObserverBuilder observerBuilder;
         int version;
         using (var serializationContext = SerializationTransaction.Create(dimensionFactory, objectBaseFactory, withIdRepository, cloneManagerForModel))
         {
            var element = XElement.Load(pkmlFileFullPath);
            version = element.GetPKMLVersion();

            convertXml(element, version);

            var serializer = findObserverSerializerFor(element);
            if (serializer == null)
               throw new OSPSuiteException(Error.CouldNotLoadSimulationFromFile(pkmlFileFullPath));

            _modellingXmlSerializerRepository.DeserializeFormulaCacheIn(element, serializationContext);
            observerBuilder = serializer.Deserialize<IObserverBuilder>(element, serializationContext);
         }

         convert(observerBuilder, version, x => x.Convert);

         return observerBuilder;
      }

      private IXmlSerializer<SerializationContext> findObserverSerializerFor(XElement element)
      {
         if (string.Equals(_amountObserverSerializer.ElementName, element.Name.LocalName))
            return _amountObserverSerializer;

         if (string.Equals(_containerObserverSerializer.ElementName, element.Name.LocalName))
            return _containerObserverSerializer;


         return null;
      }
   }
}