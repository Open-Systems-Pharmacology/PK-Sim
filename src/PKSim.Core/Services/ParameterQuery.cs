using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IParameterQuery
   {
      IEnumerable<ParameterRateMetaData> ParameterRatesFor(IContainer parameterContainer, IEnumerable<string> calculationMethods);
      IEnumerable<ParameterRateMetaData> ParameterRatesFor(IContainer parameterContainer, IEnumerable<string> calculationMethods, Func<ParameterMetaData, bool> predicate);

      IEnumerable<ParameterValueMetaData> ParameterValuesFor(IContainer parameterContainer, OriginData originData);
      IEnumerable<ParameterValueMetaData> ParameterValuesFor(IContainer parameterContainer, OriginData originData, Func<ParameterMetaData, bool> predicate);

      IReadOnlyList<ParameterDistributionMetaData> ParameterDistributionsFor(IContainer parameterContainer, SpeciesPopulation population, SubPopulation subPopulation, string parameterName);
      IEnumerable<ParameterDistributionMetaData> ParameterDistributionsFor(IContainer parameterContainer, OriginData originData);
      IEnumerable<ParameterDistributionMetaData> ParameterDistributionsFor(IContainer parameterContainer, OriginData originData, Func<ParameterMetaData, bool> predicate);

      IEnumerable<ParameterDistributionMetaData> AllParameterDistributionsFor(OriginData originData);
   }

   public class ParameterQuery : IParameterQuery
   {
      private readonly IParameterDistributionRepository _parameterDistributionRepository;
      private readonly IParameterValueRepository _parameterValueRepository;
      private readonly IParameterRateRepository _parameterRateRepository;
      private readonly IEntityPathResolver _entityPathResolver;

      public ParameterQuery(IParameterDistributionRepository distributionRepository,
                            IParameterValueRepository parameterValueRepository,
                            IParameterRateRepository parameterRateRepository,
                            IEntityPathResolver entityPathResolver)
      {
         _parameterDistributionRepository = distributionRepository;
         _parameterValueRepository = parameterValueRepository;
         _parameterRateRepository = parameterRateRepository;
         _entityPathResolver = entityPathResolver;
      }

      public IEnumerable<ParameterRateMetaData> ParameterRatesFor(IContainer parameterContainer, IEnumerable<string> calculationMethods)
      {
         return ParameterRatesFor(parameterContainer, calculationMethods, x => true);
      }

      public IEnumerable<ParameterRateMetaData> ParameterRatesFor(IContainer parameterContainer, IEnumerable<string> calculationMethods, Func<ParameterMetaData, bool> predicate)
      {
         var containerPath = _entityPathResolver.PathFor(parameterContainer);
         return from parameterRateDefinition in _parameterRateRepository.AllFor(containerPath)
                where predicate(parameterRateDefinition)
                where calculationMethods.Contains(parameterRateDefinition.CalculationMethod)
                select parameterRateDefinition;
      }

      public IEnumerable<ParameterValueMetaData> ParameterValuesFor(IContainer parameterContainer, OriginData originData)
      {
         return ParameterValuesFor(parameterContainer, originData, x => true);
      }

      public IEnumerable<ParameterValueMetaData> ParameterValuesFor(IContainer parameterContainer, OriginData originData, Func<ParameterMetaData, bool> predicate)
      {
         if (originData == null)
            return Enumerable.Empty<ParameterValueMetaData>();

         var containerPath = _entityPathResolver.PathFor(parameterContainer);
         return from parameterValueDefinition in _parameterValueRepository.AllFor(containerPath)
                where parameterValueDefinition.Species == originData.Species.Name
                where predicate(parameterValueDefinition)
                where originData.SubPopulation.Contains(parameterValueDefinition.ParameterValueVersion)
                select parameterValueDefinition;
      }

      public IReadOnlyList<ParameterDistributionMetaData> ParameterDistributionsFor(IContainer parameterContainer, SpeciesPopulation population, SubPopulation subPopulation, string parameterName)
      {
         var containerPath = _entityPathResolver.PathFor(parameterContainer);
         return distributedParameterMetaDataFor(_parameterDistributionRepository.AllFor(containerPath), population, subPopulation)
            .Where(p => string.Equals(p.ParameterName, parameterName))
            .ToList();
      }

      public IEnumerable<ParameterDistributionMetaData> ParameterDistributionsFor(IContainer parameterContainer, OriginData originData)
      {
         return ParameterDistributionsFor(parameterContainer, originData, x => true);
      }

      public IEnumerable<ParameterDistributionMetaData> ParameterDistributionsFor(IContainer parameterContainer, OriginData originData, Func<ParameterMetaData, bool> predicate)
      {
         var containerPath = _entityPathResolver.PathFor(parameterContainer);
         return distributedParameterMetaDataFor(_parameterDistributionRepository.AllFor(containerPath).Where(d => predicate(d)), originData);
      }

      public IEnumerable<ParameterDistributionMetaData> AllParameterDistributionsFor(OriginData originData)
      {
         return distributedParameterMetaDataFor(_parameterDistributionRepository.All(), originData);
      }

      private IEnumerable<ParameterDistributionMetaData> distributedParameterMetaDataFor(IEnumerable<ParameterDistributionMetaData> queryableDistributionParameters, OriginData originData)
      {
         if (originData == null)
            return Enumerable.Empty<ParameterDistributionMetaData>();

         var allDistributedParameters = (from distribution in distributedParameterMetaDataFor(queryableDistributionParameters, originData.SpeciesPopulation, originData.SubPopulation)
                                         where distribution.Gender == originData.Gender.Name
                                         select distribution);

         return allDistributedParameters.DefinedFor(originData);
      }

      private IEnumerable<ParameterDistributionMetaData> distributedParameterMetaDataFor(IEnumerable<ParameterDistributionMetaData> queryableDistributionParameters, SpeciesPopulation population, SubPopulation subPopulation)
      {
         return from distribution in queryableDistributionParameters
                where distribution.Population == population.Name
                where subPopulation.Contains(distribution.ParameterValueVersion)
                select distribution;
      }
   }
}