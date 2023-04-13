using System.Linq;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class PKSimSpatialStructure : SpatialStructure
   {
      public Organism Organism => TopContainers.First().DowncastTo<Organism>();
   }
}