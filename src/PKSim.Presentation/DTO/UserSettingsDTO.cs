using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO
{
   public class UserSettingsDTO : ValidatableDTO
   {
      private uint _decimalPlace;
      private bool _allowsScientificNotation;
      private bool _shouldRestoreWorkspaceLayout;
      private bool _showUpdateNotification;
      private bool _colorGroupObservedDataFromSameFolder;
      private string _activeSkin;
      private IconSize _iconSizeTreeView;
      private IconSize _iconSizeTab;
      private IconSize _iconSizeContextMenu;
      private string _defaultSpecies;
      private string _defaultPopulation;
      private ParameterGroupingModeId _defaultParameterGroupingMode;
      private ParameterGroupingModeIdForParameterAnalyzable _defaultParameterGroupingModeForPIAndSA;
      private double _absTol;
      private double _relTol;
      private int _numberOfBins;
      private int _numberOfIndividualsPerBin;
      private int _maximumNumberOfCoresToUse;
      private uint _mruListItemCount;
      private string _templateDatabasePath;
      private Color _changedColor;
      private Color _formulaColor;
      private Color _chartDiagramBackColor;
      private Color _chartBackColor;
      private string _defaultFractionUnboundName;
      private string _defaultLipophilicityName;
      private string _defaultSolubilityName;
      private LoadTemplateWithReference _loadTemplateWithReference;
      private PopulationAnalysisType _defaultPopulationAnalysis;
      private Scalings _defaultChartYScaling;
      private ViewLayout _preferredViewLayout;

      public UserSettingsDTO()
      {
         Rules.AddRange(AllRules.All());
      }

      public virtual uint DecimalPlace
      {
         get => _decimalPlace;
         set => SetProperty(ref _decimalPlace, value);
      }

      public virtual bool AllowsScientificNotation
      {
         get => _allowsScientificNotation;
         set => SetProperty(ref _allowsScientificNotation, value);
      }

      public virtual bool ShouldRestoreWorkspaceLayout
      {
         get => _shouldRestoreWorkspaceLayout;
         set => SetProperty(ref _shouldRestoreWorkspaceLayout, value);
      }

      public virtual bool ShowUpdateNotification
      {
         get => _showUpdateNotification;
         set => SetProperty(ref _showUpdateNotification, value);
      }

      public virtual bool ColorGroupObservedDataFromSameFolder
      {
         get => _colorGroupObservedDataFromSameFolder;
         set => SetProperty(ref _colorGroupObservedDataFromSameFolder, value);
      }

      public virtual string ActiveSkin
      {
         get => _activeSkin;
         set => SetProperty(ref _activeSkin, value);
      }

      public virtual IconSize IconSizeTreeView
      {
         get => _iconSizeTreeView;
         set => SetProperty(ref _iconSizeTreeView, value);
      }

      public virtual IconSize IconSizeTab
      {
         get => _iconSizeTab;
         set => SetProperty(ref _iconSizeTab, value);
      }

      public virtual IconSize IconSizeContextMenu
      {
         get => _iconSizeContextMenu;
         set => SetProperty(ref _iconSizeContextMenu, value);
      }

      public virtual string DefaultSpecies
      {
         get => _defaultSpecies;
         set => SetProperty(ref _defaultSpecies, value);
      }

      public virtual string DefaultPopulation
      {
         get => _defaultPopulation;
         set => SetProperty(ref _defaultPopulation, value);
      }

      public virtual ParameterGroupingModeId DefaultParameterGroupingMode
      {
         get => _defaultParameterGroupingMode;
         set => SetProperty(ref _defaultParameterGroupingMode, value);
      }

      public virtual ParameterGroupingModeIdForParameterAnalyzable DefaultParameterGroupingModeForPIAndSA
      {
         get => _defaultParameterGroupingModeForPIAndSA;
         set => SetProperty(ref _defaultParameterGroupingModeForPIAndSA, value);
      }

      public virtual double AbsTol
      {
         get => _absTol;
         set => SetProperty(ref _absTol, value);
      }

      public virtual double RelTol
      {
         get => _relTol;
         set => SetProperty(ref _relTol, value);
      }

      public virtual int NumberOfBins
      {
         get => _numberOfBins;
         set => SetProperty(ref _numberOfBins, value);
      }

      public virtual int NumberOfIndividualsPerBin
      {
         get => _numberOfIndividualsPerBin;
         set => SetProperty(ref _numberOfIndividualsPerBin, value);
      }

      public virtual int MaximumNumberOfCoresToUse
      {
         get => _maximumNumberOfCoresToUse;
         set => SetProperty(ref _maximumNumberOfCoresToUse, value);
      }

      public virtual uint MRUListItemCount
      {
         get => _mruListItemCount;
         set => SetProperty(ref _mruListItemCount, value);
      }

      public virtual string TemplateDatabasePath
      {
         get => _templateDatabasePath;
         set => SetProperty(ref _templateDatabasePath, value);
      }

      public virtual Color ChangedColor
      {
         get => _changedColor;
         set => SetProperty(ref _changedColor, value);
      }

      public virtual Color FormulaColor
      {
         get => _formulaColor;
         set => SetProperty(ref _formulaColor, value);
      }

      public virtual Color ChartDiagramBackColor
      {
         get => _chartDiagramBackColor;
         set => SetProperty(ref _chartDiagramBackColor, value);
      }

      public virtual Color ChartBackColor
      {
         get => _chartBackColor;
         set => SetProperty(ref _chartBackColor, value);
      }

      public virtual string DefaultFractionUnboundName
      {
         get => _defaultFractionUnboundName;
         set => SetProperty(ref _defaultFractionUnboundName, value);
      }

      public virtual string DefaultLipophilicityName
      {
         get => _defaultLipophilicityName;
         set => SetProperty(ref _defaultLipophilicityName, value);
      }

      public virtual string DefaultSolubilityName
      {
         get => _defaultSolubilityName;
         set => SetProperty(ref _defaultSolubilityName, value);
      }

      public virtual LoadTemplateWithReference LoadTemplateWithReference
      {
         get => _loadTemplateWithReference;
         set => SetProperty(ref _loadTemplateWithReference, value);
      }

      public virtual PopulationAnalysisType DefaultPopulationAnalysis
      {
         get => _defaultPopulationAnalysis;
         set => SetProperty(ref _defaultPopulationAnalysis, value);
      }

      public virtual Scalings DefaultChartYScaling
      {
         get => _defaultChartYScaling;
         set => SetProperty(ref _defaultChartYScaling, value);
      }

      public virtual ViewLayout PreferredViewLayout
      {
         get => _preferredViewLayout;
         set => SetProperty(ref _preferredViewLayout, value);
      }

      private static class AllRules
      {
         private static IBusinessRule nonEmpty(Expression<Func<UserSettingsDTO, string>> expression) => GenericRules.NonEmptyRule(expression);

         private static IBusinessRule numberOfCoreSmallerThanNumberOfProcessor { get; } = CreateRule.For<UserSettingsDTO>()
            .Property(x => x.MaximumNumberOfCoresToUse)
            .WithRule((x, numCore) => numCore > 0 && numCore <= Environment.ProcessorCount)
            .WithError(Error.NumberOfCoreToUseShouldBeInferiorAsTheNumberOfProcessor(Environment.ProcessorCount));

         private static IBusinessRule decimalPlaceInRange { get; } = CreateRule.For<UserSettingsDTO>()
            .Property(x => x.DecimalPlace)
            .WithRule((x, decimalPlace) => decimalPlace <= 15)
            .WithError(PKSimConstants.Rules.DecimalPlaceMustBeBetween0And15);

         public static IEnumerable<IBusinessRule> All()
         {
            return new[]
            {
               nonEmpty(x => x.DefaultFractionUnboundName),
               nonEmpty(x => x.DefaultSolubilityName),
               nonEmpty(x => x.DefaultLipophilicityName),
               numberOfCoreSmallerThanNumberOfProcessor,
               decimalPlaceInRange
            };
         }
      }
   }
}