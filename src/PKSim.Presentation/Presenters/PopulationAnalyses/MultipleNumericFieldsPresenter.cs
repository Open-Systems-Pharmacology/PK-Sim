using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IMultipleNumericFieldsPresenter : IPopulationAnalysisNumericFieldsPresenter
   {
      void SelectionChanged(FieldSelectionDTO dto);
      void MoveUp(FieldSelectionDTO dto);
      void MoveDown(FieldSelectionDTO dto);
      void SelectedItemChanged();
      Type AllowedType { get; set; }
   }

   public class MultipleNumericFieldsPresenter : AbstractPresenter<IMultipleNumericFieldsView, IMultipleNumericFieldsPresenter>, IMultipleNumericFieldsPresenter
   {
      private readonly IEventPublisher _eventPublisher;
      private PopulationPivotAnalysis _populationAnalysis;
      private List<FieldSelectionDTO> _allFieldsSelection;
      public Type AllowedType { get; set; }

      public MultipleNumericFieldsPresenter(IMultipleNumericFieldsView view, IEventPublisher eventPublisher) : base(view)
      {
         _eventPublisher = eventPublisher;
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationAnalysis = populationAnalysis.DowncastTo<PopulationPivotAnalysis>();
      }

      public void RefreshAnalysis()
      {
         var allNumericFields = _populationAnalysis.All(AllowedType);
         var allDataFields = _populationAnalysis.AllFieldsOn(PivotArea.DataArea, AllowedType);
         _allFieldsSelection = allDataFields.Select(mapFrom).ToList();
         _allFieldsSelection.Each(dto => dto.Selected = true);
         allNumericFields.Where(f => !allDataFields.Contains(f)).Each(f => _allFieldsSelection.Add(mapFrom(f)));
         _view.BindTo(_allFieldsSelection);
      }

      public void SelectionChanged(FieldSelectionDTO dto)
      {
         var targetArea = dto.Selected ? PivotArea.DataArea : PivotArea.FilterArea;
         _populationAnalysis.SetPosition(dto.PopulationAnalysisField, targetArea, indexOf(dto));
         SelectedItemChanged();
         _eventPublisher.PublishEvent(new PopulationAnalysisDataSelectionChangedEvent(_populationAnalysis));
      }

      private int indexOf(FieldSelectionDTO dto)
      {
         return _allFieldsSelection.IndexOf(dto);
      }

      public void MoveUp(FieldSelectionDTO dto)
      {
         int currentIndex = indexOf(dto);
         if (currentIndex == 0) return;
         swapItems(currentIndex, --currentIndex);
      }

      private void updateIndex(FieldSelectionDTO dto, int index)
      {
         _populationAnalysis.SetPosition(dto.PopulationAnalysisField, _populationAnalysis.GetArea(dto.PopulationAnalysisField), index);
      }

      public void MoveDown(FieldSelectionDTO dto)
      {
         int currentIndex = indexOf(dto);
         if (currentIndex == _allFieldsSelection.Count - 1) return;
         swapItems(currentIndex, ++currentIndex);
      }

      public void SelectedItemChanged()
      {
         var selectedItem = View.SelectedItem;
         if (selectedItem == null || !selectedItem.Selected)
         {
            View.UpEnabled = false;
            View.DownEnabled = false;
            return;
         }

         int currentIndex = indexOf(selectedItem);
         View.DownEnabled = (currentIndex < _allFieldsSelection.Count - 1);
         View.UpEnabled = currentIndex > 0;
      }

      private void swapItems(int sourceIndex, int targetIndex)
      {
         var sourceItem = _allFieldsSelection[sourceIndex];
         var targetItem = _allFieldsSelection[targetIndex];
         updateIndex(sourceItem, targetIndex);
         updateIndex(targetItem, sourceIndex);
         RefreshAnalysis();
         View.SelectedItem = _allFieldsSelection[targetIndex];
         _eventPublisher.PublishEvent(new PopulationAnalysisDataSelectionChangedEvent(_populationAnalysis));
      }

      private FieldSelectionDTO mapFrom(IPopulationAnalysisField field)
      {
         return new FieldSelectionDTO(field);
      }
   }
}