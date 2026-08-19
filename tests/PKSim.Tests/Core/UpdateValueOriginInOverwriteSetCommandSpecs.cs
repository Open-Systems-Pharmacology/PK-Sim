using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core;

public abstract class concern_for_UpdateValueOriginInOverwriteSetCommand : ContextSpecification<UpdateValueOriginInOverwriteSetCommand>
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
      _existingPV.ValueOrigin.Source = ValueOriginSources.Unknown;
      _existingPV.ValueOrigin.Description = "Old description";
      _overwriteParameterSet.Add(_existingPV);
      _compound.AddOverwriteParameterSet(_overwriteParameterSet);

      A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_overwriteParameterSet.Id)).Returns(_overwriteParameterSet);

      sut = new UpdateValueOriginInOverwriteSetCommand(_overwriteParameterSet, _compound, _path,
         new ValueOrigin { Source = ValueOriginSources.Publication, Description = "New description" });
   }
}

public class When_executing_the_update_value_origin_in_overwrite_set_command : concern_for_UpdateValueOriginInOverwriteSetCommand
{
   protected override void Because()
   {
      sut.Execute(_executionContext);
   }

   [Observation]
   public void should_update_the_value_origin_of_the_parameter_value()
   {
      _existingPV.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Publication);
      _existingPV.ValueOrigin.Description.ShouldBeEqualTo("New description");
   }

   [Observation]
   public void should_publish_an_overwrite_parameter_set_changed_event()
   {
      A.CallTo(() => _executionContext.PublishEvent(A<OverwriteParameterSetChangedEvent>.That.Matches(x => x.Compound == _compound && x.OverwriteParameterSet == _overwriteParameterSet))).MustHaveHappened();
   }
}

public class When_undoing_an_update_value_origin_in_overwrite_set_command : concern_for_UpdateValueOriginInOverwriteSetCommand
{
   protected override void Because()
   {
      sut.ExecuteAndInvokeInverse(_executionContext);
   }

   [Observation]
   public void should_restore_the_previous_value_origin()
   {
      _existingPV.ValueOrigin.Source.ShouldBeEqualTo(ValueOriginSources.Unknown);
      _existingPV.ValueOrigin.Description.ShouldBeEqualTo("Old description");
   }
}
