using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using DevExpress.XtraTab;
using PKSim.Assets;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Extensions;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Core
{
   public partial class BuildingBlockWizardView : WizardView
   {
      private ScreenBinder<ObjectBaseDTO> _buildingBlockBinder;

      public BuildingBlockWizardView()
      {
      }

      public BuildingBlockWizardView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      protected override void SetActiveControl()
      {
         ActiveControl = tbName;
      }

      public override XtraTabControl TabControl => tabControl;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemName.Text = PKSimConstants.UI.Name.FormatForLabel();
         this.ReziseForCurrentScreen(fractionHeight: SCREEN_RESIZE_FRACTION, fractionWidth: SCREEN_RESIZE_FRACTION);
      }

      public virtual void BindToProperties(ObjectBaseDTO populationPropertiesDTO)
      {
         _buildingBlockBinder.BindToSource(populationPropertiesDTO);
         NotifyViewChanged();
      }

      public override void InitializeBinding()
      {
         _buildingBlockBinder = new ScreenBinder<ObjectBaseDTO>();
         _buildingBlockBinder.Bind(x => x.Name).To(tbName);

         RegisterValidationFor(_buildingBlockBinder, NotifyViewChanged);
      }

      public override bool HasError => _buildingBlockBinder.HasError;
   }
}