using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core;
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

      private bool moleculeCategoryValid(string moleculeName, string category)
      {
         return !_allExistingExpressionProfileNames.Contains(ExpressionProfileName(moleculeName, category));
      }

      public string Name => CoreConstants.CompositeNameFor(MoleculeName, Category);

      private static class ExpressionProfileRules
      {
         private static IBusinessRule moleculeNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.MoleculeName, PKSimConstants.Error.MoleculeIsRequired);

         private static IBusinessRule dataSourceNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.Category, PKSimConstants.Error.CategoryIsRequired);

         private static IBusinessRule moleculeNameValid { get; } = CreateRule.For<ExpressionProfileDTO>()
            .Property(item => item.MoleculeName)
            .WithRule((dto, moleculeName) => dto.moleculeCategoryValid(moleculeName, dto.Category))
            .WithError((dto, moleculeName) => PKSimConstants.Error.NameAlreadyExistsInContainerType(moleculeName, PKSimConstants.ObjectTypes.Project));

         private static IBusinessRule categoryValid { get; } = CreateRule.For<ExpressionProfileDTO>()
            .Property(item => item.Category)
            .WithRule((dto, category) => dto.moleculeCategoryValid(dto.MoleculeName, category))
            .WithError((dto, category) => PKSimConstants.Error.NameAlreadyExistsInContainerType(category, PKSimConstants.ObjectTypes.Project));

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return moleculeNotEmpty;
            yield return moleculeNameValid;
            yield return dataSourceNotEmpty;
            yield return categoryValid;
         }
      }

      public void AddExistingExpressionProfileNames(IEnumerable<string> existingNames)
      {
         _allExistingExpressionProfileNames.AddRange(existingNames);
      }
   }
}