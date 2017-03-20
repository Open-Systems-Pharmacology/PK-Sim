using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_DistributionMetaData : ContextSpecification<DistributionMetaData>
   {
   }

   
   public class When_creating_a_distribution_meta_data_from_a_distributed_parameter : concern_for_DistributionMetaData
   {
      private IDistributedParameter _distributedParameter;
      private DistributionMetaData _result;

      protected override void Context()
      {
         base.Context();
         _distributedParameter = DomainHelperForSpecs.NormalDistributedParameter();
         _result = DistributionMetaData.From(_distributedParameter);
      }

      [Observation]
      public void should_set_the_mean_value_from_the_distributed_parameter()
      {
         _result.Mean.ShouldBeEqualTo(_distributedParameter.MeanParameter.Value);
      }

      [Observation]
      public void should_set_the_deviation_value_from_the_distributed_parameter()
      {
         _result.Deviation.ShouldBeEqualTo(_distributedParameter.DeviationParameter.Value);
      }

      [Observation]
      public void should_set_the_distribution_type_from_the_distributed_parameter()
      {
         _result.Distribution.ShouldBeEqualTo(_distributedParameter.Formula.DistributionType());
      }
   }

   
   public class When_creating_a_distribution_meta_data_from_a_distribution_meta_data : concern_for_DistributionMetaData
   {
      private IDistributionMetaData _distributionMetaData;
      private DistributionMetaData _result;

      protected override void Context()
      {
         base.Context();
         _distributionMetaData = new DistributionMetaData {Deviation = 2, Distribution = DistributionTypes.Normal, Mean = 5};
         _result = DistributionMetaData.From(_distributionMetaData);
      }

      [Observation]
      public void should_set_the_mean_value_from_the_distributed_meta_data()
      {
         _result.Mean.ShouldBeEqualTo(_distributionMetaData.Mean);
      }

      [Observation]
      public void should_set_the_deviation_value_from_the_distributed_meta_data()
      {
         _result.Deviation.ShouldBeEqualTo(_distributionMetaData.Deviation);
      }

      [Observation]
      public void should_set_the_distribution_type_from_the_distributed_meta_data()
      {
         _result.Distribution.ShouldBeEqualTo(_distributionMetaData.Distribution);
      }
   }
}