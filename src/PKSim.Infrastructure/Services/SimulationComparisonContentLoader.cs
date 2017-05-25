using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;

namespace PKSim.Infrastructure.Services
{
   public class SimulationComparisonContentLoader : ISimulationComparisonContentLoader
   {
      private readonly ISimulationComparisonMetaDataContentQuery _simulationComparisonMetaDataContentQuery;
      private readonly ICompressedSerializationManager _compressedSerializationManager;

      public SimulationComparisonContentLoader(ISimulationComparisonMetaDataContentQuery simulationComparisonMetaDataContentQuery, ICompressedSerializationManager compressedSerializationManager)
      {
         _simulationComparisonMetaDataContentQuery = simulationComparisonMetaDataContentQuery;
         _compressedSerializationManager = compressedSerializationManager;
      }

      public void LoadContentFor(ISimulationComparison simulationComparison)
      {
         var content = _simulationComparisonMetaDataContentQuery.ResultFor(simulationComparison.Id);
         if (content == null) return;

         _compressedSerializationManager.Deserialize(simulationComparison, content.Data);

         updateComparisonUsedObservedData(simulationComparison);
      }

      private static void updateComparisonUsedObservedData(ISimulationComparison simulationComparison)
      {
         var comparison = simulationComparison as IndividualSimulationComparison;
         if (comparison == null)
            return;

         var repositories = comparison.Curves.Select(x => x.xData.Repository).Union(comparison.Curves.Select(x => x.yData.Repository)).Distinct();

         repositories.Each(repository => comparison.AddObservedData(repository));
      }
   }
}