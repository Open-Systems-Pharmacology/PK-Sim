using OSPSuite.UI.Extensions;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Parameters
{
   public partial class NoItemInSelectionView : BaseUserControl, INoItemInSelectionView
   {
      private INoItemInSelectionPresenter _presenter;

      public NoItemInSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(INoItemInSelectionPresenter presenter)
      {
         _presenter = presenter;
      }

      public string Description
      {
         set { lblDescription.Text = value; }
         get { return lblDescription.Text; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         lblDescription.AsDescription();
      }
   }
}