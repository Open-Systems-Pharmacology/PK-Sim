using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class IndividualParametersSameFormulaOrValueForAllSpeciesRepository :
      StartableRepository<IndividualParameterSameFormulaOrValueForAllSpecies>,
      IIndividualParametersSameFormulaOrValueForAllSpeciesRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository _flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository;
      private readonly IEntityPathResolver _entityPathResolver;

      private readonly List<IndividualParameterSameFormulaOrValueForAllSpecies> _individualParametersSameFormulaOrValue = new List<IndividualParameterSameFormulaOrValueForAllSpecies>();

      private readonly Cache<(int, string), IndividualParameterSameFormulaOrValueForAllSpecies> _allByContainerIdAndParameterName =
         new Cache<(int, string), IndividualParameterSameFormulaOrValueForAllSpecies>(onMissingKey: x => null);

      private readonly Cache<string, IndividualParameterSameFormulaOrValueForAllSpecies> _allByParameterPath = new Cache<string, IndividualParameterSameFormulaOrValueForAllSpecies>(onMissingKey: x => null);

      public IndividualParametersSameFormulaOrValueForAllSpeciesRepository(
         IFlatContainerRepository flatContainerRepository,
         IFlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository,
         IEntityPathResolver entityPathResolver)
      {
         _flatContainerRepository = flatContainerRepository;
         _flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository = flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository;
         _entityPathResolver = entityPathResolver;
      }

      protected override void DoStart()
      {
         var flatIndividualParametersSameFormulaOrValue = _flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository.All().ToList();

         foreach (var individualParameter in flatIndividualParametersSameFormulaOrValue)
         {
            var (containerId, parameterName, isSameFormula, _, _) = individualParameter;
            var containerPath = _flatContainerRepository.ContainerPathFrom(individualParameter.ContainerId);
            var individualParameterSameFormulaOrValueForAllSpecies = new IndividualParameterSameFormulaOrValueForAllSpecies
            {
               ContainerId = containerId,
               ContainerPath = containerPath,
               ParameterName = parameterName,
               IsSameFormula = isSameFormula
            };
            _individualParametersSameFormulaOrValue.Add(individualParameterSameFormulaOrValueForAllSpecies);
            _allByContainerIdAndParameterName.Add((containerId, parameterName), individualParameterSameFormulaOrValueForAllSpecies);
            _allByParameterPath.Add(new[] {containerPath, parameterName}.ToPathString(), individualParameterSameFormulaOrValueForAllSpecies);
         }
      }

      public override IEnumerable<IndividualParameterSameFormulaOrValueForAllSpecies> All()
      {
         Start();
         return _individualParametersSameFormulaOrValue;
      }

      public bool IsSameFormula(ParameterMetaData parameterMetaData)
      {
         var (isSameFormula, _, _) = isSameFormulaOrValue(parameterMetaData);
         return isSameFormula;
      }

      public bool IsSameValue(ParameterMetaData parameterMetaData)
      {
         var (_, isSameValue, _) = isSameFormulaOrValue(parameterMetaData);
         return isSameValue;
      }

      public (bool isSame, bool exists) IsSameFormulaOrValue(ParameterMetaData parameterMetaData)
      {
         var (isSameFormula, isSameValue, exists) = isSameFormulaOrValue(parameterMetaData);
         return (isSameFormula || isSameValue, exists);
      }

      public bool IsSameFormulaOrValue(IParameter parameter)
      {
         var path = _entityPathResolver.PathFor(parameter);
         var parameterSameFormulaOrValue = _allByParameterPath[path];
         return parameterSameFormulaOrValue?.IsSame ?? false;
      }

      private (bool isSameFormula, bool isSameValue, bool exists) isSameFormulaOrValue(ParameterMetaData parameterMetaData)
      {
         Start();
         var parameterSameFormulaOrValue = _allByContainerIdAndParameterName[(parameterMetaData.ContainerId, parameterMetaData.ParameterName)];

         return parameterSameFormulaOrValue == null
            ? (false, false, false)
            : (parameterSameFormulaOrValue.IsSameFormula, parameterSameFormulaOrValue.IsSameValue, true);
      }
   }
}