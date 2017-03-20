using System.Xml.Linq;
using OSPSuite.Serializer.Xml.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Serialization.Xml.Extensions;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PopulationAnalysisFieldXmlSerializer<TField> : BaseXmlSerializer<TField> where TField : PopulationAnalysisFieldBase
   {
      public override void PerformMapping()
      {
         Map(x => x.Name);
         Map(x => x.Description);
      }
   }

   public abstract class PopulationAnalysisDataFieldXmlSerializer<TField> : PopulationAnalysisFieldXmlSerializer<TField> where TField : PopulationAnalysisDataField
   {
   }

   public abstract class PopulationAnalysisDataNumericValueFieldXmlSerializer<TField> : PopulationAnalysisDataFieldXmlSerializer<TField> where TField : PopulationAnalysisDataField, INumericValueField
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Dimension);
         Map(x => x.Scaling);
      }

      protected override XElement TypedSerialize(TField field, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(field, serializationContext);
         return element.AddDisplayUnitFor(field);
      }

      protected override void TypedDeserialize(TField field, XElement element, SerializationContext serializationContext)
      {
         base.TypedDeserialize(field, element, serializationContext);
         //We need to update the Dimension to use the merged dimension
         var dimensionRepository = IoC.Resolve<IDimensionRepository>();
         var mergedDimension = dimensionRepository.MergedDimensionFor(field);
         field.DisplayUnit = element.GetDisplayUnit(mergedDimension);
      }
   }

   public class PopulationAnalysisOutputFieldXmlSerializer : PopulationAnalysisDataNumericValueFieldXmlSerializer<PopulationAnalysisOutputField>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.QuantityPath);
         Map(x => x.QuantityType);
         Map(x => x.Color);
      }
   }

   public class PopulationAnalysisParameterFieldXmlSerializer : PopulationAnalysisDataNumericValueFieldXmlSerializer<PopulationAnalysisParameterField>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.ParameterPath);
      }
   }

   public class PopulationAnalysisCovariateFieldXmlSerializer : PopulationAnalysisFieldXmlSerializer<PopulationAnalysisCovariateField>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.Covariate);
         MapEnumerable(x => x.GroupingItems, x => x.AddGroupingItem);
      }
   }

   public class PopulationAnalysisPKParameterFieldXmlSerializer : PopulationAnalysisDataNumericValueFieldXmlSerializer<PopulationAnalysisPKParameterField>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.PKParameter);
         Map(x => x.QuantityPath);
         Map(x => x.QuantityType);
      }
   }

   public abstract class PopulationAnalysisDerivedFieldXmlSerializer<TField> : PopulationAnalysisFieldXmlSerializer<TField> where TField : PopulationAnalysisDerivedField
   {
   }

   public class PopulationAnalysisGroupingFieldXmlSerializer : PopulationAnalysisDerivedFieldXmlSerializer<PopulationAnalysisGroupingField>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.GroupingDefinition);
      }
   }

   public class PopulationAnalysisExpressionFieldXmlSerializer : PopulationAnalysisDerivedFieldXmlSerializer<PopulationAnalysisExpressionField>
   {
      protected override XElement TypedSerialize(PopulationAnalysisExpressionField derivedField, SerializationContext serializationContext)
      {
         var element = base.TypedSerialize(derivedField, serializationContext);
         element.AddAttribute(CoreConstants.Serialization.Attribute.Expression, derivedField.Expression);
         return element;
      }

      protected override void TypedDeserialize(PopulationAnalysisExpressionField derivedField, XElement element, SerializationContext serializationContext)
      {
         base.TypedDeserialize(derivedField, element, serializationContext);
         derivedField.SetExpression(element.GetAttribute(CoreConstants.Serialization.Attribute.Expression));
      }
   }
}