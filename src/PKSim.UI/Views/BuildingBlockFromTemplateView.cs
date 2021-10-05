using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using DevExpress.XtraBars;
using DevExpress.XtraTreeList;
using NHibernate.Linq;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Core.Model;

namespace PKSim.UI.Views
{
   public partial class BuildingBlockFromTemplateView : BaseModalView, IBuildingBlockFromTemplateView, IViewWithPopup
   {
      private ITemplatePresenter _presenter;
      private readonly GridViewBinder<Template> _gridViewBinder;
      public bool Updating { get; private set; }

      public BuildingBlockFromTemplateView(IImageListRetriever imageListRetriever, Shell shell)
         : base(shell)
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<Template>(gridView);
         gridView.MultiSelect = true;
         gridView.ShouldUseColorForDisabledCell = false;
         toolTipController.Initialize(imageListRetriever);
         PopupBarManager = new BarManager {Form = this, Images = imageListRetriever.AllImagesForContextMenu};
         // gridView.SelectionChanged += (o, e) => OnEvent(_presenter.SelectedParametersChanged);
         // treeView.NodeClick += nodeClick;
         // tbDescription.Enabled = false;
         // treeView.OptionsSelection.MultiSelect = true;
         // treeView.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _gridViewBinder.AutoBind(x => x.Name)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.DatabaseType)
            .AsReadOnly();

         _gridViewBinder.AutoBind(x => x.Description)
            .AsReadOnly();
      }
      // private void nodeClick(MouseEventArgs e, ITreeNode selectedNode)
      // {
      //    if (e.Button == MouseButtons.Right)
      //    {
      //       selectNode(selectedNode);
      //       _presenter.CreatePopupMenuFor(selectedNode).At(e.Location);
      //    }
      //    else
      //    {
      //       var treeNodes = treeView.Selection.Select(treeView.NodeFrom).ToList();
      //       _presenter.ActivateNodes(treeNodes);
      //    }
      // }

      // private void selectNode(ITreeNode nodeToSelect)
      // {
      //    treeView.Selection.Clear();
      //
      //    if (nodeToSelect == null)
      //       return;
      //
      //    //Ensure we only have one node selected for context menu
      //    treeView.FocusedNode = treeView.NodeFrom(nodeToSelect);
      //    treeView.Selection.Add(treeView.FocusedNode);
      // }

      public void AttachPresenter(ITemplatePresenter presenter)
      {
         _presenter = presenter;
      }

      public void SetIcon(ApplicationIcon icon)
      {
         Icon = icon;
      }

      public void SelectTemplate(Template template)
       {
      //    var node = treeView.NodeById(template.Id);
      //    treeView.SelectNode(node);
      }

      public void BindTo(IReadOnlyList<Template> availableTemplates)
      {
         _gridViewBinder.BindToSource(availableTemplates);
      }

      public BarManager PopupBarManager { get; }

      public void BeginUpdate()
      {
         gridView.BeginUpdate();
         Updating = true;

      }

      public void EndUpdate()
      {
         gridView.EndUpdate();
         Updating = false;
      }
   }
}