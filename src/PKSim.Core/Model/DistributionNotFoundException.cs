using System;
using OSPSuite.Core.Domain;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class DistributionNotFoundException : Exception
   {
      public DistributionNotFoundException(IEntity entity, string originDataReport) :
         base(PKSimConstants.Error.DistributionNotFound(entity.Name, originDataReport))
      {
      }

      public DistributionNotFoundException(IDistributionMetaData distributionMetaData) :
         base(PKSimConstants.Error.DistributionUnknown(distributionMetaData.Distribution.Id))
      {
      }
   }
}