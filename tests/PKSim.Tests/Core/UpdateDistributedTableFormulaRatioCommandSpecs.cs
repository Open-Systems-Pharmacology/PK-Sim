using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_UpdateDiscreteDistributedParameterTableFormulaCommand : ContextSpecification<UpdateDistributedTableFormulaRatioCommand>
   {
      private IParameter _tableParameter;
      protected DistributedTableFormula _distributedTableFormula;
      protected IExecutionContext _context;
      protected double _ratio;

      protected override void Context()
      {
         _tableParameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         _distributedTableFormula = new DistributedTableFormula();
         _tableParameter.Formula = _distributedTableFormula;
         _context = A.Fake<IExecutionContext>();
         _ratio = 2;
         sut = new UpdateDistributedTableFormulaRatioCommand(_tableParameter, _ratio);
      }
   }

   
   public class When_updating_the_value_of_a_distributed_parmaeter_table_using_the_ratio_command : concern_for_UpdateDiscreteDistributedParameterTableFormulaCommand
   {
      protected override void Context()
      {
         base.Context();
         _distributedTableFormula.Percentile = 0.2;
         _distributedTableFormula.AddPoint(1, 10, new DistributionMetaData { Mean = 1, Distribution = DistributionTypes.Discrete });
         _distributedTableFormula.AddPoint(2, 20, new DistributionMetaData { Mean = 2, Distribution = DistributionTypes.Discrete });

      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_scaled_the_value_with_the_ration_from_the_first_value_with_the_given_value()
      {
         _distributedTableFormula.ValueAt(1).ShouldBeEqualTo(10*_ratio);
         _distributedTableFormula.ValueAt(2).ShouldBeEqualTo(20*_ratio);
      }
   }
}	