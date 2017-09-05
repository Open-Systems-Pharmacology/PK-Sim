using OSPSuite.UI.Controls;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views
{
   partial class Shell
   {
      /// <summary>p
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>p
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
         this.applicationMenu = new DevExpress.XtraBars.Ribbon.ApplicationMenu(this.components);
         this.rightPaneAppMenu = new DevExpress.XtraBars.PopupControlContainer(this.components);
         this.panelControlAppMenuFileLabels = new DevExpress.XtraEditors.PanelControl();
         this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
         this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
         this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
         this.dockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
         this.hideContainerBottom = new DevExpress.XtraBars.Docking.AutoHideContainer();
         this._panelHistoryBrowser = new OSPSuite.UI.Controls.UxDockPanel();
         this.controlContainer3 = new DevExpress.XtraBars.Docking.ControlContainer();
         this._panelComparison = new OSPSuite.UI.Controls.UxDockPanel();
         this.controlContainer2 = new DevExpress.XtraBars.Docking.ControlContainer();
         this._panelJournalDiagram = new OSPSuite.UI.Controls.UxDockPanel();
         this.controlContainer4 = new DevExpress.XtraBars.Docking.ControlContainer();
         this.panelContainer1 = new DevExpress.XtraBars.Docking.DockPanel();
         this._panelBuildingBlockExplorer = new OSPSuite.UI.Controls.UxDockPanel();
         this.panelComparisonContainer = new DevExpress.XtraBars.Docking.ControlContainer();
         this._panelSimulationExplorer = new OSPSuite.UI.Controls.UxDockPanel();
         this.controlContainer1 = new DevExpress.XtraBars.Docking.ControlContainer();
         this._panelJournal = new OSPSuite.UI.Controls.UxDockPanel();
         this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
         this.defaultBarAndDockingController = new DevExpress.XtraBars.DefaultBarAndDockingController(this.components);
         this.hideContainerRight = new DevExpress.XtraBars.Docking.AutoHideContainer();
         ((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.applicationMenu)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.rightPaneAppMenu)).BeginInit();
         this.rightPaneAppMenu.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelControlAppMenuFileLabels)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dockManager)).BeginInit();
         this.hideContainerBottom.SuspendLayout();
         this._panelHistoryBrowser.SuspendLayout();
         this._panelComparison.SuspendLayout();
         this._panelJournalDiagram.SuspendLayout();
         this.panelContainer1.SuspendLayout();
         this._panelBuildingBlockExplorer.SuspendLayout();
         this._panelSimulationExplorer.SuspendLayout();
         this._panelJournal.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.defaultBarAndDockingController.Controller)).BeginInit();
         this.hideContainerRight.SuspendLayout();
         this.SuspendLayout();
         // 
         // xtraTabbedMdiManager
         // 
         this.xtraTabbedMdiManager.ToolTipController = this.defaultToolTipController.DefaultController;
         // 
         // defaultLookAndFeel
         // 
         this.defaultLookAndFeel.LookAndFeel.SkinName = "Caramel";
         // 
         // ribbon
         // 
         this.ribbon.ApplicationButtonDropDownControl = this.applicationMenu;
         this.ribbon.ApplicationButtonText = null;
         this.ribbon.ExpandCollapseItem.Id = 0;
         this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem});
         this.ribbon.Location = new System.Drawing.Point(0, 0);
         this.ribbon.MaxItemId = 1;
         this.ribbon.Name = "ribbon";
         this.ribbon.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
         this.ribbon.Size = new System.Drawing.Size(1248, 50);
         this.ribbon.StatusBar = this.ribbonStatusBar;
         // 
         // applicationMenu
         // 
         this.applicationMenu.Name = "applicationMenu";
         this.applicationMenu.Ribbon = this.ribbon;
         this.applicationMenu.RightPaneControlContainer = this.rightPaneAppMenu;
         // 
         // rightPaneAppMenu
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.rightPaneAppMenu, DevExpress.Utils.DefaultBoolean.Default);
         this.rightPaneAppMenu.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.rightPaneAppMenu.Appearance.Options.UseBackColor = true;
         this.rightPaneAppMenu.AutoSize = true;
         this.rightPaneAppMenu.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.rightPaneAppMenu.Controls.Add(this.panelControlAppMenuFileLabels);
         this.rightPaneAppMenu.Controls.Add(this.labelControl1);
         this.rightPaneAppMenu.Controls.Add(this.panelControl1);
         this.rightPaneAppMenu.Location = new System.Drawing.Point(234, 100);
         this.rightPaneAppMenu.Name = "rightPaneAppMenu";
         this.rightPaneAppMenu.Size = new System.Drawing.Size(310, 162);
         this.rightPaneAppMenu.TabIndex = 6;
         this.rightPaneAppMenu.Visible = false;
         // 
         // panelControlAppMenuFileLabels
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.panelControlAppMenuFileLabels, DevExpress.Utils.DefaultBoolean.Default);
         this.panelControlAppMenuFileLabels.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.panelControlAppMenuFileLabels.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panelControlAppMenuFileLabels.Location = new System.Drawing.Point(10, 19);
         this.panelControlAppMenuFileLabels.Name = "panelControlAppMenuFileLabels";
         this.panelControlAppMenuFileLabels.Size = new System.Drawing.Size(300, 143);
         this.panelControlAppMenuFileLabels.TabIndex = 2;
         // 
         // labelControl1
         // 
         this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
         this.labelControl1.Appearance.Options.UseFont = true;
         this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
         this.labelControl1.Dock = System.Windows.Forms.DockStyle.Top;
         this.labelControl1.LineLocation = DevExpress.XtraEditors.LineLocation.Bottom;
         this.labelControl1.LineVisible = true;
         this.labelControl1.Location = new System.Drawing.Point(10, 0);
         this.labelControl1.Name = "labelControl1";
         this.labelControl1.Size = new System.Drawing.Size(300, 19);
         this.labelControl1.TabIndex = 0;
         this.labelControl1.Text = "Recent Documents:";
         // 
         // panelControl1
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.panelControl1, DevExpress.Utils.DefaultBoolean.Default);
         this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.panelControl1.Dock = System.Windows.Forms.DockStyle.Left;
         this.panelControl1.Location = new System.Drawing.Point(0, 0);
         this.panelControl1.Name = "panelControl1";
         this.panelControl1.Size = new System.Drawing.Size(10, 162);
         this.panelControl1.TabIndex = 1;
         // 
         // ribbonStatusBar
         // 
         this.ribbonStatusBar.Location = new System.Drawing.Point(0, 813);
         this.ribbonStatusBar.Name = "ribbonStatusBar";
         this.ribbonStatusBar.Ribbon = this.ribbon;
         this.ribbonStatusBar.Size = new System.Drawing.Size(1248, 25);
         // 
         // dockManager
         // 
         this.dockManager.AutoHideContainers.AddRange(new DevExpress.XtraBars.Docking.AutoHideContainer[] {
            this.hideContainerBottom,
            this.hideContainerRight});
         this.dockManager.Form = this;
         this.dockManager.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.panelContainer1});
         this.dockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
         // 
         // hideContainerBottom
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.hideContainerBottom, DevExpress.Utils.DefaultBoolean.Default);
         this.hideContainerBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
         this.hideContainerBottom.Controls.Add(this._panelHistoryBrowser);
         this.hideContainerBottom.Controls.Add(this._panelComparison);
         this.hideContainerBottom.Controls.Add(this._panelJournalDiagram);
         this.hideContainerBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
         this.hideContainerBottom.Location = new System.Drawing.Point(0, 790);
         this.hideContainerBottom.Name = "hideContainerBottom";
         this.hideContainerBottom.Size = new System.Drawing.Size(1225, 23);
         // 
         // _panelHistoryBrowser
         // 
         this._panelHistoryBrowser.Controls.Add(this.controlContainer3);
         this._panelHistoryBrowser.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         this._panelHistoryBrowser.ID = new System.Guid("4f3cab55-4024-40e4-8959-2f5f2d98753b");
         this._panelHistoryBrowser.Location = new System.Drawing.Point(0, 0);
         this._panelHistoryBrowser.Name = "_panelHistoryBrowser";
         this._panelHistoryBrowser.OriginalSize = new System.Drawing.Size(200, 200);
         this._panelHistoryBrowser.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         this._panelHistoryBrowser.SavedIndex = 1;
         this._panelHistoryBrowser.Size = new System.Drawing.Size(1248, 200);
         this._panelHistoryBrowser.Text = "_panelHistoryBrowser";
         this._panelHistoryBrowser.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
         // 
         // controlContainer3
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.controlContainer3, DevExpress.Utils.DefaultBoolean.Default);
         this.controlContainer3.Location = new System.Drawing.Point(3, 25);
         this.controlContainer3.Name = "controlContainer3";
         this.controlContainer3.Size = new System.Drawing.Size(1242, 172);
         this.controlContainer3.TabIndex = 0;
         // 
         // _panelComparison
         // 
         this._panelComparison.Controls.Add(this.controlContainer2);
         this._panelComparison.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         this._panelComparison.ID = new System.Guid("33fa0872-065e-448e-a963-b30fb41f38df");
         this._panelComparison.Location = new System.Drawing.Point(0, 0);
         this._panelComparison.Name = "_panelComparison";
         this._panelComparison.OriginalSize = new System.Drawing.Size(200, 200);
         this._panelComparison.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         this._panelComparison.SavedIndex = 1;
         this._panelComparison.Size = new System.Drawing.Size(1248, 200);
         this._panelComparison.Text = "_panelComparison";
         this._panelComparison.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
         // 
         // controlContainer2
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.controlContainer2, DevExpress.Utils.DefaultBoolean.Default);
         this.controlContainer2.Location = new System.Drawing.Point(3, 25);
         this.controlContainer2.Name = "controlContainer2";
         this.controlContainer2.Size = new System.Drawing.Size(1242, 172);
         this.controlContainer2.TabIndex = 0;
         // 
         // _panelJournalDiagram
         // 
         this._panelJournalDiagram.Controls.Add(this.controlContainer4);
         this._panelJournalDiagram.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         this._panelJournalDiagram.ID = new System.Guid("94374f65-75de-476c-a1c0-2522e53b5eb5");
         this._panelJournalDiagram.Location = new System.Drawing.Point(0, 0);
         this._panelJournalDiagram.Name = "_panelJournalDiagram";
         this._panelJournalDiagram.OriginalSize = new System.Drawing.Size(200, 200);
         this._panelJournalDiagram.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
         this._panelJournalDiagram.SavedIndex = 2;
         this._panelJournalDiagram.Size = new System.Drawing.Size(1248, 200);
         this._panelJournalDiagram.Text = "_panelJournalDiagram";
         this._panelJournalDiagram.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
         // 
         // controlContainer4
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.controlContainer4, DevExpress.Utils.DefaultBoolean.Default);
         this.controlContainer4.Location = new System.Drawing.Point(3, 25);
         this.controlContainer4.Name = "controlContainer4";
         this.controlContainer4.Size = new System.Drawing.Size(1242, 172);
         this.controlContainer4.TabIndex = 0;
         // 
         // panelContainer1
         // 
         this.panelContainer1.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
         this.panelContainer1.Appearance.Options.UseBackColor = true;
         this.panelContainer1.Controls.Add(this._panelBuildingBlockExplorer);
         this.panelContainer1.Controls.Add(this._panelSimulationExplorer);
         this.panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
         this.panelContainer1.ID = new System.Guid("3c180d87-1e88-49c8-853c-6d590a53125a");
         this.panelContainer1.Location = new System.Drawing.Point(0, 50);
         this.panelContainer1.Name = "panelContainer1";
         this.panelContainer1.OriginalSize = new System.Drawing.Size(253, 200);
         this.panelContainer1.Size = new System.Drawing.Size(253, 740);
         this.panelContainer1.Text = "panelContainer1";
         // 
         // _panelBuildingBlockExplorer
         // 
         this._panelBuildingBlockExplorer.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
         this._panelBuildingBlockExplorer.Appearance.Options.UseBackColor = true;
         this._panelBuildingBlockExplorer.Controls.Add(this.panelComparisonContainer);
         this._panelBuildingBlockExplorer.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
         this._panelBuildingBlockExplorer.ID = new System.Guid("6a34348e-0dfb-4492-b0d7-2586550fff16");
         this._panelBuildingBlockExplorer.Location = new System.Drawing.Point(0, 0);
         this._panelBuildingBlockExplorer.Name = "_panelBuildingBlockExplorer";
         this._panelBuildingBlockExplorer.OriginalSize = new System.Drawing.Size(200, 372);
         this._panelBuildingBlockExplorer.Size = new System.Drawing.Size(253, 370);
         this._panelBuildingBlockExplorer.Text = "_panelBuildingBlockExplorer";
         // 
         // panelComparisonContainer
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.panelComparisonContainer, DevExpress.Utils.DefaultBoolean.Default);
         this.panelComparisonContainer.Location = new System.Drawing.Point(3, 25);
         this.panelComparisonContainer.Name = "panelComparisonContainer";
         this.panelComparisonContainer.Size = new System.Drawing.Size(246, 341);
         this.panelComparisonContainer.TabIndex = 0;
         // 
         // _panelSimulationExplorer
         // 
         this._panelSimulationExplorer.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
         this._panelSimulationExplorer.Appearance.Options.UseBackColor = true;
         this._panelSimulationExplorer.Controls.Add(this.controlContainer1);
         this._panelSimulationExplorer.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
         this._panelSimulationExplorer.FloatVertical = true;
         this._panelSimulationExplorer.ID = new System.Guid("b749a8c9-e77a-4d89-a4fa-36b9ab7ca5a7");
         this._panelSimulationExplorer.Location = new System.Drawing.Point(0, 370);
         this._panelSimulationExplorer.Name = "_panelSimulationExplorer";
         this._panelSimulationExplorer.OriginalSize = new System.Drawing.Size(200, 372);
         this._panelSimulationExplorer.Size = new System.Drawing.Size(253, 370);
         this._panelSimulationExplorer.Text = "_panelSimulationExplorer";
         // 
         // controlContainer1
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.controlContainer1, DevExpress.Utils.DefaultBoolean.Default);
         this.controlContainer1.Location = new System.Drawing.Point(3, 25);
         this.controlContainer1.Name = "controlContainer1";
         this.controlContainer1.Size = new System.Drawing.Size(246, 342);
         this.controlContainer1.TabIndex = 0;
         // 
         // _panelJournal
         // 
         this._panelJournal.Controls.Add(this.dockPanel1_Container);
         this._panelJournal.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
         this._panelJournal.ID = new System.Guid("829c94ab-e017-4200-8461-19a49154e099");
         this._panelJournal.Location = new System.Drawing.Point(0, 0);
         this._panelJournal.Name = "_panelJournal";
         this._panelJournal.OriginalSize = new System.Drawing.Size(200, 200);
         this._panelJournal.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
         this._panelJournal.SavedIndex = 1;
         this._panelJournal.Size = new System.Drawing.Size(200, 740);
         this._panelJournal.Text = "_panelWorkingJournal";
         this._panelJournal.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
         // 
         // dockPanel1_Container
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.dockPanel1_Container, DevExpress.Utils.DefaultBoolean.Default);
         this.dockPanel1_Container.Location = new System.Drawing.Point(4, 25);
         this.dockPanel1_Container.Name = "dockPanel1_Container";
         this.dockPanel1_Container.Size = new System.Drawing.Size(193, 712);
         this.dockPanel1_Container.TabIndex = 0;
         // 
         // defaultBarAndDockingController
         // 
         this.defaultBarAndDockingController.Controller.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
         this.defaultBarAndDockingController.Controller.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
         // 
         // hideContainerRight
         // 
         this.defaultToolTipController.SetAllowHtmlText(this.hideContainerRight, DevExpress.Utils.DefaultBoolean.Default);
         this.hideContainerRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(245)))), ((int)(((byte)(241)))));
         this.hideContainerRight.Controls.Add(this._panelJournal);
         this.hideContainerRight.Dock = System.Windows.Forms.DockStyle.Right;
         this.hideContainerRight.Location = new System.Drawing.Point(1225, 50);
         this.hideContainerRight.Name = "hideContainerRight";
         this.hideContainerRight.Size = new System.Drawing.Size(23, 763);
         // 
         // Shell
         // 
         this.defaultToolTipController.SetAllowHtmlText(this, DevExpress.Utils.DefaultBoolean.Default);
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "RibbonShell";
         this.ClientSize = new System.Drawing.Size(1248, 838);
         this.Controls.Add(this.rightPaneAppMenu);
         this.Controls.Add(this.panelContainer1);
         this.Controls.Add(this.hideContainerBottom);
         this.Controls.Add(this.hideContainerRight);
         this.Controls.Add(this.ribbonStatusBar);
         this.Controls.Add(this.ribbon);
         this.Name = "Shell";
         this.Ribbon = this.ribbon;
         this.StatusBar = this.ribbonStatusBar;
         this.Text = "RibbonShell";
         this.Controls.SetChildIndex(this.ribbon, 0);
         this.Controls.SetChildIndex(this.ribbonStatusBar, 0);
         this.Controls.SetChildIndex(this.hideContainerRight, 0);
         this.Controls.SetChildIndex(this.hideContainerBottom, 0);
         this.Controls.SetChildIndex(this.panelContainer1, 0);
         this.Controls.SetChildIndex(this.rightPaneAppMenu, 0);
         ((System.ComponentModel.ISupportInitialize)(this.xtraTabbedMdiManager)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.applicationMenu)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.rightPaneAppMenu)).EndInit();
         this.rightPaneAppMenu.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelControlAppMenuFileLabels)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dockManager)).EndInit();
         this.hideContainerBottom.ResumeLayout(false);
         this._panelHistoryBrowser.ResumeLayout(false);
         this._panelComparison.ResumeLayout(false);
         this._panelJournalDiagram.ResumeLayout(false);
         this.panelContainer1.ResumeLayout(false);
         this._panelBuildingBlockExplorer.ResumeLayout(false);
         this._panelSimulationExplorer.ResumeLayout(false);
         this._panelJournal.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.defaultBarAndDockingController.Controller)).EndInit();
         this.hideContainerRight.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
      private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
      private DevExpress.XtraBars.Docking.DockManager dockManager;
      private DevExpress.XtraBars.Docking.ControlContainer panelComparisonContainer;
      private DevExpress.XtraBars.Docking.ControlContainer controlContainer1;
      private DevExpress.XtraBars.Docking.ControlContainer controlContainer3;
      private OSPSuite.UI.Controls.UxDockPanel _panelBuildingBlockExplorer;
      private OSPSuite.UI.Controls.UxDockPanel _panelSimulationExplorer;
      private OSPSuite.UI.Controls.UxDockPanel _panelHistoryBrowser;
      private OSPSuite.UI.Controls.UxDockPanel _panelComparison;
      private DevExpress.XtraBars.Ribbon.ApplicationMenu applicationMenu;
      private DevExpress.XtraBars.PopupControlContainer rightPaneAppMenu;
      private DevExpress.XtraEditors.PanelControl panelControlAppMenuFileLabels;
      private DevExpress.XtraEditors.LabelControl labelControl1;
      private DevExpress.XtraEditors.PanelControl panelControl1;
      private DevExpress.XtraBars.DefaultBarAndDockingController defaultBarAndDockingController;
      private DevExpress.XtraBars.Docking.DockPanel panelContainer1;
      private DevExpress.XtraBars.Docking.AutoHideContainer hideContainerBottom;
      private DevExpress.XtraBars.Docking.ControlContainer controlContainer2;
      private OSPSuite.UI.Controls.UxDockPanel _panelJournal;
      private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
      private OSPSuite.UI.Controls.UxDockPanel _panelJournalDiagram;
      private DevExpress.XtraBars.Docking.ControlContainer controlContainer4;
      private DevExpress.XtraBars.Docking.AutoHideContainer hideContainerRight;
   }
}