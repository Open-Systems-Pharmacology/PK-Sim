﻿using System.Linq;
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
         await simulationEngine.RunAsync(simulation, new Core.Services.SimulationRunOptions());
         simulation.HasResults.ShouldBeTrue();
      }

      public void VerifySimulationCanBeCloned(IndividualSimulation simulation)
      {
         _cloner.CloneForModel(simulation);
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
         LoadProject("BuildingBlockRename_611");
         _formulation = FindByName<Formulation>("Weibull");
         _oldName = _formulation.Name;
         _simulation = FindByName<IndividualSimulation>("S3");
         sut.RenameBuildingBlock(_formulation, "NEW F");
      }

      [Observation]
      public void should_have_renamed_the_object_path_referencing_all_sub_parameters()
      {
         var totalDrugMass = _simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(CoreConstants.Parameters.TOTAL_DRUG_MASS)).First();
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
         LoadProject("BuildingBlockRename_611");
         _iv = FindByName<Protocol>("IV");
         _oral = FindByName<Protocol>("ORAL");
         _s1 = FindByName<IndividualSimulation>("S1");
         _s2 = FindByName<IndividualSimulation>("S2");
         _s3 = FindByName<IndividualSimulation>("S3");

         sut.RenameBuildingBlock(_iv, "NEW_IV");
         sut.RenameBuildingBlock(_oral, "NEW_ORAL");
      }

      [Observation]
      public void should_have_renamed_the_application_container_in_the_model()
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
      public void should_have_renamed_the_application_container_in_the_model()
      {
         _s1.Model.Root.EntityAt<Container>(Constants.APPLICATIONS, "bb").ShouldNotBeNull();
      }

      [Observation]
      public async Task should_be_able_to_run_the_simulation()
      {
         await VerifySimulationCanRun(_s1);
         VerifySimulationCanBeCloned(_s1);
      }
   }
}