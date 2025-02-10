using OSPSuite.Core.Domain;

namespace PKSim.Core.Model.Extensions
{
   public static class MoleculeAmountExtensions
   {
      public static bool IsIndividualMolecule(this MoleculeAmount moleculeAmount)
      {
         return moleculeAmount.QuantityType is QuantityType.Enzyme or QuantityType.Transporter or QuantityType.OtherProtein;
      }
   }
}