using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using CoreDistributionType = OSPSuite.Core.Domain.Formulas.DistributionType;

namespace PKSim.Core.Model
{
   public static class DistributionTypes
   {
      private static readonly ICache<CoreDistributionType, DistributionType> _allDistributionTypes = new Cache<CoreDistributionType, DistributionType>(dist => dist.Id);

      public static DistributionType Normal = create(CoreDistributionType.Normal, PKSimConstants.UI.Normal);
      public static DistributionType LogNormal = create(CoreDistributionType.LogNormal, PKSimConstants.UI.LogNormal);
      public static DistributionType Uniform = create(CoreDistributionType.Uniform, PKSimConstants.UI.Uniform);
      public static DistributionType Discrete = create(CoreDistributionType.Discrete, PKSimConstants.UI.Discrete);
      public static DistributionType Unknown = create(CoreDistributionType.Unknown, PKSimConstants.UI.Unknown);

      private static DistributionType create(CoreDistributionType id, string displayName)
      {
         var distributionType = new DistributionType(id, displayName);
         _allDistributionTypes.Add(distributionType);
         return distributionType;
      }

      public static IEnumerable<DistributionType> All()
      {
         return _allDistributionTypes;
      }

      public static DistributionType ById(CoreDistributionType distributionType)
      {
         return _allDistributionTypes[distributionType];
      }
   }

   public class DistributionType
   {
      /// <summary>
      ///    distribution id as defined in the PKSim Database
      /// </summary>
      public CoreDistributionType Id { get; }

      public string DisplayName { get; }

      public DistributionType(CoreDistributionType id, string displayName)
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