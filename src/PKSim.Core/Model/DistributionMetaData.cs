using OSPSuite.Core.Domain;
using PKSim.Core.Model.Extensions;

namespace PKSim.Core.Model
{
   public interface IDistributionMetaData
   {
      double Mean { get; set; }
      double Deviation { get; set; }
      DistributionType Distribution { get; set; }
   }

   public class DistributionMetaData : IDistributionMetaData
   {
      public double Mean { get; set; }
      public double Deviation { get; set; }
      public DistributionType Distribution { get; set; }

      public static DistributionMetaData From(IDistributedParameter distributedParameter)
      {
         var metaData = new DistributionMetaData
         {
            Mean = distributedParameter.MeanParameter.Value,
            Distribution = distributedParameter.Formula.DistributionType()
         };

         if (distributedParameter.DeviationParameter != null)
            metaData.Deviation = distributedParameter.DeviationParameter.Value;

         return metaData;
      }

      public static DistributionMetaData From(IDistributionMetaData distributionMetaData)
      {
         return new DistributionMetaData
         {
            Mean = distributionMetaData.Mean,
            Deviation = distributionMetaData.Deviation,
            Distribution = distributionMetaData.Distribution
         };
      }

      public DistributionMetaData Clone()
      {
         return From(this);
      }
   }
}