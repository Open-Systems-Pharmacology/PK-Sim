using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_UserDefinedOntogeny : ContextSpecification<UserDefinedOntogeny>
   {
      private DistributedTableFormula _table;

      protected override void Context()
      {
         _table = new DistributedTableFormula();
         _table.AddPoint(1, 10, new DistributionMetaData {Mean = 10, Deviation = 100, Distribution = DistributionType.Normal});
         _table.AddPoint(2, 20, new DistributionMetaData {Mean = 20, Deviation = 200, Distribution = DistributionType.Normal});
         sut = new UserDefinedOntogeny {Table = _table};
      }
   }

   public class When_retrieving_the_ontogeny_factors_defined_in_a_user_defined_ontogeny : concern_for_UserDefinedOntogeny
   {
      [Observation]
      public void should_return_the_Y_values_defined_in_the_underlying_table()
      {
         sut.OntogenyFactors().ShouldOnlyContainInOrder(10, 20);
      }
   }

   public class When_retrieving_the_post_menstrual_age_defined_in_a_user_defined_ontogeny : concern_for_UserDefinedOntogeny
   {
      [Observation]
      public void should_return_the_X_values_defined_in_the_underlying_table()
      {
         sut.PostmenstrualAges().ShouldOnlyContainInOrder(1, 2);
      }
   }

   public class When_retrieving_the_deviation_defined_in_a_user_defined_ontogeny : concern_for_UserDefinedOntogeny
   {
      [Observation]
      public void should_return_the_deviation_values_defined_in_the_unerlying_table()
      {
         sut.Deviations().ShouldOnlyContainInOrder(100, 200);
      }
   }
}