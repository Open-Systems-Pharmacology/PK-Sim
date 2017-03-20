using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility;

namespace PKSim.Presentation
{
   public abstract class concern_for_StartOptions : ContextSpecification<StartOptions>
   {
      protected IList<string> _args;
      private Func<string, bool> _oldFileExists;
      protected string _fileThatExists;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _fileThatExists = "c:\\i_am_alive.txt";
         _oldFileExists = FileHelper.FileExists;
         FileHelper.FileExists = x => string.Equals(x, _fileThatExists);
         sut = new StartOptions();
      }

      protected override void Context()
      {
         _args = new List<string>();
      }

      protected override void Because()
      {
         sut.InitializeFrom(_args.ToArray());
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.FileExists = _oldFileExists;
      }
   }

   public class When_initializing_the_start_options_with_the_arguments_to_open_an_existing_project_file_from_shell : concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add(_fileThatExists);
      }

      [Observation]
      public void shoud_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_option_mode_to_project()
      {
         sut.StartOptionMode.ShouldBeEqualTo(StartOptionMode.Project);
      }

      [Observation]
      public void should_not_start_the_app_in_developer_mode()
      {
         sut.IsDeveloperMode.ShouldBeFalse();
      }
   }

   public class When_initializing_the_start_options_with_the_required_arguments_to_open_an_existing_project_file : concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("/p");
         _args.Add(_fileThatExists);
      }

      [Observation]
      public void shoud_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_option_mode_to_project()
      {
         sut.StartOptionMode.ShouldBeEqualTo(StartOptionMode.Project);
      }

      [Observation]
      public void should_not_start_the_app_in_developer_mode()
      {
         sut.IsDeveloperMode.ShouldBeFalse();
      }
   }

   public class When_initializing_the_start_options_with_the_required_arguments_to_open_a_simulatiion_file_pop : concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("/pop");
         _args.Add(_fileThatExists);
         //add the flag after the population file
         _args.Add("/dev");
      }

      [Observation]
      public void shoud_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_option_mode_to_population()
      {
         sut.StartOptionMode.ShouldBeEqualTo(StartOptionMode.Population);
      }

      [Observation]
      public void should_start_the_app_in_developer_mode()
      {
         sut.IsDeveloperMode.ShouldBeTrue();
      }
   }

   public class When_initializing_the_start_options_with_the_required_arguments_to_open_a_simulatiion_file_pop_as_developer : concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("/pop");
         _args.Add(_fileThatExists);
      }

      [Observation]
      public void shoud_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_option_mode_to_population()
      {
         sut.StartOptionMode.ShouldBeEqualTo(StartOptionMode.Population);
      }

      [Observation]
      public void should_not_start_the_app_in_developer_mode()
      {
         sut.IsDeveloperMode.ShouldBeFalse();
      }
   }

   public class When_initializing_the_start_options_with_the_required_arguments_to_open_a_journal : concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("/j");
         _args.Add(_fileThatExists);
      }

      [Observation]
      public void shoud_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_option_mode_to_journal()
      {
         sut.StartOptionMode.ShouldBeEqualTo(StartOptionMode.Journal);
      }

   }

   public class When_initializing_the_start_options_with_the_required_arguments_to_open_a_journal_as_developer: concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         //adds the flag before the journal file
         _args.Add("--dev");
         _args.Add("/j");
         _args.Add(_fileThatExists);
      }

      [Observation]
      public void shoud_return_a_valid_state()
      {
         sut.IsValid().ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_the_option_mode_to_journal()
      {
         sut.StartOptionMode.ShouldBeEqualTo(StartOptionMode.Journal);
      }

      [Observation]
      public void should_start_the_app_in_developer_mode()
      {
         sut.IsDeveloperMode.ShouldBeTrue();
      }
   }

   public class When_initializing_the_start_options_with_the_required_arguments_to_start_as_developer : concern_for_StartOptions
   {
      protected override void Context()
      {
         base.Context();
         _args.Add("--dev");
      }

      [Observation]
      public void should_start_the_app_in_developer_mode()
      {
         sut.IsDeveloperMode.ShouldBeTrue();
      }
   }
}