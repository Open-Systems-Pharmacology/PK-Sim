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
   public interface IXAndYNumericFieldsPresenter : IPopulationAnalysisNumericFieldsPresenter
   {
      IEnumerable<IPopulationAnalysisField> AllAvailableFields();
      string DisplayFor(IPopulationAnalysisField field);
      void FieldSelectionChanged();
   }

   public class XAndYNumericFieldsPresenter : AbstractPresenter<IXAndYNumericFieldsView, IXAndYNumericFieldsPresenter>, IXAndYNumericFieldsPresenter
   {
      private PopulationPivotAnalysis _populationAnalysis;
      private readonly Type _allowedType;
      private readonly XandYFieldsSelectionDTO _selectionDTO;
      private IReadOnlyList<IPopulationAnalysisField> _allAvailableFields;
      private readonly NullNumericField _nullField;
      private readonly IEventPublisher _eventPublisher;

      public XAndYNumericFieldsPresenter(IXAndYNumericFieldsView view, IEventPublisher eventPublisher) : base(view)
      {
         _eventPublisher = eventPublisher;
         _allowedType = typeof (INumericValueField);
         _nullField = new NullNumericField();
         _selectionDTO = new XandYFieldsSelectionDTO {X = _nullField, Y = _nullField};
         _allAvailableFields = new List<IPopulationAnalysisField>();
      }

      public void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationAnalysis = populationAnalysis.DowncastTo<PopulationPivotAnalysis>();
      }

      public void RefreshAnalysis()
      {
         _allAvailableFields = _populationAnalysis.All(_allowedType).ToList();
         var allDataFields = allCurrentDataFields();

         if (allDataFields.Count == 0)
         {
            updateSelectionFrom(_allAvailableFields);
            updateDataFieldsBasedOnCurrentSelection();
         }
         else
            updateSelectionFrom(allDataFields);

         _view.BindTo(_selectionDTO);
      }

      private IReadOnlyList<IPopulationAnalysisField> allCurrentDataFields()
      {
         return _populationAnalysis.AllFieldsOn(PivotArea.DataArea);
      }

      private void updateSelectionFrom(IReadOnlyList<IPopulationAnalysisField> allFields)
      {
         IPopulationAnalysisField x = _nullField;
         IPopulationAnalysisField y = _nullField;

         if (allFields.Count > 0)
            x = allFields[0];

         if (allFields.Count > 1)
            y = allFields[1];

         _selectionDTO.X = x;
         _selectionDTO.Y = y;
      }

      public IEnumerable<IPopulationAnalysisField> AllAvailableFields()
      {
         yield return _nullField;
         foreach (var populationAnalysisField in _allAvailableFields)
         {
            yield return populationAnalysisField;
         }
      }

      public string DisplayFor(IPopulationAnalysisField field)
      {
         return field.Name;
      }

      public void FieldSelectionChanged()
      {
         updateDataFieldsBasedOnCurrentSelection();
         _eventPublisher.PublishEvent(new PopulationAnalysisDataSelectionChangedEvent(_populationAnalysis));
      }

      private void updateDataFieldsBasedOnCurrentSelection()
      {
         //remove old data area
         var previousDataFields = allCurrentDataFields();
         previousDataFields.Each(resetPosition);

         updatePosition(_selectionDTO.X, 0);
         updatePosition(_selectionDTO.Y, 1);
      }

      private void updatePosition(IPopulationAnalysisField field, int index)
      {
         if (field == _nullField)
            return;

         _populationAnalysis.SetPosition(field, PivotArea.DataArea, index);
      }

      private void resetPosition(IPopulationAnalysisField field)
      {
         _populationAnalysis.SetPosition(field, PivotArea.FilterArea, 0);
      }
   }
}