using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Observers;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.UI.Views.Observers
{
   public partial class ImportObserverSetView : BaseUserControl, IImportObserverSetView
   {
      private IImportObserverSetPresenter _presenter;
      private readonly GridViewBinder<ImportObserverDTO> _gridViewBinder;
      private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();
      private IGridViewColumn _colFilePath;

      public ImportObserverSetView()
      {
         InitializeComponent();
         gridView.AllowsFiltering = false;
         gridView.ShowRowIndicator = false;
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
         _gridViewBinder = new GridViewBinder<ImportObserverDTO>(gridView);
      }

      public void AttachPresenter(IImportObserverSetPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IReadOnlyList<ImportObserverDTO> observers)
      {
         _gridViewBinder.BindToSource(observers);
         selectFirstRow(observers);
      }

      public void Rebind()
      {
         _gridViewBinder.Rebind();
      }

      public void AddObserverView(IView view)
      {
         panelObservedDataDetails.FillWith(view);
      }

      public void SelectObserver(ImportObserverDTO observerDTO)
      {
         if (observerDTO == null) return;
         gridView.FocusedRowHandle = _gridViewBinder.RowHandleFor(observerDTO);
      }

      public bool ShowFilePath
      {
         get => _colFilePath.Visible;
         set => _colFilePath.Visible = value;
      }

      private void selectFirstRow(IReadOnlyList<ImportObserverDTO> observers)
      {
         SelectObserver(observers.FirstOrDefault());
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();

         _gridViewBinder.Bind(x => x.Name)
            .AsReadOnly();

         _colFilePath = _gridViewBinder.Bind(x => x.FilePath)
            .WithCaption(PKSimConstants.UI.FilePath)
            .AsReadOnly();

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _removeButtonRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _removeButtonRepository.ButtonClick += (o, e) => OnEvent(_presenter.RemoveObserver, _gridViewBinder.FocusedElement);

         btnAddFile.Click += (o, e) => OnEvent(_presenter.AddObserver, notifyViewChanged: true);
         gridView.FocusedRowChanged += (o, e) => OnEvent(selectedRowChanged, e);
      }

      private void selectedRowChanged(FocusedRowChangedEventArgs e)
      {
         var observer = _gridViewBinder.ElementAt(e.FocusedRowHandle);
         _presenter.SelectObserver(observer);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemAddFile.AdjustLargeButtonSize();
         btnAddFile.InitWithImage(ApplicationIcons.Create, PKSimConstants.UI.AddFile);
         Caption = PKSimConstants.UI.ImportPopulationSettings;
         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.ImportObserversDescription;
      }
   }
}