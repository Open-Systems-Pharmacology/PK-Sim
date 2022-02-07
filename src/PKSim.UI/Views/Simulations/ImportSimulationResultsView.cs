using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using static OSPSuite.UI.UIConstants.Size;
using static PKSim.UI.UIConstants.Size;
using UIConstants = OSPSuite.UI.UIConstants;

namespace PKSim.UI.Views.Simulations
{
   public partial class ImportSimulationResultsView : BaseModalView, IImportSimulationResultsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IImportSimulationResultsPresenter _presenter;
      private readonly GridViewBinder<SimulationResultsFileSelectionDTO> _gridViewBinder;
      private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();
      private readonly RepositoryItemButtonEdit _filePathRepository = new RepositoryItemButtonEdit();
      private readonly RepositoryItemPictureEdit _statusIconRepository = new RepositoryItemPictureEdit();
      private readonly ScreenBinder<ImportSimulationResultsDTO> _screenBinder;
      private readonly ToolTipController _toolTipController;
      private IGridViewColumn _colImage;

      public ImportSimulationResultsView(Shell shell, IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
         : base(shell)
      {
         InitializeComponent();
         gridView.AllowsFiltering = false;
         gridView.ShowRowIndicator = false;
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
         _gridViewBinder = new GridViewBinder<SimulationResultsFileSelectionDTO>(gridView) {BindingMode = BindingMode.TwoWay};
         _filePathRepository.Buttons[0].Kind = ButtonPredefines.Ellipsis;
         _screenBinder = new ScreenBinder<ImportSimulationResultsDTO>();
         _toolTipCreator = toolTipCreator;
         _toolTipController = new ToolTipController()
            .Initialize(imageListRetriever)
            .For(gridControl);

         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      public void AttachPresenter(IImportSimulationResultsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.ImportFolder)
            .To(btnBrowseForFolder);

         _screenBinder.Bind(x => x.Messages)
            .To(tbLog);

         _colImage = _gridViewBinder.Bind(x => x.Image)
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithRepository(dto => _statusIconRepository)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH);

         _gridViewBinder.Bind(x => x.FilePath)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _filePathRepository)
            .WithCaption(PKSimConstants.UI.FilePath);

         _gridViewBinder.Bind(x => x.NumberOfIndividuals)
            .WithCaption(PKSimConstants.UI.NumberOfIndividuals)
            .WithFixedWidth(PARAMETER_WIDTH)
            .WithFormat(new NullIntParameterFormatter())
            .AsReadOnly();

         _gridViewBinder.Bind(x => x.NumberOfQuantities)
            .WithCaption(PKSimConstants.UI.NumberOfOutputs)
            .WithFixedWidth(PARAMETER_WIDTH)
            .WithFormat(new NullIntParameterFormatter())
            .AsReadOnly();

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(x => _removeButtonRepository)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _removeButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.RemoveFile(_gridViewBinder.FocusedElement));
         _filePathRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.ChangePath(_gridViewBinder.FocusedElement));

         btnAddFile.Click += (o, e) => OnEvent(_presenter.AddFile);
         btnBrowseForFolder.ButtonClick += (o, e) => OnEvent(_presenter.BrowseForFolder);
         btnImport.Click += (o, e) => OnEvent(() => _presenter.StartImportProcess());

         _gridViewBinder.Changed += NotifyViewChanged;
         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public bool ImportingResults
      {
         get { return layoutControlGroup.Enabled; }
         set { layoutControlGroup.Enabled = !value; }
      }

      public void BindTo(ImportSimulationResultsDTO importSimulationResultsDTO)
      {
         _gridViewBinder.BindToSource(importSimulationResultsDTO.SimulationResultsFile);
         _screenBinder.BindToSource(importSimulationResultsDTO);
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var col = gridView.ColumnAt(e);
         if (_colImage.XtraColumn != col) return;

         var simulationResultsFile = _gridViewBinder.ElementAt(e);
         if (simulationResultsFile == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(simulationResultsFile);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(simulationResultsFile, superToolTip);
      }

      public bool ImportEnabled
      {
         get { return btnImport.Enabled; }
         set { btnImport.Enabled = value; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();

         btnAddFile.InitWithImage(ApplicationIcons.Create, text: PKSimConstants.UI.AddFile);
         layoutItemButtonAdd.AdjustLargeButtonSize();

         btnImport.InitWithImage(ApplicationIcons.Run, text: PKSimConstants.UI.StartImport);
         layoutItemButtonImport.AdjustLargeButtonSize();

         layoutGroupImportFolder.Text = PKSimConstants.UI.ImportFolder;
         layoutGroupImportSingleFiles.Text = PKSimConstants.UI.ImportFiles;

         btnBrowseForFolder.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
         Caption = PKSimConstants.UI.ImportSimulationResults;
         Icon = ApplicationIcons.ClusterExport.WithSize(IconSizes.Size16x16);
         tbLog.Properties.ReadOnly = true;
      }

      public override bool HasError
      {
         get { return _gridViewBinder.HasError || _screenBinder.HasError; }
      }
   }
}