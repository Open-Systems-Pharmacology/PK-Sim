using PKSim.Presentation.Views;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Core
{
   public partial class AccordionLayoutView : BaseContainerUserControl, IAccordionLayoutView
   {
      public AccordionLayoutView()
      {
         InitializeComponent();
      }

      public void AddView(IView view)
      {
         var group = layoutControl.Root.AddGroup();
         AddViewToGroup(group, view);

         group.ExpandButtonVisible = true;
      }

      public void FinishedAddingViews() => AddEmptyPlaceHolder(layoutControl);
   }
}