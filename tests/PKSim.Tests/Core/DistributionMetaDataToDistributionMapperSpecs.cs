using System;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_DistributionMetaDataToDistributionMapper : ContextSpecification<IDistributionMetaDataToDistributionMapper>
   {
      protected IDistributionMetaData _distributionMetaData;
      protected IDistribution _distribution;

      protected override void Context()
      {
         sut = new DistributionMetaDataToDistributionMapper();
         _distributionMetaData = new DistributionMetaData();
      }

      protected override void Because()
      {
         _distribution = sut.MapFrom(_distributionMetaData);
      }
   }

   
   public class When_converting_distribution_meta_data_for_a_normal_distribution : concern_for_DistributionMetaDataToDistributionMapper
   {
      protected override void Context()
      {
         base.Context();
         _distributionMetaData = new DistributionMetaData {Deviation = 1, Mean = 0, Distribution = DistributionTypes.Normal};
      }

      [Observation]
      public void should_return_a_normal_distribution_with_the_accurate_mean_and_deviation()
      {
         _distribution.ShouldBeAnInstanceOf<NormalDistribution>();
         _distribution.DowncastTo<NormalDistribution>().Mean.ShouldBeEqualTo(_distributionMetaData.Mean);
         _distribution.DowncastTo<NormalDistribution>().Deviation.ShouldBeEqualTo(_distributionMetaData.Deviation);
      }
   }

   
   public class When_converting_distribution_meta_data_for_a_log_normal_distribution : concern_for_DistributionMetaDataToDistributionMapper
   {
      protected override void Context()
      {
         base.Context();
         _distributionMetaData = new DistributionMetaData { Deviation = 1, Mean = 4, Distribution = DistributionTypes.LogNormal };
      }

      [Observation]
      public void should_return_a_log_normal_distribution_with_the_accurate_mean_and_deviation()
      {
         _distribution.ShouldBeAnInstanceOf<LogNormalDistribution>();
         _distribution.DowncastTo<LogNormalDistribution>().Mean.ShouldBeEqualTo(Math.Log(_distributionMetaData.Mean));
         _distribution.DowncastTo<LogNormalDistribution>().Deviation.ShouldBeEqualTo(_distributionMetaData.Deviation);
      }


   }

   
   public class When_converting_distribution_meta_data_for_a_uniform_distribution : concern_for_DistributionMetaDataToDistributionMapper
   {
      protected override void Context()
      {
         base.Context();
         _distributionMetaData = new DistributionMetaData { Deviation = 1, Mean = 4, Distribution = DistributionTypes.Uniform };
      }

      protected override void Because()
      {
      }

      [Observation]
      public void should_return_a_normal_distribution_with_deviation_set_to_0()
      {
         The.Action(() => { _distribution = sut.MapFrom(_distributionMetaData); }).ShouldThrowAn<Exception>();
      }
   }

   
   public class When_converting_distribution_meta_data_for_a_discrete_distribution : concern_for_DistributionMetaDataToDistributionMapper
   {
      protected override void Context()
      {
         base.Context();
         _distributionMetaData = new DistributionMetaData { Mean = 4, Distribution = DistributionTypes.Discrete };
      }

      [Observation]
      public void should_return_a_normal_distribution_with_deviation_set_to_0()
      {
         _distribution.ShouldBeAnInstanceOf<NormalDistribution>();
         _distribution.DowncastTo<NormalDistribution>().Mean.ShouldBeEqualTo(_distributionMetaData.Mean);
      }
   }
}	
