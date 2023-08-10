using System.Collections.Generic;
using System.Linq;
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

      private readonly List<IndividualParameterSameFormulaOrValueForAllSpecies> _individualParametersSameFormulaOrValue = new List<IndividualParameterSameFormulaOrValueForAllSpecies>();
      private readonly Cache<(int, string), IndividualParameterSameFormulaOrValueForAllSpecies> _parametersSameFormulaOrValueForAllSpeciesByContainerIdAndParameterName = 
         new Cache<(int, string), IndividualParameterSameFormulaOrValueForAllSpecies>(onMissingKey:x=>null);

      public IndividualParametersSameFormulaOrValueForAllSpeciesRepository(
         IFlatContainerRepository flatContainerRepository,
         IFlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository = flatIndividualParametersSameFormulaOrValueForAllSpeciesRepository;
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
            _parametersSameFormulaOrValueForAllSpeciesByContainerIdAndParameterName.Add((containerId, parameterName),
               individualParameterSameFormulaOrValueForAllSpecies);
         }
      }

      public override IEnumerable<IndividualParameterSameFormulaOrValueForAllSpecies> All()
      {
         Start();
         return _individualParametersSameFormulaOrValue;
      }

      public bool IsSameFormula(ParameterMetaData parameterMetaData)
      {
         var (isSameFormula, _) = IsSameFormulaOrValue(parameterMetaData);
         return isSameFormula;
      }

      public bool IsSameValue(ParameterMetaData parameterMetaData)
      {
         var (_, isSameValue) = IsSameFormulaOrValue(parameterMetaData);
         return isSameValue;
      }

      public (bool IsSameFormula, bool IsSameValue) IsSameFormulaOrValue(ParameterMetaData parameterMetaData)
      {
         var parameterSameFormulaOrValue = 
            _parametersSameFormulaOrValueForAllSpeciesByContainerIdAndParameterName[(parameterMetaData.ContainerId, parameterMetaData.ParameterName)];

         return parameterSameFormulaOrValue == null
            ? (IsSameFormula: false, IsSameValue: false)
            : (parameterSameFormulaOrValue.IsSameFormula, parameterSameFormulaOrValue.IsSameValue);
      }
   }
}
