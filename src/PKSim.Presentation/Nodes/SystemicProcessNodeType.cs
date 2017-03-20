using System.Collections.Generic;
using PKSim.Core.Model;
using OSPSuite.Assets;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.Nodes
{
   public class SystemicProcessNodeType : RootNodeType
   {
      /// <summary>
      ///    Different SystemicProcessType that will be hosted under this node
      /// </summary>
      public IEnumerable<SystemicProcessType> SystemicTypes { get; private set; }

      public static readonly SystemicProcessNodeType HepaticClearance = new SystemicProcessNodeType(SystemicProcessTypes.Hepatic);

      //GFR Displayed under the renal clearance node
      public static readonly SystemicProcessNodeType RenalClearance = new SystemicProcessNodeType(new[] {SystemicProcessTypes.Renal, SystemicProcessTypes.GFR});
      public static readonly SystemicProcessNodeType BiliaryClearance = new SystemicProcessNodeType(SystemicProcessTypes.Biliary);

      private SystemicProcessNodeType(SystemicProcessType systemicProcessType) : this(new[] {systemicProcessType})
      {
      }

      private SystemicProcessNodeType(IReadOnlyList<SystemicProcessType> systemicProcessTypes)
         : base(systemicProcessTypes[0].DisplayName, ApplicationIcons.IconByName(systemicProcessTypes[0].IconName))
      {
         SystemicTypes = systemicProcessTypes;
      }
   }
}