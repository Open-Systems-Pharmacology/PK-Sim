using OSPSuite.Core.Domain;
using PKSim.Core;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatProcess : IWithName
   {
      public string Name { get; set; }
      public bool IsTemplate { get; set; }
      public string GroupName { get; set; }
      public string ProcessType { get; set; }
      public ProcessActionType ActionType { get; set; }
      public string CalculationMethod { get; set; }
      public string Rate { get; set; }
      public string KineticType { get; set; }
      public bool CreateProcessRateParameter { get; set; }

      public bool IsActiveTransport()
      {
         return ProcessType == CoreConstants.ProcessType.ACTIVE_TRANSPORT && ActionType == ProcessActionType.Transport;
      }

      public bool IsInteraction()
      {
         return ActionType == ProcessActionType.Interaction;
      }
   }
}