using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ScaleParametersCommand : ContextSpecification<ScaleParametersCommand>
   {
      protected List<IParameter> _parameterToScale;
      protected double _factor;
      protected IExecutionContext _executionContext;
      protected IDimension _dimension;
      protected IParameterTask _parameterTask;

      protected override void Context()
      {
         _parameterToScale = new List<IParameter>();
         _executionContext = A.Fake<IExecutionContext>();
         _parameterTask = A.Fake<IParameterTask>();
         _dimension= Constants.Dimension.NO_DIMENSION;
         _factor = 10;
         A.CallTo(() => _executionContext.Resolve<IParameterTask>()).Returns(_parameterTask);
         sut = new ScaleParametersCommand(_parameterToScale, _factor);
      }
   }

   public class When_scaling_a_set_of_parameters_with_a_factor : concern_for_ScaleParametersCommand
   {
      private IParameter _para1;
      private IParameter _para2;
      private readonly double _value1 = 1;
      private readonly double _value2 = 2;
      private readonly double _value3 = 3;
      private IParameter _para3;
      private IPKSimCommand _setCommand1;
      private IPKSimCommand _setCommand2;

      protected override void Context()
      {
         base.Context();
         _para1 = DomainHelperForSpecs.ConstantParameterWithValue(_value1).WithName("para1");
         _para1.Editable = true;
         _para2 = DomainHelperForSpecs.ConstantParameterWithValue(_value2).WithName("para2");
         _para2.Editable = true;
         _para3 = DomainHelperForSpecs.ConstantParameterWithValue(_value3).WithName("para3");
         _para3.Editable = false;
         A.CallTo(() => _executionContext.BuildingBlockContaining(_para1)).Returns(A.Fake<IPKSimBuildingBlock>());
         A.CallTo(() => _executionContext.BuildingBlockContaining(_para2)).Returns(A.Fake<IPKSimBuildingBlock>());
         _parameterToScale.Add(_para1);
         _parameterToScale.Add(_para2);
         _parameterToScale.Add(_para3);
         _setCommand1= A.Fake<IPKSimCommand>();
         _setCommand2 = A.Fake<IPKSimCommand>();

         A.CallTo(() => _parameterTask.SetParameterValue(_para1, _value1 * _factor, true)).Returns(_setCommand1);
         A.CallTo(() => _parameterTask.SetParameterValue(_para2, _value2 * _factor, true)).Returns(_setCommand2);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_create_one_scale_parameter_command_for_each_editable_parameter()
      {
         sut.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_scaled_the_parameters_with_the_given_factor()
      {
         sut.All().ShouldContain(_setCommand1, _setCommand2);
      }

      [Observation]
      public void should_have_let_the_not_editale_parmater_unchanged()
      {
         A.CallTo(() => _parameterTask.SetParameterValue(_para3, A<double>._, A<bool>._)).MustNotHaveHappened();
      }
   }

   public class When_scaling_a_parameter_that_is_a_boolean_or_whose_list_of_value_is_fix : concern_for_ScaleParametersCommand
   {
      private IParameter _para1;
      private IParameter _para2;
      private readonly double _value1 = 1;
      private readonly double _value2 = 2;
      private readonly double _value3 = 3;
      private IParameter _para3;
      private IPKSimCommand _setCommand;

      protected override void Context()
      {
         base.Context();
         _para1 = DomainHelperForSpecs.ConstantParameterWithValue(_value1).WithName(CoreConstants.Parameters.EHC_ENABLED);
         _para1.Editable = true;
         _para2 = DomainHelperForSpecs.ConstantParameterWithValue(_value2).WithName("para2");
         _para2.Editable = true;
         _para3 = DomainHelperForSpecs.ConstantParameterWithValue(_value3).WithName(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM);
         _para3.Editable = true;
         _parameterToScale.Add(_para1);
         _parameterToScale.Add(_para2);
         _parameterToScale.Add(_para3);
         _setCommand= A.Fake<IPKSimCommand>();
         A.CallTo(() => _parameterTask.SetParameterValue(_para2, _value2 * _factor, true)).Returns(_setCommand);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_not_scale_the_parameter()
      {
         sut.Count.ShouldBeEqualTo(1);
         sut.All().ShouldContain(_setCommand);
      }
   }
}