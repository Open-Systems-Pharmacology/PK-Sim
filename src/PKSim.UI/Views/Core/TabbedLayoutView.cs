using OSPSuite.UI.Extensions;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Core
{
   public partial class TabbedLayoutView : BaseContainerUserControl, ITabbedLayoutView
   {
      public TabbedLayoutView()
      {
         InitializeComponent();
         InitializeResources();
      }

      public void StartAddingViews()
      {
         xtraTabControl.SuspendLayout();
      }

      public void AddView(IView view)
      {
         var layoutControl = new LayoutControl();
         var xtraTabPage = new XtraTabPage();
         xtraTabPage.FillWith(layoutControl);
         xtraTabControl.TabPages.Add(xtraTabPage);
         xtraTabPage.Text = view.Caption;
         AddViewToGroup(layoutControl.Root, view);
         
         AddEmptyPlaceHolder(layoutControl);
      }

      public void FinishedAddingViews()
      {
         xtraTabControl.ResumeLayout();

      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         xtraTabControl.HeaderLocation = TabHeaderLocation.Right;
         xtraTabControl.HeaderOrientation=TabOrientation.Horizontal;
      }
   }
}