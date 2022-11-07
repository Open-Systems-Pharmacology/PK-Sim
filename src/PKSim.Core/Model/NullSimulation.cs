using OSPSuite.Core.Domain.Data;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class NullSimulation : Simulation
   {
      public NullSimulation()
      {
         Name = PKSimConstants.UI.None;
      }

      public override bool HasResults
      {
         get { return false; }
      }

      public override TBuildingBlock BuildingBlock<TBuildingBlock>()
      {
         return default(TBuildingBlock);
      }
   }
}