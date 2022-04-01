namespace PKSim.UI.Views.Snapshots
{
   partial class LoadFromSnapshotView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.buttonStart = new DevExpress.XtraEditors.SimpleButton();
         this.logPanel = new DevExpress.XtraEditors.PanelControl();
         this.buttonEditSelectSnapshot = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemButtonSelectSnapshot = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLogPanel = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemStartButton = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.chkRunSimulations = new DevExpress.XtraEditors.CheckEdit();
         this.layoutItemRunSimulations = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.logPanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonEditSelectSnapshot.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonSelectSnapshot)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLogPanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartButton)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkRunSimulations.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRunSimulations)).BeginInit();
         this.SuspendLayout();
         // 
         // tablePanel
         // 
         this.tablePanel.Location = new System.Drawing.Point(0, 581);
         this.tablePanel.Size = new System.Drawing.Size(769, 43);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.chkRunSimulations);
         this.layoutControl.Controls.Add(this.buttonStart);
         this.layoutControl.Controls.Add(this.logPanel);
         this.layoutControl.Controls.Add(this.buttonEditSelectSnapshot);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(769, 581);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // buttonStart
         // 
         this.buttonStart.Location = new System.Drawing.Point(386, 547);
         this.buttonStart.Name = "buttonStart";
         this.buttonStart.Size = new System.Drawing.Size(371, 22);
         this.buttonStart.StyleController = this.layoutControl;
         this.buttonStart.TabIndex = 6;
         this.buttonStart.Text = "buttonStart";
         // 
         // logPanel
         // 
         this.logPanel.Location = new System.Drawing.Point(12, 60);
         this.logPanel.Name = "logPanel";
         this.logPanel.Size = new System.Drawing.Size(745, 483);
         this.logPanel.TabIndex = 5;
         // 
         // buttonEditSelectSnapshot
         // 
         this.buttonEditSelectSnapshot.Location = new System.Drawing.Point(182, 12);
         this.buttonEditSelectSnapshot.Name = "buttonEditSelectSnapshot";
         this.buttonEditSelectSnapshot.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.buttonEditSelectSnapshot.Size = new System.Drawing.Size(575, 20);
         this.buttonEditSelectSnapshot.StyleController = this.layoutControl;
         this.buttonEditSelectSnapshot.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemButtonSelectSnapshot,
            this.layoutItemLogPanel,
            this.layoutItemStartButton,
            this.emptySpaceItem1,
            this.layoutItemRunSimulations});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(769, 581);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemButtonSelectSnapshot
         // 
         this.layoutItemButtonSelectSnapshot.Control = this.buttonEditSelectSnapshot;
         this.layoutItemButtonSelectSnapshot.Location = new System.Drawing.Point(0, 0);
         this.layoutItemButtonSelectSnapshot.Name = "layoutItemButtonSelectSnapshot";
         this.layoutItemButtonSelectSnapshot.Size = new System.Drawing.Size(749, 24);
         this.layoutItemButtonSelectSnapshot.TextSize = new System.Drawing.Size(158, 13);
         // 
         // layoutItemLogPanel
         // 
         this.layoutItemLogPanel.Control = this.logPanel;
         this.layoutItemLogPanel.Location = new System.Drawing.Point(0, 48);
         this.layoutItemLogPanel.Name = "layoutItemLogPanel";
         this.layoutItemLogPanel.Size = new System.Drawing.Size(749, 487);
         this.layoutItemLogPanel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLogPanel.TextVisible = false;
         // 
         // layoutItemStartButton
         // 
         this.layoutItemStartButton.Control = this.buttonStart;
         this.layoutItemStartButton.Location = new System.Drawing.Point(374, 535);
         this.layoutItemStartButton.Name = "layoutItemStartButton";
         this.layoutItemStartButton.Size = new System.Drawing.Size(375, 26);
         this.layoutItemStartButton.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStartButton.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 535);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(374, 26);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // chkRunSimulations
         // 
         this.chkRunSimulations.Location = new System.Drawing.Point(12, 36);
         this.chkRunSimulations.Name = "chkRunSimulations";
         this.chkRunSimulations.Properties.Caption = "chkRunSimulations";
         this.chkRunSimulations.Size = new System.Drawing.Size(745, 20);
         this.chkRunSimulations.StyleController = this.layoutControl;
         this.chkRunSimulations.TabIndex = 0;
         // 
         // layoutControlItem1
         // 
         this.layoutItemRunSimulations.Control = this.chkRunSimulations;
         this.layoutItemRunSimulations.Location = new System.Drawing.Point(0, 24);
         this.layoutItemRunSimulations.Name = "layoutItemRunSimulations";
         this.layoutItemRunSimulations.Size = new System.Drawing.Size(749, 24);
         this.layoutItemRunSimulations.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemRunSimulations.TextVisible = false;
         // 
         // LoadFromSnapshotView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "LoadFromSnapshotView";
         this.ClientSize = new System.Drawing.Size(769, 624);
         this.Controls.Add(this.layoutControl);
         this.Name = "LoadFromSnapshotView";
         this.Text = "LoadFromSnapshotView";
         this.Controls.SetChildIndex(this.tablePanel, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.logPanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonEditSelectSnapshot.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonSelectSnapshot)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLogPanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartButton)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkRunSimulations.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRunSimulations)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl logPanel;
      private DevExpress.XtraEditors.ButtonEdit buttonEditSelectSnapshot;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonSelectSnapshot;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLogPanel;
      private DevExpress.XtraEditors.SimpleButton buttonStart;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStartButton;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraEditors.CheckEdit chkRunSimulations;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemRunSimulations;
   }
}