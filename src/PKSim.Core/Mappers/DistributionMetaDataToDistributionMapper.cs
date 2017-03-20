using System;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Utility;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IDistributionMetaDataToDistributionMapper : IMapper<IDistributionMetaData, IDistribution>
   {
   }

   public class DistributionMetaDataToDistributionMapper : IDistributionMetaDataToDistributionMapper
   {
      public IDistribution MapFrom(IDistributionMetaData distributionMetaData)
      {
         if (distributionMetaData.Distribution == DistributionTypes.Normal)
            return new NormalDistribution(distributionMetaData.Mean, distributionMetaData.Deviation);

         if (distributionMetaData.Distribution == DistributionTypes.LogNormal)
            return new LogNormalDistribution(Math.Log(distributionMetaData.Mean), distributionMetaData.Deviation);

         if (distributionMetaData.Distribution == DistributionTypes.Discrete)
            return new NormalDistribution(distributionMetaData.Mean, 0);

         throw new ArgumentException(string.Format("Cannot create distribution for meta data '{0}'", distributionMetaData.Distribution), "distributionMetaData");
      }
   }
}