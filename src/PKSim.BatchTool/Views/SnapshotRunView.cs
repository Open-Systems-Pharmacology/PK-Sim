using System.Drawing;
using DevExpress.XtraBars.Navigation;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;
using OSPSuite.UI.Extensions;
using OSPSuite.Utility.Collections;
using PKSim.BatchTool.Presenters;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.BatchTool.Views
{
   public partial class SnapshotRunView : BatchView, ISnapshotRunView
   {
      private ISnapshotRunPresenter _presenter;
      private readonly ScreenBinder<SnapshotRunOptions> _screenBinder = new ScreenBinder<SnapshotRunOptions>();
      private readonly Cache<IView, NavigationPage> _pageCache = new Cache<IView, NavigationPage>();
      private readonly Cache<NavigationPage, TileBarItem> _tileItemCache = new Cache<NavigationPage, TileBarItem>();

      public SnapshotRunView()
      {
         InitializeComponent();
      }

      public void BindTo(SnapshotRunOptions startOptions)
      {
         _screenBinder.BindToSource(startOptions);
      }

      public void AttachPresenter(ISnapshotRunPresenter presenter)
      {
         _presenter = presenter;
         base.AttachPresenter(presenter);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemPage.TextVisible = false;
         layoutItemTileBar.TextVisible = false;
         layoutItemPanelLog.TextVisible = false;
         tileBar.AllowSelectedItem = true;
         tileBar.AllowSelectedItemBorder = true;
         tileBar.SelectionBorderWidth = 2;
         tileBar.SelectionColorMode = SelectionColorMode.UseItemBackColor;
         Caption = Captions.SnapshotViewTitle;
         navigationFrame.SelectedPage = null;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _tileItemCache[singleFolderPage] = tileSingleFolderSelection;
         _tileItemCache[folderListPage] = tileFolderListSelection;

         tileSingleFolderSelection.ItemClick += (o, e) => OnEvent(_presenter.SingleFolderSelected);
         tileFolderListSelection.ItemClick += (o, e) => OnEvent(_presenter.FolderListSelected);

         startButton.Click += (o, e) => OnEvent(() => _presenter.RunBatch());
         stopButton.Click += (o, e) => OnEvent(() => _presenter.Exit());
      }

      public void AddSingleFolderView(IResizableView view)
      {
         _pageCache[view] = singleFolderPage;
         initView(view);
      }

      public void AddFolderListView(IResizableView view)
      {
         _pageCache[view] = folderListPage;
         initView(view);
      }

      public void SelectView(IView view)
      {
         var page = _pageCache[view];
         navigationFrame.SelectedPage = page;
         tileBar.SelectedItem = _tileItemCache[page];
      }

      private void updateNavigationFrameHeight(NavigationPage page, int height)
      {
         if (Equals(navigationFrame.SelectedPage, page))
            return;

         //add some extra padding 
         layoutItemPage.AdjustControlHeight(height + layoutItemPage.Padding.All + page.BackgroundPadding.All + 10);
      }

      private void initView(IResizableView view)
      {
         var page = _pageCache[view];
         var tileItem = _tileItemCache[page];
         page.FillWith(view);
         tileItem.Text = view.Caption.ToUpperInvariant();
         view.HeightChanged += (o, e) => updateNavigationFrameHeight(page, e.Height);

         tileItem.AppearanceItem.Normal.Font = new Font("Arial", 10, FontStyle.Bold);

         layoutItemButtonStart.AdjustSize(UIConstants.Size.LARGE_BUTTON_WIDTH, Constants.BUTTON_HEIGHT);
         startButton.InitWithImage(ApplicationIcons.Run, IconSizes.Size32x32, Captions.SnapshotStart);

         layoutItemButtonStop.AdjustSize(UIConstants.Size.LARGE_BUTTON_WIDTH, Constants.BUTTON_HEIGHT);
         stopButton.InitWithImage(ApplicationIcons.Stop, IconSizes.Size32x32, Captions.SnapshotStop);
      }

      public override void AddLogView(IView view)
      {
         panelLog.FillWith(view);
      }

      public override bool CalculateEnabled
      {
         set => startButton.Enabled = value;
      }
   }
}