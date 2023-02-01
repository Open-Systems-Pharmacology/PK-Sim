using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Presentation.Views;

namespace PKSim.UI.Views.Core
{
   public partial class AccordionLayoutView : BaseContainerUserControl, IAccordionLayoutView
   {
      public AccordionLayoutView()
      {
         InitializeComponent();
      }

      public void StartAddingViews()
      {
         layoutControl.SuspendLayout();
      }

      public void AddView(IView view)
      {
         var group = layoutControl.Root.AddGroup();
         var layoutItem = AddViewToGroup(group, layoutControl, view);
         var resizeView = view as IResizableWithDefaultHeightView;
         if (resizeView != null)
            this.AdjustLayoutItemSize(layoutItem,layoutControl,  resizeView, resizeView.DefaultHeight);

         group.ExpandButtonVisible = true;
      }

      public void FinishedAddingViews()
      {
         layoutControl.ResumeLayout();
      }
   }
}