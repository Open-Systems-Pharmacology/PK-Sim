using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Mappers
{
   public interface IIndividualToIndividualValuesMapper : IMapper<Individual, IndividualValues>
   {
      IndividualValues MapFrom(Individual individual, PathCache<IParameter> updatedParameters);
   }

   public class IndividualToIndividualValuesMapper : IIndividualToIndividualValuesMapper
   {
      private readonly IContainerTask _containerTask;

      public IndividualToIndividualValuesMapper(IContainerTask containerTask)
      {
         _containerTask = containerTask;
      }

      public IndividualValues MapFrom(Individual individual)
      {
         var pathCache = _containerTask.CacheAllChildrenSatisfying<IParameter>(individual, p => p.IsChangedByCreateIndividual);
         return MapFrom(individual, pathCache);
      }

      public IndividualValues MapFrom(Individual individual, PathCache<IParameter> updatedParameters)
      {
         var individualValues = new IndividualValues();

         individualValues.AddCovariate(Constants.Population.GENDER, individual.OriginData.Gender.DisplayName);
         individualValues.AddCovariate(Constants.Population.RACE, individual.OriginData.SpeciesPopulation.DisplayName);

         updatedParameters.KeyValues.Each(paraKey => individualValues.AddParameterValue(parameterValueFrom(paraKey)));
         return individualValues;
      }

      private static ParameterValue parameterValueFrom(KeyValuePair<string, IParameter> paraKey)
      {
         var distributedParameter = paraKey.Value as IDistributedParameter;
         double percentile = distributedParameter?.Percentile.CorrectedPercentileValue() ?? CoreConstants.DEFAULT_PERCENTILE;
         return new ParameterValue(paraKey.Key, paraKey.Value.Value, percentile);
      }
   }
}