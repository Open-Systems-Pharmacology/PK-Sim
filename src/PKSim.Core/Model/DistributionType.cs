using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public static class DistributionTypes
   {
      private static readonly ICache<string, DistributionType> _allDistributionTypes = new Cache<string, DistributionType>(dist => dist.Id);

      public static DistributionType Normal = create(CoreConstants.Distribution.Normal, PKSimConstants.UI.Normal);
      public static DistributionType LogNormal = create(CoreConstants.Distribution.LogNormal, PKSimConstants.UI.LogNormal);
      public static DistributionType Uniform = create(CoreConstants.Distribution.Uniform, PKSimConstants.UI.Uniform);
      public static DistributionType Discrete = create(CoreConstants.Distribution.Discrete, PKSimConstants.UI.Discrete);
      public static DistributionType Unknown = create(CoreConstants.Distribution.Unknown, PKSimConstants.UI.Unknown);

      private static DistributionType create(string id, string displayName)
      {
         var distributionType = new DistributionType(id, displayName);
         _allDistributionTypes.Add(distributionType);
         return distributionType;
      }

      public static IEnumerable<DistributionType> All()
      {
         return _allDistributionTypes;
      }

      public static DistributionType ById(string distributionId)
      {
         return _allDistributionTypes[distributionId];
      }
   }

   public class DistributionType
   {
      /// <summary>
      ///    distribution id as defined in the PKSim Database
      /// </summary>
      public string Id { get; private set; }

      public string DisplayName { get; private set; }

      public DistributionType(string id, string displayName)
      {
         Id = id;
         DisplayName = displayName;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}