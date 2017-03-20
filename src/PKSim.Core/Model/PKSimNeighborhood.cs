using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IPKSimNeighborhood : INeighborhood
   {
      bool Visible { get; set; }
   }

   public class PKSimNeighborhood : Neighborhood, IPKSimNeighborhood
   {
      public bool Visible { get; set; }
   }
}