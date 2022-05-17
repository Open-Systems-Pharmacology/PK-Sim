using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Snapshots.Mappers;
using OriginData = PKSim.Core.Snapshots.OriginData;

namespace PKSim.Core.Services
{
   public class DefaultIndividualRetriever : IDefaultIndividualRetriever
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IIndividualFactory _individualFactory;
      private readonly OriginDataMapper _originDataMapper;
      private readonly ICache<SpeciesPopulation, Individual> _individualCacheProSpecies = new Cache<SpeciesPopulation, Individual>();

      public DefaultIndividualRetriever(
         ISpeciesRepository speciesRepository,
         IIndividualFactory individualFactory,
         OriginDataMapper originDataMapper
      )

      {
         _speciesRepository = speciesRepository;
         _individualFactory = individualFactory;
         _originDataMapper = originDataMapper;
      }

      public Individual DefaultIndividual()
      {
         return DefaultIndividualFor(_speciesRepository.DefaultSpecies);
      }

      public Individual DefaultHuman()
      {
         return DefaultIndividualFor(_speciesRepository.FindByName(CoreConstants.Species.HUMAN));
      }

      public Individual DefaultIndividualFor(Species species)
      {
         return DefaultIndividualFor(species.DefaultPopulation);
      }

      public Gender DefaultGenderFor(SpeciesPopulation speciesPopulation)
      {
         return speciesPopulation.Genders[0];
      }

      public Individual DefaultIndividualFor(SpeciesPopulation speciesPopulation)
      {
         if (!_individualCacheProSpecies.Contains(speciesPopulation))
         {
            var originDataSnapshot = new OriginData
            {
               Species = speciesPopulation.Species,
               Population = speciesPopulation.Name,
               Gender = DefaultGenderFor(speciesPopulation).Name
            };

            //We do not need to pass any valid snapshot context in this case.
            var originData = _originDataMapper.MapToModel(originDataSnapshot, new SnapshotContext()).Result;
            _individualCacheProSpecies[speciesPopulation] = _individualFactory.CreateStandardFor(originData);
         }

         return _individualCacheProSpecies[speciesPopulation];
      }
   }
}