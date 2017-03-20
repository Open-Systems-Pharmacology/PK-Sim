using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using OSPSuite.Assets;
using OSPSuite.Utility;

namespace PKSim.Presentation.Presenters
{
   public interface IUserSettingsPresenter : IPresenter<IUserSettingsView>, ISettingsItemPresenter
   {
      IEnumerable<string> AvailableSkins { get; }
      IEnumerable<IconSize> AvailableIconSizes { get; }

      /// <summary>
      ///    Select an existing template database
      /// </summary>
      void SelectTemplateDatabase();

      /// <summary>
      ///    Create a new template database
      /// </summary>
      void CreateTemplateDatabase();

      /// <summary>
      ///    All available species name
      /// </summary>
      IEnumerable<string> AllSpecies();

      IEnumerable<string> AllSpeciesDisplayName();

      void SpeciesChanged();
      IEnumerable<string> AllPopulationsFor(string speciesName);
      IEnumerable<string> AllPopulationsDisplayName(string speciesName);
      IEnumerable<ParameterGroupingModeId> AllParameterGroupingMode();
      IEnumerable<string> AllParameterGroupingModeDisplay();

      /// <summary>
      ///    Resets the layout
      /// </summary>
      void ResetLayout();

      /// <summary>
      ///    All available population analysis type
      /// </summary>
      IEnumerable<PopulationAnalysisType> AllPopulationAnalyses();

      /// <summary>
      ///    returns the text that should be displayed for the given <paramref name="populationAnalysisType" />
      /// </summary>
      string PopulationAnalysesDisplayFor(PopulationAnalysisType populationAnalysisType);

      /// <summary>
      ///    returns the name of the icon that should be displayed for the given <paramref name="populationAnalysisType" />
      /// </summary>
      string PopulationIconNameFor(PopulationAnalysisType populationAnalysisType);

      IEnumerable<ViewLayout> AllViewLayouts();
      IEnumerable<Scalings> AllScalings();
   }

   public class UserSettingsPresenter : AbstractSubPresenter<IUserSettingsView, IUserSettingsPresenter>, IUserSettingsPresenter
   {
      private readonly ISkinManager _skinManager;
      private readonly IUserSettings _userSettings;
      private readonly IUserSettingsPersistor _userSettingsPersistor;
      private readonly IDialogCreator _dialogCreator;
      private readonly IPKSimConfiguration _configuration;
      private readonly ISpeciesRepository _speciesRepository;

      public UserSettingsPresenter(IUserSettingsView view, IUserSettings userSettings,
         ISkinManager skinManager, IUserSettingsPersistor userSettingsPersistor,
         IDialogCreator dialogCreator, IPKSimConfiguration configuration, ISpeciesRepository speciesRepository) : base(view)
      {
         _userSettings = userSettings;
         _skinManager = skinManager;
         _userSettingsPersistor = userSettingsPersistor;
         _dialogCreator = dialogCreator;
         _configuration = configuration;
         _speciesRepository = speciesRepository;
      }

      public void EditSettings()
      {
         _view.BindTo(_userSettings);
      }

      public IEnumerable<string> AvailableSkins
      {
         get { return _skinManager.All(); }
      }

      public IEnumerable<IconSize> AvailableIconSizes
      {
         get { return IconSizes.All(); }
      }

      public void SelectTemplateDatabase()
      {
         var dataBaseFile = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE);
         if (string.IsNullOrEmpty(dataBaseFile)) return;
         if (string.Equals(_userSettings.TemplateDatabasePath, dataBaseFile))
            return;
         _userSettings.TemplateDatabasePath = dataBaseFile;
      }

      public void CreateTemplateDatabase()
      {
         var dataBaseFile = _dialogCreator.AskForFileToSave(PKSimConstants.UI.CreateTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE);
         if (string.IsNullOrEmpty(dataBaseFile)) return;
         FileHelper.Copy(_configuration.TemplateUserDatabaseTemplatePath, dataBaseFile);
         _userSettings.TemplateDatabasePath = dataBaseFile;
      }

      public IEnumerable<string> AllSpecies()
      {
         return _speciesRepository.All().Select(x => x.Name);
      }

      public IEnumerable<string> AllSpeciesDisplayName()
      {
         return _speciesRepository.All().Select(species => species.DisplayName);
      }

      public void SpeciesChanged()
      {
         _userSettings.DefaultPopulation = defaultPopulationFor(_userSettings.DefaultSpecies);
         _view.RefreshAllIndividualList();
      }

      private string defaultPopulationFor(string speciesName)
      {
         var species = _speciesRepository.FindByName(speciesName);
         return species.Populations.First().Name;
      }

      public IEnumerable<string> AllPopulationsFor(string speciesName)
      {
         var species = _speciesRepository.FindByName(speciesName);
         return species.Populations.Select(x => x.Name);
      }

      public IEnumerable<string> AllPopulationsDisplayName(string speciesName)
      {
         var species = _speciesRepository.FindByName(speciesName);
         return species.Populations.Select(pop => pop.DisplayName);
      }

      public IEnumerable<ParameterGroupingModeId> AllParameterGroupingMode()
      {
         return ParameterGroupingModes.All().Select(x => x.Id);
      }

      public IEnumerable<string> AllParameterGroupingModeDisplay()
      {
         return ParameterGroupingModes.All().Select(x => x.DisplayName);
      }

      public void ResetLayout()
      {
         var ans = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyResetLayout, PKSimConstants.UI.Reset, PKSimConstants.UI.CancelButton);
         if (ans == ViewResult.No) return;

         _userSettings.ResetLayout();
      }

      public IEnumerable<PopulationAnalysisType> AllPopulationAnalyses()
      {
         return EnumHelper.AllValuesFor<PopulationAnalysisType>();
      }

      public string PopulationAnalysesDisplayFor(PopulationAnalysisType populationAnalysisType)
      {
         switch (populationAnalysisType)
         {
            case PopulationAnalysisType.TimeProfile:
               return PKSimConstants.UI.TimeProfile;
            case PopulationAnalysisType.BoxWhisker:
               return PKSimConstants.UI.BoxWhisker;
            case PopulationAnalysisType.Scatter:
               return PKSimConstants.UI.Scatter;
            case PopulationAnalysisType.Range:
               return PKSimConstants.UI.Range;
            default:
               return populationAnalysisType.ToString();
         }
      }

      public string PopulationIconNameFor(PopulationAnalysisType populationAnalysisType)
      {
         return $"{populationAnalysisType}Analysis";
      }

      public IEnumerable<ViewLayout> AllViewLayouts()
      {
         return ViewLayouts.All();
      }

      public IEnumerable<Scalings> AllScalings()
      {
         return EnumHelper.AllValuesFor<Scalings>();
      }

      public void SaveSettings()
      {
         _userSettingsPersistor.Save(_userSettings);
      }
   }
}