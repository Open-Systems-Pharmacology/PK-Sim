using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Model
{
   public interface IMoleculeBuilderFactory
   {
      MoleculeBuilder Create(QuantityType moleculeType, IFormulaCache formulaCache);
      MoleculeBuilder Create(Compound compound, CompoundProperties compoundProperties, InteractionProperties interactionProperties, IFormulaCache formulaCache);
   }
}