using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.ExpressionProfiles
{
   public class ExpressionProfileDTO : ValidatableDTO
   {
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

      private static class ExpressionProfileRules
      {
         private static IBusinessRule moleculeNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.MoleculeName, PKSimConstants.Error.MoleculeIsRequired);

         private static IBusinessRule dataSourceNotEmpty { get; } = GenericRules.NonEmptyRule<ExpressionProfileDTO>(x => x.Category, PKSimConstants.Error.CategoryIsRequired);

         // private static IBusinessRule moleculeNameValid
         // {
         //    get
         //    {
         //       return CreateRule.For<ExpressionProfileDTO>()
         //          .Property(item => item.MoleculeName)
         //          .WithRule((dto, proteinName) => dto.proteinDataSourceValid(proteinName, dto.DataSource))
         //          .WithError((dto, proteinName) => PKSimConstants.Error.NameAlreadyExistsInContainerType(proteinName, PKSimConstants.ObjectTypes.Compound));
         //    }
         // }
         //
         // private static IBusinessRule dataSourceValid
         // {
         //    get
         //    {
         //       return CreateRule.For<ExpressionProfileDTO>()
         //          .Property(item => item.DataSource)
         //          .WithRule((dto, dataSource) => dto.proteinDataSourceValid(dto.MoleculeName, dataSource))
         //          .WithError((dto, dataSource) => PKSimConstants.Error.NameAlreadyExistsInContainerType(dataSource, PKSimConstants.ObjectTypes.Compound));
         //    }
         // }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return moleculeNotEmpty;
            // yield return moleculeNameValid;
            yield return dataSourceNotEmpty;
            // yield return dataSourceValid;
         }
      }

   }


}