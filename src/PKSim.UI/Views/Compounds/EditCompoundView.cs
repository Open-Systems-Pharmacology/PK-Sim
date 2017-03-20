using OSPSuite.Assets;
using DevExpress.XtraTab;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation;
using OSPSuite.UI.Views;

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

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Compound;

      public override XtraTabControl TabControl => tabEditCompound;

      protected override int TopicId => HelpId.PKSim_DefinitionCompounds;
   }
}