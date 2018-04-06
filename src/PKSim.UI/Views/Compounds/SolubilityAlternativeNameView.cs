using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds
{
   public partial class SolubilityAlternativeNameView : ObjectBaseView, ISolubilityAlternativeNameView
   {
      private readonly UserLookAndFeel _lookAndFeel;
      private ISolubilityAlternativeNamePresenter _presenter;
      private readonly UxCheckEdit _createTableAlternative = new UxCheckEdit();

      public SolubilityAlternativeNameView()
      {
      }

      public SolubilityAlternativeNameView(IShell shell, UserLookAndFeel lookAndFeel) : base(shell)
      {
         _lookAndFeel = lookAndFeel;
         InitializeComponent();
      }

      public void AttachPresenter(ISolubilityAlternativeNamePresenter presenter)
      {
         base.AttachPresenter(presenter);
         _presenter = presenter;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         _createTableAlternative.Text = PKSimConstants.UI.CreateTableSolubilityAlternative;
         insertControlAtTop(_createTableAlternative);
      }

      private void insertControlAtTop(Control controlToAdd)
      {
         var item = layoutControl.Root.AddItem();
         item.Control = controlToAdd;
         item.InitializeAsHeader(_lookAndFeel, string.Empty);
         item.Move(layoutItemName, InsertType.Top);
      }
   }
}