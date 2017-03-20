using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IMoleculeBuilderFactory
   {
      IMoleculeBuilder Create(QuantityType moleculeType, IFormulaCache formulaCache);
      IMoleculeBuilder Create(Compound compound, CompoundProperties compoundProperties, InteractionProperties interactionProperties, IFormulaCache formulaCache);
   }
}