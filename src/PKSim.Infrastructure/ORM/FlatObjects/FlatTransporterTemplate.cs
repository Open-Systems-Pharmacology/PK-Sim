using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatTransporterTemplate
   {
      public string Gene { get; set; }
      public string Species { get; set; }
      public TransportType TransportType { get; set; }
   }
}