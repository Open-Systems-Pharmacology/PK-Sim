using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Repositories;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper : IMapper<SpeciesDatabaseMap, SpeciesDatabaseMapDTO>
   {
   }

   public class SpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper : ISpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public SpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper(ISpeciesRepository speciesRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _speciesRepository = speciesRepository;
         _representationInfoRepository = representationInfoRepository;
      }

      public SpeciesDatabaseMapDTO MapFrom(SpeciesDatabaseMap speciesDatabaseMap)
      {
         var dto = new SpeciesDatabaseMapDTO();
         var species = _speciesRepository.FindByName(speciesDatabaseMap.Species);
         dto.DatabaseFullPath = speciesDatabaseMap.DatabaseFullPath;
         dto.OriginalDatabasePathFullPath = speciesDatabaseMap.DatabaseFullPath;
         dto.SpeciesName = speciesDatabaseMap.Species;
         dto.SpeciesDisplayName = _representationInfoRepository.DisplayNameFor(species);
         return dto;
      }
   }
}