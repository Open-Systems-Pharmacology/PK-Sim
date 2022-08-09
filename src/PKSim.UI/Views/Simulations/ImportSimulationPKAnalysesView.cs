using DevExpress.XtraEditors.Controls;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class ImportSimulationPKAnalysesView : BaseModalView, IImportSimulationPKAnalysesView
   {
      private IImportSimulationPKAnalysesPresenter _presenter;
      private readonly ScreenBinder<ImportPKAnalysesDTO> _screenBinder;

      public ImportSimulationPKAnalysesView(Shell shell) : base(shell)
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<ImportPKAnalysesDTO>();
      }

      public void AttachPresenter(IImportSimulationPKAnalysesPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(ImportPKAnalysesDTO importPKAnalysesDTO)
      {
         _screenBinder.BindToSource(importPKAnalysesDTO);
         RegisterValidationFor(_screenBinder, NotifyViewChanged);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.FilePath)
            .To(tbFileToImport);

         _screenBinder.Bind(x => x.Messages)
            .To(tbLog);

         _screenBinder.Bind(x => x.FileDefined)
            .ToEnableOf(btnImport);

         tbFileToImport.ButtonClick += (o, e) => OnEvent(_presenter.SelectFile);
         btnImport.Click += (o, e) => OnEvent(() => _presenter.StartImportProcess());
      }

      public string Description
      {
         get { return lblDescription.Text; }
         set { lblDescription.Text = value; }
      }

      public bool ImportingResults
      {
         get { return !btnImport.Enabled; }
         set
         {
            btnImport.Enabled = !value;
            tbFileToImport.Enabled = btnImport.Enabled;
         }
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();

         layoutItemSelectFileToImport.Text = PKSimConstants.UI.FilePath.FormatForLabel();
         btnImport.InitWithImage(ApplicationIcons.Run, text: PKSimConstants.UI.StartImport);
         layoutItemButtonImport.AdjustLargeButtonSize(layoutControl);

         tbFileToImport.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
         Caption = PKSimConstants.UI.ImportSimulationPKAnalyses;

         lblDescription.AsDescription();
         lblDescription.Text = PKSimConstants.UI.ImportSimulationPKAnalysesDescription;
         tbLog.Properties.ReadOnly = true;
      }
   }
}