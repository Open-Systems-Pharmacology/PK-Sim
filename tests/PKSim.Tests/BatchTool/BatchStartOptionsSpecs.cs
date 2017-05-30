using System;
using System.Collections.Generic;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Validation;

namespace PKSim.BatchTool
{
   public abstract class concern_for_BatchStartOptions : ContextSpecification<BatchStartOptions>
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
         sut = new BatchStartOptions();
      }

      protected override void Because()
      {
         sut.Parse(_args.ToArray());
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         DirectoryHelper.DirectoryExists = _oldDirExists;
      }

      [Observation]
      public void it_should_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue(sut.Validate().Message);
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

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_full_keys : concern_for_BatchStartOptions
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

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_short_keys : concern_for_BatchStartOptions
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

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_mixed_keys : concern_for_BatchStartOptions
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

   public class When_parsing_valid_arguments_to_the_batch_start_options_with_the_log_file_not_defined : concern_for_BatchStartOptions
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
}