using System;
using OSPSuite.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO
{
   public class TemplateDTO
   {
      public TemplateDTO(Template template)
      {
         Template = template;
      }

      public TemplateDatabaseType DatabaseType => Template.DatabaseType;
      public string Name => Template.Name;
      public string Description => Template.Description;
      public string Version => Template.Version;
      public Template Template { get; }

      public ApplicationIcon Icon
      {
         get
         {
            switch (DatabaseType)
            {
               case TemplateDatabaseType.User:
                  return ApplicationIcons.UserSettings;
               case TemplateDatabaseType.System:
                  return ApplicationIcons.SystemSettings;
               case TemplateDatabaseType.Remote:
                  return ApplicationIcons.Up;
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
      }
   }
}