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
   public interface IIndividualToIndividualPropertiesMapper : IMapper<Individual, IndividualProperties>
   {
      IndividualProperties MapFrom(Individual individual, PathCache<IParameter> updatedParameters);
   }

   public class IndividualToIndividualPropertiesMapper : IIndividualToIndividualPropertiesMapper
   {
      private readonly IContainerTask _containerTask;

      public IndividualToIndividualPropertiesMapper(IContainerTask containerTask)
      {
         _containerTask = containerTask;
      }

      public IndividualProperties MapFrom(Individual individual)
      {
         var pathCache = _containerTask.CacheAllChildrenSatisfying<IParameter>(individual, p => p.IsChangedByCreateIndividual);
         return MapFrom(individual, pathCache);
      }

      public IndividualProperties MapFrom(Individual individual, PathCache<IParameter> updatedParameters)
      {
         var individualProperties = new IndividualProperties
            {
               Covariates = new IndividualCovariates
                  {
                     Gender = individual.OriginData.Gender,
                     Race = individual.OriginData.SpeciesPopulation
                  }
            };

         updatedParameters.KeyValues.Each(paraKey => individualProperties.AddParameterValue(parameterValueFrom(paraKey)));
         return individualProperties;
      }

      private static ParameterValue parameterValueFrom(KeyValuePair<string, IParameter> paraKey)
      {
         var distributedParameter = paraKey.Value as IDistributedParameter;
         double percentile = distributedParameter == null ? CoreConstants.DEFAULT_PERCENTILE : distributedParameter.Percentile.CorrectedPercentileValue();
         return new ParameterValue(paraKey.Key, paraKey.Value.Value, percentile);
      }
   }
}