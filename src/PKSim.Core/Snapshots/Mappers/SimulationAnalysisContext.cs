using System.Collections.Generic;
using OSPSuite.Utility.Extensions;
using ModelDataRepository = OSPSuite.Core.Domain.Data.DataRepository;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationAnalysisContext
   {
      private readonly List<ModelDataRepository> _dataRepositories = new List<ModelDataRepository>();

      public bool RunSimulation { get; set; }

      public IReadOnlyList<ModelDataRepository> DataRepositories => _dataRepositories;

      public SimulationAnalysisContext()
      {
      }

      public SimulationAnalysisContext(IEnumerable<ModelDataRepository> dataRepositories)
      {
         AddDataRepositories(dataRepositories);
      }

      public void AddDataRepositories(IEnumerable<ModelDataRepository> dataRepositories)
      {
         dataRepositories?.Each(AddDataRepository);
      }

      public void AddDataRepository(ModelDataRepository dataRepository)
      {
         if(dataRepository!=null)
            _dataRepositories.Add(dataRepository);
      }
   }
}