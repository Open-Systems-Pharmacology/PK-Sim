using System;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISystemicProcessToRootNodeTypeMapper : IMapper<SystemicProcessType, RootNodeType>
   {

   }

   public class SystemicProcessToRootNodeTypeMapper : ISystemicProcessToRootNodeTypeMapper
   {
      public RootNodeType MapFrom(SystemicProcessType systemicProcessType)
      {
         if (systemicProcessType == SystemicProcessTypes.Biliary)
            return SystemicProcessNodeType.BiliaryClearance;

         if (systemicProcessType == SystemicProcessTypes.Hepatic)
            return SystemicProcessNodeType.HepaticClearance;

         if(systemicProcessType == SystemicProcessTypes.Renal || systemicProcessType == SystemicProcessTypes.GFR)
            return SystemicProcessNodeType.RenalClearance;

         throw new ArgumentException(string.Format("The systemic process '{0}' is not recognized", systemicProcessType));
      }
   }
}
