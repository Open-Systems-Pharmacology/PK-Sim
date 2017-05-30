using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class PopulationTask : BuildingBlockTask<Population>, IPopulationTask
   {
      public PopulationTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController)
         : base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Population)
      {
      }

      public override Population AddToProject()
      {
         return AddToProject<ICreateRandomPopulationPresenter>();
      }

      public void AddToProjectBasedOn(Individual individual)
      {
         AddToProject<ICreateRandomPopulationPresenter>(x => x.CreatePopulation(individual));
      }
   }
}