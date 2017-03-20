using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.Extensions
{
   public static class MoleculeAmountExtensions
   {
      public static bool IsIndividualMolecule(this IMoleculeAmount moleculeAmount)
      {
         return moleculeAmount.QuantityType == QuantityType.Enzyme ||
                moleculeAmount.QuantityType == QuantityType.Transporter ||
                moleculeAmount.QuantityType == QuantityType.OtherProtein;
      }
   }
}