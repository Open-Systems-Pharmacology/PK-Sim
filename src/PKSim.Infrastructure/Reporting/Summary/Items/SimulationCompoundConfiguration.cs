using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.Summary.Items
{
   public class SimulationCompoundConfiguration
   {
      public string CompoundName { get; }
      public CompoundProperties CompoundProperties { get; }
      public OverwriteParameterSet OverwriteParameterSet { get; }

      public SimulationCompoundConfiguration(string compoundName, CompoundProperties compoundProperties, OverwriteParameterSet overwriteParameterSet)
      {
         CompoundName = compoundName;
         CompoundProperties = compoundProperties;
         OverwriteParameterSet = overwriteParameterSet;
      }

      public void Deconstruct(out string compoundName, out CompoundProperties compoundProperties, out OverwriteParameterSet overwriteParameterSet)
      {
         compoundName = CompoundName;
         compoundProperties = CompoundProperties;
         overwriteParameterSet = OverwriteParameterSet;
      }
   }
}