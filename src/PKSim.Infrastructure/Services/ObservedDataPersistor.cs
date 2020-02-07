using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Infrastructure.Serialization.Xml.Serializers;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.Services
{
   public interface IObservedDataPersistor : IFilePersistor<DataRepository>
   {
      DataRepository Load(string fileName);
   }

   public class ObservedDataPersistor : AbstractFilePersistor<DataRepository>, IObservedDataPersistor
   {
      private readonly IDimensionFactory _dimensionFactory;

      public ObservedDataPersistor(IPKSimXmlSerializerRepository serializerRepository, IDimensionFactory dimensionFactory, IContainer container) : base(serializerRepository, container)
      {
         _dimensionFactory = dimensionFactory;
      }

      public override void Load(DataRepository dataRepository, string fileName)
      {
         using (var context = SerializationTransaction.Create(_container, _dimensionFactory, withIdRepository: new WithIdRepository()))
         {
            var xmlSerializer = _serializerRepository.SerializerFor(dataRepository);
            var outputToDeserialize = XElement.Load(fileName);
            xmlSerializer.Deserialize(dataRepository, outputToDeserialize, context);
         }
      }

      public DataRepository Load(string fileName)
      {
         var observedData = new DataRepository();
         Load(observedData, fileName);
         return observedData;
      }
   }
}