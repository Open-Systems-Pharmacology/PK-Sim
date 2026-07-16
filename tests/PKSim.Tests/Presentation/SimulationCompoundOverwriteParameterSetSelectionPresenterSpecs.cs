using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation;

public abstract class concern_for_SimulationCompoundOverwriteParameterSetSelectionPresenter : ContextSpecification<ISimulationCompoundOverwriteParameterSetSelectionPresenter>
{
   protected ISimulationCompoundOverwriteParameterSetSelectionView _view;
   protected ILazyLoadTask _lazyLoadTask;
   protected Simulation _simulation;
   protected Compound _compound;
   protected OverwriteParameterSetSelections _selections;

   protected override void Context()
   {
      _view = A.Fake<ISimulationCompoundOverwriteParameterSetSelectionView>();
      _lazyLoadTask = A.Fake<ILazyLoadTask>();
      _simulation = A.Fake<Simulation>();
      _compound = new Compound { Name = "Aspirin" };
      _selections = new OverwriteParameterSetSelections();
      A.CallTo(() => _simulation.OverwriteParameterSetSelections).Returns(_selections);
      A.CallTo(() => _simulation.CompoundNames).Returns(new[] { "Aspirin" });
      sut = new SimulationCompoundOverwriteParameterSetSelectionPresenter(_view, _lazyLoadTask);
   }
}

public class When_editing_a_simulation_with_a_compound_having_no_overwrite_parameter_sets : concern_for_SimulationCompoundOverwriteParameterSetSelectionPresenter
{
   private SimulationCompoundOverwriteParameterSetSelectionDTO _capturedDto;

   protected override void Context()
   {
      base.Context();
      A.CallTo(() => _view.BindTo(A<SimulationCompoundOverwriteParameterSetSelectionDTO>._))
         .Invokes(call => _capturedDto = call.GetArgument<SimulationCompoundOverwriteParameterSetSelectionDTO>(0));
   }

   protected override void Because()
   {
      sut.EditSimulation(_simulation, _compound);
   }

   [Observation]
   public void should_lazy_load_the_compound()
   {
      A.CallTo(() => _lazyLoadTask.Load(_compound)).MustHaveHappened();
   }

   [Observation]
   public void should_provide_only_the_none_option()
   {
      _capturedDto.AllOverwriteParameterSets.Count.ShouldBeEqualTo(1);
      _capturedDto.AllOverwriteParameterSets[0].Name.ShouldBeEqualTo(PKSimConstants.UI.None);
   }

   [Observation]
   public void should_select_the_none_option_by_default()
   {
      _capturedDto.SelectedOverwriteParameterSet.Name.ShouldBeEqualTo(PKSimConstants.UI.None);
   }
}

public class When_editing_a_simulation_with_a_compound_having_a_default_overwrite_parameter_set : concern_for_SimulationCompoundOverwriteParameterSetSelectionPresenter
{
   private SimulationCompoundOverwriteParameterSetSelectionDTO _capturedDto;
   private OverwriteParameterSet _defaultSet;
   private OverwriteParameterSet _otherSet;

   protected override void Context()
   {
      base.Context();
      _defaultSet = new OverwriteParameterSet { Name = "RenalImpairment", IsDefault = true };
      _otherSet = new OverwriteParameterSet { Name = "HepaticImpairment" };
      _compound.AddOverwriteParameterSet(_defaultSet);
      _compound.AddOverwriteParameterSet(_otherSet);
      A.CallTo(() => _view.BindTo(A<SimulationCompoundOverwriteParameterSetSelectionDTO>._))
         .Invokes(call => _capturedDto = call.GetArgument<SimulationCompoundOverwriteParameterSetSelectionDTO>(0));
   }

   protected override void Because()
   {
      sut.EditSimulation(_simulation, _compound);
   }

   [Observation]
   public void should_show_none_first_followed_by_all_overwrite_parameter_sets()
   {
      _capturedDto.AllOverwriteParameterSets.Count.ShouldBeEqualTo(3);
      _capturedDto.AllOverwriteParameterSets[0].Name.ShouldBeEqualTo(PKSimConstants.UI.None);
      _capturedDto.AllOverwriteParameterSets.Skip(1).ShouldContain(_defaultSet);
      _capturedDto.AllOverwriteParameterSets.Skip(1).ShouldContain(_otherSet);
   }

