using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatPopulationAgeRepository : IMetaDataRepository<PopulationAgeSettings>
   {
   }

   public class FlatPopulationAgeRepository : MetaDataRepository<PopulationAgeSettings>, IFlatPopulationAgeRepository
   {
      public FlatPopulationAgeRepository(IDbGateway dbGateway,
         IDataTableToMetaDataMapper<PopulationAgeSettings> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.VIEW_POPULATION_AGE)
      {
      }
   }

   public class PopulationAgeRepository : StartableRepository<PopulationAgeSettings>, IPopulationAgeRepository
   {
      private readonly IFlatPopulationAgeRepository _flatPopulationAgeRepository;
      private readonly Cache<string, PopulationAgeSettings> _populationAgeSettings = new Cache<string, PopulationAgeSettings>();

      public PopulationAgeRepository(IFlatPopulationAgeRepository flatPopulationAgeRepository)
      {
         _flatPopulationAgeRepository = flatPopulationAgeRepository;
      }

      public override IEnumerable<PopulationAgeSettings> All()
      {
         Start();
         return _populationAgeSettings;
      }

      public PopulationAgeSettings PopulationAgeSettingsFrom(string population)
      {
         Start();
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