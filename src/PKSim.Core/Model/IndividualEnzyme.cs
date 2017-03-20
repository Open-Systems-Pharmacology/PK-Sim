using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class IndividualEnzyme : IndividualProtein
   {
      public IndividualEnzyme()
      {
         MoleculeType = QuantityType.Enzyme;
      }
   }
}