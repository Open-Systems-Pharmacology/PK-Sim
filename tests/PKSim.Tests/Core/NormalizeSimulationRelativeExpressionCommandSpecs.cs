using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Core
{
   public abstract class concern_for_NormalizeSimulationRelativeExpressionCommand : ContextSpecification<NormalizeSimulationRelativeExpressionCommand>
   {
      protected IParameter _relExp1;
      protected IParameter _relExp2;
      protected IExecutionContext _context;
      private IParameterTask _parameterTask;
      private IReadOnlyList<IParameter> _relExpParameters;

      protected override void Context()
      {
         _relExp1 = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _relExp1.Info.GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION;
         _relExp1.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _relExp1.Name = CoreConstants.Parameters.REL_EXP + "1";

         _relExp2 = DomainHelperForSpecs.ConstantParameterWithValue(20);
         _relExp2.Info.GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION;
         _relExp2.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _relExp2.Name = CoreConstants.Parameters.REL_EXP + "2";

         _context = A.Fake<IExecutionContext>();
         var container = new Container {_relExp1, _relExp2};

         _relExpParameters = new List<IParameter> {_relExp1, _relExp2};

         _parameterTask =A.Fake<IParameterTask>();
         A.CallTo(() => _parameterTask.GroupExpressionParameters(A<IReadOnlyList<IParameter>>.Ignored)).Returns(_relExpParameters);
         A.CallTo(() => _context.Resolve<IParameterTask>()).Returns(_parameterTask);
         sut = new NormalizeSimulationRelativeExpressionCommand(_relExp1,_context);
      }
   }


   public class When_normalizing_the_individual_expression_in_a_simulation : concern_for_NormalizeSimulationRelativeExpressionCommand
   {
      protected override void Context()
      {
         base.Context();
         _relExp1.Value = 40;
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_have_normalized_the_value_accordingly()
      {
         //TODO ZTMSE
//         _relExpNorm1.Value.ShouldBeEqualTo(1);
  //       _relExpNorm2.Value.ShouldBeEqualTo(0.5);
      }
   }
}	