using OSPSuite.Assets;
using DevExpress.XtraTab;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Presentation;
using OSPSuite.UI.Views;

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

      public override ApplicationIcon ApplicationIcon => _icon;

      public void UpdateIcon(ApplicationIcon speciesIcon)
      {
         _icon = speciesIcon;
      }

      public override XtraTabControl TabControl => tabEditIndividual;
   }
}