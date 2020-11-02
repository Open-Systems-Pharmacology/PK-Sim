namespace PKSim.Core.Commands
{
   public interface IBuildingBlockStructureChangeCommand : IBuildingBlockChangeCommand
   {
   }

   public abstract class BuildingBlockStructureChangeCommand : BuildingBlockChangeCommand, IBuildingBlockStructureChangeCommand
   {
   }

   public abstract class BuildingBlockIrreversibleStructureChangeCommand : PKSimCommand, IBuildingBlockStructureChangeCommand
   {
      public bool IncrementVersion { get; set; }
      public string BuildingBlockId { get; protected set; }
      public bool ShouldChangeVersion { get; set; }

      protected BuildingBlockIrreversibleStructureChangeCommand()
      {
         IncrementVersion = true;
         ShouldChangeVersion = true;
      }

      /// <summary>
      ///    Allows the inverse command to be updated with some internal parameter of the original command
      ///    that are not passed in constructor
      /// </summary>
      /// <param name="originalCommand">The original building block command</param>
      public virtual void UpdateInternalFrom(IBuildingBlockChangeCommand originalCommand)
      {
         ShouldChangeVersion = originalCommand.ShouldChangeVersion;
      }

      protected sealed override void ExecuteWith(IExecutionContext context)
      {
         PerformExecuteWith(context);
         context.UpdateBuildingBlockVersion(this);
      }

      protected abstract void PerformExecuteWith(IExecutionContext context);
   }
}