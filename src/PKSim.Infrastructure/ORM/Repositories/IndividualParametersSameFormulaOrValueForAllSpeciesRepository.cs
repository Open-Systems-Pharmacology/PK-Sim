using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static PKSim.Core.CoreConstants.Parameters;

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

            //For ontogeny table parameter, we NEVER use the same formula as they are clearly individual specific parameters but are defined as constant in the database
            if (parameterName.IsOneOf(ONTOGENY_FACTOR_TABLE, ONTOGENY_FACTOR_GI_TABLE, ONTOGENY_FACTOR_ALBUMIN_TABLE, ONTOGENY_FACTOR_AGP_TABLE))
               continue;

            var containerPath = _flatContainerRepository.ContainerPathFrom(individualParameter.ContainerId);


            var individualParameterSameFormulaOrValueForAllSpecies = new IndividualParameterSameFormulaOrValueForAllSpecies
            {
               ContainerId = containerId,
               ContainerPath = containerPath,
               ParameterName = parameterName,
               IsSameFormula = isSameFormula,
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

      public bool IsSameFormulaOrValue(ParameterMetaData parameterMetaData) => parameterSameFormulaOrValueFor(parameterMetaData) != null;

      public bool IsSameFormulaOrValue(IParameter parameter)
      {
         Start();
         var path = _entityPathResolver.PathFor(parameter);
         var parameterSameFormulaOrValue = _allByParameterPath[path];
         return parameterSameFormulaOrValue != null;
      }

      public bool IsSameFormula(ParameterMetaData parameterMetaData)
      {
         var parameterSameFormulaOrValue = parameterSameFormulaOrValueFor(parameterMetaData);
         return parameterSameFormulaOrValue?.IsSameFormula ?? false;
      }

      private IndividualParameterSameFormulaOrValueForAllSpecies parameterSameFormulaOrValueFor(ParameterMetaData parameterMetaData)
      {
         Start();
         return _allByContainerIdAndParameterName[(parameterMetaData.ContainerId, parameterMetaData.ParameterName)];
      }
   }
}