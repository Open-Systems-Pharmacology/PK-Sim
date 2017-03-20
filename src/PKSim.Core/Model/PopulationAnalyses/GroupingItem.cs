using System.Drawing;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public class GroupingItem : IUpdatable
   {
      public string Label { get; set; }
      public Color Color { get; set; }
      public Symbols Symbol { get; set; }

      public void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var groupingItem = source as GroupingItem;
         if (groupingItem == null) return;
         Label = groupingItem.Label;
         Color = groupingItem.Color;
         Symbol = groupingItem.Symbol;
      }
   }
}