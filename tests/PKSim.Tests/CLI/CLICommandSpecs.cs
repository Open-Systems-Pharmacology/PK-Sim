using CommandLine;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.CLI.Commands;
using PKSim.CLI.Core.MinimalImplementations;

namespace PKSim.CLI
{
   public class TestCommand : CLICommand<object>
   {
      public override string Name => "Test";
      public override object ToRunOptions() => new object();
   }

   public abstract class concern_for_CLICommand : ContextSpecification<TestCommand>
   {
      protected CLIUserSettings _userSettings;
      protected int _defaultNumberOfCores;

      protected override void Context()
      {
         _userSettings = new CLIUserSettings();
         _defaultNumberOfCores = _userSettings.MaximumNumberOfCoresToUse;
      }

      protected static TestCommand Parse(params string[] args)
      {
         TestCommand command = null;
         Parser.Default.ParseArguments<TestCommand>(args).WithParsed(x => command = x);
         command.ShouldNotBeNull();
         return command;
      }
   }

   public class When_parsing_a_command_with_the_cores_option : concern_for_CLICommand
   {
      protected override void Because()
      {
         sut = Parse("--cores", "3");
         sut.ApplyCoresTo(_userSettings);
      }

      [Observation]
      public void should_use_the_given_number_of_cores()
      {
         _userSettings.MaximumNumberOfCoresToUse.ShouldBeEqualTo(3);
      }
   }

   public class When_parsing_a_command_without_the_cores_option : concern_for_CLICommand
   {
      protected override void Because()
      {
         sut = Parse();
         sut.ApplyCoresTo(_userSettings);
      }

      //a standalone CLI invocation owns the whole machine by default
      [Observation]
      public void should_keep_the_default_number_of_cores()
      {
         sut.NumberOfCores.HasValue.ShouldBeFalse();
         _userSettings.MaximumNumberOfCoresToUse.ShouldBeEqualTo(_defaultNumberOfCores);
      }
   }

   public class When_parsing_a_command_with_a_cores_option_below_one : concern_for_CLICommand
   {
      protected override void Because()
      {
         sut = Parse("--cores", "0");
         sut.ApplyCoresTo(_userSettings);
      }

      [Observation]
      public void should_use_at_least_one_core()
      {
         _userSettings.MaximumNumberOfCoresToUse.ShouldBeEqualTo(1);
      }
   }
}
