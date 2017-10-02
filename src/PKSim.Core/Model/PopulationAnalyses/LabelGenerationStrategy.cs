using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public enum LabelGenerationStrategyId
   {
      Numeric,
      Alpha,
      Roman
   }

   public static class LabelGenerationStrategies
   {
      private static readonly ICache<LabelGenerationStrategyId, LabelGenerationStrategy> _allLabelGenerationStrategies = new Cache<LabelGenerationStrategyId, LabelGenerationStrategy>(x => x.Id);

      public static LabelGenerationStrategy Numeric = createStrategy(LabelGenerationStrategyId.Numeric, PKSimConstants.UI.LabelGenerationNumeric);
      public static LabelGenerationStrategy Alpha = createStrategy(LabelGenerationStrategyId.Alpha, PKSimConstants.UI.LabelGenerationAlpha);
      public static LabelGenerationStrategy Roman = createStrategy(LabelGenerationStrategyId.Roman, PKSimConstants.UI.LabelGenerationRoman);

      private static LabelGenerationStrategy createStrategy(LabelGenerationStrategyId labelGenerationStrategyId, string displayName)
      {
         var strategy = new LabelGenerationStrategy(labelGenerationStrategyId, displayName);
         _allLabelGenerationStrategies.Add(strategy);
         return strategy;
      }

      public static LabelGenerationStrategy ById(LabelGenerationStrategyId labelGenerationStrategyId)
      {
         return _allLabelGenerationStrategies[labelGenerationStrategyId];
      }

      public static IEnumerable<LabelGenerationStrategy> All()
      {
         return _allLabelGenerationStrategies; 
      }
   }

   public class LabelGenerationStrategy
   {
      public LabelGenerationStrategyId Id { get; }
      public string DisplayName { get; }

      public LabelGenerationStrategy(LabelGenerationStrategyId id, string displayName)
      {
         Id = id;
         DisplayName = displayName;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}