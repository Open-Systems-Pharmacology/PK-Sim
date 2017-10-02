using System.Drawing;
using OSPSuite.Core.Domain;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Snapshots
{
   public class PopulationAnalysisField : IWithName, IWithDescription
   {
      public string Name { get; set; }
      public string Description { get; set; }
      public string Dimension { get; set; }
      public Scalings? Scaling { get; set; }
      public string Unit { get; set; }
      public string QuantityPath { get; set; }
      public QuantityType? QuantityType { get; set; }
      public Color? Color { get; set; }
      public string ParameterPath { get; set; }
      public string PKParameter { get; set; }
      public string Covariate { get; set; }
      public GroupingItem[] GroupingItems { get; set; }
      public GroupingDefinition GroupingDefinition { get; set; }
      public PivotArea? Area { get; set; }
      public int? Index { get; set; }
   }
}