using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_RenameBuildingBlockTask : ContextWithLoadedProject<IRenameBuildingBlockTask>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("BuildingBlockRename_611");
         sut = IoC.Resolve<IRenameBuildingBlockTask>();
      }

      public async Task VerifySimulationCanRun(IndividualSimulation simulation)
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(simulation, new Core.Services.SimulationRunOptions());
         simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_renaming_the_formulation_weibull_in_the_project_BuildingBlockRename_611 : concern_for_RenameBuildingBlockTask
   {
      private Formulation _formulation;
      private IndividualSimulation _simulation;
      private string _oldName;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _formulation = FindByName<Formulation>("Weibull");
         _oldName = _formulation.Name;
         _formulation.Name = "NEW F";
         _simulation = FindByName<IndividualSimulation>("S3");
         sut.RenameUsageOfBuildingBlockInProject(_formulation, _oldName);
      }

      [Observation]
      public void should_have_renamed_the_object_path_referencing_all_sub_parameters()
      {
         var totalDrugMass = _simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(CoreConstants.Parameter.TotalDrugMass)).First();
         totalDrugMass.Formula.ObjectPaths.Each(path => path.PathAsString.Contains(_oldName).ShouldBeFalse());
         totalDrugMass.Formula.ObjectPaths.Any(path => path.PathAsString.Contains(_formulation.Name)).ShouldBeTrue();
      }

      [Observation]
      public void should_have_marked_the_simulation_as_changed()
      {
         _simulation.HasChanged.ShouldBeTrue();
      }

      [Observation]
      public async Task should_be_able_to_run_the_simulation()
      {
         await VerifySimulationCanRun(_simulation);
      }
   }

   public class When_renaming_the_application_in_the_project_BuildingBlockRename_611 : concern_for_RenameBuildingBlockTask
   {
      private IndividualSimulation _s1;
      private IndividualSimulation _s2;
      private IndividualSimulation _s3;
      private Protocol _iv;
      private Protocol _oral;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _iv = FindByName<Protocol>("IV");
         _oral = FindByName<Protocol>("ORAL");
         _s1 = FindByName<IndividualSimulation>("S1");
         _s2 = FindByName<IndividualSimulation>("S2");
         _s3 = FindByName<IndividualSimulation>("S3");

         _iv.Name = "NEW_IV";
         sut.RenameUsageOfBuildingBlockInProject(_iv, "IV");

         _oral.Name = "NEW_ORAL";
         sut.RenameUsageOfBuildingBlockInProject(_oral, "ORAL");
      }

      [Observation]
      public void should_have_renamed_the_applicaiton_container_in_the_model()
      {
         _s1.Model.Root.EntityAt<Container>(Constants.APPLICATIONS, "NEW_IV").ShouldNotBeNull();
         _s2.Model.Root.EntityAt<Container>(Constants.APPLICATIONS, "NEW_ORAL").ShouldNotBeNull();
         _s3.Model.Root.EntityAt<Container>(Constants.APPLICATIONS, "NEW_ORAL").ShouldNotBeNull();
      }

      [Observation]
      public async Task should_be_able_to_run_the_simulation()
      {
         await VerifySimulationCanRun(_s1);
         await VerifySimulationCanRun(_s2);
         await VerifySimulationCanRun(_s3);
      }
   }
}