using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.Summary.Items
{
   public class SimulationCompoundConfiguration
   {
      public string CompoundName { get; }
      public CompoundProperties CompoundProperties { get; }

      public SimulationCompoundConfiguration(string compoundName, CompoundProperties compoundProperties)
      {
         CompoundName = compoundName;
         CompoundProperties = compoundProperties;
      }

      public void Deconstruct(out string compoundName, out CompoundProperties compoundProperties)
      {
         compoundName = CompoundName;
         compoundProperties = CompoundProperties;
      }
   }
}