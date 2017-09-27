using System.Collections.Generic;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationAnalysisContext
   {
      private readonly List<ModelDataRepository> _dataRepositories = new List<ModelDataRepository>();

      public IReadOnlyList<ModelDataRepository> DataRepositories => _dataRepositories;

      public SimulationAnalysisContext()
      {
      }

      public SimulationAnalysisContext(IEnumerable<ModelDataRepository> dataRepositories)
      {
         _dataRepositories.AddRange(dataRepositories);
      }

      public void AddDataRepository(ModelDataRepository dataRepository)
      {
         _dataRepositories.Add(dataRepository);
      }
   }
}