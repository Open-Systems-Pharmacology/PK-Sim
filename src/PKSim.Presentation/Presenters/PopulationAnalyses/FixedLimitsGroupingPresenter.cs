using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IFixedLimitsGroupingPresenter : IPresenter<IFixedLimitsGroupingView>, IGroupingDefinitionPresenter
   {
      void AddFixedLimitAfter(FixedLimitGroupingDTO fixedLimitGroupingDTO);
      void RemoveFixedLimit(FixedLimitGroupingDTO fixedLimitGroupingDTO);
      void MaximumValueChanged(FixedLimitGroupingDTO fixedLimitGroupingDTO, double? newValue);
   }

   public class FixedLimitsGroupingPresenter : AbstractPresenter<IFixedLimitsGroupingView, IFixedLimitsGroupingPresenter>, IFixedLimitsGroupingPresenter
   {
      private readonly IColorGenerator _colorGenerator;
      private readonly ISymbolGenerator _symbolGenerator;
      //use notify list to enable auto update in view
      private readonly NotifyList<FixedLimitGroupingDTO> _fixedLimitDTOs;
      private PopulationAnalysisNumericField _numericField;
      private FixedLimitsGroupingDefinition _groupingDefinition;
      private IPopulationDataCollector _populationDataCollector;

      public FixedLimitsGroupingPresenter(IFixedLimitsGroupingView view, IColorGenerator colorGenerator,ISymbolGenerator symbolGenerator) : base(view)
      {
         _colorGenerator = colorGenerator;
         _symbolGenerator = symbolGenerator;
         _fixedLimitDTOs = new NotifyList<FixedLimitGroupingDTO>();
      }

      public GroupingDefinition GroupingDefinition
      {
         get
         {
            UpdateGroupingDefinition();
            return _groupingDefinition;
         }
      }

      private IEnumerable<double> calculateLimits()
      {
         var limitsInDisplayUnits = _fixedLimitDTOs.Where(x => x.MaximumEditable).Select(x => x.Maximum.Value);
         return limitsInDisplayUnits.Select(_numericField.ValueInCoreUnit);
      }

      public void InitializeWith(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector)
      {
         _numericField = populationAnalysisField as PopulationAnalysisNumericField;

         //this should never happen
         if (_numericField == null)
            throw new ArgumentException(PKSimConstants.Error.GroupingCannotBeUsedWithFieldOfType(populationAnalysisField.DataType, GroupingDefinitions.FixedLimits.DisplayName));

         if (!_numericField.CanBeUsedForGroupingIn(populationDataCollector))
            throw new PKSimException(PKSimConstants.Error.GroupingCannotBeCreatedForField(populationAnalysisField.Name));

         _groupingDefinition = new FixedLimitsGroupingDefinition(_numericField.Name);
         _populationDataCollector = populationDataCollector;
      }

      private void updateFirstAndAddLastEntry(double? lastEntryMinimum = null)
      {
         var values = _numericField.GetValues(_populationDataCollector);

         if (_fixedLimitDTOs.Count == 0)
            _fixedLimitDTOs.Add(newDTO());

         var firstDTO = _fixedLimitDTOs[0];
         firstDTO.CanDelete = false;
         firstDTO.Minimum = _numericField.ValueInDisplayUnit(values.Min());

         double? minimumInDisplayUnit = null;
         if (lastEntryMinimum != null)
            minimumInDisplayUnit = _numericField.ValueInDisplayUnit(lastEntryMinimum.Value);

         //Last item
         _fixedLimitDTOs.Add(newDTO(canDelete: false, canAdd: false,
            maximumInDisplayUnit: _numericField.ValueInDisplayUnit(values.Max()),
            maximumEditable: false, minimumInDisplayUnit: minimumInDisplayUnit));
      }

      private void bindToView()
      {
         _view.BindTo(_fixedLimitDTOs, _numericField.DisplayUnit);
      }

      public void Edit(GroupingDefinition groupingDefinition)
      {
         _groupingDefinition = groupingDefinition.DowncastTo<FixedLimitsGroupingDefinition>();
         _fixedLimitDTOs.Clear();
         for (int i = 0; i < _groupingDefinition.Limits.Count; i++)
         {
            var dto = newDTO(maximumInDisplayUnit: _numericField.ValueInDisplayUnit(_groupingDefinition.Limits[i]));
            if (i > 0)
               dto.Minimum = _fixedLimitDTOs[i - 1].Maximum;

            _fixedLimitDTOs.Add(dto);
         }

         updateFirstAndAddLastEntry(_groupingDefinition.Limits.Last());
         _fixedLimitDTOs.Each((dto, i) => dto.UpdateFrom(_groupingDefinition.Items[i]));

         bindToView();
      }

      public void UpdateGroupingDefinition()
      {
         _groupingDefinition.Items.Clear();
         _groupingDefinition.SetLimits(calculateLimits().OrderBy(x => x));
         _groupingDefinition.AddItems(_fixedLimitDTOs.Select(x => x.ToGroupingItem()));
      }

      public void StartCreate()
      {
         updateFirstAndAddLastEntry();
         bindToView();
      }

      public void AddFixedLimitAfter(FixedLimitGroupingDTO fixedLimitGroupingDTO)
      {
         if (fixedLimitGroupingDTO == null)
            return;

         //Insert a new item right after the given dto using the maximum of the previous one as minimum (already in display unit)
         var newLimit = newDTO(minimumInDisplayUnit: fixedLimitGroupingDTO.Maximum);

         _fixedLimitDTOs.Insert(_fixedLimitDTOs.IndexOf(fixedLimitGroupingDTO) + 1, newLimit);
         MaximumValueChanged(newLimit, newLimit.Maximum);
      }

      private FixedLimitGroupingDTO newDTO(double? minimumInDisplayUnit = null, double? maximumInDisplayUnit = null, bool canAdd = true, bool canDelete = true, bool maximumEditable = true)
      {
         return new FixedLimitGroupingDTO
         {
            CanAdd = canAdd,
            CanDelete = canDelete,
            Maximum = maximumInDisplayUnit,
            Minimum = minimumInDisplayUnit,
            MaximumEditable = maximumEditable,
            Color = _colorGenerator.NextColor(),
            Symbol = _symbolGenerator.NextSymbol(),
         };
      }

      public void RemoveFixedLimit(FixedLimitGroupingDTO fixedLimitGroupingDTO)
      {
         if (fixedLimitGroupingDTO == null) return;

         var index = _fixedLimitDTOs.IndexOf(fixedLimitGroupingDTO);
         _fixedLimitDTOs.Remove(fixedLimitGroupingDTO);

         //We have to update the maximum of the entry  after the one deleted with the maximum of the entry before the one deleted
         //hence -1
         var dto = _fixedLimitDTOs[index - 1];
         MaximumValueChanged(dto, dto.Maximum);
      }

      public void MaximumValueChanged(FixedLimitGroupingDTO fixedLimitGroupingDTO, double? newMaximum)
      {
         fixedLimitGroupingDTO.Maximum = newMaximum;
         var nextItem = _fixedLimitDTOs[_fixedLimitDTOs.IndexOf(fixedLimitGroupingDTO) + 1];
         nextItem.Minimum = newMaximum;
      }
   }
}