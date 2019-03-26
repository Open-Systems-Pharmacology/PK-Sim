using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewObserverSetCommand : AddBuildingBlockUICommand<ObserverSet, IObserverSetTask>
   {
      public NewObserverSetCommand(IObserverSetTask buildingBlockTask) : base(buildingBlockTask)
      {
      }
   }
}