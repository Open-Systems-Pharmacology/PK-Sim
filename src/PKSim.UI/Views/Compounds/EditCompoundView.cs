using DevExpress.XtraTab;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds
{
   public partial class EditCompoundView : BaseMdiChildTabbedView, IEditCompoundView
   {
      public EditCompoundView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditCompoundPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Compound;
      }

      public override XtraTabControl TabControl => tabEditCompound;
   }
}