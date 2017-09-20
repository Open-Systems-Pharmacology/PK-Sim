using System.Threading.Tasks;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class SimulationPropertiesMapper : SnapshotMapperBase<SimulationProperties, SimulationConfiguration>
   {
      private readonly IModelPropertiesTask _modelPropertiesTask;

      public SimulationPropertiesMapper(IModelPropertiesTask modelPropertiesTask)
      {
         _modelPropertiesTask = modelPropertiesTask;
      }

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

      public virtual ModelProperties ModelPropertiesFrom(SimulationConfiguration snapshot, ISimulationSubject simulationSubject)
      {
         return _modelPropertiesTask.DefaultFor(simulationSubject.OriginData, snapshot.Model);
      }
   }
}