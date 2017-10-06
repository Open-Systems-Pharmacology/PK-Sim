using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.CLI.Commands;
using PKSim.CLI.Core;
using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool
{
   public abstract class concern_for_JsonRunOptions : ContextSpecification<JsonRunCommand>
   {
      protected List<string> _args = new List<string>();

      protected string _inputFolderThatExists = "FolderThatExists";
      protected string _inputFolderThatDoesNotExist = "FolderThatDoesNotExist";
      protected string _outputFolder = "c:\\temp";
      protected string _logFileFullPath = "c:\\toto\\log.txt";

      private Func<string, bool> _oldDirExists;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _oldDirExists = DirectoryHelper.DirectoryExists;
         DirectoryHelper.DirectoryExists = (s) => string.Equals(s, _inputFolderThatExists);
      }

      protected override void Context()
      {
      }

      protected override void Because()
      {
         Parser.Default.ParseArguments<JsonRunCommand>(_args)
            .WithParsed(opt=>sut=opt);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         DirectoryHelper.DirectoryExists = _oldDirExists;
      }

      [Observation]
      public void it_should_have_saved_the_expected_input_and_output_values()
      {
         sut.InputFolder.ShouldBeEqualTo(_inputFolderThatExists);
         sut.OutputFolder.ShouldBeEqualTo(_outputFolder);
      }

      [Observation]
      public virtual void it_should_have_the_expected_log_path()
      {
         sut.LogFileFullPath.ShouldBeEqualTo(_logFileFullPath);
      }
   }

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_full_keys : concern_for_JsonRunOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("--input");
         _args.Add(_inputFolderThatExists);
         _args.Add("--output");
         _args.Add(_outputFolder);
         _args.Add("--log");
         _args.Add(_logFileFullPath);
      }
   }

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_short_keys : concern_for_JsonRunOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("-i");
         _args.Add(_inputFolderThatExists);
         _args.Add("-o");
         _args.Add(_outputFolder);
         _args.Add("-l");
         _args.Add(_logFileFullPath);
      }
   }

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_mixed_keys : concern_for_JsonRunOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("--input");
         _args.Add(_inputFolderThatExists);
         _args.Add("-o");
         _args.Add(_outputFolder);
         _args.Add("--log");
         _args.Add(_logFileFullPath);
      }
   }

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_the_log_file_not_defined : concern_for_JsonRunOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("--input");
         _args.Add(_inputFolderThatExists);
         _args.Add("--output");
         _args.Add(_outputFolder);
      }

      public override void it_should_have_the_expected_log_path()
      {
         sut.LogFileFullPath.ShouldBeEqualTo(Path.Combine(_outputFolder, "log.txt"));
      }
   }

   public class When_parsing_export_arguments_to_the_batch_start_options : concern_for_JsonRunOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("-xj");
         _args.Add("--input");
         _args.Add(_inputFolderThatExists);
         _args.Add("-o");
         _args.Add(_outputFolder);
         _args.Add("--log");
         _args.Add(_logFileFullPath);
      }

      [Observation]
      public void should_export_to_the_expected_format()
      {
         sut.ExportMode.HasFlag(BatchExportMode.Json).ShouldBeTrue();
         sut.ExportMode.HasFlag(BatchExportMode.Xml).ShouldBeTrue();
         sut.ExportMode.HasFlag(BatchExportMode.Csv).ShouldBeFalse();
      }
   }

   public class When_parsing_notification_arguments_to_the_batch_start_options : concern_for_JsonRunOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("-w");
         _args.Add("--input");
         _args.Add(_inputFolderThatExists);
         _args.Add("-o");
         _args.Add(_outputFolder);
         _args.Add("--log");
         _args.Add(_logFileFullPath);
      }

      [Observation]
      public void should_export_to_the_expected_format()
      {
         sut.NotificationType.HasFlag(NotificationType.Warning).ShouldBeTrue();
         sut.NotificationType.HasFlag(NotificationType.Error).ShouldBeTrue();
         sut.NotificationType.HasFlag(NotificationType.Info).ShouldBeTrue();
         sut.NotificationType.HasFlag(NotificationType.Debug).ShouldBeFalse();
      }
   }

   public class When_parsing_invalid_arguments : ContextSpecification<JsonRunCommand>
   {
      protected List<string> _args = new List<string>();

      protected override void Context()
      {
         base.Context();
         _args.Add("-w");
         _args.Add("--input");
         _args.Add("HELLO");
      }


      protected override void Because()
      {
         Parser.Default.ParseArguments<JsonRunCommand>(_args)
            .WithParsed(opt => sut = opt);
      }

      [Observation]
      public void should_not_validate_the_options()
      {
         sut.ShouldBeNull();
      }
   }
}