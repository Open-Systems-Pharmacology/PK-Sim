using PKSim.Assets;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views
{
   public partial class UserDisplayUnitsView : BaseUserControl, IUserDisplayUnitsView
   {
      public UserDisplayUnitsView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IUserDisplayUnitsPresenter presenter)
      {
      }

      public void AddView(IDisplayUnitsView view)
      {
         panelControl.FillWith(view);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.UserDisplayUnitsConfigure;
         Caption = PKSimConstants.UI.UserDisplayUnits;
         layoutControlItem.TextVisible = false;
      }
   }
}
