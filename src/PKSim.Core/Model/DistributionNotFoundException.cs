using System;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class DistributionNotFoundException : Exception
   {
      public DistributionNotFoundException(IEntity entity, string originDataReport) :
         base(PKSimConstants.Error.DistributionNotFound.FormatWith(entity.Name, originDataReport))
      {
      }

      public DistributionNotFoundException(IDistributionMetaData distributionMetaData) :
         base(PKSimConstants.Error.DistributionUnknown.FormatWith(distributionMetaData.Distribution.Id))
      {
      }
   }
}