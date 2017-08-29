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
         this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
         this.logPanel = new DevExpress.XtraEditors.PanelControl();
         this.buttonEditSelectSnapshot = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemButtonSelectSnapshot = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLogPanel = new DevExpress.XtraLayout.LayoutControlItem();
         this.buttonStart = new DevExpress.XtraEditors.SimpleButton();
         this.layoutItemStartButton = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
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
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(626, 12);
         this.btnCancel.Size = new System.Drawing.Size(131, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(468, 12);
         this.btnOk.Size = new System.Drawing.Size(154, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 578);
         this.layoutControlBase.Size = new System.Drawing.Size(769, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(224, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(769, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(456, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(158, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(614, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(135, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(228, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(228, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(228, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.buttonStart);
         this.layoutControl.Controls.Add(this.logPanel);
         this.layoutControl.Controls.Add(this.buttonEditSelectSnapshot);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(769, 578);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // logPanel
         // 
         this.logPanel.Location = new System.Drawing.Point(12, 36);
         this.logPanel.Name = "logPanel";
         this.logPanel.Size = new System.Drawing.Size(745, 504);
         this.logPanel.TabIndex = 5;
         // 
         // buttonEditSelectSnapshot
         // 
         this.buttonEditSelectSnapshot.Location = new System.Drawing.Point(173, 12);
         this.buttonEditSelectSnapshot.Name = "buttonEditSelectSnapshot";
         this.buttonEditSelectSnapshot.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.buttonEditSelectSnapshot.Size = new System.Drawing.Size(584, 20);
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
            this.emptySpaceItem1});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(769, 578);
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
         // layoutControlItem1
         // 
         this.layoutItemLogPanel.Control = this.logPanel;
         this.layoutItemLogPanel.Location = new System.Drawing.Point(0, 24);
         this.layoutItemLogPanel.Name = "layoutItemLogPanel";
         this.layoutItemLogPanel.Size = new System.Drawing.Size(749, 508);
         this.layoutItemLogPanel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLogPanel.TextVisible = false;
         // 
         // buttonStart
         // 
         this.buttonStart.Location = new System.Drawing.Point(386, 544);
         this.buttonStart.Name = "buttonStart";
         this.buttonStart.Size = new System.Drawing.Size(371, 22);
         this.buttonStart.StyleController = this.layoutControl;
         this.buttonStart.TabIndex = 6;
         this.buttonStart.Text = "buttonStart";
         // 
         // layoutControlItem2
         // 
         this.layoutItemStartButton.Control = this.buttonStart;
         this.layoutItemStartButton.Location = new System.Drawing.Point(374, 532);
         this.layoutItemStartButton.Name = "layoutItemStartButton";
         this.layoutItemStartButton.Size = new System.Drawing.Size(375, 26);
         this.layoutItemStartButton.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStartButton.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 532);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(374, 26);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
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
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
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
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl logPanel;
      private DevExpress.XtraEditors.ButtonEdit buttonEditSelectSnapshot;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonSelectSnapshot;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLogPanel;
      private DevExpress.XtraEditors.SimpleButton buttonStart;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStartButton;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}