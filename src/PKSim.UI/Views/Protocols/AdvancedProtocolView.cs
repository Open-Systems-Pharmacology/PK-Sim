using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using PKSim.UI.Views.Core;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Views;
using BaseView = DevExpress.XtraGrid.Views.Base.BaseView;
using UIConstants = OSPSuite.UI.UIConstants;

namespace PKSim.UI.Views.Protocols
{
   public partial class AdvancedProtocolView : BaseUserControlWithValueInGrid, IAdvancedProtocolView
   {
      private readonly ICache<BaseView, IGridViewBinder> _cache = new Cache<BaseView, IGridViewBinder>();
      private readonly ComboBoxUnitParameter _comboBoxUnit;
      private readonly IImageListRetriever _imageListRetriever;
      private RepositoryItemMRUEdit _formulationRepository;
      private GridViewBinder<SchemaDTO> _gridProtocolBinder;

      private IAdvancedProtocolPresenter _presenter;
      private ScreenBinder<AdvancedProtocol> _screenBinder;
      private IGridViewColumn _colAddRemoveSchema;

      public AdvancedProtocolView(IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         _imageListRetriever = imageListRetriever;
         layoutControl.OptionsView.UseParentAutoScaleFactor = true;
         gridProtocol.ViewRegistered += viewRegistered;
         gridProtocol.ViewRemoved += viewRemoved;

         _comboBoxUnit = new ComboBoxUnitParameter(gridProtocol);

         mainView.AllowsFiltering = false;
         mainView.EnableColumnContextMenu = false;
         mainView.MasterRowEmpty += mainViewMasterRowEmpty;
         mainView.MasterRowGetChildList += mainViewMasterRowGetChildList;
         mainView.MasterRowGetRelationCount += masterRowGetRelationCount;
         mainView.MasterRowGetRelationName += mainViewMasterRowGetRelationName;
         mainView.OptionsDetail.ShowDetailTabs = false;
         mainView.OptionsDetail.EnableDetailToolTip = false;
         mainView.MasterRowExpanded += masterRowExpanded;
         mainView.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
         mainView.OptionsCustomization.AllowSort = true;
         mainView.KeyDown += gridViewKeyDown;
         InitializeWithGrid(mainView);

         gridViewSchemaItems.MasterRowEmpty += schemaViewMasterRowEmpty;
         gridViewSchemaItems.MasterRowGetChildList += schemaViewMasterRowGetChildList;
         gridViewSchemaItems.MasterRowGetRelationCount += schemaViewMasterRowGetRelationCount;
         gridViewSchemaItems.MasterRowGetRelationName += schemaViewMasterRowGetRelationName;
         gridViewSchemaItems.OptionsDetail.ShowDetailTabs = false;
         gridViewSchemaItems.OptionsDetail.EnableDetailToolTip = false;
         gridViewSchemaItems.MasterRowExpanded += masterRowExpanded;
         gridViewSchemaItems.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
         gridViewSchemaItems.EnableColumnContextMenu = false;
         gridViewSchemaItems.AllowsFiltering = false;
         gridViewSchemaItems.OptionsView.ShowGroupPanel = false;
         gridViewSchemaItems.SynchronizeClones = false;
         gridViewSchemaItems.OptionsCustomization.AllowSort = true;
         gridViewSchemaItems.KeyDown += gridViewKeyDown;

         gridViewDynamicParameters.OptionsView.ShowColumnHeaders = false;
         gridViewDynamicParameters.OptionsView.ShowGroupPanel = false;
         gridViewDynamicParameters.SynchronizeClones = false;
         gridViewDynamicParameters.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
         gridViewDynamicParameters.EditorShowMode = EditorShowMode.MouseDown;


         gridViewUserDefinedTarget.OptionsView.ShowColumnHeaders = false;
         gridViewUserDefinedTarget.OptionsView.ShowGroupPanel = false;
         gridViewUserDefinedTarget.SynchronizeClones = false;
         gridViewUserDefinedTarget.OptionsView.ShowIndicator = false;

      }

      public void AttachPresenter(IAdvancedProtocolPresenter presenter)
      {
         _presenter = presenter;
      }

