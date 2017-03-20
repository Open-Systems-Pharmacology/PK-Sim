using OSPSuite.Core.Commands.Core;

namespace PKSim.Core.Commands
{
   public static class BuildingBlockChangeCommandExtensions
   {
      public static T AsInverseFor<T>(this T inverseCommand, IBuildingBlockChangeCommand originalCommand) where T : IBuildingBlockChangeCommand
      {
         CommandExtensions.AsInverseFor(inverseCommand, originalCommand);
         inverseCommand.IncrementVersion = !originalCommand.IncrementVersion;
         inverseCommand.UpdateInternalFrom(originalCommand);
         return inverseCommand;
      }
   }
}