using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatPopulationAgeRepository : IMetaDataRepository<PopulationAgeSettings>
   {
   }

   public class FlatPopulationAgeRepository : MetaDataRepository<PopulationAgeSettings>, IFlatPopulationAgeRepository
   {
      public FlatPopulationAgeRepository(IDbGateway dbGateway,
                                         IDataTableToMetaDataMapper<PopulationAgeSettings> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewPopulationAge)
      {
      }
   }

   public class PopulationAgeRepository : StartableRepository<PopulationAgeSettings>, IPopulationAgeRepository
   {
      private readonly IFlatPopulationAgeRepository _flatPopulationAgeRepository;
      private ICache<string, PopulationAgeSettings> _populationAgeSettings;

      public PopulationAgeRepository(IFlatPopulationAgeRepository flatPopulationAgeRepository)
      {
         _flatPopulationAgeRepository = flatPopulationAgeRepository;
         _populationAgeSettings = new Cache<string, PopulationAgeSettings>();
      }

      public override IEnumerable<PopulationAgeSettings> All()
      {
         return _populationAgeSettings;
      }

      public PopulationAgeSettings PopulationAgeSettingsFrom(string population)
      {
         return _populationAgeSettings[population];
      }

      protected override void DoStart()
      {
         foreach (var populationAgeSettings in _flatPopulationAgeRepository.All())
         {
            _populationAgeSettings.Add(populationAgeSettings.Population, populationAgeSettings);
         }
      }
   }

}
