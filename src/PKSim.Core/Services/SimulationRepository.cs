using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Repositories;

namespace PKSim.Core.Services
{
   public class SimulationRepository : ISimulationRepository
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public SimulationRepository(IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public IEnumerable<ISimulation> All()
      {
         return _buildingBlockRepository.All<Simulation>();
      }
   }
}