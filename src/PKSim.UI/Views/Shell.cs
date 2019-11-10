using System;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraBars;
using PKSim.Presentation.Presenters.Main;
using PKSim.Presentation.Regions;
using PKSim.Presentation.Views.Main;
using OSPSuite.Presentation;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views
{
   public partial class Shell : BaseShell, IPKSimMainView
   {
      private IPKSimMainViewPresenter _presenter;

      public Shell(IContainer container)
      {
         InitializeComponent();

         container.RegisterImplementationOf(ribbon.Manager);
         container.RegisterImplementationOf(ribbon.Manager as BarManager);
         container.RegisterImplementationOf(defaultLookAndFeel.LookAndFeel);
         container.RegisterImplementationOf(xtraTabbedMdiManager);
         container.RegisterImplementationOf(dockManager);
         container.RegisterImplementationOf(applicationMenu);
         container.RegisterImplementationOf(panelControlAppMenuFileLabels);

         //make sure that hothey are not used in page header
         defaultBarAndDockingController.Controller.AppearancesRibbon.PageHeader.TextOptions.HotkeyPrefix = HKeyPrefix.None;

         AllowDrop = true;
         DragEnter += (o, e) => this.DoWithinExceptionHandler(() => dragEnter(e));
         DragDrop += (o, e) => this.DoWithinExceptionHandler(() => dragDrop(e));
      }

      private void dragDrop(DragEventArgs e)
      {
         if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

         var filePaths = (string[]) (e.Data.GetData(DataFormats.FileDrop));
         if (!filePaths.Any()) return;

         _presenter.OpenFile(filePaths.First());
      }

      private void dragEnter(DragEventArgs e)
      {
         if (e.Data.GetDataPresent(DataFormats.FileDrop))
            e.Effect = DragDropEffects.Copy;
         else
            e.Effect = DragDropEffects.None;
      }


      protected override void OnLoad(EventArgs e)
      {
         SuspendLayout();
         WindowState = FormWindowState.Maximized;
         RaiseLoadingEvent();
         base.OnLoad(e);
         ResumeLayout();
         Refresh();
         ribbon.ForceInitialize();
         this.DoWithinExceptionHandler(() => _presenter.Run());
      }

      public override void Initialize()
      {
         base.Initialize();
         InitializeDockManager(dockManager);
         InitializeRibbon(ribbon, applicationMenu, rightPaneAppMenu);
         var imageListRetriever = IoC.Resolve<IImageListRetriever>();
         InitializeImages(imageListRetriever);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.PKSim;
         Icon = ApplicationIcon.WithSize(IconSizes.Size32x32);
      }

      protected override void RegisterRegions()
      {
         base.RegisterRegions();
         RegisterRegion(_panelBuildingBlockExplorer, RegionNames.BuildingBlockExplorer);
         RegisterRegion(_panelSimulationExplorer, RegionNames.SimulationExplorer);
         RegisterRegion(_panelHistoryBrowser, RegionNames.History);
         RegisterRegion(_panelComparison, RegionNames.Comparison);
         RegisterRegion(_panelJournal, RegionNames.Journal);
         RegisterRegion(_panelJournalDiagram, RegionNames.JournalDiagram);
      }

      public void AttachPresenter(IPKSimMainViewPresenter presenter)
      {
         _presenter = presenter;
         base.AttachPresenter(presenter);
      }
   }
}