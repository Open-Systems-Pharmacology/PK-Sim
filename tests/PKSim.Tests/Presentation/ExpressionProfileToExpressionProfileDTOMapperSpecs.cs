using FakeItEasy;
using OSPSuite.Assets;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExpressionProfileToExpressionProfileDTOMapper : ContextSpecification<IExpressionProfileToExpressionProfileDTOMapper>
   {
      protected ISpeciesRepository _speciesRepository;
      protected IUsedMoleculeRepository _usedMoleculeRepository;
      protected IPKSimProjectRetriever _projectRetriever;
      protected IMoleculePropertiesMapper _moleculePropertiesMapper;
      protected ExpressionProfile _expressionProfile1;

      protected override void Context()
      {
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _usedMoleculeRepository = A.Fake<IUsedMoleculeRepository>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _moleculePropertiesMapper = A.Fake<IMoleculePropertiesMapper>();

         sut = new ExpressionProfileToExpressionProfileDTOMapper(_speciesRepository, _usedMoleculeRepository, _projectRetriever, _moleculePropertiesMapper);

         A.CallTo(() => _usedMoleculeRepository.All()).Returns(new[] {"A", "B"});
         A.CallTo(() => _speciesRepository.All()).Returns(new[] {new Species {Name = "Human"}, new Species {Name = "Rat"}});

         _expressionProfile1 = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>("DOG", "CYP3A4", "Sick");

         A.CallTo(() => _projectRetriever.Current.All<ExpressionProfile>()).Returns(new[] {_expressionProfile1});
      }
   }

   public class When_mapping_an_expression_profile_to_an_expression_profile_dto : concern_for_ExpressionProfileToExpressionProfileDTOMapper
   {
      private ExpressionProfile _expressionProfile;
      private ExpressionProfileDTO _dto;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         A.CallTo(() => _moleculePropertiesMapper.MoleculeIconFor(_expressionProfile.Molecule)).Returns(ApplicationIcons.Enzyme);
         A.CallTo(() => _moleculePropertiesMapper.MoleculeDisplayFor(_expressionProfile.Molecule)).Returns("Display");
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_expressionProfile);
      }

      [Observation]
      public void should_return_a_dto_with_the_expected_icon()
      {
         _dto.Icon.ShouldBeEqualTo(ApplicationIcons.Enzyme);
      }

      [Observation]
      public void should_return_a_dto_with_the_expected_molecule_type()
      {
         _dto.MoleculeType.ShouldBeEqualTo("Display");
      }

      [Observation]
      public void should_have_mapped_the_basic_properties()
      {
         _dto.Species.ShouldBeEqualTo(_expressionProfile.Species);
         _dto.Category.ShouldBeEqualTo(_expressionProfile.Category);
      }

      [Observation]
      public void should_have_set_the_molecule_name_to_the_name_of_the_molecule()
      {
         _dto.MoleculeName.ShouldBeEqualTo(_expressionProfile.MoleculeName);
      }

      [Observation]
      public void should_have_set_the_list_of_available_molecules_to_the_one_defined_in_the_project()
      {
         _dto.AllMolecules.ShouldBeEqualTo(_usedMoleculeRepository.All());
      }

      [Observation]
      public void should_have_set_the_list_of_available_species_to_the_one_defined_in_the_db()
      {
         _dto.AllSpecies.ShouldBeEqualTo(_speciesRepository.All());
      }

      [Observation]
      public void should_have_added_the_list_of_all_existing_expression_profile_names()
      {
         //set the values of an existing one and make sure 
         _dto.MoleculeName = _expressionProfile1.MoleculeName;
         _dto.Species = _expressionProfile1.Species;
         _dto.Category = _expressionProfile1.Category;
         _dto.IsValid().ShouldBeFalse();
      }
   }

   public class When_mapping_an_expression_profile_to_an_expression_profile_dto_whose_molecule_name_was_not_initialized : concern_for_ExpressionProfileToExpressionProfileDTOMapper
   {
      private ExpressionProfile _expressionProfile;
      private ExpressionProfileDTO _dto;

      protected override void Context()
      {
         base.Context();
         _expressionProfile = DomainHelperForSpecs.CreateExpressionProfile<IndividualEnzyme>(moleculeName: CoreConstants.DEFAULT_EXPRESSION_PROFILE_MOLECULE_NAME);
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_expressionProfile);
      }

      [Observation]
      public void should_return_a_dto_with_an_empty_molecule_name()
      {
         _dto.MoleculeName.ShouldBeNullOrEmpty();
      }
   }
}