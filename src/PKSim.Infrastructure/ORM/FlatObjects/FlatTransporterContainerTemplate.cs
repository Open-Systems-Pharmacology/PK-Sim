using PKSim.Core.Snapshots.Services;

namespace PKSim.Infrastructure.ORM.FlatObjects
{
   public class FlatTransporterContainerTemplate : FlatTransporterTemplate
   {
      public string ContainerName { get; set; }
      public MembraneLocation MembraneLocation { get; set; }
   }
}