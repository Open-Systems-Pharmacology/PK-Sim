using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundEnzymaticProcessPresenter : ContextSpecification<ISimulationCompoundEnzymaticProcessPresenter>
   {
      protected ISimulationCompoundEnzymaticProcessView _view;
      protected IPartialProcessRetriever _partialProcessRetriever;
      protected Simulation _simulation;
      protected Compound _compound;
      protected List<SimulationPartialProcess> _partialProcesses;
      protected List<IndividualEnzyme> _allEnzymes;
      protected List<EnzymaticProcess> _processesCompound;
      protected CompoundProperties _compoundProperties;

      protected override void Context()
      {
         _view = A.Fake<ISimulationCompoundEnzymaticProcessView>();
         _partialProcessRetriever = A.Fake<IPartialProcessRetriever>();
         _compound = new Compound();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _compoundProperties = new CompoundProperties {Compound = _compound};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         _allEnzymes = new List<IndividualEnzyme>();
         _processesCompound = new List<EnzymaticProcess>();
         _partialProcesses = new List<SimulationPartialProcess>();

         A.CallTo(_partialProcessRetriever).WithReturnType<IReadOnlyList<SimulationPartialProcess>>().Returns(_partialProcesses);

         sut = new SimulationCompoundEnzymaticProcessPresenter(_view, _partialProcessRetriever);
      }
   }

   public class When_the_simulation_has_at_least_one_process_defined : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      protected override void Context()
      {
         base.Context();
         _partialProcesses.Add(new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess()});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void presenter_must_indicate_processes_defined()
      {
         sut.HasProcessesDefined.ShouldBeTrue();
      }
   }

   public class When_the_simulation_has_no_processes_defined : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void presenter_must_indicate_no_processes_defined()
      {
         sut.HasProcessesDefined.ShouldBeFalse();
      }
   }

   public class When_the_simulation_partial_process_presenter_is_editing_the_partial_processes_defined_for_a_given_compound : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme1;
      private SimulationPartialProcess _partialProcess1Enzyme2;
      private List<SimulationEnzymaticProcessSelectionDTO> _allPartialProcessDTO;
      private IndividualMolecule _enzyme1;
      private IndividualEnzyme _enzyme2;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualEnzyme {Name = "CYP3A4"};
         _enzyme2 = new IndividualEnzyme {Name = "CYP2D6"};
         _partialProcess1Enzyme1 = new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess {MetaboliteName = "Met1"}, IndividualMolecule = _enzyme1};
         _partialProcess1Enzyme2 = new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess {MetaboliteName = "Metabolite"}, IndividualMolecule = _enzyme2};

         _partialProcesses.Add(_partialProcess1Enzyme1);
         _partialProcesses.Add(_partialProcess1Enzyme2);

         A.CallTo(() => _view.BindToPartialProcesses(A<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>._))
            .Invokes(x => _allPartialProcessDTO = x.GetArgument<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>(0).ToList());

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Comp", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Metabolite", PKSimBuildingBlockType.Compound) {BuildingBlock = new Compound().WithName("Metabolite")});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_group_the_partial_processes_by_their_enzyme_name_and_display_them()
      {
         _allPartialProcessDTO.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_set_the_default_metabolite_name_to_sink_if_the_metabolite_name_was_not_found()
      {
         _allPartialProcessDTO[0].MetaboliteName.ShouldBeEqualTo(PKSimConstants.UI.Sink);
      }

      [Observation]
      public void should_set_the_default_metabolite_name_to_the_name_of_an_existing_compound_if_one_is_available()
      {
         _allPartialProcessDTO[1].MetaboliteName.ShouldBeEqualTo(_partialProcess1Enzyme2.CompoundProcess.DowncastTo<EnzymaticProcess>().MetaboliteName);
      }
   }

   public class When_adding_an_enzymatic_partial_process_to_the_simulation_configuration : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme1;
      private SimulationPartialProcess _partialProcess1Enzyme2;
      private List<SimulationEnzymaticProcessSelectionDTO> _allPartialProcessDTO;
      private IndividualMolecule _enzyme1;
      private IndividualEnzyme _enzyme2;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualEnzyme {Name = "CYP3A4"};
         _enzyme2 = new IndividualEnzyme {Name = "CYP2D6"};
         _partialProcess1Enzyme1 = new SimulationPartialProcess { CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme1 };
         _partialProcess1Enzyme2 = new SimulationPartialProcess { CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme2 };

         _partialProcesses.Add(_partialProcess1Enzyme1);
         _partialProcesses.Add(_partialProcess1Enzyme2);

         A.CallTo(() => _view.BindToPartialProcesses(A<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>._))
            .Invokes(x => _allPartialProcessDTO = x.GetArgument<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>(0).ToList());

         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      protected override void Because()
      {
         sut.AddPartialProcessMappingBaseOn(_allPartialProcessDTO[0]);
      }

      [Observation]
      public void should_have_added_a_new_partial_process_selection_based_on_the_original_one_without_any_selection_right_after_the_original_one()
      {
         _allPartialProcessDTO.Count.ShouldBeEqualTo(3);
         _allPartialProcessDTO[1].IndividualMolecule.ShouldBeEqualTo(_enzyme1);
         _allPartialProcessDTO[1].CompoundProcess.ShouldBeEqualTo(_partialProcessRetriever.NotSelectedPartialProcess);
      }
   }

   public class When_checking_if_an_enzymatic_partial_process_can_be_removed_from_the_simulation_configuration : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme1;
      private SimulationPartialProcess _partialProcess1Enzyme2;
      private List<SimulationEnzymaticProcessSelectionDTO> _allPartialProcessDTO;
      private IndividualMolecule _enzyme1;
      private IndividualEnzyme _enzyme2;
      private SimulationPartialProcess _partialProcess1Enzyme3;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualEnzyme {Name = "CYP3A4"};
         _enzyme2 = new IndividualEnzyme {Name = "CYP2D6"};
         _partialProcess1Enzyme1 = new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme1};
         _partialProcess1Enzyme2 = new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme2};
         _partialProcess1Enzyme3 = new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme2};

         _partialProcesses.Add(_partialProcess1Enzyme1);
         _partialProcesses.Add(_partialProcess1Enzyme2);
         _partialProcesses.Add(_partialProcess1Enzyme3);

         A.CallTo(() => _view.BindToPartialProcesses(A<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>._))
            .Invokes(x => _allPartialProcessDTO = x.GetArgument<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>(0).ToList());

         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_return_true_if_there_are_at_least_two_entries_for_the_same_enzyme()
      {
         sut.CanDeletePartialProcess(_allPartialProcessDTO[1]).ShouldBeTrue();
         sut.CanDeletePartialProcess(_allPartialProcessDTO[2]).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_there_is_only_one_process_for_the_enzume()
      {
         sut.CanDeletePartialProcess(_allPartialProcessDTO[0]).ShouldBeFalse();
      }
   }

   public class When_deleting_an_enzymatic_partial_process_from_the_simulation_configuration : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme1;
      private SimulationPartialProcess _partialProcess1Enzyme2;
      private List<SimulationEnzymaticProcessSelectionDTO> _allPartialProcessDTO;
      private IndividualMolecule _enzyme1;
      private IndividualEnzyme _enzyme2;
      private SimulationPartialProcess _partialProcess1Enzyme3;

      protected override void Context()
      {
         base.Context();
         _enzyme1 = new IndividualEnzyme { Name = "CYP3A4" };
         _enzyme2 = new IndividualEnzyme { Name = "CYP2D6" };
         _partialProcess1Enzyme1 = new SimulationPartialProcess { CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme1 };
         _partialProcess1Enzyme2 = new SimulationPartialProcess { CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme2 };
         _partialProcess1Enzyme3 = new SimulationPartialProcess { CompoundProcess = new EnzymaticProcess(), IndividualMolecule = _enzyme2 };

         _partialProcesses.Add(_partialProcess1Enzyme1);
         _partialProcesses.Add(_partialProcess1Enzyme2);
         _partialProcesses.Add(_partialProcess1Enzyme3);

         A.CallTo(() => _view.BindToPartialProcesses(A<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>._))
            .Invokes(x => _allPartialProcessDTO = x.GetArgument<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>(0).ToList());

         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      protected override void Because()
      {
         sut.DeletePartialProcessMapping(_allPartialProcessDTO[2]);
      }

      [Observation]
      public void should_have_remmoved_the_deleting_entry_from_the_mapped_processes()
      {
         _allPartialProcessDTO.Count.ShouldBeEqualTo(2);
         _allPartialProcessDTO.Exists(x=>x.SimulationPartialProcess==_partialProcess1Enzyme1).ShouldBeTrue();
         _allPartialProcessDTO.Exists(x=>x.SimulationPartialProcess==_partialProcess1Enzyme2).ShouldBeTrue();
         _allPartialProcessDTO.Exists(x=>x.SimulationPartialProcess==_partialProcess1Enzyme3).ShouldBeFalse();
      }

   }

   public class When_the_simulation_partial_process_presenter_is_editing_a_simulation_with_only_one_compound : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme1;

      protected override void Context()
      {
         base.Context();
         _partialProcess1Enzyme1 = A.Fake<SimulationPartialProcess>();
         _partialProcess1Enzyme1.CompoundProcess = new EnzymaticProcess {MetaboliteName = "Met1"};

         _partialProcesses.Add(_partialProcess1Enzyme1);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Comp", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hode_the_metabolite_column_since_no_metabolite_can_be_selected()
      {
         A.CallTo(() => _view.HideMultipleCompoundColumns()).MustHaveHappened();
      }
   }

   public class When_saving_the_enzymatic_selection_in_a_given_simulation_and_the_user_selects_the_sink_option : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme1;
      private SimulationPartialProcess _partialProcess1Enzyme2;

      protected override void Context()
      {
         base.Context();
         _partialProcess1Enzyme1 = A.Fake<SimulationPartialProcess>();
         _partialProcess1Enzyme1.CompoundProcess = new EnzymaticProcess {MetaboliteName = "Met1"};
         _partialProcess1Enzyme2 = A.Fake<SimulationPartialProcess>();
         _partialProcess1Enzyme2.CompoundProcess = new EnzymaticProcess {MetaboliteName = "Metabolite"};

         _partialProcesses.Add(_partialProcess1Enzyme1);
         _partialProcesses.Add(_partialProcess1Enzyme2);

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Comp", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Metabolite", PKSimBuildingBlockType.Compound) {BuildingBlock = new Compound().WithName("Metabolite")});

         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_set_the_metabolite_name_to_null()
      {
         var metaboliteProcesses = _compoundProperties.Processes.MetabolizationSelection.AllPartialProcesses().OfType<EnzymaticProcessSelection>().ToList();
         metaboliteProcesses[0].MetaboliteName.ShouldBeEqualTo(string.Empty);
         metaboliteProcesses[1].MetaboliteName.ShouldBeEqualTo("Metabolite");
      }
   }

   public class When_saving_the_enzymatic_selection_in_a_given_simulation_and_the_user_deselect_a_partial_process : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcessEnzyme;

      protected override void Context()
      {
         base.Context();
         _partialProcessEnzyme = A.Fake<SimulationPartialProcess>();
         _partialProcesses.Add(_partialProcessEnzyme);
         _partialProcessEnzyme.CompoundProcess = _partialProcessRetriever.NotSelectedPartialProcess.DowncastTo<EnzymaticProcess>();
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Comp", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      protected override void Because()
      {
         sut.SaveConfiguration();
      }

      [Observation]
      public void should_not_have_any_process_enabled_in_the_metabolization_section()
      {
         _compoundProperties.Processes.MetabolizationSelection.AllEnabledProcesses().Any().ShouldBeFalse();
      }
   }

   public class When_the_simulation_metabolism_presenter_is_checking_the_validity_of_a_configuration_containing_a_speficic_and_systemic_clearance : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      protected override void Context()
      {
         base.Context();

         _compound.AddProcess(new SystemicProcess {Name = "PLS", InternalName = "PLS", SystemicProcessType = SystemicProcessTypes.Hepatic});
         _compound.AddProcess(new EnzymaticProcess {Name = "Partial", InternalName = "MM"});
         var simulationPartialProcess = new SimulationPartialProcess {CompoundProcess = new EnzymaticProcess {MoleculeName = "MOL"}};
         A.CallTo(_partialProcessRetriever).WithReturnType<IReadOnlyList<SimulationPartialProcess>>().Returns(new[] {simulationPartialProcess});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_show_the_warning_field()
      {
         _view.WarningVisible.ShouldBeTrue();
      }

      [Observation]
      public void should_display_the_accurate_warning_message()
      {
         _view.Warning.ShouldBeEqualTo(PKSimConstants.Warning.HepaticAndSpecific);
      }
   }

   public class When_the_simulation_transport_presenter_is_checking_the_validity_of_a_configuration_containing_only_a_specific_clearance : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      protected override void Context()
      {
         base.Context();

         _compound.AddProcess(new EnzymaticProcess {Name = "Partial", InternalName = "MM"});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_the_simulation_metabolism_presenter_is_checking_the_validity_of_a_configuration_containing_only_a_plasma_clearance : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      protected override void Context()
      {
         base.Context();
         _compound.AddProcess(new SystemicProcess {Name = "PLS", InternalName = "PLS", SystemicProcessType = SystemicProcessTypes.Hepatic});
      }

      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_the_simulation_metabolism_presenter_is_checking_the_validity_of_a_configuration_containing_no_processes : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      protected override void Because()
      {
         sut.EditProcessesIn(_simulation, _compoundProperties);
      }

      [Observation]
      public void should_hide_the_warning_field()
      {
         _view.WarningVisible.ShouldBeFalse();
      }
   }

   public class When_updating_the_selected_process_for_a_given_enzyme : concern_for_SimulationCompoundEnzymaticProcessPresenter
   {
      private SimulationPartialProcess _partialProcess1Enzyme2;
      private List<SimulationEnzymaticProcessSelectionDTO> _allPartialProcessDTO;

      protected override void Context()
      {
         base.Context();
         _partialProcess1Enzyme2 = A.Fake<SimulationPartialProcess>();
         _partialProcess1Enzyme2.CompoundProcess = new EnzymaticProcess {MetaboliteName = "Metabolite"};

         _partialProcesses.Add(_partialProcess1Enzyme2);

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Comp", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Metabolite", PKSimBuildingBlockType.Compound) {BuildingBlock = new Compound().WithName("Metabolite")});


         A.CallTo(() => _view.BindToPartialProcesses(A<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>._))
            .Invokes(x => _allPartialProcessDTO = x.GetArgument<IReadOnlyCollection<SimulationEnzymaticProcessSelectionDTO>>(0).ToList());

         sut.EditProcessesIn(_simulation, _compoundProperties);
         _allPartialProcessDTO[0].MetaboliteName = "xxx";
      }

      protected override void Because()
      {
         sut.SelectionChanged(_allPartialProcessDTO[0]);
      }

      [Observation]
      public void should_also_update_the_name_of_the_metabolite()
      {
         _allPartialProcessDTO[0].MetaboliteName.ShouldBeEqualTo("Metabolite");
      }
   }
}