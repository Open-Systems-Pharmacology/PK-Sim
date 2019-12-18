using System;
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
      /// Returns the <seealso cref="ParameterValues"/> representing an individual with the characteristics defined in <paramref name="individualCharacteristics"/>.
      /// </summary>
      ParameterValue[] CreateIndividual(IndividualCharacteristics individualCharacteristics);
      DistributedParameterValue[] DistributionsFor(IndividualCharacteristics individualCharacteristics);
   }

   public class IndividualFactory : IIndividualFactory
   {
      private readonly OriginDataMapper _originDataMapper;
      private readonly ICoreIndividualFactory _individualFactory;
      private readonly IIndividualToIndividualValuesMapper _individualValuesMapper;
      private readonly IOntogenyFactorsRetriever _ontogenyFactorsRetriever;
      private readonly IEntityPathResolver _entityPathResolver;

      public IndividualFactory(
         OriginDataMapper originDataMapper,
         ICoreIndividualFactory individualFactory,
         IIndividualToIndividualValuesMapper individualValuesMapper,
         IOntogenyFactorsRetriever ontogenyFactorsRetriever,
         IEntityPathResolver entityPathResolver)
      {
         _originDataMapper = originDataMapper;
         _individualFactory = individualFactory;
         _individualValuesMapper = individualValuesMapper;
         _ontogenyFactorsRetriever = ontogenyFactorsRetriever;
         _entityPathResolver = entityPathResolver;
      }

      public ParameterValue[] CreateIndividual(IndividualCharacteristics individualCharacteristics)
      {
         var originData = originDataFrom(individualCharacteristics);
         var moleculeOntogenies = individualCharacteristics.MoleculeOntogenies;
         var individual = _individualFactory.CreateAndOptimizeFor(originData);
         var individualProperties = _individualValuesMapper.MapFrom(individual);
         var allIndividualParameters = individualProperties.ParameterValues.ToList();
         allIndividualParameters.AddRange(_ontogenyFactorsRetriever.FactorsFor(originData, moleculeOntogenies));
         return allIndividualParameters.ToArray();
      }

      public DistributedParameterValue[] DistributionsFor(IndividualCharacteristics individualCharacteristics)
      {
         var originData = originDataFrom(individualCharacteristics);
         var moleculeOntogenies = individualCharacteristics.MoleculeOntogenies;
         var individual = _individualFactory.CreateAndOptimizeFor(originData);
         return individual.GetAllChildren<IDistributedParameter>().Select(distributedParameterValueFrom).ToArray();
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