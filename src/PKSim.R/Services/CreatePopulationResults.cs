using OSPSuite.Core.Domain.Populations;

namespace PKSim.R.Services
{
   public class CreatePopulationResults
   {
      public IndividualValuesCache IndividualValuesCache { get; }
      public int Seed { get; }

      public CreatePopulationResults(IndividualValuesCache individualValuesCache, int seed)
      {
         IndividualValuesCache = individualValuesCache;
         Seed = seed;
      }
   }
}