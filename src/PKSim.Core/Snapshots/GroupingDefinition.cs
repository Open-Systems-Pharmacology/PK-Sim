using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Snapshots
{
   public class GroupingDefinition
   {
      [Required]
      public string FieldName { get; set; }
      public Dictionary<string, GroupingItem> Mapping { get; set; }
      public string Dimension { get; set; }
      public string Unit { get; set; }
      public GroupingItem[] Items { get; set; }
      public double[] Limits { get; set; }
      public int? NumberOfBins { get; set; }
      public Color? StartColor { get; set; }
      public Color? EndColor { get; set; }
      public string NamingPattern { get; set; }
      public LabelGenerationStrategyId? Strategy { get; set; }
   }
}