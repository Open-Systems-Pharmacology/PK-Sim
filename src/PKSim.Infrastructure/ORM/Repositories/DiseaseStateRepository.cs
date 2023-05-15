using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class DiseaseStateRepository : StartableRepository<DiseaseState>, IDiseaseStateRepository
   {
      private readonly IFlatDiseaseStateRepository _flatDiseaseStateRepository;
      private readonly IFlatPopulationDiseaseStateRepository _flatPopulationDiseaseStateRepository;
      private readonly IPopulationRepository _populationRepository;
      private readonly IFlatDiseaseStateToDiseaseStateMapper _diseaseStateMapper;
      private readonly IDiseaseStateImplementationRepository _diseaseStateImplementationRepository;
      private readonly Cache<string, DiseaseState> _allDiseaseSates = new Cache<string, DiseaseState>(x => x.Id, x => null);

      private readonly Cache<SpeciesPopulation, IReadOnlyList<DiseaseState>> _allDiseaseStatePerPopulation
         = new Cache<SpeciesPopulation, IReadOnlyList<DiseaseState>>(onMissingKey: x => Array.Empty<DiseaseState>());

      private DiseaseState _healthyState;

      public DiseaseStateRepository(
         IFlatDiseaseStateRepository flatDiseaseStateRepository,
         IFlatPopulationDiseaseStateRepository flatPopulationDiseaseStateRepository,
         IPopulationRepository populationRepository,
         IFlatDiseaseStateToDiseaseStateMapper diseaseStateMapper,
         IDiseaseStateImplementationRepository diseaseStateImplementationRepository)
      {
         _flatDiseaseStateRepository = flatDiseaseStateRepository;
         _flatPopulationDiseaseStateRepository = flatPopulationDiseaseStateRepository;
         _populationRepository = populationRepository;
         _diseaseStateMapper = diseaseStateMapper;
         _diseaseStateImplementationRepository = diseaseStateImplementationRepository;
      }

      protected override void DoStart()
      {
         _allDiseaseSates.AddRange(_flatDiseaseStateRepository.All().MapAllUsing(_diseaseStateMapper));

         foreach (var query in _flatPopulationDiseaseStateRepository.All().GroupBy(x => x.Population))
         {
            var population = _populationRepository.FindByName(query.Key);
            var diseaseStates = new List<DiseaseState>();
            diseaseStates.AddRange(query.Select(x => _allDiseaseSates[x.DiseaseState]));
            _allDiseaseStatePerPopulation[population] = diseaseStates;
         }

         _healthyState = _allDiseaseSates.FindByName(CoreConstants.DiseaseStates.HEALTHY);
      }

      public override IEnumerable<DiseaseState> All()
      {
         Start();
         return _allDiseaseSates;
      }

      public IReadOnlyList<DiseaseState> AllFor(SpeciesPopulation population)
      {
         Start();
         return _allDiseaseStatePerPopulation[population];
      }

      public IReadOnlyList<DiseaseState> AllForExpressionProfile(Species species, QuantityType quantityType)
      {
         if (!species.IsHuman)
            return Array.Empty<DiseaseState>();

         //returns all disease state where an implementation can be applied to expression profile
         return All().Select(x => new {diseaseState = x, impl = _diseaseStateImplementationRepository.FindFor(x)})
            .Where(x => x.impl.CanBeAppliedToExpressionProfile(quantityType)).Select(x => x.diseaseState).ToList();
      }

      public DiseaseState HealthyState
      {
         get
         {
            Start();
            return _healthyState;
         }
      }
   }
}