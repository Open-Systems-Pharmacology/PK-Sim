using System.Xml.Linq;
using OSPSuite.Serializer.Xml;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Extensions
{
   public static class XElementExtensions
   {
      public static XElement CreateSimulationReferenceListElement<TSimulation>(this IXmlSerializerRepository<SerializationContext> serializerRepository, ISimulationComparison<TSimulation> simulationComparison) where TSimulation : Simulation
      {
         return serializerRepository.CreateObjectReferenceListElement(simulationComparison, x => x.AllSimulations, CoreConstants.Serialization.SimulationList, CoreConstants.Serialization.Simulation);
      }

      public static XElement CreateObservedDataReferenceListElement(this IXmlSerializerRepository<SerializationContext> serializerRepository, IWithObservedData withObservedData)
      {
         return serializerRepository.CreateObjectReferenceListElement(withObservedData, x => x.AllObservedData(), CoreConstants.Serialization.ObservedDataList, CoreConstants.Serialization.ObservedData);
      }

      public static void AddReferencedSimulations<TSimulation>(this XElement comparisonElement, ISimulationComparison<TSimulation> simulationComparison, IWithIdRepository withIdRepository, ILazyLoadTask lazyLoadTask) where TSimulation : Simulation
      {
         comparisonElement.AddReferencedObject<ISimulationComparison<TSimulation>, TSimulation>(
            simulationComparison, x => x.AddSimulation, withIdRepository, lazyLoadTask, CoreConstants.Serialization.Simulation);
      }

      public static void AddReferencedObservedData(this XElement withObservedDataElement, IWithObservedData withObservedData, IWithIdRepository withIdRepository)
      {
         withObservedDataElement.AddReferencedObject<IWithObservedData, DataRepository>(
            withObservedData, x => x.AddObservedData, withIdRepository, null, CoreConstants.Serialization.ObservedData);
      }
   }
}