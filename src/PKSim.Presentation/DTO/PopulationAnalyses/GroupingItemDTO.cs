using System.Collections.Generic;
using System.Drawing;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Presentation.DTO;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class GroupingItemDTO : DxValidatableDTO
   {
      public GroupingItemDTO()
      {
         Color = Color.Empty;
         Symbol = Symbols.Circle;
         Rules.AddRange(AllRules.All);
      }

      private string _label;

      public string Label
      {
         get { return _label; }
         set
         {
            _label = value;
            OnPropertyChanged(() => Label);
         }
      }

      private Color _color;

      public virtual Color Color
      {
         get { return _color; }
         set
         {
            _color = value;
            OnPropertyChanged(() => Color);
         }
      }

      private Symbols _symbol;

      public virtual Symbols Symbol
      {
         get { return _symbol; }
         set
         {
            _symbol = value;
            OnPropertyChanged(() => Symbol);
         }
      }

      public GroupingItem ToGroupingItem()
      {
         return new GroupingItem
         {
            Color = Color,
            Label = Label,
            Symbol = Symbol
         };
      }

      public GroupingItemDTO From(GroupingItem groupingItem)
      {
         var dto = new GroupingItemDTO();
         dto.UpdateFrom(groupingItem);
         return dto;
      }

      public virtual void UpdateFrom(GroupingItem groupingItem)
      {
         Color = groupingItem.Color;
         Label = groupingItem.Label;
         Symbol = groupingItem.Symbol;
      }

      private static class AllRules
      {
         private static IBusinessRule labelDefined
         {
            get { return GenericRules.NonEmptyRule<GroupingItemDTO>(x => x.Label); }
         }

         public static IEnumerable<IBusinessRule> All
         {
            get { yield return labelDefined; }
         }
      }
   }
}