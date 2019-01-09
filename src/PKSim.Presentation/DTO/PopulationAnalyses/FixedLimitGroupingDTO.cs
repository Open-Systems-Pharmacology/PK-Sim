using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Validation;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class FixedLimitGroupingDTO : GroupingItemDTO
   {
      private double? _minimum;
      private double? _maximum;
      public bool MaximumEditable { get; set; }
      public bool CanDelete { get; set; }
      public bool CanAdd { get; set; }

      public FixedLimitGroupingDTO()
      {
         Rules.AddRange(AllRules.All);
      }

      public double? Minimum
      {
         get => _minimum;
         set => SetProperty(ref _minimum, value);
      }

      public double? Maximum
      {
         get => _maximum;
         set => SetProperty(ref _maximum, value);
      }

      private static class AllRules
      {
         private static IBusinessRule maximumDefined { get; } = CreateRule.For<FixedLimitGroupingDTO>()
            .Property(item => item.Maximum)
            .WithRule((param, value) => !param.MaximumEditable || value.HasValue)
            .WithError(PKSimConstants.Rules.Parameter.MaxShouldBeDefined);

         public static IEnumerable<IBusinessRule> All
         {
            get { yield return maximumDefined; }
         }
      }
   }
}