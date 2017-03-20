using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
   public class NewRandomPopulationCommand : AddBuildingBlockUICommand<Population, IPopulationTask>
   {
      public NewRandomPopulationCommand(IPopulationTask populationTask) : base(populationTask)
      {
      }
   }
}