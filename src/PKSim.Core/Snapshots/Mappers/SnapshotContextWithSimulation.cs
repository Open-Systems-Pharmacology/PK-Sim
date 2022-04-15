namespace PKSim.Core.Snapshots.Mappers
{
   public class SnapshotContextWithSimulation : SnapshotContext
   {
      public Model.Simulation Simulation { get; }

      public SnapshotContextWithSimulation(Model.Simulation simulation, SnapshotContext baseContext) : base(baseContext)
      {
         Simulation = simulation;
      }
   }
}