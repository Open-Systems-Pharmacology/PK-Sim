namespace PKSim.BatchTool.Views
{
   partial class GenerateTrainingMaterialView
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
         this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
         this.btnOutputFolder = new DevExpress.XtraEditors.ButtonEdit();
         this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
         this.btnGenerate = new DevExpress.XtraEditors.SimpleButton();
         this.panelLog = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemLog = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonCancel = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemOutputFolder = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonGenerate = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.btnOutputFolder.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutputFolder)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonGenerate)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.btnOutputFolder);
         this.layoutControl1.Controls.Add(this.btnCancel);
         this.layoutControl1.Controls.Add(this.btnGenerate);
         this.layoutControl1.Controls.Add(this.panelLog);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(673, 577);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // btnOutputFolder
         // 
         this.btnOutputFolder.Location = new System.Drawing.Point(131, 12);
         this.btnOutputFolder.Name = "btnOutputFolder";
         this.btnOutputFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnOutputFolder.Size = new System.Drawing.Size(530, 20);
         this.btnOutputFolder.StyleController = this.layoutControl1;
         this.btnOutputFolder.TabIndex = 7;
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(338, 543);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(323, 22);
         this.btnCancel.StyleController = this.layoutControl1;
         this.btnCancel.TabIndex = 6;
         this.btnCancel.Text = "btnCancel";
         // 
         // btnGenerate
         // 
         this.btnGenerate.Location = new System.Drawing.Point(12, 543);
         this.btnGenerate.Name = "btnGenerate";
         this.btnGenerate.Size = new System.Drawing.Size(204, 22);
         this.btnGenerate.StyleController = this.layoutControl1;
         this.btnGenerate.TabIndex = 5;
         this.btnGenerate.Text = "btnGenerate";
         // 
         // panelLog
         // 
         this.panelLog.Location = new System.Drawing.Point(131, 36);
         this.panelLog.Name = "panelLog";
         this.panelLog.Size = new System.Drawing.Size(530, 503);
         this.panelLog.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemLog,
            this.layoutItemButtonCancel,
            this.layoutItemOutputFolder,
            this.emptySpaceItem2,
            this.layoutItemButtonGenerate});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Size = new System.Drawing.Size(673, 577);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemLog
         // 
         this.layoutItemLog.Control = this.panelLog;
         this.layoutItemLog.Location = new System.Drawing.Point(0, 24);
         this.layoutItemLog.Name = "layoutItemLog";
         this.layoutItemLog.Size = new System.Drawing.Size(653, 507);
         this.layoutItemLog.TextSize = new System.Drawing.Size(116, 13);
         // 
         // layoutItemButtonCancel
         // 
         this.layoutItemButtonCancel.Control = this.btnCancel;
         this.layoutItemButtonCancel.Location = new System.Drawing.Point(326, 531);
         this.layoutItemButtonCancel.Name = "layoutItemButtonCancel";
         this.layoutItemButtonCancel.Size = new System.Drawing.Size(327, 26);
         this.layoutItemButtonCancel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonCancel.TextVisible = false;
         // 
         // layoutItemOutputFolder
         // 
         this.layoutItemOutputFolder.Control = this.btnOutputFolder;
         this.layoutItemOutputFolder.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOutputFolder.Name = "layoutItemOutputFolder";
         this.layoutItemOutputFolder.Size = new System.Drawing.Size(653, 24);
         this.layoutItemOutputFolder.TextSize = new System.Drawing.Size(116, 13);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.Location = new System.Drawing.Point(208, 531);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(118, 26);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonGenerate
         // 
         this.layoutItemButtonGenerate.Control = this.btnGenerate;
         this.layoutItemButtonGenerate.Location = new System.Drawing.Point(0, 531);
         this.layoutItemButtonGenerate.Name = "layoutItemButtonGenerate";
         this.layoutItemButtonGenerate.Size = new System.Drawing.Size(208, 26);
         this.layoutItemButtonGenerate.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonGenerate.TextVisible = false;
         // 
         // GenerateTrainingMaterialView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "GenerateTrainingMaterialView";
         this.ClientSize = new System.Drawing.Size(673, 577);
         this.Controls.Add(this.layoutControl1);
         this.Name = "GenerateTrainingMaterialView";
         this.Text = "GenerateTrainingMaterialView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.btnOutputFolder.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutputFolder)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonGenerate)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.PanelControl panelLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLog;
      private DevExpress.XtraEditors.SimpleButton btnGenerate;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonGenerate;
      private DevExpress.XtraEditors.SimpleButton btnCancel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonCancel;
      private DevExpress.XtraEditors.ButtonEdit btnOutputFolder;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOutputFolder;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
   }
}