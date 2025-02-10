using System;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Maths.Statistics;
using OSPSuite.Utility;
using DistributionType = OSPSuite.Core.Domain.Formulas.DistributionType;

namespace PKSim.Core.Mappers
{
   public interface IDistributionMetaDataToDistributionMapper : IMapper<IDistributionMetaData, IDistribution>
   {
   }

   public class DistributionMetaDataToDistributionMapper : IDistributionMetaDataToDistributionMapper
   {
      public IDistribution MapFrom(IDistributionMetaData distributionMetaData)
      {
         if (distributionMetaData.Distribution == DistributionType.Normal)
            return new NormalDistribution(distributionMetaData.Mean, distributionMetaData.Deviation);

         if (distributionMetaData.Distribution == DistributionType.LogNormal)
            return new LogNormalDistribution(Math.Log(distributionMetaData.Mean), distributionMetaData.Deviation);

         if (distributionMetaData.Distribution == DistributionType.Discrete)
            return new NormalDistribution(distributionMetaData.Mean, 0);

         throw new ArgumentException($"Cannot create distribution for meta data '{distributionMetaData.Distribution}'");
      }
   }
}