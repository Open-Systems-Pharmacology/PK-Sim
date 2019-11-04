using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;

namespace PKSim.Core.Services
{
   public class FileSelection : Notifier, IValidatable
   {
      public IBusinessRuleSet Rules { get; private set; }

      public FileSelection()
      {
         Rules = new BusinessRuleSet(AllRules.All());
         FilePath = string.Empty;
      }

      private string _filePath;

      public virtual string FilePath
      {
         get => _filePath;
         set => SetProperty(ref _filePath, value);
      }

      private string _description;

      public virtual string Description
      {
         get => _description;
         set => SetProperty(ref _description, value);
      }

      private static class AllRules
      {
         private static IBusinessRule fileNotEmpty
         {
            get { return GenericRules.NonEmptyRule<FileSelection>(x => x.FilePath); }
         }

         internal static IEnumerable<IBusinessRule> All()
         {
            yield return fileNotEmpty;
         }
      }
   }
}