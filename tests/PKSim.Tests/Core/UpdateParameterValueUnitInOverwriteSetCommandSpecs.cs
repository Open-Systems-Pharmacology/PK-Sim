using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Format;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core;

public abstract class concern_for_UpdateParameterValueUnitInOverwriteSetCommand : ContextSpecification<UpdateParameterValueUnitInOverwriteSetCommand>
{
   protected Compound _compound;
   protected OverwriteParameterSet _overwriteParameterSet;
   protected IExecutionContext _executionContext;
   protected ParameterValue _existingPV;
   protected IDimension _lengthDimension;
   protected const string _path = "Organism|Aspirin|Radius";

   protected override void Context()
   {
      _executionContext = A.Fake<IExecutionContext>();
      _compound = new Compound { Name = "Aspirin", Id = "CompId" };
      _overwriteParameterSet = new OverwriteParameterSet { Name = "MySet", Id = "SetId" };
      _lengthDimension = DomainHelperForSpecs.LengthDimensionForSpecs();

      _existingPV = new ParameterValue
      {
         Path = _path.ToObjectPath(),
         Value = 1.0,
         Dimension = _lengthDimension,
         DisplayUnit = _lengthDimension.Unit("cm")
      };
      _overwriteParameterSet.Add(_existingPV);
      _compound.AddOverwriteParameterSet(_overwriteParameterSet);

      A.CallTo(() => _executionContext.BuildingBlockContaining(_compound)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<Compound>(_compound.Id)).Returns(_compound);
      A.CallTo(() => _executionContext.Get<OverwriteParameterSet>(_overwriteParameterSet.Id)).Returns(_overwriteParameterSet);

      sut = new UpdateParameterValueUnitInOverwriteSetCommand(_overwriteParameterSet, _compound, _path, "mm");
   }
}

public class When_executing_the_update_parameter_value_unit_in_overwrite_set_command : concern_for_UpdateParameterValueUnitInOverwriteSetCommand
{
   protected override void Because()
   {
      sut.Execute(_executionContext);
   }

   [Observation]
   public void should_keep_the_displayed_number_and_recalculate_the_value_in_base_unit()
   {
      //1 m displayed as 100 cm becomes 100 mm, so the base unit value drops to 0.1 m
      _existingPV.DisplayUnit.Name.ShouldBeEqualTo("mm");
      _existingPV.ConvertToDisplayUnit(_existingPV.Value).ShouldBeEqualTo(100);
      _existingPV.Value.ShouldBeEqualTo(0.1);
   }

   [Observation]
   public void should_describe_the_change_as_a_value_change()
   {
      //the same number appears on both sides: the unit change is a value change, not a re-display
      var displayValue = new NumericFormatter<double>(NumericFormatterOptions.Instance).Format(100);
      sut.Description.Contains($"from '{displayValue} cm' to '{displayValue} mm'").ShouldBeTrue();
   }

   [Observation]
   public void should_publish_an_overwrite_parameter_set_changed_event()
   {
      A.CallTo(() => _executionContext.PublishEvent(A<OverwriteParameterSetChangedEvent>.That.Matches(x => x.Compound == _compound && x.OverwriteParameterSet == _overwriteParameterSet))).MustHaveHappened();
   }
}

public class When_undoing_an_update_parameter_value_unit_in_overwrite_set_command : concern_for_UpdateParameterValueUnitInOverwriteSetCommand
{
   protected override void Because()
   {
      sut.ExecuteAndInvokeInverse(_executionContext);
   }

   [Observation]
   public void should_restore_the_previous_display_unit_and_value()
   {
      _existingPV.DisplayUnit.Name.ShouldBeEqualTo("cm");
      _existingPV.Value.ShouldBeEqualTo(1.0);
   }
}
