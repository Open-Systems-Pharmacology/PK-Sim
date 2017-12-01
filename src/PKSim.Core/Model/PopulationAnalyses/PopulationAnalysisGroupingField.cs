using System;
using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class PopulationAnalysisGroupingField : PopulationAnalysisDerivedField, IStringValueField
   {
      public virtual GroupingDefinition GroupingDefinition { get; private set; }
      public virtual GroupingItem ReferenceGroupingItem { get; set; }

      [Obsolete("For serialization")]
      public PopulationAnalysisGroupingField() : this(null)
      {
      }

      public PopulationAnalysisGroupingField(GroupingDefinition groupingDefinition) : base(typeof (string))
      {
         GroupingDefinition = groupingDefinition;
      }

      /// <summary>
      ///    This method compares the given values using the ordering of the labels.
      /// </summary>
      /// <remarks>
      ///    The user has specified a list of string representations for grouping continous data
      ///    and therefor the specified labels represent an interval of underlying data values.
      ///    The data is ordered and therefor the values of the field should be sorted by the index of the value in
      ///    the labels array.
      /// </remarks>
      /// <param name="value1">For an interval grouping definition, a label, otherwise a value</param>
      /// <param name="value2">For an interval grouping definition, a label, otherwise a value</param>
      public override int Compare(object value1, object value2)
      {
         int compare;
         if (this.CouldCompareValuesToReference(value1, value2, out compare))
            return compare;
            
         return GroupingDefinition.Compare(value1, value2);
      }

      public override string Expression => GroupingDefinition.GetExpression();

      public override void UpdateExpression(IPopulationDataCollector populationDataCollector)
      {
         calculateLimitsFor(populationDataCollector);
      }

      private void calculateLimitsFor(IPopulationDataCollector populationDataCollector)
      {
         //limits only needs to be generated for bin grouping information
         var binsGroupingDefinition = GroupingDefinition as BinsGroupingDefinition;
         if (binsGroupingDefinition == null)
            return;

         var numericField = PopulationAnalysis.FieldByName(GroupingDefinition.FieldName).DowncastTo<PopulationAnalysisNumericField>();
         binsGroupingDefinition.CreateLimits(populationDataCollector, numericField);
      }

      public override bool IsDerivedTypeFor(Type fieldType)
      {
         var field = PopulationAnalysis.FieldByName(GroupingDefinition.FieldName);
         if (field == null)
            return false;

         var derivedField = field as PopulationAnalysisDerivedField;
         if (derivedField != null)
            return derivedField.IsDerivedTypeFor(fieldType);

         return field.IsAnImplementationOf(fieldType);
      }

      public override void RenameReferencedField(string oldFieldName, string newFieldName)
      {
         if (!string.Equals(ReferencedFieldName, oldFieldName))
            return;

         GroupingDefinition.FieldName = newFieldName;
      }

      public virtual string ReferencedFieldName => GroupingDefinition.FieldName;

      public override IReadOnlyCollection<string> ReferencedFieldNames => new[] {ReferencedFieldName};

      public override bool CanBeUsedFor(Type dataType)
      {
         return GroupingDefinition.CanBeUsedFor(dataType);
      }

      public virtual void RenameReferencedFieldTo(string fieldName)
      {
         RenameReferencedField(ReferencedFieldName, fieldName);
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var groupingField = source as PopulationAnalysisGroupingField;
         if (groupingField == null) return;
         GroupingDefinition = cloneManager.Clone(groupingField.GroupingDefinition);

         //Reference grouping item will be set on the fly when creating flat data table and does not need to be cloned
         //ReferenceGroupingItem 
      }

      public IReadOnlyList<GroupingItem> GroupingItems => this.GroupingItemsWithReference(GroupingDefinition.GroupingItems);
   }
}