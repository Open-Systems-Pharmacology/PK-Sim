using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public static class PopulationPKAnalysesExtensions
   {
      public static bool IsNull(this PopulationSimulationPKAnalyses populationPKAnalyses)
      {
         return populationPKAnalyses == null || populationPKAnalyses.IsAnImplementationOf<NullPopulationSimulationPKAnalyses>();
      }
   }
}