using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.ContainerName;

namespace PKSim.Presentation.DTO.ExpressionProfiles
{
   public class ExpressionProfileDTO : ValidatableDTO
   {
      public ApplicationIcon Icon { get; set; }
      private string _category;
      private string _moleculeName;

      public ExpressionProfileDTO()
      {
         Rules.AddRange(ExpressionProfileRules.All());
      }

      public Species Species { get; set; }

      public virtual string Category
      {
         get => _category;
         set => SetProperty(ref _category, value.TrimmedValue());
      }

      public virtual string MoleculeName
      {
         get => _moleculeName;
         set => SetProperty(ref _moleculeName, value.TrimmedValue());
      }

      public IEnumerable<Species> AllSpecies { get; set; }

      public IEnumerable<string> AllMolecules { get; set; }

      public string MoleculeType { get; set; }

      private readonly List<string> _allExistingExpressionProfileNames = new List<string>();

      private bool moleculeSpeciesCategoryValid(string moleculeName, Species species, string category)
      {
         return !_allExistingExpressionProfileNames.Contains(ExpressionProfileName(moleculeName, species, category));
      }

      public string Name => ExpressionProfileName(MoleculeName, Species, Category);

      private static class ExpressionProfileRules
      {
         private static IBusinessRule moleculeNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.MoleculeName, PKSimConstants.Error.MoleculeIsRequired);

         private static IBusinessRule dataSourceNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.Category, PKSimConstants.Error.CategoryIsRequired);

         private static IBusinessRule speciesNotNull { get; } = GenericRules.NotNull<ExpressionProfileDTO, Species>(x => x.Species, PKSimConstants.Error.SpeciesIsRequired);

         private static IBusinessRule moleculeNameValid { get; } = CreateRule.For<ExpressionProfileDTO>()
            .Property(item => item.MoleculeName)
            .WithRule((dto, moleculeName) => dto.moleculeSpeciesCategoryValid(moleculeName, dto.Species, dto.Category))
            .WithError((dto, moleculeName) => PKSimConstants.Error.NameAlreadyExistsInContainerType(moleculeName, PKSimConstants.ObjectTypes.Project));

         private static IBusinessRule categoryValid { get; } = CreateRule.For<ExpressionProfileDTO>()
            .Property(item => item.Category)
            .WithRule((dto, category) => dto.moleculeSpeciesCategoryValid(dto.MoleculeName, dto.Species, category))
            .WithError((dto, category) => PKSimConstants.Error.NameAlreadyExistsInContainerType(category, PKSimConstants.ObjectTypes.Project));

         private static IBusinessRule speciesValid { get; } = CreateRule.For<ExpressionProfileDTO>()
            .Property(item => item.Species)
            .WithRule((dto, species) => dto.moleculeSpeciesCategoryValid(dto.MoleculeName, species, dto.Category))
            .WithError((dto, species) => PKSimConstants.Error.NameAlreadyExistsInContainerType(species.Name, PKSimConstants.ObjectTypes.Project));

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return speciesNotNull;
            yield return moleculeNotEmpty;
            yield return dataSourceNotEmpty;
            yield return moleculeNameValid;
            yield return categoryValid;
            yield return speciesValid;
         }
      }

      public void AddExistingExpressionProfileNames(IEnumerable<string> existingNames)
      {
         _allExistingExpressionProfileNames.AddRange(existingNames);
      }
   }
}