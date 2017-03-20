using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_SetParameterUnitCommand : ContextSpecification<IPKSimReversibleCommand>
   {
      protected IParameter _parameter;
      protected Unit _newUnit;
      protected Unit _oldUnit;
      private double _oldValueInGuiUnit;
      protected double _newValue;
      protected IExecutionContext _executionContext;
      protected double _oldValue;
      private IDimension _dimension;

      protected override void Context()
      {
         _dimension = A.Fake<IDimension>();
         _oldValue = 20;
         _parameter = new PKSimParameter()
            .WithFormula(new ExplicitFormula(_oldValue.ToString()))
            .WithId("tralala")
            .WithDimension(_dimension);

         _newUnit = A.Fake<Unit>();
         A.CallTo(() => _newUnit.Name).Returns("_newUnit");
         _oldUnit = A.Fake<Unit>();
         A.CallTo(() => _oldUnit.Name).Returns("_oldUnit");

         A.CallTo(() => _dimension.Unit(_newUnit.Name)).Returns(_newUnit);
         A.CallTo(() => _dimension.Unit(_oldUnit.Name)).Returns(_oldUnit);

         _parameter.DisplayUnit = _oldUnit;
         _executionContext = A.Fake<IExecutionContext>();
         A.CallTo(() => _executionContext.Get<IParameter>(_parameter.Id)).Returns(_parameter);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameter)).Returns(A.Fake<IPKSimBuildingBlock>());

         _oldValueInGuiUnit = 250;
         _newValue = 150;
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_oldUnit, _parameter.Value)).Returns(_oldValueInGuiUnit);
         A.CallTo(() => _dimension.UnitValueToBaseUnitValue(_newUnit, _oldValueInGuiUnit)).Returns(_newValue);

         A.CallTo(() => _dimension.UnitValueToBaseUnitValue(_oldUnit, _oldValueInGuiUnit)).Returns(_oldValue);
         A.CallTo(() => _dimension.BaseUnitValueToUnitValue(_newUnit, _newValue)).Returns(_oldValueInGuiUnit);
         sut = new SetParameterUnitCommand(_parameter, _newUnit);
      }
   }

   
   public class When_executing_the_set_parameter_unit_command : concern_for_SetParameterUnitCommand
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_set_the_new_user_unit_into_the_given_parameter()
      {
         _parameter.DisplayUnit.ShouldBeEqualTo(_newUnit);
      }

      [Observation]
      public void should_update_the_parameter_value()
      {
         _parameter.Value.ShouldBeEqualTo(_newValue);
      }
   }

   
   public class When_executing_the_set_parameter_unit_inverse_command : concern_for_SetParameterUnitCommand
   {
      private IReversibleCommand<IExecutionContext> _inverseCommand;

      protected override void Context()
      {
         base.Context();
         sut.Execute(_executionContext);
         sut.RestoreExecutionData(_executionContext);
         _inverseCommand = sut.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _inverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_restore_the_orginal_unit()
      {
         _parameter.DisplayUnit.ShouldBeEqualTo(_oldUnit);
      }

      [Observation]
      public void should_restore_the_orginal_value()
      {
         _parameter.Value.ShouldBeEqualTo(_oldValue);
      }
   }

   
   public class When_executing_the_set_parameter_unit_inverse_command_for_a_parameter_whose_value_was_using_the_formula : concern_for_SetParameterUnitCommand
   {
      private IReversibleCommand<IExecutionContext> _inverseCommand;

      protected override void Context()
      {
         base.Context();
         _parameter.IsFixedValue = false;
         sut.Execute(_executionContext);
         sut.RestoreExecutionData(_executionContext);
         _inverseCommand = sut.InverseCommand(_executionContext);
      }

      protected override void Because()
      {
         _inverseCommand.Execute(_executionContext);
      }

      [Observation]
      public void should_mark_the_parameter_as_using_the_origin_formula()
      {
         _parameter.IsFixedValue.ShouldBeFalse();
      }
   }
}