   [Observation]
   public void should_auto_select_the_default_overwrite_parameter_set()
   {
      _capturedDto.SelectedOverwriteParameterSet.ShouldBeEqualTo(_defaultSet);
   }
}

public class When_editing_a_simulation_with_an_existing_selection_that_is_not_the_default : concern_for_SimulationCompoundOverwriteParameterSetSelectionPresenter
{
   private SimulationCompoundOverwriteParameterSetSelectionDTO _capturedDto;
   private OverwriteParameterSet _defaultSet;
   private OverwriteParameterSet _selectedSet;

   protected override void Context()
   {
      base.Context();
      _defaultSet = new OverwriteParameterSet { Name = "RenalImpairment", IsDefault = true };
      _selectedSet = new OverwriteParameterSet { Name = "HepaticImpairment" };
      _compound.AddOverwriteParameterSet(_defaultSet);
      _compound.AddOverwriteParameterSet(_selectedSet);
      _selections.SetSelectionForCompound("Aspirin", _selectedSet);
      A.CallTo(() => _view.BindTo(A<SimulationCompoundOverwriteParameterSetSelectionDTO>._))
         .Invokes(call => _capturedDto = call.GetArgument<SimulationCompoundOverwriteParameterSetSelectionDTO>(0));
   }

   protected override void Because()
   {
      sut.EditSimulation(_simulation, _compound);
   }

   [Observation]
   public void should_use_the_existing_selection_rather_than_the_default()
   {
      _capturedDto.SelectedOverwriteParameterSet.ShouldBeEqualTo(_selectedSet);
   }
}

public class When_saving_a_real_overwrite_parameter_set_selection : concern_for_SimulationCompoundOverwriteParameterSetSelectionPresenter
{
   private OverwriteParameterSet _userSelectedSet;
   private SimulationCompoundOverwriteParameterSetSelectionDTO _capturedDto;

   protected override void Context()
   {
      base.Context();
      _userSelectedSet = new OverwriteParameterSet { Name = "HepaticImpairment" };
      _compound.AddOverwriteParameterSet(_userSelectedSet);
      A.CallTo(() => _view.BindTo(A<SimulationCompoundOverwriteParameterSetSelectionDTO>._))
         .Invokes(call => _capturedDto = call.GetArgument<SimulationCompoundOverwriteParameterSetSelectionDTO>(0));
      sut.EditSimulation(_simulation, _compound);
      _capturedDto.SelectedOverwriteParameterSet = _userSelectedSet;
   }

   protected override void Because()
   {
      sut.SaveConfiguration();
   }

   [Observation]
   public void should_add_the_selection_to_the_simulation()
   {
      A.CallTo(() => _simulation.AddOverwriteParameterSetSelection("Aspirin", _userSelectedSet)).MustHaveHappened();
   }
}

public class When_saving_a_none_selection : concern_for_SimulationCompoundOverwriteParameterSetSelectionPresenter
{
   private OverwriteParameterSet _existingSet;
   private SimulationCompoundOverwriteParameterSetSelectionDTO _capturedDto;

   protected override void Context()
   {
      base.Context();
      _existingSet = new OverwriteParameterSet { Name = "RenalImpairment", IsDefault = true };
      _compound.AddOverwriteParameterSet(_existingSet);
      A.CallTo(() => _view.BindTo(A<SimulationCompoundOverwriteParameterSetSelectionDTO>._))
         .Invokes(call => _capturedDto = call.GetArgument<SimulationCompoundOverwriteParameterSetSelectionDTO>(0));
      sut.EditSimulation(_simulation, _compound);
      //user clears to "None"
      _capturedDto.SelectedOverwriteParameterSet = _capturedDto.AllOverwriteParameterSets.First(x => x.Name == PKSimConstants.UI.None);
   }

   protected override void Because()
   {
      sut.SaveConfiguration();
   }

   [Observation]
   public void should_remove_the_selection_from_the_simulation()
   {
      A.CallTo(() => _simulation.RemoveOverwriteParameterSetSelection("Aspirin")).MustHaveHappened();
   }

   [Observation]
   public void should_not_add_a_selection_to_the_simulation()
   {
      A.CallTo(() => _simulation.AddOverwriteParameterSetSelection(A<string>._, A<OverwriteParameterSet>._)).MustNotHaveHappened();
   }
}