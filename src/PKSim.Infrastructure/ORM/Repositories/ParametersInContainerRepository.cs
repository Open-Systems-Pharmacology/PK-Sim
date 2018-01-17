using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ParametersInContainerRepository : IParametersInContainerRepository
   {
      private readonly IParameterValueRepository _parameterValueRepository;
      private readonly IParameterDistributionRepository _parameterDistributionRepository;
      private readonly IParameterRateRepository _parameterRateRepository;
      private readonly IEntityPathResolver _entityPathResolver;

      public ParametersInContainerRepository(
         IParameterValueRepository parameterValueRepository,
         IParameterDistributionRepository parameterDistributionRepository,
         IParameterRateRepository parameterRateRepository,
         IEntityPathResolver entityPathResolver)
      {
         _parameterValueRepository = parameterValueRepository;
         _parameterDistributionRepository = parameterDistributionRepository;
         _parameterRateRepository = parameterRateRepository;
         _entityPathResolver = entityPathResolver;
      }

      public ParameterMetaData ParameterMetaDataFor(IParameter parameter)
      {
         if (parameter == null)
            return null;

         if (parameter.IsDistributed())
            return parameterMetaDataFrom(_parameterDistributionRepository, parameter);

         if (!parameter.Formula.IsConstant())
            return parameterMetaDataFrom(_parameterRateRepository, parameter);

         return parameterMetaDataFrom(_parameterValueRepository, parameter);
      }

      private ParameterMetaData parameterMetaDataFrom<TParameterMetaData>(IParameterMetaDataRepository<TParameterMetaData> parameterMetaDataRepository, IParameter parameter) where TParameterMetaData : ParameterMetaData
      {
         var container = parameter.ParentContainer;
         if (container == null)
            return null;

         var containerPath = _entityPathResolver.PathFor(container);
         return parameterMetaDataRepository.ParameterMetaDataFor(containerPath, parameter.Name);
      }
   }
}