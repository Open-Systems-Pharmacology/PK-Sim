using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class IntervalGroupingDefinition : GroupingDefinition, IWithDisplayUnit
   {
      /// <summary>
      ///    Dimension of the referenced field
      /// </summary>
      public IDimension Dimension { get; set; }

      public virtual List<GroupingItem> Items { get; }

      /// <summary>
      ///    Limits used to calculate the intervals. The limit should be ordered
      /// </summary>
      public virtual List<double> Limits { get; protected set; }

      /// <summary>
      ///    Display unit of the referenced field
      /// </summary>
      public Unit DisplayUnit { get; set; }

      protected IntervalGroupingDefinition(string field)
      {
         FieldName = field;
         Items = new List<GroupingItem>();
      }

      public virtual void AddItem(GroupingItem groupingItem)
      {
         Items.Add(groupingItem);
      }

      public virtual void AddItems(IEnumerable<GroupingItem> groupingItems)
      {
         groupingItems.Each(AddItem);
      }

      /// <summary>
      ///    This method generates an expression labelling intervals of values with lables.
      /// </summary>
      /// <remarks>
      ///    Intervals are [-Inf;L1[,[L1;L2[,[L2;L3[,..,[Ln-1;Ln[,[Ln;Inf].
      /// </remarks>
      public override string GetExpression()
      {
         if (Limits == null || Items == null)
            return string.Empty;

         var expression = new StringBuilder();

         var i = 0;
         var labels = Labels.ToList();
         foreach (var limit in Limits)
            expression.AppendFormat("iif([{0}] < {1}, '{2}', ", FieldName, limit.ConvertedTo<string>(), labels[i++]);

         expression.AppendFormat("'{0}'", labels[i]);
         for (var j = 0; j < Limits.Count; j++)
            expression.Append(")");

         return expression.ToString();
      }

      public override bool CanBeUsedFor(Type dataType) => dataType.IsNumeric();

      public override int Compare(object value1, object value2)
      {
         var labels = Labels.ToList();
         int labelIndex1 = labels.IndexOf(value1.DowncastTo<string>());
         int labelIndex2 = labels.IndexOf(value2.DowncastTo<string>());
         return Comparer.Default.Compare(labelIndex1, labelIndex2);
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var intervalGrouping = source as IntervalGroupingDefinition;
         if (intervalGrouping == null)
            return;

         Dimension = intervalGrouping.Dimension;
         DisplayUnit = intervalGrouping.DisplayUnit;
         AddItems(intervalGrouping.Items.Select(cloneManager.Clone));
      }

      public override IReadOnlyList<GroupingItem> GroupingItems => Items;
   }
}