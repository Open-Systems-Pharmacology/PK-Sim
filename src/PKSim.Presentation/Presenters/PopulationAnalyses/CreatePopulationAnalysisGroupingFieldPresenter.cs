using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.PopulationAnalyses
{
   public interface ICreatePopulationAnalysisGroupingFieldPresenter : IDisposablePresenter
   {
      /// <summary>
      ///    The new name selected by the user for the derived field
      /// </summary>
      string FieldName { get; }

      /// <summary>
      ///    Start the use case of creating a grouping derived field for the field <paramref name="populationAnalysisField" />
      ///    and the simulation <paramref name="populationDataCollector" />
      /// </summary>
      /// <returns>The <see cref="GroupingDefinition" /> defined by the user or null if the action was cancelled</returns>
      GroupingDefinition CreateGrouping(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector);

      /// <summary>
      ///    The list of all available <see cref="GroupingDefinitionItem" /> that the user can choose from
      /// </summary>
      IEnumerable<GroupingDefinitionItem> AvailableGroupings { get; }

      /// <summary>
      ///    This is called whenever the grouping method was changed by the user
      /// </summary>
      void SelectedGroupingChanged();
   }

   public class CreatePopulationAnalysisGroupingFieldPresenter : AbstractDisposablePresenter<ICreatePopulationAnalysisGroupingFieldView, ICreatePopulationAnalysisGroupingFieldPresenter>, ICreatePopulationAnalysisGroupingFieldPresenter
   {
      private readonly IGroupingDefinitionToGroupingDefinitionPresenterMapper _groupingDefinitionPresenterMapper;
      private readonly IEventPublisher _eventPublisher;
      private readonly GroupingFieldDTO _groupingFieldDTO;
      private readonly ICache<GroupingDefinitionItem, IGroupingDefinitionPresenter> _presenterCache;
      private IPopulationAnalysisField _populationAnalysisField;
      private IGroupingDefinitionPresenter _activePresenter;
      private IPopulationDataCollector _populationDataCollector;

      public CreatePopulationAnalysisGroupingFieldPresenter(ICreatePopulationAnalysisGroupingFieldView view, IGroupingDefinitionToGroupingDefinitionPresenterMapper groupingDefinitionPresenterMapper, IEventPublisher eventPublisher)
         : base(view)
      {
         _groupingDefinitionPresenterMapper = groupingDefinitionPresenterMapper;
         _eventPublisher = eventPublisher;
         _groupingFieldDTO = new GroupingFieldDTO();
         _presenterCache = new Cache<GroupingDefinitionItem, IGroupingDefinitionPresenter>();
      }

      public GroupingDefinition CreateGrouping(IPopulationAnalysisField populationAnalysisField, IPopulationDataCollector populationDataCollector)
      {
         _populationAnalysisField = populationAnalysisField;
         _populationDataCollector = populationDataCollector;
         _groupingFieldDTO.AddUsedNames(populationAnalysisField.PopulationAnalysis.AllFields.Select(x => x.Name));
         _view.Caption = PKSimConstants.UI.CreateGroupingForField(populationAnalysisField.Name);

         //initialize the default grouping definition depending on the available groupings
         _groupingFieldDTO.GroupingDefinitionItem = AvailableGroupings.First();
         updateView();

         _view.Display();
         return _view.Canceled ? null : _activePresenter.GroupingDefinition;
      }

      private void updateView()
      {
         _view.BindTo(_groupingFieldDTO);
         SelectedGroupingChanged();
      }

      public IEnumerable<GroupingDefinitionItem> AvailableGroupings
      {
         get
         {
            if (_populationAnalysisField.IsAnImplementationOf<PopulationAnalysisNumericField>())
               return GroupingDefinitions.All<IntervalGroupingDefinition>();

            if (_populationAnalysisField.IsAnImplementationOf<PopulationAnalysisCovariateField>())
               return GroupingDefinitions.All<ValueMappingGroupingDefinition>();

            return Enumerable.Empty<GroupingDefinitionItem>();
         }
      }

      public void SelectedGroupingChanged()
      {
         _activePresenter = retrieveGroupingPresenter(_groupingFieldDTO.GroupingDefinitionItem);
         updateGroupingView();
      }

      private void updateGroupingView()
      {
         _view.SetGroupingView(_activePresenter.BaseView);
         ViewChanged();
      }

      private IGroupingDefinitionPresenter retrieveGroupingPresenter(GroupingDefinitionItem groupingDefinitionItem)
      {
         if (!_presenterCache.Contains(groupingDefinitionItem))
         {
            var presenter = _groupingDefinitionPresenterMapper.MapFrom(groupingDefinitionItem);
            presenter.StatusChanged += groupingDefinitionPresenterChanged;
            _presenterCache[groupingDefinitionItem] = presenter;
            presenter.InitializeWith(_populationAnalysisField, _populationDataCollector);
            presenter.StartCreate();
         }

         return _presenterCache[groupingDefinitionItem];
      }

      protected override void Cleanup()
      {
         try
         {
            _presenterCache.Each(p => p.StatusChanged -= groupingDefinitionPresenterChanged);
            _presenterCache.Each(p => p.ReleaseFrom(_eventPublisher));
            _presenterCache.Clear();
         }
         finally
         {
            base.Cleanup();
         }
      }

      public override bool CanClose
      {
         get
         {
            var canClose = base.CanClose;
            if (_activePresenter != null)
               canClose = canClose && _activePresenter.CanClose;

            return canClose;
         }
      }

      public override void ViewChanged()
      {
         base.ViewChanged();
         View.OkEnabled = CanClose;
      }

      private void groupingDefinitionPresenterChanged(object sender, EventArgs e)
      {
         ViewChanged();
      }

      public string FieldName
      {
         get { return _groupingFieldDTO.Name; }
      }
   }
}