using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Services
{
   public class SimulationAnalysesLoader : ISimulationAnalysesLoader
   {
      private readonly ISimulationAnalysesQuery _simulationAnalysesQuery;
      private readonly ICompressedSerializationManager _serializationManager;

      public SimulationAnalysesLoader(ISimulationAnalysesQuery simulationAnalysesQuery, ICompressedSerializationManager serializationManager)
      {
         _simulationAnalysesQuery = simulationAnalysesQuery;
         _serializationManager = serializationManager;
      }

      public void LoadAnalysesFor(PopulationSimulation populationSimulation)
      {
         var simulationAnalysesMetaData = _simulationAnalysesQuery.ResultFor(populationSimulation.Id);
         PopulationSimulationPKAnalyses pkAnalyses = new NullPopulationSimulationPKAnalyses();
         if (simulationAnalysesMetaData != null)
            pkAnalyses = _serializationManager.Deserialize<PopulationSimulationPKAnalyses>(simulationAnalysesMetaData.Content.Data);

         populationSimulation.PKAnalyses = pkAnalyses;
      }
   }
}