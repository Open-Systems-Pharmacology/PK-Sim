using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views;

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
      IReadOnlyList<ParameterGroupingMode> AllParameterGroupingMode();
      IReadOnlyList<ParameterGroupingModeForParameterAnalyzable> AllParameterGroupingModeForPIAndSA();

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

      IReadOnlyList<ViewLayout> AllViewLayouts();
      IReadOnlyList<Scalings> AllScalings();
   }

   public class UserSettingsPresenter : AbstractSubPresenter<IUserSettingsView, IUserSettingsPresenter>, IUserSettingsPresenter
   {
      private readonly ISkinManager _skinManager;
      private readonly IUserSettings _userSettings;
      private readonly IUserSettingsPersistor _userSettingsPersistor;
      private readonly IDialogCreator _dialogCreator;
      private readonly IPKSimConfiguration _configuration;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IUserSettingsToUserSettingsDTOMapper _mapper;
      private UserSettingsDTO _userSettingsDTO;

      public UserSettingsPresenter(IUserSettingsView view, IUserSettings userSettings,
         ISkinManager skinManager, IUserSettingsPersistor userSettingsPersistor,
         IDialogCreator dialogCreator, IPKSimConfiguration configuration, ISpeciesRepository speciesRepository,
         IUserSettingsToUserSettingsDTOMapper mapper) : base(view)
      {
         _userSettings = userSettings;
         _skinManager = skinManager;
         _userSettingsPersistor = userSettingsPersistor;
         _dialogCreator = dialogCreator;
         _configuration = configuration;
         _speciesRepository = speciesRepository;
         _mapper = mapper;
      }

      public void EditSettings()
      {
         _userSettingsDTO = _mapper.MapFrom(_userSettings);
         _view.BindTo(_userSettingsDTO);
      }

      public IEnumerable<string> AvailableSkins => _skinManager.All();

      public IEnumerable<IconSize> AvailableIconSizes => IconSizes.All();

      public void SelectTemplateDatabase()
      {
         var dataBaseFile = _dialogCreator.AskForFileToOpen(PKSimConstants.UI.SelectTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE);
         if (string.IsNullOrEmpty(dataBaseFile)) return;
         if (string.Equals(_userSettingsDTO.TemplateDatabasePath, dataBaseFile))
            return;
         _userSettingsDTO.TemplateDatabasePath = dataBaseFile;
      }

      public void CreateTemplateDatabase()
      {
         var dataBaseFile = _dialogCreator.AskForFileToSave(PKSimConstants.UI.CreateTemplateDatabasePath, CoreConstants.Filter.TEMPLATE_DATABASE_FILE_FILTER, CoreConstants.DirectoryKey.DATABASE);
         if (string.IsNullOrEmpty(dataBaseFile)) return;
         FileHelper.Copy(_configuration.TemplateUserDatabaseTemplatePath, dataBaseFile);
         _userSettingsDTO.TemplateDatabasePath = dataBaseFile;
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
         _userSettingsDTO.DefaultPopulation = defaultPopulationFor(_userSettingsDTO.DefaultSpecies);
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

      public IReadOnlyList<ParameterGroupingModeForParameterAnalyzable> AllParameterGroupingModeForPIAndSA()
      {
         return ParameterGroupingModesForParameterAnalyzable.All();
      }

      public IReadOnlyList<ParameterGroupingMode> AllParameterGroupingMode()
      {
         return ParameterGroupingModes.All();
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

      public IReadOnlyList<ViewLayout> AllViewLayouts()
      {
         return ViewLayouts.All().ToList();
      }

      public IReadOnlyList<Scalings> AllScalings()
      {
         return EnumHelper.AllValuesFor<Scalings>().ToList();
      }

      public void SaveSettings()
      {
         _mapper.UpdateUserSettingsFrom(_userSettingsDTO, _userSettings);
         _userSettingsPersistor.Save(_userSettings);
      }
   }
}