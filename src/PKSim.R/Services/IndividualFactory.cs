using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Snapshots.Mappers;
using PKSim.R.Domain;
using ICoreIndividualFactory = PKSim.Core.Model.IIndividualFactory;
using OriginData = PKSim.Core.Snapshots.OriginData;

namespace PKSim.R.Services
{
   public interface IIndividualFactory
   {
      /// <summary>
      /// Returns the <seealso cref="CreateIndividualResults"/> representing an individual with the characteristics defined in <paramref name="individualCharacteristics"/>.
      /// </summary>
      CreateIndividualResults CreateIndividual(IndividualCharacteristics individualCharacteristics);
      DistributedParameterValue[] DistributionsFor(IndividualCharacteristics individualCharacteristics);
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
         var individual = _individualFactory.CreateAndOptimizeFor(originData);
         var individualProperties = _individualValuesMapper.MapFrom(individual);
         var allIndividualParameters = individualProperties.ParameterValues;
         var ontogenyParameters = _ontogenyFactorsRetriever.FactorsFor(originData, moleculeOntogenies);
         var allDistributedParameterCache = _containerTask.CacheAllChildren<IDistributedParameter>(individual);
         var distributedParameters = new List<ParameterValue>();
         var derivedParameters = new List<ParameterValue>();

         foreach (var individualParameter in allIndividualParameters)
         {
            if(allDistributedParameterCache.Contains(individualParameter.ParameterPath))
               distributedParameters.Add(individualParameter);
            else
               derivedParameters.Add(individualParameter);
         }
         distributedParameters.AddRange(ontogenyParameters);

         return new CreateIndividualResults(distributedParameters.ToArray(), derivedParameters.ToArray());
      }

      public DistributedParameterValue[] DistributionsFor(IndividualCharacteristics individualCharacteristics)
      {
         var originData = originDataFrom(individualCharacteristics);
         var moleculeOntogenies = individualCharacteristics.MoleculeOntogenies;
         var individual = _individualFactory.CreateAndOptimizeFor(originData);
         var distributedParameters = individual.GetAllChildren<IDistributedParameter>().Select(distributedParameterValueFrom).ToList();
         distributedParameters.AddRange(_ontogenyFactorsRetriever.DistributionFactorsFor(originData, moleculeOntogenies));
         return distributedParameters.ToArray();
      }

      private Core.Model.OriginData originDataFrom(OriginData originData) => _originDataMapper.MapToModel(originData).Result;

      private DistributedParameterValue distributedParameterValueFrom(IDistributedParameter parameter)
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

         return new DistributedParameterValue(parameterPath, parameter.Value, parameter.Percentile, p1, p2, distributionType);
      }
   }
}