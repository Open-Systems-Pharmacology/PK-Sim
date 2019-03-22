using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewObserverBuildingBlockCommand : AddBuildingBlockUICommand<PKSimObserverBuildingBlock, IObserverBuildingBlockTask>
   {
      public NewObserverBuildingBlockCommand(IObserverBuildingBlockTask buildingBlockTask) : base(buildingBlockTask)
      {
      }
   }
}