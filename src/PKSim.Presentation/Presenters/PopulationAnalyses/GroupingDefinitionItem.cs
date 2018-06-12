using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public static class GroupingDefinitions
   {
      private static readonly ICache<GroupingDefinitionItem, string> _allGroupings = new Cache<GroupingDefinitionItem, string>();

      public static GroupingDefinitionItem FixedLimits = addGrouping<FixedLimitsGroupingDefinition>(PKSimConstants.UI.FixedLimitsGroupingDefinition);
      public static GroupingDefinitionItem NumberOfBins = addGrouping<NumberOfBinsGroupingDefinition>(PKSimConstants.UI.NumberOfBinsGroupingDefinition);
      public static GroupingDefinitionItem ValueMapping = addGrouping<ValueMappingGroupingDefinition>(PKSimConstants.UI.ValueMappingGroupingDefinition);

      private static GroupingDefinitionItem addGrouping<T>(string displayName) where T : GroupingDefinition
      {
         var groupingDefintion = new GroupingDefinitionItem(typeof (T), displayName);
         _allGroupings.Add(groupingDefintion, groupingDefintion.DisplayName);
         return groupingDefintion;
      }

      public static IEnumerable<GroupingDefinitionItem> All<T>() where T : GroupingDefinition
      {
         return _allGroupings.Keys.Where(x => x.GroupingDefinitionType.IsAnImplementationOf<T>());
      }
    
      public static GroupingDefinitionItem For(GroupingDefinition groupingDefinition)
      {
         return _allGroupings.Keys.First(x => x.GroupingDefinitionType == groupingDefinition.GetType());
      }
   }

   public class GroupingDefinitionItem
   {
      public string DisplayName { get; private set; }
      public Type GroupingDefinitionType { get; private set; }

      public GroupingDefinitionItem(Type groupingDefinition, string displayName)
      {
         GroupingDefinitionType = groupingDefinition;
         DisplayName = displayName;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}