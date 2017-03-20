using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Reflection;
using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public abstract class BuildingBlockInSimulationUICommandBase<TBuildingBlock> : IUICommand where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected readonly ISimulationTask _simulationTask;
      private WeakRef<TBuildingBlock> _templateBuildingBlockReference;
      private WeakRef<Simulation> _simulationReference;
      private WeakRef<UsedBuildingBlock> _usedBuildingBlockReference;

      protected BuildingBlockInSimulationUICommandBase(ISimulationTask simulationTask)
      {
         _simulationTask = simulationTask;
      }

      public UsedBuildingBlock UsedBuildingBlock
      {
         get { return _usedBuildingBlockReference.Target; }
         set { _usedBuildingBlockReference = new WeakRef<UsedBuildingBlock>(value); }
      }

      public Simulation Simulation
      {
         get { return _simulationReference.Target; }
         set { _simulationReference = new WeakRef<Simulation>(value); }
      }

      public TBuildingBlock TemplateBuildingBlock
      {
         get { return _templateBuildingBlockReference.Target; }
         set { _templateBuildingBlockReference = new WeakRef<TBuildingBlock>(value); }
      }

      public abstract void Execute();
   }

   public class UpdateBuildingBlockInSimulationUICommand<TBuildingBlock> : BuildingBlockInSimulationUICommandBase<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      public UpdateBuildingBlockInSimulationUICommand(ISimulationTask simulationTask)
         : base(simulationTask)
      {
      }

      public override void Execute()
      {
         _simulationTask.UpdateUsedBuildingBlockInSimulation(TemplateBuildingBlock, UsedBuildingBlock, Simulation);
      }
   }

   public class CommitBuildingBlockFromSimulationUICommand<TBuildingBlock> : BuildingBlockInSimulationUICommandBase<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      public CommitBuildingBlockFromSimulationUICommand(ISimulationTask simulationTask) : base(simulationTask)
      {
      }

      public override void Execute()
      {
         _simulationTask.CommitBuildingBlockToRepository(TemplateBuildingBlock, UsedBuildingBlock, Simulation);
      }
   }

   public class BuildingBlockDiffUICommand<TBuildingBlock> : BuildingBlockInSimulationUICommandBase<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      public BuildingBlockDiffUICommand(ISimulationTask simulationTask) : base(simulationTask)
      {
      }

      public override void Execute()
      {
         _simulationTask.ShowDifferencesBetween(TemplateBuildingBlock, UsedBuildingBlock, Simulation);
      }
   }
}