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

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatSpeciesRepository : IMetaDataRepository<FlatSpecies>
   {
   }

   public class FlatSpeciesRepository : MetaDataRepository<FlatSpecies>, IFlatSpeciesRepository
   {
      public FlatSpeciesRepository(IDbGateway dbGateway, IDataTableToMetaDataMapper<FlatSpecies> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewSpecies)
      {
      }
   }

   public class SpeciesRepository : StartableRepository<Species>, ISpeciesRepository
   {
      private readonly ICache<string, Species> _allSpecies = new Cache<string, Species>(x => x.Name, x => null);
      private readonly IFlatSpeciesToSpeciesMapper _flatSpeciesMapper;
      private readonly IFlatSpeciesRepository _flatSpeciesRepository;
      private readonly ICoreUserSettings _userSettings;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      
      public SpeciesRepository(IFlatSpeciesRepository flatSpeciesRepository, IFlatSpeciesToSpeciesMapper flatSpeciesMapper,
         ICoreUserSettings userSettings, IRepresentationInfoRepository representationInfoRepository)
      {
         _flatSpeciesRepository = flatSpeciesRepository;
         _flatSpeciesMapper = flatSpeciesMapper;
         _userSettings = userSettings;
         _representationInfoRepository = representationInfoRepository;
      }

      public override IEnumerable<Species> All()
      {
         Start();
         return _allSpecies;
      }

      protected override void DoStart()
      {
         _allSpecies.AddRange(_flatSpeciesRepository.All().MapAllUsing(_flatSpeciesMapper));
         _allSpecies.Each(updateDisplayInfo);
      }

      private void updateDisplayInfo(Species species)
      {
         var representationInfo = _representationInfoRepository.InfoFor(species);
         species.Description = representationInfo.Description;
         species.DisplayName = representationInfo.DisplayName;
      }

      public Species DefaultSpecies
      {
         get
         {
            Start();
            return _allSpecies[_userSettings.DefaultSpecies] ?? _allSpecies.First();
         }
      }
   }
}