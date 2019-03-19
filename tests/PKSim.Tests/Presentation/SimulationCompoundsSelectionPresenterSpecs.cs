using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundsSelectionPresenter : ContextSpecification<ISimulationCompoundsSelectionPresenter>
   {
      protected ICompoundTask _compoundTask;
      protected ISimulationCompoundsSelectionView _view;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      protected List<Compound> _allCompoundTemplates;
      protected Compound _compound1;
      protected Compound _compound2;
      protected NotifyList<CompoundSelectionDTO> _compoundDTOList;
      protected IBuildingBlockSelectionDisplayer _buildingBlockSelectionDisplayer;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound1 = new Compound().WithName("C1");
         _compound2 = new Compound().WithName("C2");
         _allCompoundTemplates = new List<Compound> {_compound1, _compound2};
      }

      protected override void Context()
      {
         _compoundTask = A.Fake<ICompoundTask>();
         _view = A.Fake<ISimulationCompoundsSelectionView>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         _buildingBlockSelectionDisplayer = A.Fake<IBuildingBlockSelectionDisplayer>();


         A.CallTo(() => _buildingBlockRepository.All<Compound>()).Returns(_allCompoundTemplates);
         A.CallTo(() => _view.BindTo(A<NotifyList<CompoundSelectionDTO>>._))
            .Invokes(x => _compoundDTOList = x.GetArgument<NotifyList<CompoundSelectionDTO>>(0));

         sut = new SimulationCompoundsSelectionPresenter(_view, _buildingBlockRepository, _buildingBlockInSimulationManager, _compoundTask, _buildingBlockSelectionDisplayer);

         sut.Initialize();

         CompoundSelectionDTOFor(_compound1).Selected = true;
      }

      protected CompoundSelectionDTO CompoundSelectionDTOFor(Compound compound)
      {
         return _compoundDTOList.FirstOrDefault(x => x.BuildingBlock == compound);
      }
   }

   public class When_initializing_the_simulation_compound_selection_presenter : concern_for_SimulationCompoundsSelectionPresenter
   {
      protected override void Because()
      {
         sut.SelectionChanged(CompoundSelectionDTOFor(_compound1));
      }

      [Observation]
      public void should_bind_to_the_view_with_the_list_of_available_compounds()
      {
         _compoundDTOList.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_hide_the_warnings()
      {
         A.CallTo(() => _view.HideError()).MustHaveHappened();
      }
   }

   public class When_loading_a_compound_from_template : concern_for_SimulationCompoundsSelectionPresenter
   {
      private Compound _newCompound;

      protected override void Context()
      {
         base.Context();
         _newCompound = new Compound().WithName("NEW COMPOUND");
         A.CallTo(() => _compoundTask.LoadSingleFromTemplate()).Returns(_newCompound);
      }

      protected override void Because()
      {
         sut.LoadCompound();
      }

      [Observation]
      public void should_add_the_compound_to_the_possible_selection()
      {
         _compoundDTOList.Select(x => x.BuildingBlock).ShouldContain(_newCompound);
      }

      [Observation]
      public void should_select_the_compound()
      {
         sut.SelectedCompounds.ShouldContain(_newCompound);
      }

      [Observation]
      public void should_keep_the_exinsting_selection()
      {
         sut.SelectedCompounds.ShouldContain(_compound1);
      }
   }

   public class When_checking_if_the_compound_selection_list_can_be_closed : concern_for_SimulationCompoundsSelectionPresenter
   {
      [Observation]
      public void should_return_true_if_at_lease_one_compound_is_selected()
      {
         sut.CanClose.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_no_compound_is_selected()
      {
         _compoundDTOList.Clear();
         sut.CanClose.ShouldBeFalse();
      }
   }

   public class When_selecting_the_same_compound_twice_in_the_compound_selection_view : concern_for_SimulationCompoundsSelectionPresenter
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         //another compound with the same name
         _allCompoundTemplates.Add(new Compound().WithName(_compound1.Name));
      }

      protected override void Because()
      {
         //this is number 1 because ordered by name
         _compoundDTOList[1].Selected = true;
         sut.SelectionChanged(_compoundDTOList[1]);
      }

      [Observation]
      public void should_deselect_the_original_compound()
      {
         _compoundDTOList[0].Selected.ShouldBeFalse();
      }
   }

   public class When_no_compound_is_selected_in_the_compound_selection_view : concern_for_SimulationCompoundsSelectionPresenter
   {
      protected override void Because()
      {
         _compoundDTOList.Clear();
      }

      [Observation]
      public void should_show_a_warning_to_the_user()
      {
         A.CallTo(() => _view.SetError(PKSimConstants.Error.AtLeastOneCompoundMustBeSelected)).MustHaveHappened();
      }
   }

   public class When_retrieving_the_tool_tip_to_be_displayed_for_a_given_compound_selection : concern_for_SimulationCompoundsSelectionPresenter
   {
      private IEnumerable<ToolTipPart> _toolTips;
      private CompoundSelectionDTO _compoundSelectionDTO;

      protected override void Context()
      {
         base.Context();
         _toolTips = new List<ToolTipPart>();
         _compoundSelectionDTO = new CompoundSelectionDTO {BuildingBlock = _compound1};
         A.CallTo(() => _buildingBlockSelectionDisplayer.ToolTipFor(_compound1)).Returns(_toolTips);
      }

      [Observation]
      public void should_return_the_tool_tip_defined_for_the_compound()
      {
         sut.ToolTipFor(_compoundSelectionDTO).ShouldBeEqualTo(_toolTips);
      }
   }

   public class When_updating_the_selected_compound_using_the_template : concern_for_SimulationCompoundsSelectionPresenter
   {
      protected override void Because()
      {
         sut.UpdateSelectedCompound(_compound2);
      }

      [Observation]
      public void should_select_the_compound()
      {
         _compoundDTOList[1].Selected.ShouldBeTrue();
      }
   }
}