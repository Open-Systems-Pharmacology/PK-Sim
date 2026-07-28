using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using IContainer = OSPSuite.Core.Domain.IContainer;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_renaming_an_expression_profile : ContextForSimulationIntegration<IRenameBuildingBlockTask>
   {
      protected Individual _individual;
      protected ExpressionProfile _expressionProfile;
      protected PKSimProject _project;
      protected IBuildingBlockInProjectManager _buildingBlockInProjectManager;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IRenameBuildingBlockTask>();
         _buildingBlockInProjectManager = IoC.Resolve<IBuildingBlockInProjectManager>();

         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualEnzyme>(_individual, "CYP3A4");

         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, compound, protocol) as IndividualSimulation;

         _project = new PKSimProject();
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_expressionProfile);
         _project.AddBuildingBlock(compound);
         _project.AddBuildingBlock(protocol);
         _project.AddBuildingBlock(_simulation);

         IoC.Resolve<ICoreWorkspace>().Project = _project;
         var registrationTask = IoC.Resolve<IRegistrationTask>();
         _project.All<IPKSimBuildingBlock>().Each(registrationTask.Register);

         //simulate a saved project: nothing has changed yet
         _project.HasChanged = false;
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _project.All<IPKSimBuildingBlock>().Each(Unregister);
      }
   }

   public class When_renaming_the_molecule_of_an_expression_profile_used_by_an_individual_used_in_a_simulation : concern_for_renaming_an_expression_profile
   {
      protected override void Because()
      {
         sut.RenameBuildingBlock(_expressionProfile, "CYP2D6|Human|Standard");
      }

      [Observation]
      public void should_have_renamed_the_molecule_in_the_individual()
      {
         _individual.MoleculeByName("CYP2D6").ShouldNotBeNull();
         _individual.MoleculeByName("CYP3A4").ShouldBeNull();
      }

      [Observation]
      public void should_have_renamed_all_molecule_containers_in_the_individual()
      {
         _individual.AllMoleculeContainersFor(_individual.MoleculeByName("CYP2D6")).Count.ShouldBeGreaterThan(0);
         _individual.GetAllChildren<IContainer>(x => x.IsNamed("CYP3A4")).Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_marked_the_individual_as_changed_so_that_the_rename_is_persisted()
      {
         _individual.HasChanged.ShouldBeTrue();
      }

      [Observation]
      public void should_have_marked_the_expression_profile_as_changed()
      {
         _expressionProfile.HasChanged.ShouldBeTrue();
      }

      [Observation]
      public void should_have_marked_the_simulation_as_out_of_sync_with_its_building_blocks()
      {
         _buildingBlockInProjectManager.StatusFor(_simulation).ShouldBeEqualTo(BuildingBlockStatus.Red);
      }
   }

   public abstract class concern_for_renaming_an_expression_profile_mapped_to_a_process : ContextForSimulationIntegration<IRenameBuildingBlockTask>
   {
      protected Individual _individual;
      protected ExpressionProfile _expressionProfile;
      protected Compound _compound;
      protected Protocol _protocol;
      protected PKSimProject _project;
      protected const string _transportProcessName = "ActProc1";

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IRenameBuildingBlockTask>();
         var context = IoC.Resolve<IExecutionContext>();

         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _expressionProfile = DomainFactoryForSpecs.CreateExpressionProfileAndAddToIndividual<IndividualTransporter>(_individual, "Tr1");
         new SetTransportTypeCommand(_expressionProfile.Molecule.DowncastTo<IndividualTransporter>(), TransportType.Efflux, context).Run(context);

         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var transportProcess = IoC.Resolve<ICloneManager>()
            .Clone(IoC.Resolve<ICompoundProcessRepository>().ProcessByName(CoreConstantsForSpecs.Process.ACTIVE_TRANSPORT_SPECIFIC_MM))
            .WithName(_transportProcessName);
         _compound.AddProcess(transportProcess);

         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol).DowncastTo<IndividualSimulation>();
         addTransportSelectionTo(_simulation, _expressionProfile.MoleculeName);
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         _project = new PKSimProject();
         _project.AddBuildingBlock(_individual);
         _project.AddBuildingBlock(_expressionProfile);
         _project.AddBuildingBlock(_compound);
         _project.AddBuildingBlock(_protocol);
         _project.AddBuildingBlock(_simulation);

         IoC.Resolve<ICoreWorkspace>().Project = _project;
         var registrationTask = IoC.Resolve<IRegistrationTask>();
         _project.All<IPKSimBuildingBlock>().Each(registrationTask.Register);
      }

      protected static void addTransportSelectionTo(Simulation simulation, string moleculeName)
      {
         simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddPartialProcessSelection(new ProcessSelection { ProcessName = _transportProcessName, MoleculeName = moleculeName });
      }

      protected ProcessSelection TransportSelectionOf(Simulation simulation) =>
         simulation.CompoundPropertiesList.First().Processes.TransportAndExcretionSelection.AllPartialProcesses().First();

      protected override void Because()
      {
         sut.RenameBuildingBlock(_expressionProfile, "Tr2|Human|Standard");
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _project.All<IPKSimBuildingBlock>().Each(Unregister);
      }
   }

   public class When_renaming_the_molecule_of_an_expression_profile_mapped_to_a_process_in_a_simulation : concern_for_renaming_an_expression_profile_mapped_to_a_process
   {
      [Observation]
      public void should_have_renamed_the_molecule_referenced_by_the_process_selection_of_the_simulation()
      {
         TransportSelectionOf(_simulation).MoleculeName.ShouldBeEqualTo("Tr2");
      }
   }

   public class When_undoing_the_rename_of_the_molecule_of_an_expression_profile_mapped_to_a_process_in_a_simulation :
      concern_for_renaming_an_expression_profile_mapped_to_a_process
   {
      protected override void Because()
      {
         //Go through the command that is actually put in the history, so that the undo uses the production inverse
         var context = IoC.Resolve<IExecutionContext>();
         new RenameEntityCommand(_expressionProfile, "Tr2|Human|Standard", context)
            .ExecuteAndInvokeInverse(context);
      }

      [Observation]
      public void should_have_restored_the_molecule_name_in_the_individual()
      {
         _individual.MoleculeByName("Tr1").ShouldNotBeNull();
         _individual.MoleculeByName("Tr2").ShouldBeNull();
      }

      [Observation]
      public void should_have_restored_the_molecule_referenced_by_the_process_selection_of_the_simulation()
      {
         TransportSelectionOf(_simulation).MoleculeName.ShouldBeEqualTo("Tr1");
      }
   }

   public class When_reconfiguring_a_simulation_after_the_molecule_of_an_expression_profile_was_renamed :
      concern_for_renaming_an_expression_profile_mapped_to_a_process
   {
      [Observation]
      public void should_offer_a_simulation_subject_whose_molecule_matches_the_process_selection()
      {
         //This is what the Configure Simulation wizard pre-selects for the simulation subject step
         var offeredSubject = IoC.Resolve<IBuildingBlockInProjectManager>()
            .TemplateBuildingBlocksUsedBy<ISimulationSubject>(_simulation).Single();

         var mappedMoleculeName = TransportSelectionOf(_simulation).MoleculeName;
         offeredSubject.MoleculeByName(mappedMoleculeName).ShouldNotBeNull();
      }
   }

   public class When_recreating_a_simulation_after_the_molecule_of_an_expression_profile_was_renamed : concern_for_renaming_an_expression_profile_mapped_to_a_process
   {
      [Observation]
      public void should_be_able_to_create_the_simulation_model_from_the_renamed_building_blocks()
      {
         var recreatedSimulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol);
         addTransportSelectionTo(recreatedSimulation, TransportSelectionOf(_simulation).MoleculeName);

         DomainFactoryForSpecs.AddModelToSimulation(recreatedSimulation);

         recreatedSimulation.Model.ShouldNotBeNull();
      }
   }
}
