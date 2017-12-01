using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public enum SystemicProcessTypeId
   {
      Hepatic,
      Renal,
      Biliary,
      GFR
   }

   public static class SystemicProcessTypes
   {
      private static readonly Cache<SystemicProcessTypeId, SystemicProcessType> _allApplicationTypes = new Cache<SystemicProcessTypeId, SystemicProcessType>(proc => proc.SystemicProcessTypeId);

      public static SystemicProcessType Hepatic = create(SystemicProcessTypeId.Hepatic, CoreConstants.Organ.Liver, PKSimConstants.UI.TotalHepaticClearance);
      public static SystemicProcessType Renal = create(SystemicProcessTypeId.Renal, CoreConstants.Organ.Kidney, PKSimConstants.UI.RenalClearance);
      public static SystemicProcessType Biliary = create(SystemicProcessTypeId.Biliary, CoreConstants.Organ.Gallbladder, PKSimConstants.UI.BiliaryClearance);
      public static SystemicProcessType GFR = create(SystemicProcessTypeId.GFR, CoreConstants.Organ.Kidney, PKSimConstants.UI.GlomerularFiltration);

      public static SystemicProcessType ById(SystemicProcessTypeId systemicProcessTypeId)
      {
         return _allApplicationTypes[systemicProcessTypeId];
      }

      public static SystemicProcessType ById(string systemicProcessTypeId)
      {
         return _allApplicationTypes[EnumHelper.ParseValue<SystemicProcessTypeId>(systemicProcessTypeId)];
      }

      private static SystemicProcessType create(SystemicProcessTypeId systemicProcessTypeId, string iconName, string displayName)
      {
         var systemicProcessType = new SystemicProcessType(systemicProcessTypeId, iconName, displayName);
         _allApplicationTypes.Add(systemicProcessType);
         return systemicProcessType;
      }
   }

   public class SystemicProcessType
   {
      /// <summary>
      ///    Systemic process id
      /// </summary>
      public SystemicProcessTypeId SystemicProcessTypeId { get; private set; }

      /// <summary>
      ///    Name of icon used to represent the process
      /// </summary>
      public string IconName { get; private set; }

      public string DisplayName { get; private set; }

      public SystemicProcessType(SystemicProcessTypeId systemicProcessTypeId, string iconName, string displayName)
      {
         DisplayName = displayName;
         IconName = iconName;
         SystemicProcessTypeId = systemicProcessTypeId;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}