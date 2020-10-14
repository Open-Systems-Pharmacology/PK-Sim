using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public interface IBuildingBlockChangeCommand : IPKSimReversibleCommand
   {
      /// <summary>
      ///    Specifies if the command should increment or decrement the version of the building block changed by the execute
      /// </summary>
      bool IncrementVersion { get; set; }

      /// <summary>
      ///    Returns the building block id of the building block being currently changed by the command
      /// </summary>
      string BuildingBlockId { get; }

      /// <summary>
      ///    Specifies if the command should increment the version of the building block commands or not. Default is true.
      ///    Set to false, the version of the building block will not be change. Used for instance when adding an alternative
      /// </summary>
      bool ShouldChangeVersion { get; set; }

      /// <summary>
      ///    Update internal properties of the building block change command, especially useful to create
      ///    inverse command initialized with all necessary flags
      /// </summary>
      /// <param name="originalCommand">Command from which the parameter should be updated</param>
      void UpdateInternalFrom(IBuildingBlockChangeCommand originalCommand);
   }

   public abstract class BuildingBlockChangeCommand : PKSimReversibleCommand, IBuildingBlockChangeCommand
   {
      public bool IncrementVersion { get; set; }
      public string BuildingBlockId { get; protected set; }
      public bool ShouldChangeVersion { get; set; }

      protected BuildingBlockChangeCommand()
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

      public override void RestoreExecutionData(IExecutionContext context)
      {
         context.Get(BuildingBlockId);
      }

      protected abstract void PerformExecuteWith(IExecutionContext context);
   }

   public abstract class BuildingBlockChangeCommand<T> : BuildingBlockChangeCommand
      where T : class, IPKSimBuildingBlock
   {
      protected T _buildingBlock;

      protected BuildingBlockChangeCommand(T buildingBlock)
      {
         BuildingBlockId = buildingBlock.Id;
         _buildingBlock = buildingBlock;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         ObjectType = context.TypeFor(_buildingBlock);
         context.UpdateBuildingBlockPropertiesInCommand(this, _buildingBlock);
      }

      protected override void ClearReferences()
      {
         _buildingBlock = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _buildingBlock = context.Get<T>(BuildingBlockId);
      }
   }
}