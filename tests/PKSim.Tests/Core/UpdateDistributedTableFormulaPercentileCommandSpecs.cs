using System.Linq;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Commands;
using PKSim.Core.Mappers;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using static OSPSuite.Core.Domain.Formulas.DistributionType;

namespace PKSim.Core
{
   public abstract class concern_for_UpdateDistributedTableFormulaPercentileCommand : ContextSpecification<UpdateDistributedTableFormulaPercentileCommand>
   {
      private IParameter _tableParameter;
      protected DistributedTableFormula _distributedTableFormula;
      protected IExecutionContext _context;

      protected override void Context()
      {
         _tableParameter = DomainHelperForSpecs.ConstantParameterWithValue(5);
         _distributedTableFormula = new DistributedTableFormula();
         _tableParameter.Formula = _distributedTableFormula;
         _context = A.Fake<IExecutionContext>();
         A.CallTo(() => _context.Resolve<IDistributionMetaDataToDistributionMapper>()).Returns(new DistributionMetaDataToDistributionMapper());
         sut = new UpdateDistributedTableFormulaPercentileCommand(_tableParameter, 0.5);
      }
   }

   
   public class Updating_a_distributed_table_from_a_percentile : concern_for_UpdateDistributedTableFormulaPercentileCommand
   {
      private NormalDistribution _normalDistribution0;
      private NormalDistribution _normalDistribution1 ;

      protected override void Context()
      {
         base.Context();
         _distributedTableFormula.Percentile = 0.2;
         _normalDistribution0 = new NormalDistribution(1, 2);
         _distributedTableFormula.AddPoint(1, _normalDistribution0.CalculateValueFromPercentile(0.2), new DistributionMetaData { Mean = 1, Deviation = 2, Distribution = Normal });
         _normalDistribution1 = new NormalDistribution(2, 2.5);
         _distributedTableFormula.AddPoint(2, _normalDistribution1.CalculateValueFromPercentile(0.2), new DistributionMetaData { Mean = 2, Deviation = 2.5, Distribution = Normal });
      }

      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_have_set_the_percentile_in_the_table_formula()
      {
         _distributedTableFormula.Percentile.ShouldBeEqualTo(0.5);
      }

      [Observation]
      public void should_have_generated_new_values_in_the_table_reflecting_the_percentile_change_according_to_the_distribution_define_for_each_point_in_the_table()
      {
         _distributedTableFormula.AllPoints().ElementAt(0).Y.ShouldBeEqualTo(_normalDistribution0.CalculateValueFromPercentile(0.5));
         _distributedTableFormula.AllPoints().ElementAt(1).Y.ShouldBeEqualTo(_normalDistribution1.CalculateValueFromPercentile(0.5 ));
      }
   }
}	