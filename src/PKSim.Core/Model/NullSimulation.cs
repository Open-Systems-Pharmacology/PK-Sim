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

      public override DataColumn FabsOral(string compoundName)
      {
         throw new System.NotImplementedException();
      }

      public override DataColumn PeripheralVenousBloodColumn(string compoundName)
      {
         throw new System.NotImplementedException();
      }

      public override DataColumn VenousBloodColumn(string compoundName)
      {
         throw new System.NotImplementedException();
      }
   }
}