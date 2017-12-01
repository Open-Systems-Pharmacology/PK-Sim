using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
    public class ValueMappingGroupingDefinition : GroupingDefinition
   {
      public Cache<string, GroupingItem> Mapping { get; }

      [Obsolete("For serialization")]
      public ValueMappingGroupingDefinition() : this(string.Empty)
      {
      }

      public ValueMappingGroupingDefinition(string fieldName)
      {
         FieldName = fieldName;
         Mapping = new Cache<string, GroupingItem>();
      }

      public void AddValueLabel(string value, GroupingItem groupingItem)
      {
         Mapping.Add(value, groupingItem);
      }

   
      /// <summary>
      ///    This method creates an expression labelling mapped values with mapped labels.
      /// </summary>
      public override string GetExpression()
      {
         var expression = new StringBuilder();

         var values = Mapping.Keys.ToArray();
         var labels = Labels;

         for (var i = 0; i < Mapping.Count(); i++)
            expression.AppendFormat("iif([{0}] = '{1}', '{2}', ", FieldName, values[i], labels[i]);

         expression.Append("'Unknown'");
         for (var i = 0; i < Mapping.Count(); i++)
            expression.Append(")");

         return expression.ToString();
      }

      public override bool CanBeUsedFor(Type dataType)
      {
         return dataType == typeof (string);
      }

      public override IReadOnlyList<GroupingItem> GroupingItems => Mapping.ToList();

      public override int Compare(object value1, object value2)
      {
         var labelList = Labels.ToList();
         int labelIndex1 = labelList.IndexOf(value1.DowncastTo<string>());
         int labelIndex2 = labelList.IndexOf(value2.DowncastTo<string>());
         return Comparer.Default.Compare(labelIndex1, labelIndex2);
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var valueMapping = source as ValueMappingGroupingDefinition;
         valueMapping?.Mapping.KeyValues.Each(kv => Mapping.Add(kv.Key, cloneManager.Clone(kv.Value)));
      }
   }
}