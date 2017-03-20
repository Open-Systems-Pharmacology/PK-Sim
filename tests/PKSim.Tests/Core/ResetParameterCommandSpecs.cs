using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Commands;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_ResetParameterCommand : ContextSpecification<ResetParameterCommand>
   {
      protected IParameter _parameterToReset;
      protected IExecutionContext _executionContext;
      protected double _originValue;
      private IDimension _dimension;

   
      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _dimension = A.Fake<IDimension>();
         _originValue = 10;
         var container = new Container();
         var oneParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         _parameterToReset = DomainHelperForSpecs.ConstantParameterWithValue(_originValue).WithId("tralala").WithDimension(_dimension);
         _parameterToReset.Formula = new ExplicitFormula("10");
         container.Add(oneParameter);
         container.Add(_parameterToReset);
         _parameterToReset.Formula.AddObjectPath(objectPathFactory.CreateRelativeFormulaUsablePath(_parameterToReset,oneParameter));
         A.CallTo(() => _executionContext.Get<IParameter>(_parameterToReset.Id)).Returns(_parameterToReset);
         A.CallTo(() => _executionContext.BuildingBlockContaining(_parameterToReset)).Returns(A.Fake<IPKSimBuildingBlock>());

         sut = new ResetParameterCommand(_parameterToReset);
      }
   }

   
   public class When_executing_the_reset_command : concern_for_ResetParameterCommand
   {
      protected override void Context()
      {
         base.Context();
         _parameterToReset.Value = 25;
      }

      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_reseted_the_parameter_value_to_its_original_value()
      {
         _parameterToReset.Value.ShouldBeEqualTo(_originValue);
      }

      [Observation]
      public void the_parameter_should_have_been_marked_as_fixed()
      {
         _parameterToReset.IsFixedValue.ShouldBeFalse();
      }
   }
}	