using System.Collections.Generic;
using System.Linq;
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
         set
         {
            SetProperty(ref _category, value.TrimmedValue());
            OnPropertyChanged(() => Name);
         }
      }

      public virtual string MoleculeName
      {
         get => _moleculeName;
         set
         {
            SetProperty(ref _moleculeName, value.TrimmedValue());
            OnPropertyChanged(() => Name);
         }
      }

      public IEnumerable<Species> AllSpecies { get; set; }

      public IEnumerable<string> AllMolecules { get; set; }

      public IEnumerable<string> AllCategories { get; set; }

      public string MoleculeType { get; set; }

      private readonly List<string> _allExistingExpressionProfileNames = new List<string>();

      private bool moleculeSpeciesCategoryValid(string name) => !_allExistingExpressionProfileNames.Contains(name?.TrimmedValue()?.ToLower());

      public string Name => ExpressionProfileName(MoleculeName, Species, Category);

      private static class ExpressionProfileRules
      {
         private static IBusinessRule moleculeNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.MoleculeName, PKSimConstants.Error.MoleculeIsRequired);

         private static IBusinessRule categoryNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.Category, PKSimConstants.Error.CategoryIsRequired);

         private static IBusinessRule speciesNotNull { get; } = GenericRules.NotNull<ExpressionProfileDTO, Species>(x => x.Species, PKSimConstants.Error.SpeciesIsRequired);

         private static IBusinessRule nameValid { get; } = CreateRule.For<ExpressionProfileDTO>()
            .Property(item => item.Name)
            .WithRule((dto, name) => dto.moleculeSpeciesCategoryValid(name))
            .WithError((dto, name) => PKSimConstants.Error.NameAlreadyExistsInContainerType(name, PKSimConstants.ObjectTypes.Project));

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return speciesNotNull;
            yield return moleculeNotEmpty;
            yield return categoryNotEmpty;
            yield return nameValid;
         }
      }

      public void AddExistingExpressionProfileNames(IEnumerable<string> existingNames)
      {
         _allExistingExpressionProfileNames.AddRange(existingNames.Select(x => x.ToLower()));
      }
   }
}