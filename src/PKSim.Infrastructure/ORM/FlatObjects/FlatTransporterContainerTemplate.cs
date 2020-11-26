using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatTransporterContainerTemplate
   {
      public string ProcessName { get; set; }
      public string Gene { get; set; }
      public string Species { get; set; }
      public string OrganName { get; set; }
      public string CompartmentName { get; set; }
      public TransportType TransportType { get; set; }
      public MembraneLocation MembraneLocation { get; set; }
      public TransportDirectionId TransportDirection { get; set; } = TransportDirectionId.None;
      public string MembraneLocationDisplayName { get; set; }
   }
}