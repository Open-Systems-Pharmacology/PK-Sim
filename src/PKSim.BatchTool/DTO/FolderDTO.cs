using System.IO;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.BatchTool.DTO
{
   public class FolderDTO : ValidatableDTO
   {
      private string _folder;

      public string Folder
      {
         get => _folder;
         set
         {
            _folder = value;
            OnPropertyChanged(() => Folder);
         }
      }

      public FolderDTO()
      {
         Rules.Add(AllRules.FolderDefined);
         Rules.Add(AllRules.FolderExists);
      }

      private static class AllRules
      {
         public static IBusinessRule FolderDefined { get; } = GenericRules.NonEmptyRule<FolderDTO>(x => x.Folder);

         public static IBusinessRule FolderExists { get; } = CreateRule.For<FolderDTO>()
            .Property(item => item.Folder)
            .WithRule((dto, folder) =>
            {
               if (string.IsNullOrEmpty(folder))
                  return false;

               new DirectoryInfo(folder);
               return true;
            })
            .WithError(PKSimConstants.Error.ValueIsRequired);
      }
   }
}