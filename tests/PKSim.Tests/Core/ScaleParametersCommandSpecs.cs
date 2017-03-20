using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core
{
   public abstract class concern_for_ScaleParametersCommand : ContextSpecification<ScaleParametersCommand>
   {
      protected List<IParameter> _parameterToScale;
      protected double _factor;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _parameterToScale = new List<IParameter>();
         _executionContext = A.Fake<IExecutionContext>();
         _factor = 10;
         sut = new ScaleParametersCommand(_parameterToScale, _factor);
      }
   }

   public class When_scaling_a_set_of_parameters_with_a_factor : concern_for_ScaleParametersCommand
   {
      private IParameter _para1;
      private IParameter _para2;
      private double _value1 = 1;
      private double _value2 = 2;
      private double _value3 = 3;
      private IParameter _para3;

      protected override void Context()
      {
         base.Context();
         _para1 = DomainHelperForSpecs.ConstantParameterWithValue(_value1).WithName("para1").WithDimension(A.Fake<IDimension>());
         _para1.Editable = true;
         _para2 = DomainHelperForSpecs.ConstantParameterWithValue(_value2).WithName("para2").WithDimension(A.Fake<IDimension>());
         _para2.Editable = true;
         _para3 = DomainHelperForSpecs.ConstantParameterWithValue(_value3).WithName("para3").WithDimension(A.Fake<IDimension>());
         _para3.Editable = false;
         A.CallTo(() => _executionContext.BuildingBlockContaining(_para1)).Returns(A.Fake<IPKSimBuildingBlock>());
         A.CallTo(() => _executionContext.BuildingBlockContaining(_para2)).Returns(A.Fake<IPKSimBuildingBlock>());
         _parameterToScale.Add(_para1);
         _parameterToScale.Add(_para2);
         _parameterToScale.Add(_para3);
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
         _para1.Value.ShouldBeEqualTo(_value1 * _factor);
         _para2.Value.ShouldBeEqualTo(_value2 * _factor);
      }

      [Observation]
      public void should_have_let_the_not_editale_parmater_unchanged()
      {
         _para3.Value.ShouldBeEqualTo(_value3);
      }
   }

   public class When_scaling_a_parameter_that_is_a_boolean_or_whose_list_of_value_is_fix : concern_for_ScaleParametersCommand
   {
      private IParameter _para1;
      private IParameter _para2;
      private double _value1 = 1;
      private double _value2 = 2;
      private double _value3 = 3;
      private IParameter _para3;

      protected override void Context()
      {
         base.Context();
         _para1 = DomainHelperForSpecs.ConstantParameterWithValue(_value1).WithName(CoreConstants.Parameter.EHC_ENABLED).WithDimension(A.Fake<IDimension>());
         _para1.Editable = true;
         _para2 = DomainHelperForSpecs.ConstantParameterWithValue(_value2).WithName("para2").WithDimension(A.Fake<IDimension>());
         _para2.Editable = true;
         _para3 = DomainHelperForSpecs.ConstantParameterWithValue(_value3).WithName(CoreConstants.Parameter.PARTICLE_DISPERSE_SYSTEM).WithDimension(A.Fake<IDimension>());
         _para3.Editable = true;
         A.CallTo(_executionContext).WithReturnType<IPKSimBuildingBlock>().Returns(A.Fake<IPKSimBuildingBlock>());
         _parameterToScale.Add(_para1);
         _parameterToScale.Add(_para2);
         _parameterToScale.Add(_para3);
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }
       
      [Observation]
      public void should_not_scale_the_parameter()
      {
         sut.Count.ShouldBeEqualTo(1);
      }
   }
}