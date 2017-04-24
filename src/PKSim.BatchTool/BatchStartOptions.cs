using System.IO;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.BatchTool
{
   public class BatchStartOptions : IValidatable
   {
      private static readonly string[] INPUTS = {"--input", "-i"};
      private static readonly string[] OUTPUTS = {"--output", "-o"};
      private static readonly string[] LOGS = {"--log", "-l"};
      private const string DEFAULT_LOG_FILE_NAME = "log.txt";

      public string InputFolder { get; private set; }
      public string OutputFolder { get; private set; }
      public string LogFileFullPath { get; private set; }
      public IBusinessRuleSet Rules { get; } = new BusinessRuleSet();

      public static BatchStartOptions From(string[] args)
      {
         var options = new BatchStartOptions();
         options.Parse(args);
         return options;
      }

      public BatchStartOptions()
      {
         Rules.AddRange(new []
         {
            AllRules.InputFolderDefined,
            AllRules.OutputFolderDefined,
            AllRules.LogFileDefined,
            AllRules.InputFolderExists,
         });
      }

      public void Parse(string[] args)
      {
         for (int i = 0; i < args.Length - 1; i++)
         {
            var value = args[i + 1];
            if (args[i].IsOneOf(INPUTS))
               updateInputFolderFrom(value);

            else if (args[i].IsOneOf(OUTPUTS))
               updateOutputFolderFrom(value);

            else if (args[i].IsOneOf(LOGS))
               updateLogFolderFrom(value);
         }
      }

      private void updateLogFolderFrom(string logFileFullPath)
      {
         LogFileFullPath = logFileFullPath;
      }

      private void updateOutputFolderFrom(string outputFolder)
      {
         OutputFolder = outputFolder;
         if (string.IsNullOrEmpty(LogFileFullPath))
         {
            LogFileFullPath = Path.Combine(OutputFolder, DEFAULT_LOG_FILE_NAME);
         }
      }

      private void updateInputFolderFrom(string inputFolder)
      {
         InputFolder = inputFolder;
      }

      private static class AllRules
      {
         public static IBusinessRule OutputFolderDefined { get; } = GenericRules.NonEmptyRule<BatchStartOptions>(x => x.OutputFolder);
         public static IBusinessRule InputFolderDefined { get; } = GenericRules.NonEmptyRule<BatchStartOptions>(x => x.InputFolder);
         public static IBusinessRule LogFileDefined { get; } = GenericRules.NonEmptyRule<BatchStartOptions>(x => x.LogFileFullPath);
         public static IBusinessRule InputFolderExists { get; } = CreateRule.For<BatchStartOptions>()
            .Property(item => item.InputFolder)
            .WithRule((dto, folder) =>
            {
               if (string.IsNullOrEmpty(folder))
                  return false;

               return DirectoryHelper.DirectoryExists(folder);
            })
            .WithError(PKSimConstants.Error.ValueIsRequired);
      }
   }
}