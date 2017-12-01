using System;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.BatchTool.DTO;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.Services;

namespace PKSim.BatchTool.Views
{
   public partial class SingleFolderSnapshotView : BaseUserControl, ISingleFolderSnapshotView
   {
      private ISingleFolderSnapshotPresenter _presenter;
      private readonly ScreenBinder<SnapshotSingleFolderDTO> _screenBinder = new ScreenBinder<SnapshotSingleFolderDTO>();
      private SnapshotSingleFolderDTO _snapshotSingleFolderDTO;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public SingleFolderSnapshotView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISingleFolderSnapshotPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(SnapshotSingleFolderDTO snapshotSingleFolderDTO)
      {
         _snapshotSingleFolderDTO = snapshotSingleFolderDTO;
         _screenBinder.BindToSource(snapshotSingleFolderDTO);
         radioGroupExportMode.EditValue = snapshotSingleFolderDTO.ExportMode;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.InputFolder)
            .To(btnInputFolderSelection);

         _screenBinder.Bind(x => x.OutputFolder)
            .To(btnOutputFolderSelection);

         btnInputFolderSelection.ButtonClick += (o, e) => OnEvent(_presenter.SelectInputFolder);
         btnOutputFolderSelection.ButtonClick += (o, e) => OnEvent(_presenter.SelectOutputFolder);

         radioGroupExportMode.SelectedIndexChanged += (o, e) => OnEvent(exportModeChanged);

         RegisterValidationFor(_screenBinder, NotifyViewChanged, NotifyViewChanged);
      }

      private void exportModeChanged()
      {
         _snapshotSingleFolderDTO.ExportMode = (SnapshotExportMode)radioGroupExportMode.EditValue;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemInputFolder.Text = Captions.SelectSnapshotInputFolder.FormatForLabel();
         layoutItemOutputFolder.Text = Captions.SelectSnapshotOutputFolder.FormatForLabel();
         layoutItemExportMode.TextVisible = false;
         Caption = Captions.SnapshotSingleFolder;
         radioGroupExportMode.AddExportModes(layoutItemExportMode);
      }

      public override bool HasError => _screenBinder.HasError;

      public void AdjustHeight() => HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));

      public void Repaint()
      {
         //nothing to do
      }

      public int OptimalHeight => layoutGroupProperties.Height;
   }
}