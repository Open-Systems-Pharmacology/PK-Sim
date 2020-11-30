using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Core.Snapshots
{
   public class ExpressionContainer : Parameter
   {
      public MembraneLocation? MembraneLocation { get; set; }
      public TransportDirectionId? TransportDirection { get; set; }
      public string CompartmentName { get; set; }
   }
}