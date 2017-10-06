namespace PKSim.BatchTool.Views
{
   partial class SnapshotRunView
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         _screenBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         DevExpress.XtraEditors.TileItemElement tileItemElement11 = new DevExpress.XtraEditors.TileItemElement();
         DevExpress.XtraEditors.TileItemElement tileItemElement12 = new DevExpress.XtraEditors.TileItemElement();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.stopButton = new DevExpress.XtraEditors.SimpleButton();
         this.startButton = new DevExpress.XtraEditors.SimpleButton();
         this.navigationFrame = new DevExpress.XtraBars.Navigation.NavigationFrame();
         this.singleFolderPage = new DevExpress.XtraBars.Navigation.NavigationPage();
         this.folderListPage = new DevExpress.XtraBars.Navigation.NavigationPage();
         this.tileBar = new DevExpress.XtraBars.Navigation.TileBar();
         this.tileBarGroup = new DevExpress.XtraBars.Navigation.TileBarGroup();
         this.tileSingleFolderSelection = new DevExpress.XtraBars.Navigation.TileBarItem();
         this.tileFolderListSelection = new DevExpress.XtraBars.Navigation.TileBarItem();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTileBar = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPage = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonStart = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonStop = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.panelLog = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemPanelLog = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.navigationFrame)).BeginInit();
         this.navigationFrame.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTileBar)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPage)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonStart)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonStop)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelLog)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelLog);
         this.layoutControl.Controls.Add(this.stopButton);
         this.layoutControl.Controls.Add(this.startButton);
         this.layoutControl.Controls.Add(this.navigationFrame);
         this.layoutControl.Controls.Add(this.tileBar);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(895, 498, 450, 400);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(1034, 747);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // stopButton
         // 
         this.stopButton.Location = new System.Drawing.Point(265, 713);
         this.stopButton.Name = "stopButton";
         this.stopButton.Size = new System.Drawing.Size(248, 22);
         this.stopButton.StyleController = this.layoutControl;
         this.stopButton.TabIndex = 7;
         this.stopButton.Text = "stopButton";
         // 
         // startButton
         // 
         this.startButton.Location = new System.Drawing.Point(517, 713);
         this.startButton.Name = "startButton";
         this.startButton.Size = new System.Drawing.Size(505, 22);
         this.startButton.StyleController = this.layoutControl;
         this.startButton.TabIndex = 6;
         this.startButton.Text = "startButton";
         // 
         // navigationFrame
         // 
         this.navigationFrame.Controls.Add(this.singleFolderPage);
         this.navigationFrame.Controls.Add(this.folderListPage);
         this.navigationFrame.Location = new System.Drawing.Point(110, 107);
         this.navigationFrame.Name = "navigationFrame";
         this.navigationFrame.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[] {
            this.singleFolderPage,
            this.folderListPage});
         this.navigationFrame.SelectedPage = this.singleFolderPage;
         this.navigationFrame.Size = new System.Drawing.Size(912, 245);
         this.navigationFrame.TabIndex = 5;
         this.navigationFrame.Text = "navigationFrame1";
         // 
         // singleFolderPage
         // 
         this.singleFolderPage.Name = "singleFolderPage";
         this.singleFolderPage.Size = new System.Drawing.Size(912, 245);
         // 
         // folderListPage
         // 
         this.folderListPage.Caption = "folderListPage";
         this.folderListPage.Name = "folderListPage";
         this.folderListPage.Size = new System.Drawing.Size(912, 245);
         // 
         // tileBar
         // 
         this.tileBar.AllowDrag = false;
         this.tileBar.Cursor = System.Windows.Forms.Cursors.Default;
         this.tileBar.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
         this.tileBar.Groups.Add(this.tileBarGroup);
         this.tileBar.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Far;
         this.tileBar.Location = new System.Drawing.Point(110, 12);
         this.tileBar.MaxId = 3;
         this.tileBar.Name = "tileBar";
         this.tileBar.Padding = new System.Windows.Forms.Padding(22, 2, 22, 2);
         this.tileBar.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
         this.tileBar.Size = new System.Drawing.Size(912, 91);
         this.tileBar.TabIndex = 4;
         this.tileBar.Text = "tileBar1";
         // 
         // tileBarGroup
         // 
         this.tileBarGroup.Items.Add(this.tileSingleFolderSelection);
         this.tileBarGroup.Items.Add(this.tileFolderListSelection);
         this.tileBarGroup.Name = "tileBarGroup";
         // 
         // tileSingleFolderSelection
         // 
         this.tileSingleFolderSelection.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
         tileItemElement11.Text = "tileFolderSelection";
         this.tileSingleFolderSelection.Elements.Add(tileItemElement11);
         this.tileSingleFolderSelection.Id = 0;
         this.tileSingleFolderSelection.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
         this.tileSingleFolderSelection.Name = "tileSingleFolderSelection";
         // 
         // tileFolderListSelection
         // 
         this.tileFolderListSelection.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
         tileItemElement12.Text = "tileFileSelection";
         this.tileFolderListSelection.Elements.Add(tileItemElement12);
         this.tileFolderListSelection.Id = 1;
         this.tileFolderListSelection.ItemSize = DevExpress.XtraBars.Navigation.TileBarItemSize.Wide;
         this.tileFolderListSelection.Name = "tileFolderListSelection";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemTileBar,
            this.layoutItemPage,
            this.layoutItemButtonStart,
            this.layoutItemButtonStop,
            this.emptySpaceItem1,
            this.layoutItemPanelLog});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Size = new System.Drawing.Size(1034, 747);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemTileBar
         // 
         this.layoutItemTileBar.Control = this.tileBar;
         this.layoutItemTileBar.Location = new System.Drawing.Point(0, 0);
         this.layoutItemTileBar.MaxSize = new System.Drawing.Size(0, 95);
         this.layoutItemTileBar.MinSize = new System.Drawing.Size(191, 95);
         this.layoutItemTileBar.Name = "layoutItemTileBar";
         this.layoutItemTileBar.Size = new System.Drawing.Size(1014, 95);
         this.layoutItemTileBar.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemTileBar.TextSize = new System.Drawing.Size(95, 13);
         // 
         // layoutItemPage
         // 
         this.layoutItemPage.Control = this.navigationFrame;
         this.layoutItemPage.Location = new System.Drawing.Point(0, 95);
         this.layoutItemPage.Name = "layoutItemPage";
         this.layoutItemPage.Size = new System.Drawing.Size(1014, 249);
         this.layoutItemPage.TextSize = new System.Drawing.Size(95, 13);
         // 
         // layoutItemButtonStart
         // 
         this.layoutItemButtonStart.Control = this.startButton;
         this.layoutItemButtonStart.Location = new System.Drawing.Point(505, 701);
         this.layoutItemButtonStart.Name = "layoutItemButtonStart";
         this.layoutItemButtonStart.Size = new System.Drawing.Size(509, 26);
         this.layoutItemButtonStart.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonStart.TextVisible = false;
         // 
         // layoutItemButtonStop
         // 
         this.layoutItemButtonStop.Control = this.stopButton;
         this.layoutItemButtonStop.Location = new System.Drawing.Point(253, 701);
         this.layoutItemButtonStop.Name = "layoutItemButtonStop";
         this.layoutItemButtonStop.Size = new System.Drawing.Size(252, 26);
         this.layoutItemButtonStop.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonStop.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 701);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(253, 26);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // panelLog
         // 
         this.panelLog.Location = new System.Drawing.Point(110, 356);
         this.panelLog.Name = "panelLog";
         this.panelLog.Size = new System.Drawing.Size(912, 353);
         this.panelLog.TabIndex = 8;
         // 
         // layoutItemPanelLog
         // 
         this.layoutItemPanelLog.Control = this.panelLog;
         this.layoutItemPanelLog.Location = new System.Drawing.Point(0, 344);
         this.layoutItemPanelLog.Name = "layoutItemPanelLog";
         this.layoutItemPanelLog.Size = new System.Drawing.Size(1014, 357);
         this.layoutItemPanelLog.TextSize = new System.Drawing.Size(95, 13);
         // 
         // SnapshotRunView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SnapshotRunView";
         this.ClientSize = new System.Drawing.Size(1034, 747);
         this.Controls.Add(this.layoutControl);
         this.Name = "SnapshotRunView";
         this.Text = "SnapshotRunView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.navigationFrame)).EndInit();
         this.navigationFrame.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTileBar)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPage)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonStart)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonStop)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelLog)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraBars.Navigation.TileBar tileBar;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTileBar;
      private DevExpress.XtraBars.Navigation.TileBarGroup tileBarGroup;
      private DevExpress.XtraBars.Navigation.TileBarItem tileSingleFolderSelection;
      private DevExpress.XtraBars.Navigation.TileBarItem tileFolderListSelection;
      private DevExpress.XtraBars.Navigation.NavigationFrame navigationFrame;
      private DevExpress.XtraBars.Navigation.NavigationPage singleFolderPage;
      private DevExpress.XtraBars.Navigation.NavigationPage folderListPage;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPage;
      private DevExpress.XtraEditors.SimpleButton stopButton;
      private DevExpress.XtraEditors.SimpleButton startButton;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonStart;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonStop;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraEditors.PanelControl panelLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanelLog;
   }
}