using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public interface IPKSimSpatialStructure : ISpatialStructure
   {
      Organism Organism { get; }
   }

   public class PKSimSpatialStructure : SpatialStructure, IPKSimSpatialStructure
   {
      public Organism Organism
      {
         get { return TopContainers.First().DowncastTo<Organism>(); }
      }
   }
}