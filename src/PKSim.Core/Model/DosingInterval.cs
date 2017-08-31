using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public enum DosingIntervalId
   {
      Single,
      DI_6_6_12,
      DI_6_6_6_6,
      DI_8_8_8,
      DI_12_12,
      DI_24,
   }

   public class DosingInterval
   {
      public DosingIntervalId Id { get; }
      public string DisplayName { get; }
      public int IntervalLength { get; }

      public DosingInterval(DosingIntervalId id, string displayName, int intervalLength)
      {
         Id = id;
         DisplayName = displayName;
         IntervalLength = intervalLength;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }

   public static class DosingIntervals
   {
      private static readonly ICache<DosingIntervalId, DosingInterval> _allDosingIntervals = new Cache<DosingIntervalId, DosingInterval>(dosingType => dosingType.Id);

      public static DosingInterval Single = create(DosingIntervalId.Single, PKSimConstants.UI.Single, 0);
      public static DosingInterval DI_12_12 = create(DosingIntervalId.DI_12_12, PKSimConstants.UI.DI_12_12, 720);
      public static DosingInterval DI_8_8_8 = create(DosingIntervalId.DI_8_8_8, PKSimConstants.UI.DI_8_8_8, 480);
      public static DosingInterval DI_24 = create(DosingIntervalId.DI_24, PKSimConstants.UI.DI_24, 1440);
      public static DosingInterval DI_6_6_6_6 = create(DosingIntervalId.DI_6_6_6_6, PKSimConstants.UI.DI_6_6_6_6, 360);
      public static DosingInterval DI_6_6_12 = create(DosingIntervalId.DI_6_6_12, PKSimConstants.UI.DI_6_6_12, 1440);

      public static DosingInterval ById(DosingIntervalId dosingIntervalId)
      {
         return _allDosingIntervals[dosingIntervalId];
      }

      private static DosingInterval create(DosingIntervalId id, string name, int intervalLength)
      {
         var dosingInterval = new DosingInterval(id, name, intervalLength);
         _allDosingIntervals.Add(dosingInterval);
         return dosingInterval;
      }

      public static IEnumerable<DosingInterval> All()
      {
         return _allDosingIntervals;
      }
   }
}