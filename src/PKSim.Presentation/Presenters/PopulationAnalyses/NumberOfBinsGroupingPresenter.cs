using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface INumberOfBinsGroupingPresenter : IPresenter<INumberOfBinsGroupingView>, IGroupingDefinitionPresenter
   {
      /// <summary>
      ///    This method is called whenever the user change the any feature that would require a generation of labels
      /// </summary>
      void GenerateLabels();

      IEnumerable<LabelGenerationStrategy> AvailableStrategies { get; }
      IEnumerable<Symbols> AvailableSymbols { get; }
      string NamingPatternDescription { get; }
      string NamingPatternDescriptionToolTip { get; }
   }

   public class NumberOfBinsGroupingPresenter : AbstractPresenter<INumberOfBinsGroupingView, INumberOfBinsGroupingPresenter>, INumberOfBinsGroupingPresenter
   {
      private readonly IGroupingLabelGenerator _groupingLabelGenerator;
      private readonly IColorGradientGenerator _colorGradientGenerator;
      private readonly BinSizeGroupingDTO _binSizeGroupingDTO;
      private PopulationAnalysisNumericField _numericField;
      private IPopulationDataCollector _populationDataCollector;
      private NumberOfBinsGroupingDefinition _groupingDefinition;

      public NumberOfBinsGroupingPresenter(INumberOfBinsGroupingView view, IGroupingLabelGenerator groupingLabelGenerator, IColorGradientGenerator colorGradientGenerator) : base(view)
      {
         _groupingLabelGenerator = groupingLabelGenerator;
         _colorGradientGenerator = colorGradientGenerator;
         _binSizeGroupingDTO = new BinSizeGroupingDTO();
      }

      public GroupingDefinition GroupingDefinition
      {
         get
         {
            UpdateGroupingDefinition();
            return _groupingDefinition;
         }
      }

      private GroupingItem groupingItemFromLabel(GroupingItemDTO groupingLabelDTO)
      {
         var groupingItem = groupingLabelDTO.ToGroupingItem();
         groupingItem.Symbol = _binSizeGroupingDTO.Symbol;
         return groupingItem;
      }

      public void InitializeWith(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector)
      {
         _numericField = populationAnalysisField as PopulationAnalysisNumericField;

         //this should never happen
         if (_numericField == null)
            throw new ArgumentException(PKSimConstants.Error.GroupingCannotBeUsedWithFieldOfType(populationAnalysisField.DataType, GroupingDefinitions.NumberOfBins.DisplayName));

         if (!_numericField.CanBeUsedForGroupingIn(populationDataCollector))
            throw new PKSimException(PKSimConstants.Error.GroupingCannotBeCreatedForField(populationAnalysisField.Name));

         _populationDataCollector = populationDataCollector;

         _groupingDefinition = new NumberOfBinsGroupingDefinition(_numericField.Name);
      }

      private void bindToView()
      {
         _view.BindTo(_binSizeGroupingDTO);
      }

      public void Edit(GroupingDefinition groupingDefinition)
      {
         _groupingDefinition = groupingDefinition.DowncastTo<NumberOfBinsGroupingDefinition>();
         _binSizeGroupingDTO.NumberOfBins = _groupingDefinition.NumberOfBins;
         _binSizeGroupingDTO.StartColor = _groupingDefinition.StartColor;
         _binSizeGroupingDTO.EndColor = _groupingDefinition.EndColor;
         _binSizeGroupingDTO.NamingPattern = _groupingDefinition.NamingPattern;
         _binSizeGroupingDTO.Strategy = _groupingDefinition.Strategy;

         foreach (var groupingItem in _groupingDefinition.Items)
         {
            var label = new GroupingItemDTO();
            label.UpdateFrom(groupingItem);
            _binSizeGroupingDTO.Labels.Add(label);
         }
         _binSizeGroupingDTO.Symbol = _binSizeGroupingDTO.Labels.First().Symbol;
         bindToView();
      }

      public void UpdateGroupingDefinition()
      {
         updateFromDto(_groupingDefinition);
      }

      public void StartCreate()
      {
         bindToView();
         GenerateLabels();
      }

      private void updateFromDto(NumberOfBinsGroupingDefinition groupingDefinition)
      {
         groupingDefinition.Items.Clear();
         groupingDefinition.NumberOfBins = _binSizeGroupingDTO.NumberOfBins;
         groupingDefinition.StartColor = _binSizeGroupingDTO.StartColor;
         groupingDefinition.EndColor = _binSizeGroupingDTO.EndColor;
         groupingDefinition.NamingPattern = _binSizeGroupingDTO.NamingPattern;
         groupingDefinition.Strategy = _binSizeGroupingDTO.Strategy;
         groupingDefinition.AddItems(_binSizeGroupingDTO.Labels.Select(groupingItemFromLabel));
      }

      public void GenerateLabels()
      {
         generateAutomaticLabels();
         _view.RefreshLabels();
         ViewChanged();
      }

      private void generateAutomaticLabels()
      {
         var labels = _binSizeGroupingDTO.Labels;
         labels.Clear();
         var newLabels = _groupingLabelGenerator.GenerateLabels(_populationDataCollector, _numericField, createOptions(),_binSizeGroupingDTO.NumberOfBins);
         var gradients = _colorGradientGenerator.GenerateGradient(_binSizeGroupingDTO.StartColor, _binSizeGroupingDTO.EndColor, _binSizeGroupingDTO.NumberOfBins);
         newLabels.Each((label, i) => labels.Add(new GroupingItemDTO {Label = label, Color = gradients[i]}));
      }

      private LabelGenerationOptions createOptions()
      {
         return new LabelGenerationOptions
         {
            Pattern = _binSizeGroupingDTO.NamingPattern,
            Strategy = _binSizeGroupingDTO.Strategy
         };
      }
     
      public IEnumerable<LabelGenerationStrategy> AvailableStrategies
      {
         get { return LabelGenerationStrategies.All(); }
      }

      public IEnumerable<Symbols> AvailableSymbols
      {
         get { return EnumHelper.AllValuesFor<Symbols>(); }
      }

      public string NamingPatternDescription
      {
         get { return PKSimConstants.UI.NamingPatternDescription(LabelGenerationOptions.ITERATION_PATTERN, LabelGenerationOptions.START_INTERVAL_PATTERN, LabelGenerationOptions.END_INTERVAL_PATTERN); }
      }

      public string NamingPatternDescriptionToolTip
      {
         get { return PKSimConstants.UI.NamingPatternDescriptionToolTip(LabelGenerationOptions.ITERATION_PATTERN, LabelGenerationOptions.START_INTERVAL_PATTERN, LabelGenerationOptions.END_INTERVAL_PATTERN); }
      }
   }
}