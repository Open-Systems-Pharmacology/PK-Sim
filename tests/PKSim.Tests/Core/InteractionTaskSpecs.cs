using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_InteractionTask : ContextSpecification<IInteractionTask>
   {
      protected override void Context()
      {
         sut = new InteractionTask();
      }
   }

   public class When_checking_if_a_compound_is_used_as_metabolite_of_a_reaction : concern_for_InteractionTask
   {
      private Compound _compound;
      private Compound _metabolite;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound().WithName("Comp");
         _metabolite = new Compound().WithName("Metabolite");
         _simulation = new IndividualSimulation();
         _simulation.Properties = new SimulationProperties();
         _simulation.Properties.AddCompoundProperties(new CompoundProperties {Compound = _compound});
         _simulation.Properties.AddCompoundProperties(new CompoundProperties {Compound = _metabolite});

         var enzymaticProcessSelection = new EnzymaticProcessSelection {CompoundName = _compound.Name, MetaboliteName = _metabolite.Name};
         _simulation.CompoundPropertiesFor(_compound).Processes.MetabolizationSelection.AddPartialProcessSelection(enzymaticProcessSelection);
      }

      [Observation]
      public void should_return_true_if_the_compound_is_indeed_a_metabolite()
      {
         sut.IsMetabolite(_metabolite, _simulation).ShouldBeTrue();
      }

      [Observation]
      public void should_return_flase_if_the_compound_is_not_used_as_metabolite()
      {
         sut.IsMetabolite(_compound, _simulation).ShouldBeFalse();
      }
   }

   public class When_retreiving_the_interaction_containers_defined_in_a_simulation : concern_for_InteractionTask
   {
      private IContainer _processContainer;
      private IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         var root = new Container();
         _simulation = new IndividualSimulation();

         _simulation.Model = new OSPSuite.Core.Domain.Model {Root = root};
         var inhibitorContainer = new Container().WithName("I");
         _processContainer = new Container().WithName("Proc");
         var anotherContainer = new Container().WithName("Proc2");
         inhibitorContainer.Add(_processContainer);
         inhibitorContainer.Add(anotherContainer);
         root.Add(inhibitorContainer);

         _simulation.Properties = new SimulationProperties();
         _simulation.Properties.InteractionProperties.AddInteraction(new InteractionSelection {CompoundName = "I", MoleculeName = "CYP", ProcessName = "Proc"});
      }

      [Observation]
      public void should_return_the_global_container_in_model_matching_the_interactions_defined_in_the_simulation()
      {
         sut.AllInteractionContainers(_simulation).ShouldOnlyContain(_processContainer);
      }
   }

   public abstract class When_checking_if_a_compound_is_involved_in_some_interaction_processes : concern_for_InteractionTask
   {
      protected Compound _compound;
      protected CompoundProperties _compoundProperties;
      protected InteractionSelection _interactionSelection;
      protected IndividualSimulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation();

         _compound = new Compound().WithId("Comp");
         _simulation.Properties = new SimulationProperties();
         _compoundProperties = new CompoundProperties {Compound = _compound};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         _interactionSelection = new InteractionSelection {MoleculeName = "CYP3A4", ProcessName = "Proc"};
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
      }
   }

   public class When_checking_if_a_compound_metabolized_by_CYP3A4_is_involved_in_some_interaction_processes_for_a_simulation_with_interactions_for_CYP3A4 : When_checking_if_a_compound_is_involved_in_some_interaction_processes
   {
      protected override void Context()
      {
         base.Context();
         _simulation.Properties.InteractionProperties.AddInteraction(_interactionSelection);
         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(new ProcessSelection {ProcessName = "P1", MoleculeName = _interactionSelection.MoleculeName});
      }

      [Observation]
      public void should_return_true()
      {
         sut.HasInteractionInvolving(_compound, _simulation).ShouldBeTrue();
      }
   }

   public class When_checking_if_a_compound_is_involved_in_some_interaction_processes_for_a_simulation_without_interaction : When_checking_if_a_compound_is_involved_in_some_interaction_processes
   {
      [Observation]
      public void should_return_false()
      {
         sut.HasInteractionInvolving(_compound, _simulation).ShouldBeFalse();
      }
   }

   public class When_checking_if_a_compound_only_taking_part_in_plasma_clearance_processes_is_involved_in_some_interaction_processes_for_a_simulation_with_interactions : When_checking_if_a_compound_is_involved_in_some_interaction_processes
   {
      protected override void Context()
      {
         base.Context();
         _simulation.Properties.InteractionProperties.AddInteraction(_interactionSelection);
         _compoundProperties.Processes.MetabolizationSelection.AddSystemicProcessSelection(new SystemicProcessSelection {ProcessType = SystemicProcessTypes.Hepatic});
      }

      [Observation]
      public void should_return_false()
      {
         sut.HasInteractionInvolving(_compound, _simulation).ShouldBeFalse();
      }
   }

   public class When_checking_if_a_compound_metabolized_by_CYP3A4_is_involved_in_some_interaction_processes_for_a_simulation_with_interactions_for_CYP2D6 : When_checking_if_a_compound_is_involved_in_some_interaction_processes
   {
      protected override void Context()
      {
         base.Context();
         _simulation.Properties.InteractionProperties.AddInteraction(_interactionSelection);
         _compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(new ProcessSelection {ProcessName = "P1", MoleculeName = "CYP2D6"});
      }

      [Observation]
      public void should_return_false()
      {
         sut.HasInteractionInvolving(_compound, _simulation).ShouldBeFalse();
      }
   }

   public class When_retrieving_the_interaction_processes_defined_for_a_given_molecule_and_a_given_interaction_type : concern_for_InteractionTask
   {
      private InhibitionProcess _mixedInhibition;
      private InhibitionProcess _competitiveInhibition;
      protected Simulation _simulation;
      private Compound _compound1;
      private Compound _compound2;
      private InteractionProperties _interactionProperties;
      protected string _moleculeName = "ENZYME";
      protected string _drugName = "DRUG";

      protected override void Context()
      {
         base.Context();

         _simulation = A.Fake<Simulation>();
         _mixedInhibition = new InhibitionProcess {InteractionType = InteractionType.MixedInhibition}.WithName("MixedInhibition");
         _competitiveInhibition = new InhibitionProcess {InteractionType = InteractionType.CompetitiveInhibition}.WithName("CompetitiveInhibition");
         _compound1 = new Compound().WithName("Compound1");
         _compound2 = new Compound().WithName("Compound2");
         _compound2.AddProcess(_mixedInhibition);
         _compound2.AddProcess(_competitiveInhibition);

         _interactionProperties = new InteractionProperties();
         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);
         A.CallTo(() => _simulation.Compounds).Returns(new[] {_compound1, _compound2});

         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _mixedInhibition.Name, CompoundName = _compound2.Name});
         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _moleculeName, ProcessName = _competitiveInhibition.Name, CompoundName = _compound2.Name});
      }

      [Observation]
      public void should_return_an_empty_enumeration_if_no_interaction_was_defined_for_that_molecule()
      {
         sut.AllInteractionProcessesFor("A MOLECULE", InteractionType.MixedInhibition, _simulation).ShouldBeEmpty();
      }

      [Observation]
      public void should_return_an_empty_enumeration_if_interaction_were_defined_for_that_molecule_but_for_another_type()
      {
         sut.AllInteractionProcessesFor(_moleculeName, InteractionType.UncompetitiveInhibition, _simulation).ShouldBeEmpty();
      }

      [Observation]
      public void should_return_the_expected_interacton_processes_otherwise()
      {
         sut.AllInteractionProcessesFor(_moleculeName, InteractionType.MixedInhibition, _simulation).ShouldOnlyContain(_mixedInhibition);
         sut.AllInteractionProcessesFor(_moleculeName, InteractionType.CompetitiveInhibition, _simulation).ShouldOnlyContain(_competitiveInhibition);
      }

      [Observation]
      public void should_filter_out_auto_inhibition_terms_if_a_compound_name_is_specified_that_is_triggering_inhibition()
      {
         sut.AllInteractionProcessesFor(_moleculeName, InteractionType.MixedInhibition, _simulation, _compound2.Name).ShouldBeEmpty();
      }

      [Observation]
      public void should_not_filter_out_auto_inhibition_terms_if_a_compound_name_is_specified()
      {
         sut.AllInteractionProcessesFor(_moleculeName, InteractionType.MixedInhibition, _simulation, _compound1.Name).ShouldOnlyContain(_mixedInhibition);
      }
   }
}