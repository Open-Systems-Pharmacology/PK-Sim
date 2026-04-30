using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using IBuildingBlockRepository = PKSim.Core.Repositories.IBuildingBlockRepository;

namespace PKSim.Core;

public abstract class concern_for_OverwriteParameterSetTask : ContextSpecification<OverwriteParameterSetTask>
{
   protected IExecutionContext _executionContext;
   protected IBuildingBlockRepository _buildingBlockRepository;
   protected ILazyLoadTask _lazyLoadTask;
   protected Compound _compound;
   protected OverwriteParameterSet _overwriteParameterSet;
   protected const string _path = "Organism|Aspirin|Lipophilicity";

   protected override void Context()
   {
      _executionContext = A.Fake<IExecutionContext>();
      _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
      _lazyLoadTask = A.Fake<ILazyLoadTask>();
      A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new List<Simulation>());

      _compound = new Compound { Name = "Aspirin", Id = "CompId" };
      _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet", Id = "SetId" };
      _overwriteParameterSet.Add(new ParameterValue { Path = _path.ToObjectPath(), Value = 1.0 });
      _compound.AddOverwriteParameterSet(_overwriteParameterSet);

      A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_overwriteParameterSet.Id)).Returns(_overwriteParameterSet);

      sut = new OverwriteParameterSetTask(_executionContext, _buildingBlockRepository, _lazyLoadTask);
   }
}

public class When_updating_a_parameter_value_through_the_task : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.UpdateParameterValue(_overwriteParameterSet, _compound, _path, 9.0);
   }

   [Observation]
   public void should_update_the_value_in_the_set()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).Value.ShouldBeEqualTo(9.0);
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_updating_a_parameter_value_with_the_same_value : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.UpdateParameterValue(_overwriteParameterSet, _compound, _path, 1.0);
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}

public class When_updating_a_parameter_value_for_a_path_not_in_the_set : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.UpdateParameterValue(_overwriteParameterSet, _compound, "Organism|Aspirin|Permeability", 9.0);
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}

public class When_removing_a_parameter_value_through_the_task : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.RemoveParameterValue(_overwriteParameterSet, _compound, _path);
   }

   [Observation]
   public void should_remove_the_parameter_from_the_set()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).ShouldBeNull();
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_removing_a_parameter_value_for_a_path_not_in_the_set : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.RemoveParameterValue(_overwriteParameterSet, _compound, "Organism|Aspirin|Permeability");
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}

public abstract class concern_for_OverwriteParameterSetTask_setting_default : concern_for_OverwriteParameterSetTask
{
   protected OverwriteParameterSet _otherSet;
   protected OverwriteParameterSet _previousDefault;

   protected override void Context()
   {
      base.Context();
      _previousDefault = new OverwriteParameterSet { Name = "PreviousDefault", Id = "PreviousDefaultId", IsDefault = true };
      _otherSet = new OverwriteParameterSet { Name = "Other", Id = "OtherId" };
      _compound.AddOverwriteParameterSet(_previousDefault);
      _compound.AddOverwriteParameterSet(_otherSet);

      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_previousDefault.Id)).Returns(_previousDefault);
      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_otherSet.Id)).Returns(_otherSet);
   }
}

public class When_setting_a_set_as_default_through_the_task : concern_for_OverwriteParameterSetTask_setting_default
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.SetIsDefault(_otherSet, _compound, true);
   }

   [Observation]
   public void should_set_is_default_on_the_target_set()
   {
      _otherSet.IsDefault.ShouldBeTrue();
   }

   [Observation]
   public void should_clear_is_default_on_the_previously_default_set()
   {
      _previousDefault.IsDefault.ShouldBeFalse();
   }

   [Observation]
   public void should_leave_the_originally_added_set_unchanged()
   {
      _overwriteParameterSet.IsDefault.ShouldBeFalse();
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_clearing_the_default_flag_through_the_task : concern_for_OverwriteParameterSetTask_setting_default
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.SetIsDefault(_previousDefault, _compound, false);
   }

   [Observation]
   public void should_clear_is_default_on_the_target_set()
   {
      _previousDefault.IsDefault.ShouldBeFalse();
   }

   [Observation]
   public void should_leave_other_sets_unchanged()
   {
      _otherSet.IsDefault.ShouldBeFalse();
      _overwriteParameterSet.IsDefault.ShouldBeFalse();
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_setting_a_set_as_default_that_is_already_default : concern_for_OverwriteParameterSetTask_setting_default
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.SetIsDefault(_previousDefault, _compound, true);
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }

   [Observation]
   public void should_leave_state_unchanged()
   {
      _previousDefault.IsDefault.ShouldBeTrue();
      _otherSet.IsDefault.ShouldBeFalse();
   }
}

