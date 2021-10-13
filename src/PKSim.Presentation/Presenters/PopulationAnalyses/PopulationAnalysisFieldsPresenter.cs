using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface IPopulationAnalysisFieldsPresenter : IPresenterWithContextMenu<IPopulationAnalysisField>,
      IPresenter<IPopulationAnalysisFieldsView>, IPopulationAnalysisPresenter
   {
      /// <summary>
      ///    Typically called from the view when a field was selected by the user
      /// </summary>
      void FieldSelected(IPopulationAnalysisField populationAnalysisField);

      /// <summary>
      ///    Removes the <paramref name="populationAnalysisField" /> from the analysis and all
      ///    <see cref="PopulationAnalysisDerivedField" /> depending on it.
      /// </summary>
      void RemoveField(IPopulationAnalysisField populationAnalysisField);

      /// <summary>
      ///    Adds the <paramref name="populationAnalysisField" /> from the analysis
      /// </summary>
      void AddField(IPopulationAnalysisField populationAnalysisField);

      /// <summary>
      ///    Creates a derived field based on the <paramref name="populationAnalysisField" /> given as parameter.
      ///    if the derived field was not specified, simply use the first field defined in the analysis
      /// </summary>
      void CreateDerivedFieldFor(IPopulationAnalysisField populationAnalysisField);

      /// <summary>
      ///    Creates a derived field based. The origin field on which the derived field should be created
      ///    will be selected by the user
      /// </summary>
      void CreateDerivedField();

      /// <summary>
      ///    This is called whenever the display unit of a field is changed
      /// </summary>
      /// <param name="populationAnalysisField">Field whose display unit is being edited</param>
      /// <param name="oldDisplayUnit">Previous display unit</param>
      /// <param name="newDisplayUnit">New display unit</param>
      void FieldUnitChanged(PopulationAnalysisFieldDTO populationAnalysisField, Unit oldDisplayUnit, Unit newDisplayUnit);

      /// <summary>
      ///    This is called whenever the scaling of a field is changed
      /// </summary>
      /// <param name="populationAnalysisField">Field whose scaling is being edited</param>
      /// <param name="oldScaling">Previous scaling</param>
      /// <param name="newScaling">New scaling</param>
      void FieldScalingChanged(PopulationAnalysisFieldDTO populationAnalysisField, Scalings oldScaling, Scalings newScaling);

      /// <summary>
      ///    This is called whenever the color of a field is changed
      /// </summary>
      /// <param name="populationAnalysisField">Field whose color is being edited</param>
      /// <param name="oldColor">Previous color</param>
      /// <param name="newColor">New color</param>
      void FieldColorChanged(PopulationAnalysisFieldDTO populationAnalysisField, Color oldColor, Color newColor);

      /// <summary>
      ///    This is called whenever the name of the field is changed
      /// </summary>
      /// <param name="populationAnalysisField">Field whose name is being edited</param>
      /// <param name="oldName">Previous name</param>
      /// <param name="newName">New name</param>
      void FieldNameChanged(PopulationAnalysisFieldDTO populationAnalysisField, string oldName, string newName);

      /// <summary>
      ///    Edit the <see cref="PopulationAnalysisDerivedField" /> given as parameter
      /// </summary>
      /// <param name="derivedField">Derived field that should be edited</param>
      void EditDerivedField(PopulationAnalysisDerivedField derivedField);

      /// <summary>
      ///    Loads a derived field from the template database for the input field <paramref name="populationAnalysisDataField" />
      /// </summary>
      Task LoadDerivedFieldFromTemplateFor(PopulationAnalysisDataField populationAnalysisDataField);

      void SaveDerivedFieldToTemplate(PopulationAnalysisDerivedField derivedField);

      /// <summary>
      ///    Event is raised whenever an action (for example delete) led to a no field being selected
      /// </summary>
      event EventHandler NoFieldSelected;

      /// <summary>
      ///    Removes the field currently being selected
      /// </summary>
      void RemoveSelection();

      /// <summary>
      ///    Event is thrown whenever a derived field is being selected
      /// </summary>
      event EventHandler<DerivedFieldSelectedEventArgs> DerivedFieldSelected;

      /// <summary>
      ///    Specifies whether the scaling field should be displayed. Default is <c>true</c>
      /// </summary>
      bool ScalingVisible { get; set; }

      string EmptySelectionMessage { get; set; }
   }

   public abstract class PopulationAnalysisFieldsPresenter : AbstractPresenter<IPopulationAnalysisFieldsView, IPopulationAnalysisFieldsPresenter>, IPopulationAnalysisFieldsPresenter
   {
      //this is a list of field types that will be filtered out from the analyses and managed in the presenter
      private readonly IReadOnlyList<Type> _fieldTypes;
      private readonly IPopulationAnalysesContextMenuFactory _contextMenuFactory;
      protected readonly IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      private readonly IEventPublisher _eventPublisher;
      private readonly IPopulationAnalysisGroupingFieldCreator _populationAnalysisGroupingFieldCreator;
      private readonly IPopulationAnalysisTemplateTask _populationAnalysisTemplateTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper _fieldDTOMapper;
      protected PopulationAnalysis _populationAnalysis;
      protected IPopulationDataCollector PopulationDataCollector { get; private set; }
      public event EventHandler NoFieldSelected = delegate { };
      public event EventHandler<DerivedFieldSelectedEventArgs> DerivedFieldSelected = delegate { };
      public string EmptySelectionMessage { get; set; }

      protected PopulationAnalysisFieldsPresenter(IPopulationAnalysisFieldsView view, IReadOnlyList<Type> fieldTypes, IPopulationAnalysesContextMenuFactory contextMenuFactory,
         IPopulationAnalysisFieldFactory populationAnalysisFieldFactory, IEventPublisher eventPublisher, IPopulationAnalysisGroupingFieldCreator populationAnalysisGroupingFieldCreator,
         IPopulationAnalysisTemplateTask populationAnalysisTemplateTask, IDialogCreator dialogCreator, IPopulationAnalysisFieldToPopulationAnalysisFieldDTOMapper fieldDTOMapper)
         : base(view)
      {
         _fieldTypes = fieldTypes;
         _contextMenuFactory = contextMenuFactory;
         _populationAnalysisFieldFactory = populationAnalysisFieldFactory;
         _eventPublisher = eventPublisher;
         _populationAnalysisGroupingFieldCreator = populationAnalysisGroupingFieldCreator;
         _populationAnalysisTemplateTask = populationAnalysisTemplateTask;
         _dialogCreator = dialogCreator;
         _fieldDTOMapper = fieldDTOMapper;
         view.ColorSelectionVisible = false;
         view.CreateGroupingButtonVisible = true;
         EmptySelectionMessage = PKSimConstants.UI.ChooseFieldsToDisplay;
      }

      public void ShowContextMenu(IPopulationAnalysisField populationAnalysisField, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(populationAnalysisField, this);
         contextMenu.Show(_view, popupLocation);
      }

      public virtual void StartAnalysis(IPopulationDataCollector populationDataCollector, PopulationAnalysis populationAnalysis)
      {
         _populationAnalysis = populationAnalysis;
         PopulationDataCollector = populationDataCollector;
         UpdateView();
      }

      public bool ScalingVisible
      {
         set => View.ScalingVisible = value;
         get => View.ScalingVisible;
      }

      public virtual void FieldSelected(IPopulationAnalysisField populationAnalysisField)
      {
         updateGroupingEnable(populationAnalysisField);

         if (populationAnalysisField == null)
         {
            NoFieldSelected(this, EventArgs.Empty);
            return;
         }

         var derivedField = populationAnalysisField as PopulationAnalysisDerivedField;
         if (derivedField != null)
         {
            DerivedFieldSelected(this, new DerivedFieldSelectedEventArgs(derivedField));
            return;
         }
      }

      public virtual void RemoveField(IPopulationAnalysisField populationAnalysisField)
      {
         if (populationAnalysisField == null)
            return;

         var allDerivedFields = _populationAnalysis.AllFieldsReferencing(populationAnalysisField);
         if (allDerivedFields.Any())
         {
            var res = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyRemoveFieldUsedInGrouping(populationAnalysisField.Name, allDerivedFields.Select(x => x.Name)));
            if (res == ViewResult.No) return;
         }

         _populationAnalysis.Remove(populationAnalysisField);
         allDerivedFields.Each(_populationAnalysis.Remove);

         _eventPublisher.PublishEvent(new FieldRemovedFromPopulationAnalysisEvent(_populationAnalysis, populationAnalysisField));
         UpdateView();
      }

      public virtual void AddField(IPopulationAnalysisField populationAnalysisField)
      {
         if (populationAnalysisField == null)
            return;

         if (_populationAnalysis.Has(populationAnalysisField))
            return;

         _populationAnalysis.Add(populationAnalysisField);
         _eventPublisher.PublishEvent(new FieldAddedToPopulationAnalysisEvent(_populationAnalysis, populationAnalysisField));
         UpdateView();
      }

      public virtual void RemoveSelection()
      {
         RemoveField(SelectedField());
      }

      public void CreateDerivedFieldFor(IPopulationAnalysisField populationAnalysisField)
      {
         AddField(_populationAnalysisGroupingFieldCreator.CreateGroupingFieldFor(populationAnalysisField, PopulationDataCollector));
      }

      public void EditDerivedField(PopulationAnalysisDerivedField derivedField)
      {
         if (!_populationAnalysisGroupingFieldCreator.EditDerivedField(derivedField, PopulationDataCollector))
            return;

         updateSelectedFieldWithCurrent();
         _eventPublisher.PublishEvent(new FieldAddedToPopulationAnalysisEvent(_populationAnalysis, derivedField));
      }

      public void CreateDerivedField()
      {
         CreateDerivedFieldFor(SelectedField());
      }

      protected IPopulationAnalysisField SelectedField()
      {
         var selectedDTO = _view.SelectedField;
         return selectedDTO?.Field;
      }

      protected T SelectedField<T>() where T : IPopulationAnalysisField
      {
         return SelectedField().DowncastTo<T>();
      }

      public void FieldNameChanged(PopulationAnalysisFieldDTO dto, string oldName, string newName)
      {
         var populationAnalysisField = dto.Field;
         _populationAnalysis.RenameField(oldName, newName);
         _eventPublisher.PublishEvent(new FieldRenamedInPopulationAnalysisEvent(_populationAnalysis, populationAnalysisField));
      }

      public void FieldUnitChanged(PopulationAnalysisFieldDTO dto, Unit oldDisplayUnit, Unit newDisplayUnit)
      {
         _eventPublisher.PublishEvent(new FieldUnitChangedInPopulationAnalysisEvent(_populationAnalysis, dto.Field));
      }

      public void FieldColorChanged(PopulationAnalysisFieldDTO populationAnalysisField, Color oldColor, Color newColor)
      {
         _eventPublisher.PublishEvent(new PopulationAnalysisChartSettingsChangedEvent(_populationAnalysis));
      }

      public void FieldScalingChanged(PopulationAnalysisFieldDTO dto, Scalings oldScaling, Scalings newScaling)
      {
         _eventPublisher.PublishEvent(new PopulationAnalysisChartSettingsChangedEvent(_populationAnalysis));
      }

      public async Task LoadDerivedFieldFromTemplateFor(PopulationAnalysisDataField populationAnalysisDataField)
      {
         var newDerivedField = await _populationAnalysisTemplateTask.LoadDerivedFieldFor(_populationAnalysis, populationAnalysisDataField);
         if (newDerivedField == null)
            return;

         AddField(newDerivedField);
      }

      public void SaveDerivedFieldToTemplate(PopulationAnalysisDerivedField derivedField)
      {
         _populationAnalysisTemplateTask.SaveDerivedField(derivedField);
      }

      protected virtual void UpdateView()
      {
         var allFields = _populationAnalysis.All(_fieldTypes, withDerived: true)
            .Select(_fieldDTOMapper.MapFrom).ToList();

         View.BindTo(allFields);
         updateGroupingEnable(SelectedField());
         updateSelectedFieldWithCurrent();
      }

      private void updateSelectedFieldWithCurrent()
      {
         FieldSelected(SelectedField());
      }

      private void updateGroupingEnable(IPopulationAnalysisField populationAnalysisField)
      {
         if (populationAnalysisField == null)
            _view.CreateGroupingButtonEnabled = false;
         else
            _view.CreateGroupingButtonEnabled = populationAnalysisField.IsAnImplementationOf<PopulationAnalysisDataField>();
      }

      protected virtual TField FieldBy<TField>(Func<TField, bool> query) where TField : IPopulationAnalysisField
      {
         return _populationAnalysis.All<TField>().Where(query).FirstOrDefault();
      }
   }
}