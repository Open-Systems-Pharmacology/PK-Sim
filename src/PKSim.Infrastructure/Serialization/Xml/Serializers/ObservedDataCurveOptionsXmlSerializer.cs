using System.Xml.Linq;
using OSPSuite.Utility.Container;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Infrastructure.Serialization.Xml.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ObservedDataCurveOptionsXmlSerializer : BaseXmlSerializer<ObservedDataCurveOptions>
   {
      public override void PerformMapping()
      {
         Map(x => x.ColumnId);
         Map(x => x.Caption);
         Map(x => x.CurveOptions);
      }
   }

   public class ObservedDataCollectionXmlSerializer : BaseXmlSerializer<ObservedDataCollection>
   {
      public override void PerformMapping()
      {
         Map(x => x.ApplyGroupingToObservedData);
         MapEnumerable(x => x.ObservedDataCurveOptions(), x => x.AddCurveOptions);
      }

      protected override XElement TypedSerialize(ObservedDataCollection observedDataCollection, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(observedDataCollection, serializationContext);
         element.Add(SerializerRepository.CreateObservedDataReferenceListElement(observedDataCollection));
         return element;
      }

      protected override void TypedDeserialize(ObservedDataCollection observedDataCollection, XElement element, SerializationContext serializationContext)
      {
         base.TypedDeserialize(observedDataCollection, element, serializationContext);
         var withIdRepository = IoC.Resolve<IWithIdRepository>();
         element.AddReferencedObservedData(observedDataCollection, withIdRepository);
      }
   }
}