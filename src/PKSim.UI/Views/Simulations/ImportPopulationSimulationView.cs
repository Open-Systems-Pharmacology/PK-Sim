using System;
using System.Windows.Forms;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Extensions;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class ImportPopulationSimulationView : BaseModalView, IImportPopulationSimulationView
   {
      private IImportPopulationSimulationPresenter _presenter;
      private readonly ScreenBinder<ImportPopulationSimulationDTO> _screenBinder;
      private readonly UxBuildingBlockSelection _uxPopulationSelection;
      private readonly Cache<PopulationImportMode, RadioButton> _allRadioButtons;
      private ImportPopulationSimulationDTO _importPopulationSimulationDTO;

      public ImportPopulationSimulationView(Shell shell) : base(shell)
      {
         InitializeComponent();
         _uxPopulationSelection = new UxBuildingBlockSelection();
         _screenBinder = new ScreenBinder<ImportPopulationSimulationDTO>();

         rbBuildingBockSelection.Tag = PopulationImportMode.BuildingBlock;
         rbPopulationFileSelection.Tag = PopulationImportMode.File;
         rbPopulationSizeSelection.Tag = PopulationImportMode.Size;

         _allRadioButtons = new Cache<PopulationImportMode, RadioButton>(importModeFrom)
         {
            rbBuildingBockSelection,
            rbPopulationFileSelection,
            rbPopulationSizeSelection
         };
      }

      private static PopulationImportMode importModeFrom(RadioButton radioButton)
      {
         return radioButton.Tag.DowncastTo<PopulationImportMode>();
      }

      public void AttachPresenter(IImportPopulationSimulationPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ImportPopulationSimulationDTO importPopulationSimulationDTO)
      {
         _importPopulationSimulationDTO = importPopulationSimulationDTO;
         _screenBinder.BindToSource(importPopulationSimulationDTO);
         _allRadioButtons[importPopulationSimulationDTO.PopulationImportMode].Checked = true;
         layoutItemBuildingBlockSelection.Text = _uxPopulationSelection.BuildingBlockType.FormatForLabel();
      }

      public bool ImportEnabled
      {
         get => btnStartImport.Enabled;
         set => btnStartImport.Enabled = value;
      }

      public bool SimulationSelectionVisible
      {
         get => layoutControlGroupSimulationFileSelection.Visible;
         set => layoutControlGroupSimulationFileSelection.Visibility = LayoutVisibilityConvertor.FromBoolean(value);
      }

      public override void InitializeBinding()
      {
         panelBuildingBlockSelection.FillWith((Control) _uxPopulationSelection);
         _screenBinder.Bind(x => x.Population)
            .To(_uxPopulationSelection);

         _screenBinder.Bind(x => x.FilePath)
            .To(tbSimulationFile);

         _screenBinder.Bind(x => x.PopulationFile)
            .To(tbPopulationFile);

         _screenBinder.Bind(x => x.NumberOfIndividuals)
            .To(tbNumberOfIndividuals);

         _screenBinder.Bind(x => x.Messages)
            .To(tbLog);


         btnStartImport.Click += (o, e) => OnEvent(_presenter.StartImport);
         tbSimulationFile.Click += (o, e) => OnEvent(_presenter.SelectSimulationFile, notifyViewChanged: true);
         tbPopulationFile.Click += (o, e) => OnEvent(_presenter.SelectPopulationFile, notifyViewChanged: true);
         _allRadioButtons.Each(rb => rb.CheckedChanged += (o, e) => OnEvent(() => updateImportMode(importModeFrom(rb), rb.Checked)));
         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      private void updateImportMode(PopulationImportMode importMode, bool selected)
      {
         populationSelectionEnabled = false;
         tbNumberOfIndividuals.Enabled = false;
         tbPopulationFile.Enabled = false;

         if (!selected)
            return;

         //only perform UI update when radio button is the one being selected
         switch (importMode)
         {
            case PopulationImportMode.BuildingBlock:
               populationSelectionEnabled = true;
               break;
            case PopulationImportMode.File:
               tbPopulationFile.Enabled = true;
               break;
            case PopulationImportMode.Size:
               tbNumberOfIndividuals.Enabled = true;
               break;
            default:
               throw new ArgumentOutOfRangeException("importMode");
         }

         _importPopulationSimulationDTO.PopulationImportMode = importMode;
         NotifyViewChanged();
      }

      private bool populationSelectionEnabled
      {
         set
         {
            _uxPopulationSelection.Enabled = value;
            panelBuildingBlockSelection.Enabled = value;
            _uxPopulationSelection.DisplayNotification = value;
         }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         tbPopulationFile.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
         tbSimulationFile.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

         layoutItemSimulationFileSelection.Text = PKSimConstants.UI.SimulationFilePath.FormatForLabel();
         layoutItemPopulationFileSelection.Text = PKSimConstants.UI.PopulationFilePath.FormatForLabel();
         layoutItemNumberOfIndividuals.Text = PKSimConstants.UI.NumberOfIndividuals.FormatForLabel();
         btnStartImport.InitWithImage(ApplicationIcons.Run, text: PKSimConstants.UI.StartImport);
         layoutItemButtonImport.AdjustLargeButtonSize(layoutControl);

         layoutControlGroupSimulationFileSelection.Text = PKSimConstants.ObjectTypes.Simulation;
         layoutControlGroupPopulationSelection.Text = PKSimConstants.ObjectTypes.Population;

         rbBuildingBockSelection.Text = PKSimConstants.UI.UsePopulationBuildingBlock;
         rbPopulationFileSelection.Text = PKSimConstants.UI.UsePopulationFileCSV;
         rbPopulationSizeSelection.Text = PKSimConstants.UI.NewPopulationFromSize;

         ApplicationIcon = ApplicationIcons.PopulationSimulationLoad;
         Caption = PKSimConstants.UI.ImportPopulationSimulation;

         tbLog.Properties.ReadOnly = true;
      }

      public override bool HasError => _screenBinder.HasError;
   }
}