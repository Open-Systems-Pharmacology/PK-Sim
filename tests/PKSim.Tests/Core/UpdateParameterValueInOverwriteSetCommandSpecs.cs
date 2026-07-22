using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core;

public abstract class concern_for_UpdateParameterValueInOverwriteSetCommand : ContextSpecification<UpdateParameterValueInOverwriteSetCommand>
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

public class When_executing_the_update_parameter_value_in_overwrite_set_command : concern_for_UpdateParameterValueInOverwriteSetCommand
{
   protected override void Context()
   {
      base.Context();
      sut = new UpdateParameterValueInOverwriteSetCommand(_overwriteParameterSet, _compound, _path, 7.5);
   }

   protected override void Because()
   {
      sut.Execute(_executionContext);
   }

   [Observation]
   public void should_update_the_parameter_value_to_the_new_value()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).Value.ShouldBeEqualTo(7.5);
   }

   [Observation]
   public void should_publish_an_overwrite_parameter_set_changed_event()
   {
      A.CallTo(() => _executionContext.PublishEvent(A<OverwriteParameterSetChangedEvent>.That.Matches(x => x.Compound == _compound && x.OverwriteParameterSet == _overwriteParameterSet))).MustHaveHappened();
   }
}

public class When_undoing_an_update_parameter_value_in_overwrite_set_command : concern_for_UpdateParameterValueInOverwriteSetCommand
{
   protected override void Context()
   {
      base.Context();
      sut = new UpdateParameterValueInOverwriteSetCommand(_overwriteParameterSet, _compound, _path, 7.5);
   }

   protected override void Because()
   {
      sut.ExecuteAndInvokeInverse(_executionContext);
   }

   [Observation]
   public void should_restore_the_previous_parameter_value()
   {
      _overwriteParameterSet.ParameterValueByPath(_path).Value.ShouldBeEqualTo(1.0);
   }
}
