using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Office.Utils;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IMultiParameterEditPresenter :
      IPresenter<IMultiParameterEditView>,
      IParameterSetPresenter,
      IPresenterWithContextMenu<IParameterDTO>
   {
      /// <summary>
      ///    Returns <c>true</c> if the parameter is distributed otherwise <c>false</c>
      /// </summary>
      bool ParameterIsDistributed(IParameterDTO parameterDTO);

      /// <summary>
      ///    Is the grouping panel visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool GroupingVisible { set; }

      /// <summary>
      ///    Is the scaling panel visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool ScalingVisible { set; }

      /// <summary>
      ///    Is the header panel visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool HeaderVisible { set; }

      /// <summary>
      ///    Is the row indicator visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool RowIndicatorVisible { set; }

      /// <summary>
      ///    Are the container names visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool ContainerVisible { set; }

      /// <summary>
      ///    Is the distribution column visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool DistributionVisible { set; }

      /// <summary>
      ///    Is the value origin column visible?
      ///    Default is <c>true</c>
      /// </summary>
      bool ValueOriginVisible { set; }

      /// <summary>
      ///    Forces the name column to be always visible. This should be called after the Edit method
      /// </summary>
      bool ParameterNameVisible { set; }

      /// <summary>
      ///    Configure the editor to display the most simple view possible
      ///    Grouping, scaling, favorite will be hidden 
      ///    Default is <c>false</c>
      /// </summary>
      bool IsSimpleEditor { set; }

      /// <summary>
      ///    Group by the column with the given <paramref name="pathElementId" />
      /// </summary>
      void GroupBy(PathElementId pathElementId);

      /// <summary>
      ///    Clear the presenter of the displayed parameters
      /// </summary>
      void Clear();

      /// <summary>
      ///    True if multiple parameter can be selected.
      ///    Default is <c>true</c>
      /// </summary>
      bool AllowMultiSelect { set; }

      /// <summary>
      ///    Set the caption for the underling view
      /// </summary>
      string Caption { set; }

      /// <summary>
      ///    Returns the optimal height of the view required to display all parameters at once
      /// </summary>
      int OptimalHeight { get; }

      /// <summary>
      ///    Performs a refresh of the view
      /// </summary>
      void RefreshData();

      /// <summary>
      ///    Event is raised whenever the a parameter was changed (value or unit)
      /// </summary>
      event Action<IParameter> ParameterChanged;

      /// <summary>
      ///    Triggers a save action from the view to the data (only necessary in embedded popup scenarios)
      /// </summary>
      void SaveEditor();

      /// <summary>
      ///    Typically called from the view, this method will raised the <see cref="OnSelectedParametersChanged" /> event
      /// </summary>
      void SelectedParametersChanged();

      /// <summary>
      ///    if set to true, parameter will be compared for sort only if sharing the same hiearchy of visible groups
      ///    It is useful for events, default is <c>false</c>
      /// </summary>
      bool UseAdvancedSortingMode { set; }

      IReadOnlyList<IParameter> SelectedParameters { get; set; }

      /// <summary>
      ///    Event is raised whenever the selection of parameters in the UI has changed
      /// </summary>
      event Action OnSelectedParametersChanged;

      void DisableEdit();
   }

   public class MultiParameterEditPresenter : ParameterSetPresenter<IMultiParameterEditView, IMultiParameterEditPresenter>, IMultiParameterEditPresenter
   {
      private readonly IScaleParametersPresenter _scaleParametersPresenter;
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;
      private readonly IParameterContextMenuFactory _contextMenuFactory;
      public event Action<IParameter> ParameterChanged = delegate { };
      public event Action OnSelectedParametersChanged = delegate { };

      protected const int NO_COLUMN = -1;

      public MultiParameterEditPresenter(IMultiParameterEditView view, IScaleParametersPresenter scaleParametersPresenter,
         IEditParameterPresenterTask editParameterPresenterTask, IParameterTask parameterTask, IParameterToParameterDTOMapper parameterDTOMapper,
         IParameterContextMenuFactory contextMenuFactory)
         : base(view, editParameterPresenterTask, parameterTask)
      {
         _scaleParametersPresenter = scaleParametersPresenter;
         _parameterDTOMapper = parameterDTOMapper;
         _contextMenuFactory = contextMenuFactory;
         _scaleParametersPresenter.InitializeWith(this);
         _view.SetScaleParameterView(_scaleParametersPresenter.View);
         Description = string.Empty;
         IsSimpleEditor = false;
         UseAdvancedSortingMode = false;
      }

      public override void Edit(IEnumerable<IParameter> parameters)
      {
         base.Edit(parameters);
         AllParametersDTO.Clear();
         AllParametersDTO.AddRange(_visibleParameters.MapAllUsing(_parameterDTOMapper).Cast<ParameterDTO>());

         _view.DistributionVisible = AllParametersDTO.Any(ParameterIsDistributed);

         EnumHelper.AllValuesFor<PathElementId>().Each(updateColumnVisibility);
         updateCategoryVisibility();

         _scaleParametersPresenter.Enabled = _visibleParameters.Any(x => x.Editable);
         _view.BindTo(AllParametersDTO);

         PerformDefaultGrouping(_visibleParameters);
      }

      protected virtual void PerformDefaultGrouping(IReadOnlyList<IParameter> parameters)
      {
         bool parameterNameVisible = !AllParametersHaveTheSameDisplayName || _visibleParameters.Count == 1;
         ParameterNameVisible = parameterNameVisible;

         if (parameterNameVisible) return;

         //only group by category if the parameter names are not displayed
         _view.GroupByCategory();
      }

      public override bool ShowFavorites
      {
         set => _view.FavoritesVisible = value;
      }

      protected bool AllParametersHaveTheSameDisplayName
      {
         get { return !haveDifferent(x => x.DisplayName); }
      }

      private void updateColumnVisibility(PathElementId pathElementId)
      {
         _view.SetVisibility(pathElementId, AllParametersDTO.HasDistinctValuesAt(pathElementId));
      }

      private void updateCategoryVisibility()
      {
         _view.CategoryVisible = AllParametersDTO.HasDistinctCategories();
      }

      private bool haveDifferent(Func<IParameterDTO, string> parameterDisplayName)
      {
         return AllParametersDTO.AllDistinctValues(parameterDisplayName).Count > 1;
      }

      public bool ParameterIsDistributed(IParameterDTO parameterDTO)
      {
         var distributedParameter = ParameterFrom(parameterDTO) as IDistributedParameter;
         if (distributedParameter == null) return false;
         return distributedParameter.Formula.DistributionType() != DistributionTypes.Discrete;
      }

      public override void SetParameterPercentile(IParameterDTO parameterDTO, double percentileInPercent)
      {
         base.SetParameterPercentile(parameterDTO, percentileInPercent);
         ParameterChanged(ParameterFrom(parameterDTO));
      }

      public override void SetParameterValue(IParameterDTO parameterDTO, double valueInGuiUnit)
      {
         base.SetParameterValue(parameterDTO, valueInGuiUnit);
         ParameterChanged(ParameterFrom(parameterDTO));
      }

      public override void SetParameterUnit(IParameterDTO parameterDTO, Unit displayUnit)
      {
         base.SetParameterUnit(parameterDTO, displayUnit);
         ParameterChanged(ParameterFrom(parameterDTO));
      }

      public bool ScalingVisible
      {
         set => _view.ScalingVisible = value;
      }

      public bool AllowMultiSelect
      {
         set => _view.AllowMultiSelect = value;
      }

      public string Caption
      {
         set => _view.Caption = value;
      }

      public int OptimalHeight => _view.OptimalHeight;

      public void RefreshData()
      {
         _view.RefreshData();
      }

      public bool HeaderVisible
      {
         set => _view.HeaderVisible = value;
      }

      public bool RowIndicatorVisible
      {
         set => _view.ShowRowIndicator = value;
      }

      public bool ContainerVisible
      {
         set => _view.SetVisibility(PathElementId.Container, value);
      }

      public void SaveEditor()
      {
         _view.SaveEditor();
      }

      public void SelectedParametersChanged() => OnSelectedParametersChanged();

      public bool UseAdvancedSortingMode
      {
         set => _view.UseAdvancedSortingMode = value;
      }

      public bool DistributionVisible
      {
         set => _view.DistributionVisible = value;
      }

      public bool ValueOriginVisible
      {
         set => _view.ValueOriginVisible = value;
      }

      public bool ParameterNameVisible
      {
         set => _view.ParameterNameVisible = value;
      }

      public bool IsSimpleEditor
      {
         set
         {
            ScalingVisible = !value;
            GroupingVisible = !value;
            DistributionVisible = !value;
            ContainerVisible = !value;
            AllowMultiSelect = !value;
            ShowFavorites = !value;
            Description = string.Empty;
         }
      }

      public void GroupBy(PathElementId pathElementId)
      {
         View.GroupBy(pathElementId);
      }

      public void Clear()
      {
         _view.DeleteBinding();
      }

      public bool GroupingVisible
      {
         set => _view.GroupingVisible = value;
      }

      protected override IEnumerable<IParameterDTO> AllVisibleParameterDTOs => _view.AllVisibleParameters;

      public virtual IReadOnlyList<IParameter> SelectedParameters
      {
         get => ParametersFrom(_view.SelectedParameters).ToList();
         set
         {
            if (value == null)
               return;

            _view.SelectedParameters = AllParametersDTO.Where(x => value.Contains(x.Parameter)).ToList();
         }
      }

      public override void AddCommand(ICommand command)
      {
         base.AddCommand(command);
         RefreshData();
      }

      public void DisableEdit()
      {
         _view.ReadOnly = true;
      }


      public void ShowContextMenu(IParameterDTO parameterDTO, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(parameterDTO, this);
         contextMenu.Show(_view, popupLocation);
      }
   }
}