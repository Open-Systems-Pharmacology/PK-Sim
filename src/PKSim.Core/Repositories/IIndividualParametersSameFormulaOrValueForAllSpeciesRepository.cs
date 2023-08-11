using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IIndividualParametersSameFormulaOrValueForAllSpeciesRepository : IStartableRepository<IndividualParameterSameFormulaOrValueForAllSpecies>
   {
      bool IsSameFormula(ParameterMetaData parameterMetaData);
      bool IsSameValue(ParameterMetaData parameterMetaData);
      (bool IsSameFormula, bool IsSameValue) IsSameFormulaOrValue(ParameterMetaData parameterMetaData);
   }
}
