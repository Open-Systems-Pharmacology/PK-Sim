using System.Collections.Generic;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Core
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
         get { return _filePath; }
         set
         {
            _filePath = value;
            OnPropertyChanged(() => FilePath);
         }
      }

      private string _description;

      public virtual string Description
      {
         get { return _description; }
         set
         {
            _description = value;
            OnPropertyChanged(() => Description);
         }
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