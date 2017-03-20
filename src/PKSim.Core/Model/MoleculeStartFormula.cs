using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Start formula of a molecule amount in container
   /// </summary>
   public interface IMoleculeStartFormula
   {
      /// <summary>
      ///    Database path to the molecule, e.g. "ORGANISM\Liver\Plasma\DRUG"
      /// </summary>
      IObjectPath MoleculePath { get; }

      /// <summary>
      ///    Calculation method and rate for given formula
      /// </summary>
      RateKey RateKey { get; }
   }

   public class MoleculeStartFormula : IMoleculeStartFormula
   {
      public MoleculeStartFormula(IObjectPath moleculePath, string calculationMethod, string rate)
      {
         MoleculePath = moleculePath;
         RateKey = new RateKey(calculationMethod, rate);
      }

      public IObjectPath MoleculePath { get; private set; }
      public RateKey RateKey { get; private set; }
   }
}