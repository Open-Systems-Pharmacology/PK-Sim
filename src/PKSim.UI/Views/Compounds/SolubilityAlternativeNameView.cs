using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Utils;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
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
      private readonly UxCheckEdit _createTableAlternative = new UxCheckEdit();
      private readonly ScreenBinder<ISolubilityAlternativeNamePresenter> _screenBinder = new ScreenBinder<ISolubilityAlternativeNamePresenter>();

      public SolubilityAlternativeNameView(IShell shell, UserLookAndFeel lookAndFeel) : base(shell)
      {
         _lookAndFeel = lookAndFeel;
         InitializeComponent();
      }

      public void AttachPresenter(ISolubilityAlternativeNamePresenter presenter)
      {
         _screenBinder.BindToSource(presenter);
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _screenBinder.Bind(x => x.CreateAsTable)
            .To(_createTableAlternative)
            .WithCaption(PKSimConstants.UI.CreateTableSolubilityAlternative);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
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