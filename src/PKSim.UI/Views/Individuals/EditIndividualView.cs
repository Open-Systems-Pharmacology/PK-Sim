using DevExpress.XtraTab;
using OSPSuite.Assets;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.UI.Views.Individuals
{
   public partial class EditIndividualView : BaseMdiChildTabbedView, IEditIndividualView
   {
      private ApplicationIcon _icon = ApplicationIcons.Individual;

      public EditIndividualView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IEditIndividualPresenter presenter)
      {
         _presenter = presenter;
      }

      public override XtraTabControl TabControl => tabEditIndividual;
   }
}