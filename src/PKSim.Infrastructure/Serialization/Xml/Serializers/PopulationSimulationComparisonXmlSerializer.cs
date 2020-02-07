using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.Xml.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class PopulationSimulationComparisonXmlSerializer : ObjectBaseXmlSerializer<PopulationSimulationComparison>, IPKSimXmlSerializer
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         MapEnumerable(x => x.Analyses, x => x.AddAnalysis);
         Map(x => x.ReferenceGroupingItem);
         Map(x => x.SelectedDistributions);
      }

      protected override XElement TypedSerialize(PopulationSimulationComparison comparison, SerializationContext context)
      {
         var element = base.TypedSerialize(comparison, context);
         element.Add(SerializerRepository.CreateSimulationReferenceListElement(comparison));
         if (comparison.HasReference)
            element.AddAttribute(CoreConstants.Serialization.Attribute.ReferenceSimulation, comparison.ReferenceSimulation.Id);

         return element;
      }

      protected override void TypedDeserialize(PopulationSimulationComparison comparison, XElement comparisonElement, SerializationContext context)
      {
         base.TypedDeserialize(comparison, comparisonElement, context);

         var lazyLoadTask = context.Resolve<ILazyLoadTask>();
         var withIdRepository = context.Resolve<IWithIdRepository>();

         comparisonElement.AddReferencedSimulations(comparison, withIdRepository, lazyLoadTask);
         var referenceSimulationId = comparisonElement.GetAttribute(CoreConstants.Serialization.Attribute.ReferenceSimulation);
         if (!string.IsNullOrEmpty(referenceSimulationId))
            comparison.ReferenceSimulation = withIdRepository.Get<PopulationSimulation>(referenceSimulationId);
      }
   }
}