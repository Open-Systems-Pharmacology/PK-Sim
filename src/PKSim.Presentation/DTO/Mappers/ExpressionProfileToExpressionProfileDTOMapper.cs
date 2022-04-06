using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IExpressionProfileToExpressionProfileDTOMapper : IMapper<ExpressionProfile, ExpressionProfileDTO>
   {
   }

   public class ExpressionProfileToExpressionProfileDTOMapper : IExpressionProfileToExpressionProfileDTOMapper
   {
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IUsedMoleculeRepository _usedMoleculeRepository;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IMoleculePropertiesMapper _moleculePropertiesMapper;
      private readonly IUsedExpressionProfileCategoryRepository _usedExpressionProfileCategoryRepository;

      public ExpressionProfileToExpressionProfileDTOMapper(
         ISpeciesRepository speciesRepository,
         IUsedMoleculeRepository usedMoleculeRepository,
         IPKSimProjectRetriever projectRetriever,
         IMoleculePropertiesMapper moleculePropertiesMapper,
         IUsedExpressionProfileCategoryRepository usedExpressionProfileCategoryRepository)
      {
         _speciesRepository = speciesRepository;
         _usedMoleculeRepository = usedMoleculeRepository;
         _projectRetriever = projectRetriever;
         _moleculePropertiesMapper = moleculePropertiesMapper;
         _usedExpressionProfileCategoryRepository = usedExpressionProfileCategoryRepository;
      }

      public ExpressionProfileDTO MapFrom(ExpressionProfile expressionProfile)
      {
         var dto = new ExpressionProfileDTO
         {
            Icon = _moleculePropertiesMapper.MoleculeIconFor(expressionProfile.Molecule),
            Species = expressionProfile.Species,
            Category = expressionProfile.Category,
            MoleculeName = moleculeNameFor(expressionProfile),
            AllMolecules = _usedMoleculeRepository.All(),
            AllCategories = _usedExpressionProfileCategoryRepository.All(),
            AllSpecies = _speciesRepository.All(),
            MoleculeType = _moleculePropertiesMapper.MoleculeDisplayFor(expressionProfile.Molecule),
         };

         dto.AddExistingExpressionProfileNames(_projectRetriever.Current.All<ExpressionProfile>().AllNames().Except(new[] {expressionProfile.Name}));
         return dto;
      }

      private string moleculeNameFor(ExpressionProfile expressionProfile)
      {
         return string.Equals(expressionProfile.MoleculeName, CoreConstants.DEFAULT_EXPRESSION_PROFILE_MOLECULE_NAME) ? string.Empty : expressionProfile.MoleculeName;
      }
   }
}