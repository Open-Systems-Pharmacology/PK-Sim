using OSPSuite.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class SimulationPartialProcessDTO
   {
      public SimulationPartialProcessStatus Status { get; set; }
      public string ProcessId { get; set; }
      public string ProcessDisplayName { get; set; }
      public ApplicationIcon Icon { get; set; }
      public string Message { get; set; }

      public override string ToString()
      {
         return ProcessDisplayName;
      }

      public override bool Equals(object obj)
      {
         return Equals(obj as SimulationPartialProcessDTO);
      }

      public bool Equals(SimulationPartialProcessDTO other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Equals(other.ProcessId, ProcessId);
      }

      public override int GetHashCode()
      {
         return (ProcessId != null ? ProcessId.GetHashCode() : 0);
      }
   }
}