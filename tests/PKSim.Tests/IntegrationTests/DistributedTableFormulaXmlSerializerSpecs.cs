using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_distributed_table_formula : ContextForSerialization<DistributedTableFormula>
   {
      private DistributedTableFormula _distributedTableFormula;
      private DistributedTableFormula _deserializedFormula;

      protected override void Context()
      {
         base.Context();
         var formulaFactory = IoC.Resolve<IFormulaFactory>();
         _distributedTableFormula = formulaFactory.CreateDistributedTableFormula().WithName("TOTO");
         _distributedTableFormula.AddPoint(0, 1, new DistributionMetaData {Deviation = 0.5, Distribution = DistributionTypes.Normal, Mean = 1});
         _distributedTableFormula.AddPoint(2, 3, new DistributionMetaData {Deviation = 0.8, Distribution = DistributionTypes.LogNormal, Mean = 2});
      }

      protected override void Because()
      {
         _deserializedFormula = SerializeAndDeserialize(_distributedTableFormula);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_stream()
      {
         _deserializedFormula.ShouldNotBeNull();
      }

      [Observation]
      public void the_deserialize_table_should_have_the_same_points_as_the_original_formula()
      {
         _deserializedFormula.AllPoints().Count().ShouldBeEqualTo(2);

         _deserializedFormula.AllPoints().ElementAt(0).X.ShouldBeEqualTo(0);
         _deserializedFormula.AllPoints().ElementAt(0).Y.ShouldBeEqualTo(1);

         _deserializedFormula.AllPoints().ElementAt(1).X.ShouldBeEqualTo(2);
         _deserializedFormula.AllPoints().ElementAt(1).Y.ShouldBeEqualTo(3);
      }

      [Observation]
      public void the_deserialize_table_should_have_the_same_distribution_meta_data_as_the_original_formula()
      {
         _deserializedFormula.AllDistributionMetaData().Count().ShouldBeEqualTo(2);
         var distributionMetaData = _deserializedFormula.AllDistributionMetaData().ElementAt(0);
         distributionMetaData.Deviation.ShouldBeEqualTo(0.5);
         distributionMetaData.Distribution.ShouldBeEqualTo(DistributionTypes.Normal);
         distributionMetaData.Mean.ShouldBeEqualTo(1);

         distributionMetaData = _deserializedFormula.AllDistributionMetaData().ElementAt(1);
         distributionMetaData.Deviation.ShouldBeEqualTo(0.8);
         distributionMetaData.Distribution.ShouldBeEqualTo(DistributionTypes.LogNormal);
         distributionMetaData.Mean.ShouldBeEqualTo(2);
      }
   }
}