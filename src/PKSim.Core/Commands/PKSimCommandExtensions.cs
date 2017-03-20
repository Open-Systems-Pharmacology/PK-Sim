using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Commands
{
   public static class PKSimCommandExtensions
   {
      /// <summary>
      ///    Replace the occurence of the term Template with the given name
      /// </summary>
      public static void ReplaceNameTemplateWithName(this IPKSimCommand command, string name)
      {
         command.replaceTemplateWithValue(CoreConstants.ContainerName.NameTemplate, name);
      }

      public static T UpdatePropertiesFrom<T>(this T command, IPKSimCommand originalCommand) where T : IPKSimCommand
      {
         if (originalCommand == null) return command;
         command.BuildingBlockName = originalCommand.BuildingBlockName;
         command.BuildingBlockType = originalCommand.BuildingBlockType;
         return command;
      }

      public static void ReplaceTypeTemplateWithType(this IPKSimCommand command, string type)
      {
         command.replaceTemplateWithValue(CoreConstants.ContainerName.TypeTemplate, type);
      }

      private static void replaceTemplateWithValue(this IPKSimCommand command, string template, string value)
      {
         command.Description = replaceIn(command.Description, template, value);
         command.ExtendedDescription = replaceIn(command.ExtendedDescription, template, value);
         command.BuildingBlockName = replaceIn(command.BuildingBlockName, template, value);
         command.BuildingBlockType = replaceIn(command.BuildingBlockType, template, value);

         var macroCommand = command as IPKSimMacroCommand;
         if (macroCommand == null) return;
         macroCommand.All().Each(c => c.replaceTemplateWithValue(template, value));
      }

      private static string replaceIn(string originalString, string template, string replacement)
      {
         if (!string.IsNullOrEmpty(originalString))
            originalString = originalString.Replace(template, replacement);

         return originalString;
      }
   }
}