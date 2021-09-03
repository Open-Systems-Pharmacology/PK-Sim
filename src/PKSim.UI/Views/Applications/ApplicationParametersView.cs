using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Applications;
using PKSim.Presentation.Presenters.Applications;
using PKSim.Presentation.Views.Applications;
using BaseView = DevExpress.XtraGrid.Views.Base.BaseView;

namespace PKSim.UI.Views.Applications
{
   public partial class ApplicationParametersView : BaseUserControl, IApplicationParametersView
   {
      private readonly IImageListRetriever _imageListRetriever;
      private IApplicationParametersPresenter _presenter;
      private GridViewBinder<ApplicationDTO> _gridApplicationsBinder;
      private readonly ICache<BaseView, GridViewBinder<IParameterDTO>> _cache = new Cache<BaseView, GridViewBinder<IParameterDTO>>();
      private readonly ComboBoxUnitParameter _comboBoxUnit;

      public ApplicationParametersView(IImageListRetriever imageListRetriever)
      {
         _imageListRetriever = imageListRetriever;
         InitializeComponent();
         gridApplications.ViewRegistered += viewRegistered;
         gridApplications.ViewRemoved += viewRemoved;
         _comboBoxUnit = new ComboBoxUnitParameter(gridApplications);

         mainView.OptionsView.ShowGroupPanel = false;
         mainView.MasterRowGetChildList += mainViewMasterRowGetChildList;
         mainView.OptionsDetail.ShowDetailTabs = false;
         mainView.OptionsDetail.EnableDetailToolTip = false;
         mainView.ShowColumnHeaders = false;

         gridViewParameters.OptionsView.ShowGroupPanel = false;
         gridViewParameters.MultiSelect = false;
         gridViewParameters.SynchronizeClones = false;
         gridViewParameters.ShowColumnHeaders = true;
         gridViewParameters.ShowColumnChooser = false;
         gridViewParameters.HiddenEditor += (o, e) => { _comboBoxUnit.Visible = false; };
      }

      private void mainViewMasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
      {
         var applicationDTO = _gridApplicationsBinder.ElementAt(e.RowHandle);
         if (applicationDTO == null) return;

         e.ChildList = applicationDTO.Parameters;
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
         if (_cache.Contains(e.View))
            return;

         var gridView = e.View.DowncastTo<GridView>();
         var dataSource = e.View.DataSource as IEnumerable<IParameterDTO>;
         var binder = new GridViewBinder<IParameterDTO>(gridView) {BindingMode = BindingMode.OneWay};
         initParameterBinding(binder);
         binder.BindToSource(dataSource);
         _cache.Add(gridView, binder);
      }

      public void ExpandAllRows(GridView view)
      {
         view.BeginUpdate();
         try
         {
            int dataRowCount = view.DataRowCount;
            for (int rHandle = 0; rHandle < dataRowCount; rHandle++)
            {
               view.SetMasterRowExpanded(rHandle, true);
            }
         }
         finally
         {
            view.EndUpdate();
         }
      }

      private void initParameterBinding(GridViewBinder<IParameterDTO> parameterBinder)
      {
         parameterBinder.Bind(x => x.DisplayName)
            .WithCaption(PKSimConstants.UI.Name)
            .AsReadOnly();

         parameterBinder.Bind(x => x.Value)
            .WithCaption(PKSimConstants.UI.Value)
            .WithFormat(param => param.ParameterFormatter())
            .WithEditorConfiguration((activeEditor, param) => _comboBoxUnit.UpdateUnitsFor(activeEditor, param))
            .WithOnValueUpdating((p, valueInGuiUnit) => OnEvent(() => _presenter.SetParameterValue(p, valueInGuiUnit.NewValue)));

         parameterBinder.Bind(x => x.IsFavorite)
            .WithCaption(PKSimConstants.UI.Favorites)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH)
            .WithRepository(x => new UxRepositoryItemCheckEdit(parameterBinder.GridView))
            .WithToolTip(PKSimConstants.UI.FavoritesToolTip)
            .WithOnValueUpdating((o, e) => OnEvent(() => _presenter.SetFavorite(o, e.NewValue)));
      }

      public void AttachPresenter(IApplicationParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridApplicationsBinder = new GridViewBinder<ApplicationDTO>(mainView) {BindingMode = BindingMode.OneWay};
         _gridApplicationsBinder.Bind(x => x.Name)
            .WithRepository(applicationDisplay)
            .WithShowButton(ShowButtonModeEnum.ShowOnlyInEditor)
            .AsReadOnly();

         //Create a dummy column for the detail view to avoid auto binding to details
         gridViewParameters.Columns.AddField("Dummy");
         _comboBoxUnit.ParameterUnitSet += (p, unit) => OnEvent(() => _presenter.SetParameterUnit(p, unit));
      }

      private RepositoryItem applicationDisplay(ApplicationDTO applicationDTO)
      {
         var applicationRepository = new UxRepositoryItemImageComboBox(mainView, _imageListRetriever);
         return applicationRepository.AddItem(applicationDTO.Name, applicationDTO.Icon);
      }

      public void BindTo(IEnumerable<ApplicationDTO> allApplications)
      {
         _gridApplicationsBinder.BindToSource(allApplications);
         mainView.RefreshData();
         ExpandAllRows(mainView);
      }

      public bool ParameterNameEditable
      {
         //nothing to do 
         set { }
      }

      public bool ParameterNameVisible
      {
         //nothing to do 
         set { }
      }

      public override bool HasError
      {
         get { return _gridApplicationsBinder.HasError || _cache.Any(x => x.HasError); }
      }
   }
}