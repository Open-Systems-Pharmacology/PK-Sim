using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewEventCommand : AddBuildingBlockUICommand<PKSimEvent, IEventTask>
   {
      public NewEventCommand(IEventTask eventTask) : base(eventTask)
      {
      }
   }
}