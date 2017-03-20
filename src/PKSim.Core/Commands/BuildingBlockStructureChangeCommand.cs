namespace PKSim.Core.Commands
{
   public interface IBuildingBlockStructureChangeCommand : IBuildingBlockChangeCommand
   {
   }

   public abstract class BuildingBlockStructureChangeCommand : BuildingBlockChangeCommand, IBuildingBlockStructureChangeCommand
   {
   }
}