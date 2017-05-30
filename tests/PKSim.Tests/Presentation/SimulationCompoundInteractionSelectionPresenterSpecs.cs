using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationInteractionSelectionPresenter : ContextSpecification<ISimulationCompoundInteractionSelectionPresenter>
   {
      protected IInteractionProcessRetriever _interactionProcessRetriever;
      protected ISimulationCompoundInteractionSelectionView _view;
      protected Simulation _simulation;
      protected InteractionProperties _interactionProperties;
      protected List<SimulationPartialProcess> _allSimulationInteractionProcesses;
      protected IndividualMolecule _molecule1;
      protected InteractionProcess _interactionProcess1;
      protected InteractionProcess _interactionProcess2;
      protected InteractionProcess _interactionProcess3;
      protected IEnumerable<SimulationInteractionProcessSelectionDTO> _allSimulationInteractionProcessSelectionDTO;
      protected IndividualEnzyme _undefinedLiver;
      protected bool _useDefaultMap;

      protected override void Context()
      {
         _interactionProcessRetriever = A.Fake<IInteractionProcessRetriever>();
         _view = A.Fake<ISimulationCompoundInteractionSelectionView>();
         sut = new SimulationCompoundInteractionSelectionPresenter(_view, _interactionProcessRetriever);

         _interactionProperties = new InteractionProperties();
         _allSimulationInteractionProcesses = new List<SimulationPartialProcess>();
         _simulation = A.Fake<Simulation>();
         var individual = new Individual().WithName("Enzyme");
         _molecule1 = new IndividualEnzyme().WithName("CYP3A4");
         _undefinedLiver = new IndividualEnzyme().WithName(CoreConstants.Molecule.UndefinedLiver);
         individual.AddMolecule(_molecule1);
         individual.AddMolecule(_undefinedLiver);

         _interactionProcess1 = new InhibitionProcess().WithName("INTERACTION1");
         _interactionProcess2 = new InhibitionProcess().WithName("INTERACTION2");
         _interactionProcess3 = new InhibitionProcess().WithName("INTERACTION3");
         var comp1 = new Compound();
         comp1.AddProcess(_interactionProcess1);
         var comp2 = new Compound();
         comp2.AddProcess(_interactionProcess2);
         comp2.AddProcess(_interactionProcess3);

         _allSimulationInteractionProcesses.Add(new SimulationPartialProcess {CompoundProcess = _interactionProcess1, IndividualMolecule = _molecule1});
         _allSimulationInteractionProcesses.Add(new SimulationPartialProcess {CompoundProcess = _interactionProcess2, IndividualMolecule = _molecule1});

         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);
         A.CallTo(() => _simulation.Individual).Returns(individual);
         A.CallTo(() => _simulation.Compounds).Returns(new[] {comp1, comp2});

         A.CallTo(_interactionProcessRetriever).WithReturnType<IReadOnlyList<SimulationPartialProcess>>()
            .Invokes(x => _useDefaultMap = x.GetArgument<bool>(2))
            .Returns(_allSimulationInteractionProcesses);

         A.CallTo(() => _view.BindTo(A<IEnumerable<SimulationInteractionProcessSelectionDTO>>._))
            .Invokes(x => _allSimulationInteractionProcessSelectionDTO = x.GetArgument<IEnumerable<SimulationInteractionProcessSelectionDTO>>(0));
      }
   }

   public class When_editing_the_interaction_selection_defined_in_a_given_simulation_being_created : concern_for_SimulationInteractionSelectionPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_retrieve_all_availalbe_interactions_and_display_them_in_the_view()
      {
         _allSimulationInteractionProcessSelectionDTO.Count().ShouldBeEqualTo(2);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(0).IndividualMolecule.ShouldBeEqualTo(_molecule1);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(1).IndividualMolecule.ShouldBeEqualTo(_molecule1);

         _allSimulationInteractionProcessSelectionDTO.ElementAt(0).CompoundProcess.ShouldBeEqualTo(_interactionProcess1);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(1).CompoundProcess.ShouldBeEqualTo(_interactionProcess2);
      }

      [Observation]
      public void should_use_the_default_mapping()
      {
         _useDefaultMap.ShouldBeTrue();
      }

      [Observation]
      public void shoul_hide_the_warning_from_the_user()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_editing_the_interaction_selection_defined_in_a_given_simulation_being_with_irrerversible_inhibition : concern_for_SimulationInteractionSelectionPresenter
   {
      protected override void Context()
      {
         base.Context();
         _interactionProcess2.InteractionType = InteractionType.IrreversibleInhibition;
      }

      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      [Observation]
      public void should_retrieve_all_availalbe_interactions_and_display_them_in_the_view()
      {
         _allSimulationInteractionProcessSelectionDTO.Count().ShouldBeEqualTo(2);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(0).IndividualMolecule.ShouldBeEqualTo(_molecule1);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(1).IndividualMolecule.ShouldBeEqualTo(_molecule1);

         _allSimulationInteractionProcessSelectionDTO.ElementAt(0).CompoundProcess.ShouldBeEqualTo(_interactionProcess1);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(1).CompoundProcess.ShouldBeEqualTo(_interactionProcess2);
      }

      [Observation]
      public void should_show_the_warning_to_the_user()
      {
         _view.WarningVisible.ShouldBeTrue();
      }
   }

   public class When_editing_the_interaction_selection_defined_in_a_given_simulation_being_configured : concern_for_SimulationInteractionSelectionPresenter
   {
      protected override void Because()
      {
         sut.EditSimulation(_simulation, CreationMode.Configure);
      }

      [Observation]
      public void should_not_use_the_default_mapping()
      {
         _useDefaultMap.ShouldBeFalse();
      }

      [Observation]
      public void shoul_hide_the_warning_from_the_user()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_retrieving_the_list_of_all_availalbe_interaction_processes : concern_for_SimulationInteractionSelectionPresenter
   {
      private IEnumerable<InteractionProcess> _allInteractionProcesses;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      protected override void Because()
      {
         _allInteractionProcesses = sut.AllCompoundProcesses();
      }

      [Observation]
      public void should_return_all_interaction_processes_defined_in_the_compound()
      {
         _allInteractionProcesses.ShouldContain(_interactionProcess1, _interactionProcess2, _interactionProcess3);
      }

      [Observation]
      public void should_use_the_default_mapping()
      {
         _allInteractionProcesses.ShouldContain(_interactionProcess1, _interactionProcess2, _interactionProcess3);
      }

      [Observation]
      public void should_add_the_non_process_at_the_top()
      {
         _allInteractionProcesses.ElementAt(0).ShouldBeAnInstanceOf<NoInteractionProcess>();
      }
   }

   public class When_retrieving_the_list_of_all_availalbe_molecules : concern_for_SimulationInteractionSelectionPresenter
   {
      private IEnumerable<IndividualMolecule> _allMolecules;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      protected override void Because()
      {
         _allMolecules = sut.AllIndividualMolecules();
      }

      [Observation]
      public void should_return_all_molecules_defined_in_the_individual()
      {
         _allMolecules.ShouldOnlyContain(_molecule1);
      }

      [Observation]
      public void should_not_return_the_undefined_molecules()
      {
         _allMolecules.ShouldNotContain(_undefinedLiver);
      }
   }

   public class When_adding_a_new_interaction_to_the_simulation : concern_for_SimulationInteractionSelectionPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
      }

      protected override void Because()
      {
         sut.AddInteraction();
      }

      [Observation]
      public void should_add_a_new_interaction_mapped_to_the_first_molecule_and_without_any_selection()
      {
         var newInteraction = _allSimulationInteractionProcessSelectionDTO.Last();
         newInteraction.IndividualMolecule.ShouldBeEqualTo(_molecule1);
         newInteraction.CompoundProcess.ShouldBeAnInstanceOf<NoInteractionProcess>();
      }
   }

   public class When_removing_an_interaction_from_the_simulation : concern_for_SimulationInteractionSelectionPresenter
   {
      private SimulationInteractionProcessSelectionDTO _interactionToRemove;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         _interactionToRemove = _allSimulationInteractionProcessSelectionDTO.Last();
      }

      protected override void Because()
      {
         sut.RemoveInteraction(_interactionToRemove);
      }

      [Observation]
      public void should_remove_the_interactction_from_the_user_interface()
      {
         _allSimulationInteractionProcessSelectionDTO.Count().ShouldBeEqualTo(1);
         _allSimulationInteractionProcessSelectionDTO.Contains(_interactionToRemove).ShouldBeFalse();
      }
   }

   public class When_saving_the_interaction_mapping_in_the_simulation : concern_for_SimulationInteractionSelectionPresenter
   {
      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         _allSimulationInteractionProcessSelectionDTO.ElementAt(0).SimulationPartialProcess.CompoundProcess = _interactionProcessRetriever.NotSelectedInteractionProcess;
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_only_create_an_interaction_mapping_for_all_processes()
      {
         _interactionProperties.Interactions.Count.ShouldBeEqualTo(2);
         _interactionProperties.Interactions[0].ProcessName.ShouldBeEqualTo(string.Empty);
         _interactionProperties.Interactions[0].MoleculeName.ShouldBeEqualTo(_molecule1.Name);

         _interactionProperties.Interactions.Count.ShouldBeEqualTo(2);
         _interactionProperties.Interactions[1].ProcessName.ShouldBeEqualTo(_interactionProcess2.Name);
         _interactionProperties.Interactions[1].MoleculeName.ShouldBeEqualTo(_molecule1.Name);
      }
   }

   public class When_the_user_is_selecting_an_interaction_process_that_is_already_used : concern_for_SimulationInteractionSelectionPresenter
   {
      private SimulationInteractionProcessSelectionDTO _dto;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         _dto = _allSimulationInteractionProcessSelectionDTO.ElementAt(0);
      }

      [Observation]
      public void should_warn_the_user_that_the_selection_is_not_accepted()
      {
         The.Action(() => sut.CompoundProcessSelectionChanged(_dto, _interactionProcess2)).ShouldThrowAn<CannotSelectThePartialProcessMoreThanOnceException>();
      }

      [Observation]
      public void should_refresh_the_view_to_ensure_that_the_previously_selected_value_is_shown_again_to_the_user()
      {
         try
         {
            sut.CompoundProcessSelectionChanged(_dto, _interactionProcess2);
         }
         catch (CannotSelectThePartialProcessMoreThanOnceException)
         {
            A.CallTo(() => _view.Repaint()).MustHaveHappened();
         }
      }
   }

   public class When_the_user_is_selecting_an_interaction_process_that_is_not_already_used : concern_for_SimulationInteractionSelectionPresenter
   {
      private SimulationInteractionProcessSelectionDTO _dto;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         _dto = _allSimulationInteractionProcessSelectionDTO.ElementAt(0);
      }

      protected override void Because()
      {
         sut.CompoundProcessSelectionChanged(_dto, _interactionProcess3);
      }

      [Observation]
      public void should_have_updated_the_selected_process_for_the_given_molecule()
      {
         _dto.CompoundProcess.ShouldBeEqualTo(_interactionProcess3);
      }
   }

   public class When_the_user_deselects_many_interaction_processes : concern_for_SimulationInteractionSelectionPresenter
   {
      private SimulationInteractionProcessSelectionDTO _dto1;
      private SimulationInteractionProcessSelectionDTO _dto2;

      protected override void Context()
      {
         base.Context();
         sut.EditSimulation(_simulation, CreationMode.New);
         _dto1 = _allSimulationInteractionProcessSelectionDTO.ElementAt(0);
         _dto2 = _allSimulationInteractionProcessSelectionDTO.ElementAt(1);
      }

      protected override void Because()
      {
         sut.CompoundProcessSelectionChanged(_dto1, _interactionProcessRetriever.NotSelectedInteractionProcess.DowncastTo<InteractionProcess>());
         sut.CompoundProcessSelectionChanged(_dto2, _interactionProcessRetriever.NotSelectedInteractionProcess.DowncastTo<InteractionProcess>());
      }

      [Observation]
      public void should_have_updated_the_selected_process_for_the_given_molecule()
      {
         _dto1.CompoundProcess.ShouldBeEqualTo(_interactionProcessRetriever.NotSelectedInteractionProcess);
         _dto2.CompoundProcess.ShouldBeEqualTo(_interactionProcessRetriever.NotSelectedInteractionProcess);
      }

      [Observation]
      public void should_hide_the_warning()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }
}