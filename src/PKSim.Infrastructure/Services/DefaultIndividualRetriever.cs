using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Infrastructure.Services
{
   public class DefaultIndividualRetriever : IDefaultIndividualRetriever
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IIndividualFactory _individualFactory;
      private readonly IIndividualSettingsDTOToOriginDataMapper _individualSettingsMapper;
      private readonly IIndividualDefaultValueRetriever _individualDefaultValueRetriever;
      private readonly ICache<SpeciesPopulation, Individual> _indvidualCacheProSpecies = new Cache<SpeciesPopulation, Individual>();

      public DefaultIndividualRetriever(
         ISpeciesRepository speciesRepository,
         IIndividualFactory individualFactory,
         IIndividualSettingsDTOToOriginDataMapper individualSettingsMapper,
         IIndividualDefaultValueRetriever individualDefaultValueRetriever)
      {
         _speciesRepository = speciesRepository;
         _individualFactory = individualFactory;
         _individualSettingsMapper = individualSettingsMapper;
         _individualDefaultValueRetriever = individualDefaultValueRetriever;
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

      public Individual DefaultIndividualFor(SpeciesPopulation speciesPopulation)
      {
         if (!_indvidualCacheProSpecies.Contains(speciesPopulation))
         {
            var individualDTO = _individualDefaultValueRetriever.DefaultSettingFor(speciesPopulation);
            _indvidualCacheProSpecies[speciesPopulation] = _individualFactory.CreateStandardFor(_individualSettingsMapper.MapFrom(individualDTO));
         }

         return _indvidualCacheProSpecies[speciesPopulation];
      }
   }
}