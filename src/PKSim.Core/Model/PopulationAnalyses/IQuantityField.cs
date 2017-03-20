using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface IQuantityField : INumericValueField
   {
      string QuantityPath { get; set; }
      QuantityType QuantityType { get; set; }
   }
}