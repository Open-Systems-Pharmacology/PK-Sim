using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.PopulationAnalyses
{
   public interface INumericValueField : IPopulationAnalysisField, IWithDisplayUnit
   {
      Scalings Scaling { get; set; }
   }
}