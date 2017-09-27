using System.Xml.Linq;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class GroupingDefinitionXmlSerializer<TGrouping> : BaseXmlSerializer<TGrouping> where TGrouping : GroupingDefinition
   {
      public override void PerformMapping()
      {
         Map(x => x.FieldName);
      }
   }

   public class ValueMappingGroupingDefinitionXmlSerializer : GroupingDefinitionXmlSerializer<ValueMappingGroupingDefinition>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Mapping);
      }
   }

   public abstract class IntervalGroupingDefinitionXmlSerializer<TGrouping> : GroupingDefinitionXmlSerializer<TGrouping> where TGrouping : IntervalGroupingDefinition
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Dimension);
         MapEnumerable(x => x.Items, x => x.AddItem);
      }

      protected override XElement TypedSerialize(TGrouping grouping, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(grouping,serializationContext);
         return element.AddDisplayUnitFor(grouping);
      }

      protected override void TypedDeserialize(TGrouping grouping, XElement element, SerializationContext serializationContext)
      {
         base.TypedDeserialize(grouping, element, serializationContext);
         element.UpdateDisplayUnit(grouping);
      }
   }

   public class FixedLimitsGroupingDefinitionXmlSerializer : IntervalGroupingDefinitionXmlSerializer<FixedLimitsGroupingDefinition>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         //only save limits for FixedLimits grouping
         Map(x => x.Limits);
      }
   }

   public class NumberOfBinsGroupingDefinitionXmlSerializer : IntervalGroupingDefinitionXmlSerializer<NumberOfBinsGroupingDefinition>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.NumberOfBins);
         Map(x => x.StartColor);
         Map(x => x.EndColor);
         Map(x => x.NamingPattern);
         Map(x => x.Strategy);
      }
   }
}