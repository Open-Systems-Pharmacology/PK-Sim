using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewProtocolCommand : AddBuildingBlockUICommand<Protocol, IProtocolTask>
   {
      public NewProtocolCommand(IProtocolTask buildingBlockTask) : base(buildingBlockTask)
      {
      }
   }
}