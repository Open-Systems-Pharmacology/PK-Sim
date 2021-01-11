using DevExpress.XtraLayout.Utils;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Core
{
   public partial class ConfigurableLayoutView : BaseContainerUserControl, IConfigurableLayoutView
   {
      public ConfigurableLayoutView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IConfigurableLayoutPresenter presenter)
      {
         /*nothing to do*/
      }

      public void Clear()
      {
         layoutMainControl.Clear(clearHiddenItems: true, disposeControls: true);
      }

      public void SetView(IView view)
      {
         var layoutItem = AddViewToGroup(layoutMainControl.Root, view);
         layoutItem.Padding = new Padding(0);
      }
   }
}