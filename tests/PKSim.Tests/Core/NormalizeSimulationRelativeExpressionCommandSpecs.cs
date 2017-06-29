using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_NormalizeSimulationRelativeExpressionCommand : ContextSpecification<NormalizeSimulationRelativeExpressionCommand>
   {
      protected IParameter _relExp1;
      protected IParameter _relExp2;
      protected IExecutionContext _context;
      private IParameterTask _parameterTask;
      private ICache<IParameter, IParameter> _normParameters;
      protected IParameter _relExpNorm1;
      protected IParameter _relExpNorm2;

      protected override void Context()
      {
         _relExp1 = DomainHelperForSpecs.ConstantParameterWithValue(10);
         _relExp1.Info.GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION;
         _relExp1.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _relExp1.Name = CoreConstants.Parameter.RelExp + "1";

         _relExp2 = DomainHelperForSpecs.ConstantParameterWithValue(20);
         _relExp2.Info.GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION;
         _relExp2.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _relExp2.Name = CoreConstants.Parameter.RelExp + "2";

         _context = A.Fake<IExecutionContext>();
         var container = new Container();
         container.Add(_relExp1);
         container.Add(_relExp2);

         _normParameters = new Cache<IParameter, IParameter>();
         _relExpNorm1 = DomainHelperForSpecs.ConstantParameterWithValue(0.5);
         _normParameters.Add(_relExp1,_relExpNorm1);
         _relExpNorm2 = DomainHelperForSpecs.ConstantParameterWithValue(1);
         _normParameters.Add(_relExp2,_relExpNorm2);

         _parameterTask =A.Fake<IParameterTask>();
         A.CallTo(() => _parameterTask.GroupExpressionParameters(A<IReadOnlyList<IParameter>>.Ignored)).Returns(_normParameters);
         A.CallTo(() => _context.Resolve<IParameterTask>()).Returns(_parameterTask);
         sut = new NormalizeSimulationRelativeExpressionCommand(_relExp1,_context);
      }
   }

   
   public class When_normalizing_the_individual_expression_in_a_simulation_to_zero : concern_for_NormalizeSimulationRelativeExpressionCommand
   {
      protected override void Context()
      {
         base.Context();
         _relExp1.Value = 0;
         _relExp2.Value = 0;
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_have_set_all_normed_value_to_0_if_all_rel_exp_are_0()
      {
         _relExpNorm1.Value.ShouldBeEqualTo(0);         
         _relExpNorm2.Value.ShouldBeEqualTo(0);         
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
         _relExpNorm1.Value.ShouldBeEqualTo(1);
         _relExpNorm2.Value.ShouldBeEqualTo(0.5);
      }
   }
}	