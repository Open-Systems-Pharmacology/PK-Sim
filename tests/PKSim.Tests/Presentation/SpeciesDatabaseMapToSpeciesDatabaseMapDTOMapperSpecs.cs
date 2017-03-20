using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_SpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper : ContextSpecification<ISpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper>
   {
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected ISpeciesRepository _speciesRepository;

      protected override void Context()
      {
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         sut = new SpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper(_speciesRepository, _representationInfoRepository);
      }
   }

   
   public class When_mapping_a_species_database_map_to_the_corresponding_dto : concern_for_SpeciesDatabaseMapToSpeciesDatabaseMapDTOMapper
   {
      private SpeciesDatabaseMap _speciesDatabaseMap;
      private string _displayName;
      private SpeciesDatabaseMapDTO _result;

      protected override void Context()
      {
         base.Context();
         _displayName = "DisplayName";
         _speciesDatabaseMap = new SpeciesDatabaseMap {DatabaseFullPath = "Path", Species = "Species"};
         var species = A.Fake<Species>().WithName(_speciesDatabaseMap.Species);
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {species});
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(species)).Returns(_displayName);

      }

      protected override void Because()
      {
         _result = sut.MapFrom(_speciesDatabaseMap);
      }

      [Observation]
      public void should_return_a_dto_whose_species_name_was_set_to_the_name_of_the_species()
      {
         _result.SpeciesName.ShouldBeEqualTo(_speciesDatabaseMap.Species);
      }

      [Observation]
      public void should_have_set_the_species_display_name_of_the_dto_to_the_one_defined_in_the_application()
      {
         _result.SpeciesDisplayName.ShouldBeEqualTo( _displayName);
      }

      [Observation]
      public void should_have_set_the_database_path_and_the_original_path_of_the_database_to_the_path_defined_in_the_map()
      {
         _result.DatabaseFullPath.ShouldBeEqualTo(_speciesDatabaseMap.DatabaseFullPath);
         _result.OriginalDatabasePathFullPath.ShouldBeEqualTo(_speciesDatabaseMap.DatabaseFullPath);
      }
   }
}	