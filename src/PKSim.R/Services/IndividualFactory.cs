using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Snapshots.Mappers;
using PKSim.R.Domain;
using ICoreIndividualFactory = PKSim.Core.Model.IIndividualFactory;

namespace PKSim.R.Services
{
   public interface IIndividualFactory
   {
      /// <summary>
      ///    Returns the <seealso cref="CreateIndividualResults" /> representing an individual with the characteristics defined
      ///    in <paramref name="individualCharacteristics" />.
      /// </summary>
      CreateIndividualResults CreateIndividual(IndividualCharacteristics individualCharacteristics);

      DistributedParameterValueWithUnit[] DistributionsFor(IndividualCharacteristics individualCharacteristics);
   }

   public class IndividualFactory : IIndividualFactory
   {
      private readonly OriginDataMapper _originDataMapper;
      private readonly ICoreIndividualFactory _individualFactory;
      private readonly IIndividualToIndividualValuesMapper _individualValuesMapper;
      private readonly IOntogenyFactorsRetriever _ontogenyFactorsRetriever;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IContainerTask _containerTask;

      public IndividualFactory(
         OriginDataMapper originDataMapper,
         ICoreIndividualFactory individualFactory,
         IIndividualToIndividualValuesMapper individualValuesMapper,
         IOntogenyFactorsRetriever ontogenyFactorsRetriever,
         IEntityPathResolver entityPathResolver,
         IContainerTask containerTask)
      {
         _originDataMapper = originDataMapper;
         _individualFactory = individualFactory;
         _individualValuesMapper = individualValuesMapper;
         _ontogenyFactorsRetriever = ontogenyFactorsRetriever;
         _entityPathResolver = entityPathResolver;
         _containerTask = containerTask;
      }

      public CreateIndividualResults CreateIndividual(IndividualCharacteristics individualCharacteristics)
      {
         var originData = originDataFrom(individualCharacteristics);
         var moleculeOntogenies = individualCharacteristics.MoleculeOntogenies;
         var individual = _individualFactory.CreateAndOptimizeFor(originData, individualCharacteristics.Seed);
         var individualProperties = _individualValuesMapper.MapFrom(individual);
         var allIndividualParameters = individualProperties.ParameterValues;
         var ontogenyParameters = _ontogenyFactorsRetriever.FactorsFor(originData, moleculeOntogenies).Select(x => new ParameterValueWithUnit(x));
         var allDistributedParameterCache = _containerTask.CacheAllChildren<IDistributedParameter>(individual);
         var allIndividualParametersCache = _containerTask.CacheAllChildren<IParameter>(individual);
         var distributedParameters = new Cache<string, ParameterValueWithUnit>(x => x.ParameterPath);
         var derivedParameters = new List<ParameterValueWithUnit>();

         //Add Age and Height parameter that is not distributed at the moment
         if (originData.Population.IsAgeDependent)
         {
            distributedParameters.Add(parameterValueFrom(individual.Organism.Parameter(CoreConstants.Parameters.AGE)));
            distributedParameters.Add(parameterValueFrom(individual.Organism.Parameter(Constants.Parameters.GESTATIONAL_AGE)));
         }

         if (originData.Population.IsHeightDependent)
            distributedParameters.Add(parameterValueFrom(individual.Organism.Parameter(CoreConstants.Parameters.HEIGHT)));

         distributedParameters.Add(parameterValueFrom(individual.Organism.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_AGP)));
         distributedParameters.Add(parameterValueFrom(individual.Organism.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_ALBUMIN)));

         foreach (var individualParameter in allIndividualParameters)
         {
            var parameter = allIndividualParametersCache[individualParameter.ParameterPath];
            if (parameter == null)
               continue;

            var parameterValue = new ParameterValueWithUnit(individualParameter, parameter?.BaseUnitName());
            if (allDistributedParameterCache.Contains(individualParameter.ParameterPath))
               distributedParameters.Add(parameterValue);

            //Do not add parameters that were added specifically 
            else if (!distributedParameters.Contains(individualParameter.ParameterPath))
               derivedParameters.Add(parameterValue);
         }


         //add Ontogeny parameters
         distributedParameters.AddRange(ontogenyParameters);

         return new CreateIndividualResults(distributedParameters.ToArray(), derivedParameters.ToArray(), individual.Seed);
      }

      public DistributedParameterValueWithUnit[] DistributionsFor(IndividualCharacteristics individualCharacteristics)
      {
         var originData = originDataFrom(individualCharacteristics);
         var moleculeOntogenies = individualCharacteristics.MoleculeOntogenies;
         var individual = _individualFactory.CreateAndOptimizeFor(originData, individualCharacteristics.Seed);
         var distributedParameters = individual.GetAllChildren<IDistributedParameter>().Select(distributedParameterValueFrom).ToList();
         //Ontogeny factors have no units
         distributedParameters.AddRange(_ontogenyFactorsRetriever.DistributionFactorsFor(originData, moleculeOntogenies)
            .Select(x => new DistributedParameterValueWithUnit(x, "")));
         return distributedParameters.ToArray();
      }

      private OriginData originDataFrom(Core.Snapshots.OriginData originData)
      {
         return _originDataMapper.MapToModel(originData).Result;
      }

      private ParameterValueWithUnit parameterValueFrom(IParameter parameter)
      {
         var parameterValue = new ParameterValue(_entityPathResolver.PathFor(parameter), parameter.Value, CoreConstants.DEFAULT_PERCENTILE);
         return new ParameterValueWithUnit(parameterValue, parameter.BaseUnitName());
      }

      private DistributedParameterValueWithUnit distributedParameterValueFrom(IDistributedParameter parameter)
      {
         var parameterPath = _entityPathResolver.PathFor(parameter);
         var distributionType = parameter.Formula.DistributionType();
         double p1 = 0, p2 = 0;
         if (distributionType == DistributionTypes.Normal || distributionType == DistributionTypes.LogNormal)
         {
            p1 = parameter.MeanParameter.Value;
            p2 = parameter.DeviationParameter.Value;
         }
         else if (distributionType == DistributionTypes.Uniform)
         {
            p1 = parameter.Parameter(Constants.Distribution.MINIMUM).Value;
            p2 = parameter.Parameter(Constants.Distribution.MAXIMUM).Value;
         }

         var distributedParameterValue =
            new DistributedParameterValue(parameterPath, parameter.Value, parameter.Percentile, p1, p2, distributionType);
         return new DistributedParameterValueWithUnit(distributedParameterValue, parameter.BaseUnitName());
      }
   }
}