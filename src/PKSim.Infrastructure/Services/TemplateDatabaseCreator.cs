using System.IO;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class TemplateDatabaseCreator : ITemplateDatabaseCreator
   {
      private readonly IDialogCreator _dialogCreator;
      private readonly ICoreUserSettings _userSettings;
      private readonly IPKSimConfiguration _configuration;

      public TemplateDatabaseCreator(IDialogCreator dialogCreator, ICoreUserSettings userSettings, IPKSimConfiguration configuration)
      {
         _dialogCreator = dialogCreator;
         _userSettings = userSettings;
         _configuration = configuration;
      }

      public void CreateDefaultTemplateDatabase()
      {
         var userTemplateDatasePath = _userSettings.TemplateDatabasePath;

         if (FileHelper.FileExists(userTemplateDatasePath))
         {
            //Template database path exists and is NOT an old access database. Nothing to do
            if (!userTemplateDatabaseFileIsAccessDb(userTemplateDatasePath))
               return;

            _dialogCreator.MessageBoxInfo(PKSimConstants.UI.UserTemplateDatabaseDatabaseUsedOldFormatAndCannotBeLoaded(userTemplateDatasePath, CoreConstants.TEMPLATE_DATABASE_CONVERSION_WIKI_URL));
         }

         //Ensure that we set the default template path database
         _userSettings.TemplateDatabasePath = _configuration.DefaultTemplateUserDatabasePath;

         //The default file was already created in the default location. nothing to do
         if (FileHelper.FileExists(_userSettings.TemplateDatabasePath))
            return;

         FileHelper.Copy(_configuration.TemplateUserDatabaseTemplatePath, _userSettings.TemplateDatabasePath);
      }

      private bool userTemplateDatabaseFileIsAccessDb(string userTemplateDatabasePath)
      {
         var fileExtension = new FileInfo(userTemplateDatabasePath).Extension;
         return string.Equals(fileExtension, CoreConstants.Filter.MDB_EXTENSION);
      }
   }
}