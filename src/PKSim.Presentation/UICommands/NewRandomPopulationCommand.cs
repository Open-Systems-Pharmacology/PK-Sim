using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewRandomPopulationCommand : AddBuildingBlockUICommand<Population, IPopulationTask>
   {
      public NewRandomPopulationCommand(IPopulationTask populationTask) : base(populationTask)
      {
      }
   }
}