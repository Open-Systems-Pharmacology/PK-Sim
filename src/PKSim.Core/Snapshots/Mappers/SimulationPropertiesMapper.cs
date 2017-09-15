using System.Threading.Tasks;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationPropertiesMapper : SnapshotMapperBase<SimulationProperties, SimulationConfiguration>
   {
      public override Task<SimulationConfiguration> MapToSnapshot(SimulationProperties simulationProperties)
      {
         return SnapshotFrom(simulationProperties, x =>
         {
            x.Model = simulationProperties.ModelProperties.ModelConfiguration.ModelName;
            x.AllowAging = simulationProperties.AllowAging;
         });
      }

      public override Task<SimulationProperties> MapToModel(SimulationConfiguration snapshot)
      {
         throw new System.NotImplementedException();
      }
   }
}