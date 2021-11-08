using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.ExpressionProfiles;
using static PKSim.Assets.PKSimConstants.UI;

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

      public ExpressionProfileToExpressionProfileDTOMapper(
         ISpeciesRepository speciesRepository,
         IUsedMoleculeRepository usedMoleculeRepository,
         IPKSimProjectRetriever projectRetriever)
      {
         _speciesRepository = speciesRepository;
         _usedMoleculeRepository = usedMoleculeRepository;
         _projectRetriever = projectRetriever;
      }

      public ExpressionProfileDTO MapFrom(ExpressionProfile expressionProfile)
      {
         var  dto =  new ExpressionProfileDTO
         {
            Icon = ApplicationIcons.IconByName(expressionProfile.Icon),
            Species = expressionProfile.Species,
            Category = expressionProfile.Category,
            MoleculeName = expressionProfile.MoleculeName,
            AllMolecules = _usedMoleculeRepository.All(),
            AllSpecies = _speciesRepository.All(),
            MoleculeType = moleculeTypeDisplayFor(expressionProfile.Molecule.MoleculeType),
         };

         dto.AddExistingExpressionProfileNames(_projectRetriever.Current.All(PKSimBuildingBlockType.ExpressionProfile).AllNames().Except(new[] {expressionProfile.Name}));
         return dto;
      }

      private string moleculeTypeDisplayFor(QuantityType moleculeType)
      {
         switch (moleculeType)
         {
            case QuantityType.Transporter:
               return TransportProtein;
            case QuantityType.Enzyme:
               return MetabolizingEnzyme;
            case QuantityType.OtherProtein:
               return ProteinBindingPartner;
            default:
               return moleculeType.ToString();
         }
      }
   }
}