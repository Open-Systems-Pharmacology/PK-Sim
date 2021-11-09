using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
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

      public ExpressionProfileToExpressionProfileDTOMapper(
         ISpeciesRepository speciesRepository,
         IUsedMoleculeRepository usedMoleculeRepository,
         IPKSimProjectRetriever projectRetriever,
         IMoleculePropertiesMapper moleculePropertiesMapper)
      {
         _speciesRepository = speciesRepository;
         _usedMoleculeRepository = usedMoleculeRepository;
         _projectRetriever = projectRetriever;
         _moleculePropertiesMapper = moleculePropertiesMapper;
      }

      public ExpressionProfileDTO MapFrom(ExpressionProfile expressionProfile)
      {
         var dto = new ExpressionProfileDTO
         {
            Icon = _moleculePropertiesMapper.MoleculeIconFor(expressionProfile.Molecule),
            Species = expressionProfile.Species,
            Category = expressionProfile.Category,
            MoleculeName = expressionProfile.MoleculeName,
            AllMolecules = _usedMoleculeRepository.All(),
            AllSpecies = _speciesRepository.All(),
            MoleculeType = _moleculePropertiesMapper.MoleculeDisplayFor(expressionProfile.Molecule),
         };

         dto.AddExistingExpressionProfileNames(_projectRetriever.Current.All(PKSimBuildingBlockType.ExpressionProfile).AllNames().Except(new[] {expressionProfile.Name}));
         return dto;
      }
   }
}