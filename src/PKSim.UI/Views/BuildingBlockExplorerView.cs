using OSPSuite.UI.Services;
using DevExpress.XtraTreeList;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Views.Main;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views
{
   public partial class BuildingBlockExplorerView : BaseExplorerView, IBuildingBlockExplorerView
   {
      public BuildingBlockExplorerView(IImageListRetriever imageListRetriever)
         : base(imageListRetriever)
      {
         InitializeComponent();
         treeView.CompareNodeValues += compareNodeValues;
         treeView.OptionsSelection.MultiSelect = true;
      }

      public void AttachPresenter(IBuildingBlockExplorerPresenter presenter)
      {
         base.AttachPresenter(presenter);
      }

      private void compareNodeValues(object sender, CompareNodeValuesEventArgs e)
      {
         if (e.Node1 == null)
            return;

         //we do not want to sort the root nodes
         if (e.Node1.Level == 0)
            e.Result = 0;
      }
   }
}