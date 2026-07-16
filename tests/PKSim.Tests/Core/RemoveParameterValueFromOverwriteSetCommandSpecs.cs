using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core;

public abstract class concern_for_RemoveParameterValueFromOverwriteSetCommand : ContextSpecification<RemoveParameterValueFromOverwriteSetCommand>
{
   protected Compound _compound;
   protected OverwriteParameterSet _overwriteParameterSet;
   protected IExecutionContext _executionContext;
   protected ParameterValue _existingPV;
   protected const string _path = "Organism|Aspirin|Lipophilicity";

   protected override void Context()
   {
      _executionContext = A.Fake<IExecutionContext>();
      _compound = new Compound { Name = "Aspirin", Id = "CompId" };
      _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet", Id = "SetId" };

      _existingPV = new ParameterValue { Path = _path.ToObjectPath(), Value = 1.0 };
      _overwriteParameterSet.Add(_existingPV);
      _compound.AddOverwriteParameterSet(_overwriteParameterSet);

      A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_overwriteParameterSet.Id)).Returns(_overwriteParameterSet);
   }
}

public class When_executing_the_remove_parameter_value_from_overwrite_set_command : concern_for_RemoveParameterValueFromOverwriteSetCommand
{
   protected override void Context()
   {
      base.Context();
      sut = new RemoveParameterValueFromOverwriteSetCommand(_overwriteParameterSet, _compound, _path);
   }

   protected override void Because()
   {
      sut.Execute(_executionContext);
   }

   [Observation]
   public void should_remove_the_parameter_from_the_set()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).ShouldBeNull();
      _overwriteParameterSet.ParameterValues.Count.ShouldBeEqualTo(0);
   }

   [Observation]
   public void should_publish_an_overwrite_parameter_set_changed_event()
   {
      A.CallTo(() => _executionContext.PublishEvent(A<OverwriteParameterSetChangedEvent>.That.Matches(x => x.Compound == _compound && x.OverwriteParameterSet == _overwriteParameterSet))).MustHaveHappened();
   }
}

public class When_undoing_a_remove_parameter_value_from_overwrite_set_command : concern_for_RemoveParameterValueFromOverwriteSetCommand
{
   protected override void Context()
   {
      base.Context();
      sut = new RemoveParameterValueFromOverwriteSetCommand(_overwriteParameterSet, _compound, _path);
   }

   protected override void Because()
   {
      sut.ExecuteAndInvokeInverse(_executionContext);
   }

   [Observation]
   public void should_restore_the_parameter_with_its_original_value()
   {
      var restored = _overwriteParameterSet.ParameterValueByPath(_path);
      restored.ShouldNotBeNull();
      restored.Value.ShouldBeEqualTo(1.0);
   }
}
