using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public abstract class GroupingDefinition : IComparer<object>, IUpdatable
   {
      /// <summary>
      ///    The name of the underlying field being referenced by the grouping definition
      /// </summary>
      public string FieldName { get; set; }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var groupingDefinition = source as GroupingDefinition;
         if (groupingDefinition == null) return;
         FieldName = groupingDefinition.FieldName;
      }

      public abstract string GetExpression();

      /// <summary>
      ///    Returns true if the grouping definition can be defined for a field of type <paramref name="dataType" /> otherwise
      ///    false
      /// </summary>
      public abstract bool CanBeUsedFor(Type dataType);

      public abstract int Compare(object x, object y);

      public abstract IReadOnlyList<GroupingItem> GroupingItems { get; }

      public virtual IReadOnlyList<string> Labels => GroupingItems.Select(x => x.Label).ToList();
   }
}