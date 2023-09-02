using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IIndividualParametersSameFormulaOrValueForAllSpeciesRepository : IStartableRepository<IndividualParameterSameFormulaOrValueForAllSpecies>
   {
      bool IsSameFormula(ParameterMetaData parameterMetaData);
      bool IsSameValue(ParameterMetaData parameterMetaData);
      (bool isSame, bool exists) IsSameFormulaOrValue(ParameterMetaData parameterMetaData);
      bool IsSameFormulaOrValue(IParameter parameter);
   }
}
