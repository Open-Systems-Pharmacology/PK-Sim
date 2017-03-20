using System.IO;
using PKSim.Assets;
using OSPSuite.Utility.Validation;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.BatchTool.Presenters
{
   public class OutputBatchDTO : ValidatableDTO
   {
      private string _outputFolder;

      public string OutputFolder
      {
         get { return _outputFolder; }
         set
         {
            _outputFolder = value;
            OnPropertyChanged(() => OutputFolder);
         }
      }

      public OutputBatchDTO()
      {
         Rules.Add(AllRules.OutputFolderExists);
         Rules.Add(AllRules.OutputFolderWellFormed);
      }

      private static class AllRules
      {
         public static IBusinessRule OutputFolderExists
         {
            get { return GenericRules.NonEmptyRule<OutputBatchDTO>(x => x.OutputFolder); }
         }

         public static IBusinessRule OutputFolderWellFormed
         {
            get
            {
               return CreateRule.For<OutputBatchDTO>()
                  .Property(item => item.OutputFolder)
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
   }

   public class InputAndOutputBatchDTO : OutputBatchDTO
   {
      private string _inputFolder;

      public string InputFolder
      {
         get { return _inputFolder; }
         set
         {
            _inputFolder = value;
            OnPropertyChanged(() => InputFolder);
         }
      }

      public InputAndOutputBatchDTO()
      {
         Rules.Add(AllRules.InputFolderExists);
      }

      private static class AllRules
      {
         public static IBusinessRule InputFolderExists
         {
            get { return GenericRules.NonEmptyRule<InputAndOutputBatchDTO>(x => x.InputFolder); }
         }
      }
   }
}