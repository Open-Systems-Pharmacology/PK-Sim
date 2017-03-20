using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IMoleculeStartFormulaRepository : IStartableRepository<IMoleculeStartFormula>
   {
      /// <summary>
      ///    Calculation method and rate for given molecule path.
      ///    <para></para>
      ///    Calculation method must be part of <paramref name="modelProperties" />
      ///    <para></para>
      ///    <paramref name="moleculePath" /> must be a database path to the molecule (e.g. "ORGANISM\Liver\Plasma\DRUG")
      /// </summary>
      /// <returns>Rate key if found, null if not found</returns>
      RateKey RateKeyFor(IObjectPath moleculePath, ModelProperties modelProperties);
   }
}