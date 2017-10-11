namespace PKSim.BatchTool.Views
{
   partial class BatchMainView
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
         this.btnSnapshotsRun = new DevExpress.XtraEditors.SimpleButton();
         this.btnStartBatchRun = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemStartBatchRun = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemStartSnapshotRun = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartBatchRun)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartSnapshotRun)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.btnSnapshotsRun);
         this.layoutControl1.Controls.Add(this.btnStartBatchRun);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(526, 428);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // btnSnapshotsRun
         // 
         this.btnSnapshotsRun.Location = new System.Drawing.Point(12, 38);
         this.btnSnapshotsRun.Name = "btnSnapshotsRun";
         this.btnSnapshotsRun.Size = new System.Drawing.Size(502, 22);
         this.btnSnapshotsRun.StyleController = this.layoutControl1;
         this.btnSnapshotsRun.TabIndex = 8;
         this.btnSnapshotsRun.Text = "btnSnapshotsRun";
         // 
         // btnStartBatchRun
         // 
         this.btnStartBatchRun.Location = new System.Drawing.Point(12, 12);
         this.btnStartBatchRun.Name = "btnStartBatchRun";
         this.btnStartBatchRun.Size = new System.Drawing.Size(502, 22);
         this.btnStartBatchRun.StyleController = this.layoutControl1;
         this.btnStartBatchRun.TabIndex = 4;
         this.btnStartBatchRun.Text = "btnStartBatchRun";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemStartBatchRun,
            this.layoutItemStartSnapshotRun});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(526, 428);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemStartBatchRun
         // 
         this.layoutItemStartBatchRun.Control = this.btnStartBatchRun;
         this.layoutItemStartBatchRun.Location = new System.Drawing.Point(0, 0);
         this.layoutItemStartBatchRun.Name = "layoutItemStartBatchRun";
         this.layoutItemStartBatchRun.Size = new System.Drawing.Size(506, 26);
         this.layoutItemStartBatchRun.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStartBatchRun.TextVisible = false;
         // 
         // layoutItemStartSnapshotRun
         // 
         this.layoutItemStartSnapshotRun.Control = this.btnSnapshotsRun;
         this.layoutItemStartSnapshotRun.Location = new System.Drawing.Point(0, 26);
         this.layoutItemStartSnapshotRun.Name = "layoutItemStartSnapshotRun";
         this.layoutItemStartSnapshotRun.Size = new System.Drawing.Size(506, 382);
         this.layoutItemStartSnapshotRun.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStartSnapshotRun.TextVisible = false;
         // 
         // BatchMainView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "BatchModeView";
         this.ClientSize = new System.Drawing.Size(526, 428);
         this.Controls.Add(this.layoutControl1);
         this.Name = "BatchMainView";
         this.Text = "BatchModeView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartBatchRun)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartSnapshotRun)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl1;
      private DevExpress.XtraEditors.SimpleButton btnStartBatchRun;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStartBatchRun;
      private DevExpress.XtraEditors.SimpleButton btnSnapshotsRun;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStartSnapshotRun;
   }
}