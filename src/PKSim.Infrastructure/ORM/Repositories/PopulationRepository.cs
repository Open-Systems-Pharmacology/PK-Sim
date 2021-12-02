using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatPopulationRepository : IMetaDataRepository<FlatPopulation>
   {
   }

   public class FlatPopulationRepository : MetaDataRepository<FlatPopulation>, IFlatPopulationRepository
   {
      public FlatPopulationRepository(IDbGateway dbGateway,
                                      IDataTableToMetaDataMapper<FlatPopulation> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_POPULATIONS)
      {
      }
   }

   public class PopulationRepository : StartableRepository<SpeciesPopulation>, IPopulationRepository
   {
      private readonly IFlatPopulationRepository _flatPopulationRepository;
      private readonly IFlatPopulationToPopulationMapper _flatPopMapper;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly ICoreUserSettings _userSettings;
      private IReadOnlyList<SpeciesPopulation> _allPopulations;

      public PopulationRepository(IFlatPopulationRepository flatPopulationRepository, IFlatPopulationToPopulationMapper flatPopMapper,
                                  IRepresentationInfoRepository representationInfoRepository, ICoreUserSettings userSettings)
      {
         _flatPopulationRepository = flatPopulationRepository;
         _flatPopMapper = flatPopMapper;
         _representationInfoRepository = representationInfoRepository;
         _userSettings = userSettings;
      }

      public override IEnumerable<SpeciesPopulation> All()
      {
         Start();
         return _allPopulations;
      }

      public SpeciesPopulation DefaultPopulationFor(Species species)
      {
         return species.Populations.FindByName(_userSettings.DefaultPopulation) ?? species.DefaultPopulation;
      }

      public SpeciesPopulation FindByIndex(int raceIndex)
      {
         return All().FirstOrDefault(x => x.RaceIndex == raceIndex);
      }

      protected override void DoStart()
      {
         var flatPopulations = _flatPopulationRepository.All();
         _allPopulations =flatPopulations.MapAllUsing(_flatPopMapper);
         _allPopulations.Each(updateDisplayInfo);
      }

      private void updateDisplayInfo(SpeciesPopulation speciesPopulation)
      {
         var representationInfo = _representationInfoRepository.InfoFor(speciesPopulation);
         speciesPopulation.Description = representationInfo.Description;
         speciesPopulation.DisplayName = representationInfo.DisplayName;
      }
   }
}