public class When_clearing_the_default_flag_on_a_set_that_is_not_default : concern_for_OverwriteParameterSetTask_setting_default
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.SetIsDefault(_otherSet, _compound, false);
   }

   [Observation]
   public void should_return_an_empty_command()
   {
      _result.IsEmpty().ShouldBeTrue();
   }
}

public class When_removing_an_overwrite_parameter_set_not_used_in_any_simulation : concern_for_OverwriteParameterSetTask
{
   private ICommand _result;

   protected override void Because()
   {
      _result = sut.RemoveSet(_overwriteParameterSet, _compound);
   }

   [Observation]
   public void should_remove_the_set_from_the_compound()
   {
      _compound.OverwriteParameterSets.ShouldNotContain(_overwriteParameterSet);
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_removing_an_overwrite_parameter_set_used_by_a_simulation : concern_for_OverwriteParameterSetTask
{
   private IndividualSimulation _simulation;

   protected override void Context()
   {
      base.Context();
      _simulation = new IndividualSimulation { Name = "Sim1" };
      // Simulations hold clones of compounds, so the selection points to a clone of the set with a different id.
      var clonedSet = new OverwriteParameterSet { Name = _overwriteParameterSet.Name, Id = "ClonedSetId" };
      _simulation.OverwriteParameterSetSelections.SetSelectionForCompound(_compound.Name, clonedSet);
      A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new List<Simulation> { _simulation });
   }

   [Observation]
   public void should_throw_a_cannot_delete_exception_and_leave_the_set_on_the_compound()
   {
      The.Action(() => sut.RemoveSet(_overwriteParameterSet, _compound)).ShouldThrowAn<CannotDeleteOverwriteParameterSetException>();
      _compound.OverwriteParameterSets.ShouldContain(_overwriteParameterSet);
   }

   [Observation]
   public void should_lazy_load_the_simulation_before_inspecting_its_selections()
   {
      The.Action(() => sut.RemoveSet(_overwriteParameterSet, _compound)).ShouldThrowAn<CannotDeleteOverwriteParameterSetException>();
      A.CallTo(() => _lazyLoadTask.Load<Simulation>(_simulation)).MustHaveHappened();
   }
}

public class When_removing_an_overwrite_parameter_set_used_only_by_a_set_with_the_same_name_in_another_compound : concern_for_OverwriteParameterSetTask
{
   private IndividualSimulation _simulation;
   private ICommand _result;

   protected override void Context()
   {
      base.Context();
      _simulation = new IndividualSimulation { Name = "Sim1" };
      // Selection is for a different compound that happens to have a same-named set — must NOT block deletion.
      var otherCompoundSet = new OverwriteParameterSet { Name = _overwriteParameterSet.Name, Id = "OtherCompoundSetId" };
      _simulation.OverwriteParameterSetSelections.SetSelectionForCompound("OtherCompound", otherCompoundSet);
      A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new List<Simulation> { _simulation });
   }

   protected override void Because()
   {
      _result = sut.RemoveSet(_overwriteParameterSet, _compound);
   }

   [Observation]
   public void should_remove_the_set_from_the_compound()
   {
      _compound.OverwriteParameterSets.ShouldNotContain(_overwriteParameterSet);
   }

   [Observation]
   public void should_return_a_non_empty_command()
   {
      _result.IsEmpty().ShouldBeFalse();
   }
}

public class When_removing_an_overwrite_parameter_set_with_multiple_simulations_in_the_project : concern_for_OverwriteParameterSetTask
{
   private IndividualSimulation _userSimulation;
   private IndividualSimulation _otherSimulation;

   protected override void Context()
   {
      base.Context();
      _userSimulation = new IndividualSimulation { Name = "User" };
      // Use a clone of the set (different id, same name) to mirror how UsedBuildingBlock holds compounds.
      var clonedSet = new OverwriteParameterSet { Name = _overwriteParameterSet.Name, Id = "ClonedSetId" };
      _userSimulation.OverwriteParameterSetSelections.SetSelectionForCompound(_compound.Name, clonedSet);

      _otherSimulation = new IndividualSimulation { Name = "Other" };

      A.CallTo(() => _buildingBlockRepository.All<Simulation>()).Returns(new List<Simulation> { _userSimulation, _otherSimulation });
   }

   [Observation]
   public void should_throw_a_cannot_delete_exception()
   {
      The.Action(() => sut.RemoveSet(_overwriteParameterSet, _compound)).ShouldThrowAn<CannotDeleteOverwriteParameterSetException>();
   }
}
