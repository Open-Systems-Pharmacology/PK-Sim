using OSPSuite.Utility;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IUserSettingsToUserSettingsDTOMapper : IMapper<IUserSettings, UserSettingsDTO>
   {
      void UpdateUserSettingsFrom(UserSettingsDTO userSettingsDTO, IUserSettings userSettings);
   }

   public class UserSettingsToUserSettingsDTOMapper : IUserSettingsToUserSettingsDTOMapper
   {
      public UserSettingsDTO MapFrom(IUserSettings userSettings)
      {
         return new UserSettingsDTO
         {
            DecimalPlace = userSettings.DecimalPlace,
            AllowsScientificNotation = userSettings.AllowsScientificNotation,
            ShouldRestoreWorkspaceLayout = userSettings.ShouldRestoreWorkspaceLayout,
            ShowUpdateNotification = userSettings.ShowUpdateNotification,
            ColorGroupObservedDataFromSameFolder = userSettings.ColorGroupObservedDataFromSameFolder,
            ActiveSkin = userSettings.ActiveSkin,
            IconSizeTreeView = userSettings.IconSizeTreeView,
            IconSizeTab = userSettings.IconSizeTab,
            IconSizeContextMenu = userSettings.IconSizeContextMenu,
            DefaultSpecies = userSettings.DefaultSpecies,
            DefaultPopulation = userSettings.DefaultPopulation,
            DefaultParameterGroupingMode = userSettings.DefaultParameterGroupingMode,
            DefaultParameterGroupingModeForPIAndSA = userSettings.DefaultParameterGroupingModeForPIAndSA,
            AbsTol = userSettings.AbsTol,
            RelTol = userSettings.RelTol,
            NumberOfBins = userSettings.NumberOfBins,
            NumberOfIndividualsPerBin = userSettings.NumberOfIndividualsPerBin,
            MaximumNumberOfCoresToUse = userSettings.MaximumNumberOfCoresToUse,
            MRUListItemCount = userSettings.MRUListItemCount,
            TemplateDatabasePath = userSettings.TemplateDatabasePath,
            ChangedColor = userSettings.ChangedColor,
            FormulaColor = userSettings.FormulaColor,
            ChartDiagramBackColor = userSettings.ChartDiagramBackColor,
            ChartBackColor = userSettings.ChartBackColor,
            DefaultFractionUnboundName = userSettings.DefaultFractionUnboundName,
            DefaultLipophilicityName = userSettings.DefaultLipophilicityName,
            DefaultSolubilityName = userSettings.DefaultSolubilityName,
            LoadTemplateWithReference = userSettings.LoadTemplateWithReference,
            DefaultPopulationAnalysis = userSettings.DefaultPopulationAnalysis,
            DefaultChartYScaling = userSettings.DefaultChartYScaling,
            PreferredViewLayout = userSettings.PreferredViewLayout,
         };
      }

      public void UpdateUserSettingsFrom(UserSettingsDTO userSettingsDTO, IUserSettings userSettings)
      {
         userSettings.DecimalPlace = userSettingsDTO.DecimalPlace;
         userSettings.AllowsScientificNotation = userSettingsDTO.AllowsScientificNotation;
         userSettings.ShouldRestoreWorkspaceLayout = userSettingsDTO.ShouldRestoreWorkspaceLayout;
         userSettings.ShowUpdateNotification = userSettingsDTO.ShowUpdateNotification;
         userSettings.ColorGroupObservedDataFromSameFolder = userSettingsDTO.ColorGroupObservedDataFromSameFolder;
         userSettings.ActiveSkin = userSettingsDTO.ActiveSkin;
         userSettings.IconSizeTreeView = userSettingsDTO.IconSizeTreeView;
         userSettings.IconSizeTab = userSettingsDTO.IconSizeTab;
         userSettings.IconSizeContextMenu = userSettingsDTO.IconSizeContextMenu;
         userSettings.DefaultSpecies = userSettingsDTO.DefaultSpecies;
         userSettings.DefaultPopulation = userSettingsDTO.DefaultPopulation;
         userSettings.DefaultParameterGroupingMode = userSettingsDTO.DefaultParameterGroupingMode;
         userSettings.DefaultParameterGroupingModeForPIAndSA = userSettingsDTO.DefaultParameterGroupingModeForPIAndSA;
         userSettings.AbsTol = userSettingsDTO.AbsTol;
         userSettings.RelTol = userSettingsDTO.RelTol;
         userSettings.NumberOfBins = userSettingsDTO.NumberOfBins;
         userSettings.NumberOfIndividualsPerBin = userSettingsDTO.NumberOfIndividualsPerBin;
         userSettings.MaximumNumberOfCoresToUse = userSettingsDTO.MaximumNumberOfCoresToUse;
         userSettings.MRUListItemCount = userSettingsDTO.MRUListItemCount;
         userSettings.TemplateDatabasePath = userSettingsDTO.TemplateDatabasePath;
         userSettings.ChangedColor = userSettingsDTO.ChangedColor;
         userSettings.FormulaColor = userSettingsDTO.FormulaColor;
         userSettings.ChartDiagramBackColor = userSettingsDTO.ChartDiagramBackColor;
         userSettings.ChartBackColor = userSettingsDTO.ChartBackColor;
         userSettings.DefaultFractionUnboundName = userSettingsDTO.DefaultFractionUnboundName;
         userSettings.DefaultLipophilicityName = userSettingsDTO.DefaultLipophilicityName;
         userSettings.DefaultSolubilityName = userSettingsDTO.DefaultSolubilityName;
         userSettings.LoadTemplateWithReference = userSettingsDTO.LoadTemplateWithReference;
         userSettings.DefaultPopulationAnalysis = userSettingsDTO.DefaultPopulationAnalysis;
         userSettings.DefaultChartYScaling = userSettingsDTO.DefaultChartYScaling;
         userSettings.PreferredViewLayout = userSettingsDTO.PreferredViewLayout;
      }
   }
}
