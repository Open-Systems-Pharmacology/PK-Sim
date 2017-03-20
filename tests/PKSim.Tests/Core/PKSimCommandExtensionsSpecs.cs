using PKSim.Core.Commands;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_PKSimCommandExtensions : StaticContextSpecification
   {
      protected PKSimMacroCommand _macroCommand;
      protected IPKSimCommand _subCommand;

      protected override void Context()
      {
         _macroCommand = new PKSimMacroCommand();
         _macroCommand.Description = "my name is " + CoreConstants.ContainerName.NameTemplate;
         _macroCommand.ExtendedDescription = "my extended is " + CoreConstants.ContainerName.NameTemplate;

         _subCommand = A.Fake<IPKSimCommand>();
         _subCommand.Description = null;
         _subCommand.ExtendedDescription = "extended is " + CoreConstants.ContainerName.NameTemplate;

         _macroCommand.Add(_subCommand);
      }
   }

   
   public class When_replacing_the_occurence_of_the_template_keyword_in_a_command : concern_for_PKSimCommandExtensions
   {
      protected override void Because()
      {
         _macroCommand.ReplaceNameTemplateWithName("toto");
      }

      [Observation]
      public void should_replace_the_keyword_in_the_description_and_the_extended_description()
      {
         _macroCommand.Description.ShouldBeEqualTo("my name is toto");
         _macroCommand.ExtendedDescription.ShouldBeEqualTo("my extended is toto");
      }

      [Observation]
      public void should_replace_the_keywords_in_the_sub_commands()
      {
         _subCommand.ExtendedDescription.ShouldBeEqualTo("extended is toto");

      }
   }

   
   public class When_updating_the_properties_from_a_null_command : concern_for_PKSimCommandExtensions
   {
      private IPKSimCommand _command;

      protected override void Context()
      {
         base.Context();
         _command = new PKSimMacroCommand();
      }

      protected override void Because()
      {
         _command.UpdatePropertiesFrom(null);
      }

      [Observation]
      public void should_not_crash()
      {
         
      }
   }

   
   public class When_updating_the_properties_from_a_defined_command : concern_for_PKSimCommandExtensions
   {
      private IPKSimCommand _command;
      private IPKSimCommand _originalCommand;
      private IPKSimCommand _res;

      protected override void Context()
      {
         base.Context();
         _command = new PKSimMacroCommand();
         _originalCommand = new PKSimMacroCommand {BuildingBlockType = "BB", BuildingBlockName = "Name"};
      }

      protected override void Because()
      {
        _res= _command.UpdatePropertiesFrom(_originalCommand);
      }

      [Observation]
      public void should_have_updated_the_building_block_type()
      {
         _command.BuildingBlockType.ShouldBeEqualTo("BB");
      }

      [Observation]
      public void should_have_updated_the_building_name()
      {
         _command.BuildingBlockName.ShouldBeEqualTo("Name");
      }

      [Observation]
      public void should_return_the_original_command()
      {
         _res.ShouldBeEqualTo(_command);
      }
   }
}	