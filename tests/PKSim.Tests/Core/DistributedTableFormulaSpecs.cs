using System.Linq;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using OSPSuite.BDDHelper;

namespace PKSim.Core
{
   public abstract class concern_for_DistributedTableFormula : ContextSpecification<DistributedTableFormula>
   {
      protected override void Context()
      {
         sut = new DistributedTableFormula();
      }
   }

   
   public class When_adding_points_to_the_distributed_table_formula : concern_for_DistributedTableFormula
   {
      private DistributionMetaData _meta1;
      private DistributionMetaData _meta2;

      protected override void Context()
      {
         base.Context();
         _meta1 = new DistributionMetaData {Mean = 1, Deviation = 2, Distribution = DistributionTypes.Normal};
         _meta2 = new DistributionMetaData {Mean = 3, Deviation = 4, Distribution = DistributionTypes.LogNormal};
      }
      protected override void Because()
      {
         sut.AddPoint(10,20, _meta1);
         sut.AddPoint(5,20, _meta2);
      }

      [Observation]
      public void should_add_the_distribution_meta_data_at_the_right_index()
      {
         sut.AllDistributionMetaData().ElementAt(0).ShouldBeEqualTo(_meta2);
         sut.AllDistributionMetaData().ElementAt(1).ShouldBeEqualTo(_meta1);
      }
   }

   
   public class When_adding_points_to_the_distributed_that_already_exists : concern_for_DistributedTableFormula
   {
      private DistributionMetaData _meta1;
      private DistributionMetaData _meta2;

      protected override void Context()
      {
         base.Context();
         _meta1 = new DistributionMetaData { Mean = 1, Deviation = 2, Distribution = DistributionTypes.Normal };
         _meta2 = new DistributionMetaData { Mean = 3, Deviation = 4, Distribution = DistributionTypes.LogNormal };
      }
      protected override void Because()
      {
         sut.AddPoint(10, 20, _meta1);
         sut.AddPoint(10, 20, _meta2);
      }

      [Observation]
      public void should_have_updated_the_distribution_meta_data()
      {
         sut.AllDistributionMetaData().Count().ShouldBeEqualTo(1);
         sut.AllDistributionMetaData().ElementAt(0).ShouldBeEqualTo(_meta2);
      }
   }

   
   public class When_removing_a_point_from_the_table : concern_for_DistributedTableFormula
   {
      private DistributionMetaData _meta1;

      protected override void Context()
      {
         base.Context();
         _meta1 = new DistributionMetaData { Mean = 1, Deviation = 2, Distribution = DistributionTypes.Normal };
         sut.AddPoint(10, 20, _meta1);
      }
      protected override void Because()
      {
         sut.RemovePoint(10,20);
      }

      [Observation]
      public void should_have_removed_the_corresponding_meta_data()
      {
         sut.AllDistributionMetaData().Count().ShouldBeEqualTo(0);
      }
   }
}	