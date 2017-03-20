using PKSim.Core.Model;
using OSPSuite.Core.Domain;

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
      public string MembraneLocationDisplayName { get; set; }
   }
}