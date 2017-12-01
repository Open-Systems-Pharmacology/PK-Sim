using System;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using PKSim.BatchTool.DTO;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.Views
{
   public partial class FolderListSnapshotView : BaseUserControl, IFolderListSnapshotView
   {
      private IFolderListSnapshotPresenter _presenter;
      private readonly ScreenBinder<SnapshotFolderListDTO> _screenBinder = new ScreenBinder<SnapshotFolderListDTO>();
      private readonly GridViewBinder<FolderDTO> _gridViewBinder;
      private readonly UxRepositoryItemButtonEdit _deleteRepository = new UxRemoveButtonRepository();
      private SnapshotFolderListDTO _snapshotFolderListDTO;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public FolderListSnapshotView()
      {
         InitializeComponent();
         gridView.AllowsFiltering = false;
         gridView.ShowColumnHeaders = false;
         _gridViewBinder = new GridViewBinder<FolderDTO>(gridView);
      }

      public void AttachPresenter(IFolderListSnapshotPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SnapshotFolderListDTO snapshotFolderListDTO)
      {
         _snapshotFolderListDTO = snapshotFolderListDTO;
         _screenBinder.BindToSource(snapshotFolderListDTO);
         _gridViewBinder.BindToSource(snapshotFolderListDTO.Folders);
         radioGroupExportMode.EditValue = snapshotFolderListDTO.ExportMode;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         buttonSelectFolder.Properties.Buttons[0].Kind = ButtonPredefines.Glyph;
         buttonSelectFolder.Properties.Buttons[0].Caption = Captions.Browse;
         buttonSelectFolder.ButtonClick += (o, e) => OnEvent(_presenter.SelectFolder);

         buttonAdd.Click += (o, e) => OnEvent(() => _presenter.AddFolder(buttonSelectFolder.Text));
         buttonClearList.Click += (o, e) => OnEvent(_presenter.ClearFolderList);
         buttonImportList.Click += (o, e) => OnEvent(_presenter.ImportFolderList);
         buttonExportList.Click += (o, e) => OnEvent(_presenter.ExportFolderList);

         _screenBinder.Bind(x => x.CurrentFolder)
            .To(buttonSelectFolder);

         _gridViewBinder.Bind(x => x.Folder);
         _gridViewBinder.AddUnboundColumn()
            .WithCaption(UIConstants.EMPTY_COLUMN)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _deleteRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH);

         _gridViewBinder.Changed += NotifyViewChanged;

         radioGroupExportMode.SelectedIndexChanged += (o, e) => OnEvent(exportModeChanged);
         _deleteRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.RemoveFolder(_gridViewBinder.FocusedElement));
      }

      private void exportModeChanged()
      {
         _snapshotFolderListDTO.ExportMode = (SnapshotExportMode) radioGroupExportMode.EditValue;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemButtonAdd.AdjustButtonSizeWithImageOnly();
         buttonAdd.InitWithImage(ApplicationIcons.Add, imageLocation: ImageLocation.MiddleCenter);

         layoutItemSelectFolder.TextVisible = false;

         layoutItemButtonClearList.AdjustButtonSize();
         buttonClearList.InitWithImage(ApplicationIcons.Delete, Text = Captions.ClearList);

         layoutItemButtonExportList.AdjustButtonSize();
         buttonExportList.InitWithImage(ApplicationIcons.Save, Text = Captions.ExportList);

         layoutItemButtonImportList.AdjustButtonSize();
         buttonImportList.InitWithImage(ApplicationIcons.LoadFromTemplate, Text = Captions.ImportList);

         Caption = Captions.SnapshotFolderList;
         radioGroupExportMode.AddExportModes(layoutItemExportMode);
         layoutItemExportMode.TextVisible = false;
      }

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void Repaint()
      {
         //nothing to do
      }

      public int OptimalHeight => layoutItemButtonAdd.Height + Math.Max(gridView.OptimalHeight, 100) + layoutItemButtonClearList.Height;
   }
}