using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_RenameBuildingBlockTask : ContextWithLoadedProject<IRenameBuildingBlockTask>
   {
      private ICloner _cloner;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _cloner = IoC.Resolve<ICloner>();
         sut = IoC.Resolve<IRenameBuildingBlockTask>();
      }

      public async Task VerifySimulationCanRun(IndividualSimulation simulation)
      {
         var simulationEngine = IoC.Resolve<IIndividualSimulationEngine>();
         await simulationEngine.RunAsync(simulation, new SimulationRunOptions());
         simulation.HasResults.ShouldBeTrue();
      }

      public void VerifySimulationCanBeCloned(IndividualSimulation simulation)
      {
         _cloner.CloneForModel(simulation);
      }
   }

   public class When_renaming_the_application_in_the_project_rename_application_v8 : concern_for_RenameBuildingBlockTask
   {
      private IndividualSimulation _s1;
      private Protocol _iv;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("RenameApplication_V8");
         _iv = FindByName<Protocol>("aa");
         _s1 = FindByName<IndividualSimulation>("aa");

         sut.RenameBuildingBlock(_iv, "bb");
      }

      [Observation]
      public async Task should_be_able_to_run_the_simulation()
      {
         await VerifySimulationCanRun(_s1);
         VerifySimulationCanBeCloned(_s1);
      }
   }
}