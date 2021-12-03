using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class DiseaseStateRepository : StartableRepository<DiseaseState>, IDiseaseStateRepository
   {
      private readonly IFlatDiseaseStateRepository _flatDiseaseStateRepository;
      private readonly IFlatPopulationDiseaseStateRepository _flatPopulationDiseaseStateRepository;
      private readonly IPopulationRepository _populationRepository;
      private readonly IFlatDiseaseStateToDiseaseStateMapper _diseaseStateMapper;
      private readonly Cache<string, DiseaseState> _allDiseaseSates = new Cache<string, DiseaseState>(x => x.Id, x => null);
      private readonly Cache<SpeciesPopulation, IReadOnlyList<DiseaseState>> _allDiseaseStatePerPopulation 
         = new Cache<SpeciesPopulation, IReadOnlyList<DiseaseState>>(onMissingKey: x=> Array.Empty<DiseaseState>());

      public DiseaseStateRepository(
         IFlatDiseaseStateRepository flatDiseaseStateRepository,
         IFlatPopulationDiseaseStateRepository flatPopulationDiseaseStateRepository,
         IPopulationRepository populationRepository,
         IFlatDiseaseStateToDiseaseStateMapper diseaseStateMapper)
      {
         _flatDiseaseStateRepository = flatDiseaseStateRepository;
         _flatPopulationDiseaseStateRepository = flatPopulationDiseaseStateRepository;
         _populationRepository = populationRepository;
         _diseaseStateMapper = diseaseStateMapper;
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

         HealthyState = new DiseaseState
         {
            Id = CoreConstants.ContainerName.HEALTHY,
            Name = CoreConstants.ContainerName.HEALTHY,
            DisplayName = CoreConstants.ContainerName.HEALTHY,
         };
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

      public DiseaseState HealthyState { get; private set; }
   }
}