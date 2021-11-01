using System;
using OSPSuite.Assets;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO
{
   public class TemplateDTO
   {
      public TemplateDTO(Template template)
      {
         Template = template;
         RemoteTemplate = template as RemoteTemplate;
      }

      public Template Template { get; }

      //May be null.
      public RemoteTemplate RemoteTemplate { get; }

      public string DatabaseTypeDisplay
      {
         get
         {
            switch (Template.DatabaseType)
            {
               case TemplateDatabaseType.User:
                  return PKSimConstants.UI.UserTemplates;
               case TemplateDatabaseType.System:
                  return PKSimConstants.UI.SystemTemplates;
               case TemplateDatabaseType.Remote:
                  return PKSimConstants.UI.RemoteTemplates;
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
      }

      public string Name => Template.Name;

      public TemplateDatabaseType DatabaseType => Template.DatabaseType;

      public string Description => Template.Description;

      public string Version => RemoteTemplate?.Version;

      public string Url => RemoteTemplate?.RepositoryUrl;

      public ApplicationIcon Icon
      {
         get
         {
            switch (Template.DatabaseType)
            {
               case TemplateDatabaseType.User:
                  return ApplicationIcons.Population;
               case TemplateDatabaseType.System:
                  return ApplicationIcons.Settings;
               case TemplateDatabaseType.Remote:
                  return ApplicationIcons.Up;
               default:
                  throw new ArgumentOutOfRangeException();
            }
         }
      }
   }
}