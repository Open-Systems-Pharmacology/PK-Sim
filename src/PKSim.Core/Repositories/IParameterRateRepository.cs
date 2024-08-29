using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IParameterRateRepository : IParameterMetaDataRepository<ParameterRateMetaData>
   {
      ParameterRateMetaData ParameterMetaDataFor(string containerPath, string parameterName, string calculationMethod);
   }
}