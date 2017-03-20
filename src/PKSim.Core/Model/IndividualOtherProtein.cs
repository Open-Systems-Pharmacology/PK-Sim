using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class IndividualOtherProtein : IndividualProtein
   {
      public IndividualOtherProtein()
      {
         MoleculeType = QuantityType.OtherProtein;
      }
   }
}