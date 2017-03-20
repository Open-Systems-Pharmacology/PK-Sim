using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_ResetParametersCommand : ContextSpecification<ResetParametersCommand>
   {
      protected IList<IParameter> _parameterToReset;

      protected override void Context()
      {
         _parameterToReset = new List<IParameter>();
         sut = new ResetParametersCommand(_parameterToReset);
      }
   }

   
   public class When_reseting_a_list_of_parameters : concern_for_ResetParametersCommand
   {
      private IParameter _paraWithValueUnchanged1;
      private IParameter _paraWithValueChanged2;
      private IParameter _paraWithValueUnchanged3;
      private IExecutionContext _executionContext;
      private IDistributedParameter _distributedParameter;

      protected override void Context()
      {
         base.Context();
         _executionContext = A.Fake<IExecutionContext>();
         var container = new Container();
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         _paraWithValueUnchanged1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("para1").WithDimension(A.Fake<IDimension>());
         _paraWithValueUnchanged1.IsFixedValue = false;
         _paraWithValueChanged2 = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("para2").WithDimension(A.Fake<IDimension>());
         _paraWithValueChanged2.Id = "tralala";
         _paraWithValueChanged2.Formula = new ExplicitFormula("15");
         _paraWithValueChanged2.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_paraWithValueChanged2, _paraWithValueUnchanged1));
         _paraWithValueChanged2.Value = 20;
         _paraWithValueUnchanged3 = DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("para3");
         _distributedParameter = DomainHelperForSpecs.NormalDistributedParameter().WithName("para4");
         _distributedParameter.Value = 1.2;
         _distributedParameter.IsFixedValue = false;
         _paraWithValueUnchanged3.IsFixedValue = false;
         container.Add(_paraWithValueChanged2);
         container.Add(_paraWithValueUnchanged1);
         container.Add(_paraWithValueUnchanged3);
         container.Add(_distributedParameter);
         _parameterToReset.Add(_paraWithValueUnchanged1);
         _parameterToReset.Add(_paraWithValueChanged2);
         _parameterToReset.Add(_paraWithValueUnchanged3);
         _parameterToReset.Add(_distributedParameter);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_paraWithValueChanged2)).Returns(A.Fake<IPKSimBuildingBlock>());
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_only_create_a_reset_command_for_those_parameters_whose_values_was_set_by_the_user()
      {
         sut.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_not_create_a_reset_command_for_the_distributed_parameter_even_if_the_value_was_set_by_the_user()
      {
         sut.All().ElementAt(0).DowncastTo<ResetParameterCommand>().ParameterId.ShouldBeEqualTo(_paraWithValueChanged2.Id);
      }
   }
}