      private void gridViewKeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode != Keys.Return) return;
         var gridView = sender as ColumnView;
         if (gridView == null) return;
         gridView.CloseEditor();
         gridView.MoveNext();
      }

      public override void InitializeBinding()
      {
         //auto bind necessary to allow for error check
         var schemaButtonRepository = createAddRemoveButtonRepository();
         _screenBinder = new ScreenBinder<AdvancedProtocol>();

         _screenBinder.Bind(x => x.TimeUnit)
            .To(cbTimeUnit)
            .WithValues(x => _presenter.AllTimeUnits());

         _gridProtocolBinder = new GridViewBinder<SchemaDTO>(mainView)
         {
            ValidationMode = ValidationMode.LeavingCell,
            BindingMode = BindingMode.OneWay
         };

         var colStartTime = _gridProtocolBinder.AutoBind(x => x.StartTime)
            .WithCaption(PKSimConstants.UI.StartTime)
            .WithFormat(schemaDto => schemaDto.StartTimeParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, schemaDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, schemaDTO.StartTimeParameter));

         colStartTime.OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.StartTimeParameter, valueInGuiUnit.NewValue);
         colStartTime.XtraColumn.SortOrder = ColumnSortOrder.Ascending;

         _gridProtocolBinder.AutoBind(x => x.NumberOfRepetitions)
            .WithCaption(PKSimConstants.UI.NumberOfRepetitions)
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.NumberOfRepetitionsParameter, valueInGuiUnit.NewValue);

         _gridProtocolBinder.AutoBind(x => x.TimeBetweenRepetitions)
            .WithCaption(PKSimConstants.UI.TimeBetweenRepetitions)
            .WithFormat(schemaDto => schemaDto.TimeBetweenRepetitionsParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, schemaDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, schemaDTO.TimeBetweenRepetitionsParameter))
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.TimeBetweenRepetitionsParameter, valueInGuiUnit.NewValue);

         _gridProtocolBinder.AutoBind(x => x.EndTime)
            .WithCaption(PKSimConstants.UI.EndTime)
            .WithFormat(schemaDto => schemaDto.TimeBetweenRepetitionsParameter.ParameterFormatter())
            .AsReadOnly();

         _comboBoxUnit.ParameterUnitSet += setParameterUnit;

         _colAddRemoveSchema = _gridProtocolBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => schemaButtonRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);


         _gridProtocolBinder.Changed += NotifyViewChanged;
         _screenBinder.Changed += unitChanged;

         schemaButtonRepository.ButtonClick += (o, e) => OnEvent(() => schemaButtonRepositoryButtonClick(o, e, _gridProtocolBinder.FocusedElement));
         //Create a dummy column for the detail view to avoid auto binding to details
         gridViewSchemaItems.Columns.AddField("Dummy");
         gridViewDynamicParameters.Columns.AddField("Dummy");
         gridViewUserDefinedTarget.Columns.AddField("Dummy");
      }

      private void initializeSchemaItemBinding(GridViewBinder<SchemaItemDTO> schemaItemBinder)
      {
         var applicationRepository = new UxRepositoryItemImageComboBox(schemaItemBinder.GridView, _imageListRetriever);
         var schemaItemButtonRepository = createAddRemoveButtonRepository();

         var colStartTime = schemaItemBinder.AutoBind(x => x.StartTime)
            .WithCaption(PKSimConstants.UI.StartTime)
            .WithFormat(dto => dto.StartTimeParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, schemaItemDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, schemaItemDTO.StartTimeParameter));
         colStartTime.OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.StartTimeParameter, valueInGuiUnit.NewValue);
         colStartTime.XtraColumn.SortOrder = ColumnSortOrder.Ascending;

         schemaItemBinder.AutoBind(x => x.Dose)
            .WithCaption(PKSimConstants.UI.Dose)
            .WithFormat(dto => dto.DoseParameter.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, schemaItemDTO) => _comboBoxUnit.UpdateUnitsFor(activeEditor, schemaItemDTO.DoseParameter))
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto.DoseParameter, valueInGuiUnit.NewValue);

         var appTypeColumn = schemaItemBinder.AutoBind(x => x.ApplicationType)
            .WithRepository(x => configureApplicationRepository(applicationRepository))
            .WithCaption(PKSimConstants.UI.ApplicationType)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         appTypeColumn.OnChanged += dto => updateApplicationParameter(schemaItemBinder, dto);
         appTypeColumn.OnValueUpdating += (dto, applicationType) => setApplicationType(dto, applicationType.NewValue);

         var formulationColumn = schemaItemBinder.AutoBind(x => x.FormulationKey);

         formulationColumn.WithRepository(formulationRepository)
            .WithCaption(PKSimConstants.UI.PlaceholderFormulation)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         formulationColumn.OnValueUpdating += (dto, formulationType) => OnEvent(() => _presenter.SetFormulationType(dto, formulationType.NewValue));

         schemaItemBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => schemaItemButtonRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         schemaItemBinder.Changed += NotifyViewChanged;

         schemaItemButtonRepository.ButtonClick += (o, e) => OnEvent(() => schemaItemButtonRepositoryButtonClick(o, e,  schemaItemBinder.FocusedElement));
      }

      private void initializeDynamicParameterBinding(GridViewBinder<IParameterDTO> parameterBinder)
      {
         parameterBinder.Bind(x => x.DisplayName)
            .AsReadOnly();

         parameterBinder.Bind(x => x.Value)
            .WithFormat(dto => dto.ParameterFormatter())
            .WithEditorConfiguration(_comboBoxUnit.UpdateUnitsFor)
            .OnValueUpdating += (dto, valueInGuiUnit) => setParameterValue(dto, valueInGuiUnit.NewValue);

         parameterBinder.Changed += NotifyViewChanged;
      }

      private void initializeUserDefinedTargetParameterBinding(GridViewBinder<SchemaItemTargetDTO> schemaItemTargetBinder)
      {
         schemaItemTargetBinder.Bind(x => x.Name)
            .AsReadOnly();

         schemaItemTargetBinder.Bind(x => x.Target);

         schemaItemTargetBinder.Changed += NotifyViewChanged;
      }
      private void schemaButtonRepositoryButtonClick(object sender, ButtonPressedEventArgs e, SchemaDTO schemaDTO)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
            _presenter.AddNewSchema();
         else
         {
            refreshMasterRow(_gridProtocolBinder.GridView, _gridProtocolBinder.RowHandleFor(schemaDTO));
            _presenter.RemoveSchema(_gridProtocolBinder.FocusedElement);
         }
      }

      private void schemaItemButtonRepositoryButtonClick(object sender, ButtonPressedEventArgs e,  SchemaItemDTO schemaItemDTO)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
            _presenter.AddSchemaItemTo(schemaItemDTO.ParentSchema, schemaItemDTO);
         else
         {
            //before removing: collapse 
            int rowHandle = _gridProtocolBinder.RowHandleFor(schemaItemDTO.ParentSchema);
            refreshMasterRow(_gridProtocolBinder.GridView, rowHandle);
            _presenter.RemoveSchemaItem(schemaItemDTO);
            _gridProtocolBinder.GridView.ExpandMasterRow(rowHandle);
         }
      }

      private RepositoryItem formulationRepository(SchemaItemDTO schemaItemDTO)
      {
         if (_formulationRepository == null)
         {
            _formulationRepository = new RepositoryItemMRUEdit();
            _presenter.AllFormulationKeys().Each(key => _formulationRepository.Items.Add(key));
         }
         _formulationRepository.ReadOnly = !schemaItemDTO.NeedsFormulation;
         _formulationRepository.Enabled = schemaItemDTO.NeedsFormulation;

         return _formulationRepository;
      }

      private void updateApplicationParameter(GridViewBinder<SchemaItemDTO> binder, SchemaItemDTO dto)
      {
         refreshMasterRow(binder.GridView, binder.RowHandleFor(dto));
      }

      //This method collapse the master row and refreshes the data
      private void refreshMasterRow(GridView gridView, int rowHandle)
      {
         //force open details to close 
         gridView.CollapseMasterRow(rowHandle);
         gridView.RefreshData();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemTimeUnit.Text = PKSimConstants.UI.TimeUnit.FormatForLabel();
      }

      private void unitChanged()
      {
         OnEvent( _presenter.ProtocolUnitChanged);
      }

      private void setApplicationType(SchemaItemDTO dto, ApplicationType newApplicationType)
      {
         OnEvent(() => _presenter.SetApplicationType(dto, newApplicationType));
      }

      private void setParameterValue(IParameterDTO parameterDTO, double newValue)
      {
         OnEvent(() => _presenter.SetParameterValue(parameterDTO, newValue));
      }

      private void setParameterUnit(IParameterDTO parameterDTO, Unit newUnit)
      {
         OnEvent(() => _presenter.SetParameterUnit(parameterDTO, newUnit));
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         return _colAddRemoveSchema.XtraColumn != column;
      }

      private RepositoryItemButtonEdit createAddRemoveButtonRepository()
      {
         var schemaItemButtonRepository = new RepositoryItemButtonEdit {TextEditStyle = TextEditStyles.HideTextEditor};
         schemaItemButtonRepository.Buttons[0].Kind = ButtonPredefines.Plus;
         schemaItemButtonRepository.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
         return schemaItemButtonRepository;
      }

      private RepositoryItem configureApplicationRepository(UxRepositoryItemImageComboBox applicationRepository)
      {
         applicationRepository.FillImageComboBoxRepositoryWith(_presenter.AllApplications(), app => _imageListRetriever.ImageIndex(app.IconName));
         return applicationRepository;
      }

      private void masterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
      {
         e.RelationCount = 1;
      }

      private void masterRowExpanded(object sender, CustomMasterRowEventArgs e)
      {
         GridView masterView = (GridView) sender;
         GridView detailView = (GridView) masterView.GetDetailView(e.RowHandle, e.RelationIndex);

         detailView?.BestFitColumns();
      }

      private void mainViewMasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
      {
         e.IsEmpty = false;
      }

      private void mainViewMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
      {
         e.RelationName = "SchemaItems";
      }

      private void schemaViewMasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
      {
         var schemaItem = schemaItemAt(sender, e.RowHandle);
         e.RelationName = schemaItem.IsUserDefined ? "UserDefinedTarget" : "DynamicParameters";
      }

      private void mainViewMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
      {
         var schema = _gridProtocolBinder.ElementAt(e.RowHandle);
         if (schema == null) return;

         e.ChildList = schema.SchemaItems;
      }

      private void schemaViewMasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
      {
         e.RelationCount = 2;
      }

      private void schemaViewMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
      {
         var schemaItem = schemaItemAt(sender, e.RowHandle);
         e.ChildList = _presenter.DynamicContentFor(schemaItem);
      }

      private void schemaViewMasterRowEmpty(object sender, MasterRowEmptyEventArgs e)
      {
         var schemaItem = schemaItemAt(sender, e.RowHandle);
         e.IsEmpty = !_presenter.HadDynamicContent(schemaItem);
      }

      private SchemaItemDTO schemaItemAt(object sender, int rowHandle)
      {
         var binder = schemaItemBinderAt(sender);
         return binder?.ElementAt(rowHandle);
      }

      private GridViewBinder<SchemaItemDTO> schemaItemBinderAt(object baseView)
      {
         var schemaItemView = (GridView) baseView;
         return _cache[schemaItemView] as GridViewBinder<SchemaItemDTO>;
      }

      private void viewRemoved(object sender, ViewOperationEventArgs e)
      {
         if (!_cache.Contains(e.View))
            return;
         var binder = _cache[e.View];
         binder.Dispose();
         _cache.Remove(e.View);
      }

      private void viewRegistered(object sender, ViewOperationEventArgs e)
      {
         var dataSource = e.View.DataSource;
         if (dataSource.IsAnImplementationOf<IEnumerable<SchemaItemDTO>>())
            registerBinderFor<SchemaItemDTO>(e.View, initializeSchemaItemBinding);

         if (dataSource.IsAnImplementationOf<IEnumerable<IParameterDTO>>())
            registerBinderFor<IParameterDTO>(e.View, initializeDynamicParameterBinding);

         if(dataSource.IsAnImplementationOf<IEnumerable<SchemaItemTargetDTO>>())
            registerBinderFor<SchemaItemTargetDTO>(e.View, initializeUserDefinedTargetParameterBinding);

      }

      private void registerBinderFor<T>(BaseView view, Action<GridViewBinder<T>> initBinding) where T : class
      {
         var dataSource = view.DataSource as IEnumerable<T>;
         if (!_cache.Contains(view))
         {
            var gridView = view.DowncastTo<GridView>();
            var binder = new GridViewBinder<T>(gridView) {BindingMode = BindingMode.OneWay};
            _cache.Add(gridView, binder);
            initBinding(binder);
         }

         var gridViewBinder = _cache[view].DowncastTo<GridViewBinder<T>>();
         gridViewBinder.BindToSource(dataSource);
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Protocol;

      public override bool HasError
      {
         get { return _gridProtocolBinder.HasError || _cache.Any(x => x.HasError); }
      }

      public void BindToSchemas(IEnumerable<SchemaDTO> allSchemas)
      {
         _gridProtocolBinder.BindToSource(allSchemas);
      }

      public void UpdateEndTime()
      {
         mainView.RefreshData();
      }

      public void BindToProperties(AdvancedProtocol advancedProtocol)
      {
         _screenBinder.BindToSource(advancedProtocol);
      }

      public void Rebind()
      {
         _gridProtocolBinder.Rebind();
         _cache.Each(binder => binder.Rebind());
      }
   }
}