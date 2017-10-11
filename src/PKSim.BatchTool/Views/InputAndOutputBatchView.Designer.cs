namespace PKSim.BatchTool.Views
{
   partial class InputAndOutputBatchView<TStartOptions>
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
         this.panelLog = new DevExpress.XtraEditors.PanelControl();
         this.btnOutputFolder = new DevExpress.XtraEditors.ButtonEdit();
         this.btnCalculate = new DevExpress.XtraEditors.SimpleButton();
         this.btnInputFolder = new DevExpress.XtraEditors.ButtonEdit();
         this.btnClose = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnOutputFolder.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnInputFolder.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.panelLog);
         this.layoutControl1.Controls.Add(this.btnOutputFolder);
         this.layoutControl1.Controls.Add(this.btnCalculate);
         this.layoutControl1.Controls.Add(this.btnInputFolder);
         this.layoutControl1.Controls.Add(this.btnClose);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(808, 641);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // panelLog
         // 
         this.panelLog.Location = new System.Drawing.Point(12, 60);
         this.panelLog.Name = "panelLog";
         this.panelLog.Size = new System.Drawing.Size(784, 517);
         this.panelLog.TabIndex = 8;
         // 
         // btnOutputFolder
         // 
         this.btnOutputFolder.Location = new System.Drawing.Point(86, 36);
         this.btnOutputFolder.Name = "btnOutputFolder";
         this.btnOutputFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnOutputFolder.Size = new System.Drawing.Size(710, 20);
         this.btnOutputFolder.StyleController = this.layoutControl1;
         this.btnOutputFolder.TabIndex = 7;
         // 
         // btnCalculate
         // 
         this.btnCalculate.Location = new System.Drawing.Point(12, 581);
         this.btnCalculate.Name = "btnCalculate";
         this.btnCalculate.Size = new System.Drawing.Size(784, 22);
         this.btnCalculate.StyleController = this.layoutControl1;
         this.btnCalculate.TabIndex = 6;
         this.btnCalculate.Text = "Calculate...";
         // 
         // btnInputFolder
         // 
         this.btnInputFolder.Location = new System.Drawing.Point(86, 12);
         this.btnInputFolder.Name = "btnInputFolder";
         this.btnInputFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnInputFolder.Size = new System.Drawing.Size(710, 20);
         this.btnInputFolder.StyleController = this.layoutControl1;
         this.btnInputFolder.TabIndex = 5;
         // 
         // btnClose
         // 
         this.btnClose.Location = new System.Drawing.Point(659, 607);
         this.btnClose.Name = "btnClose";
         this.btnClose.Size = new System.Drawing.Size(137, 22);
         this.btnClose.StyleController = this.layoutControl1;
         this.btnClose.TabIndex = 4;
         this.btnClose.Text = "Close";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.layoutControlItem5});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(808, 641);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.btnClose;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(647, 595);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(141, 26);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 595);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(647, 26);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.btnInputFolder;
         this.layoutControlItem2.CustomizationFormText = "Output Folder:";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(788, 24);
         this.layoutControlItem2.Text = "Input Folder:";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(71, 13);
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.btnCalculate;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(0, 569);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(788, 26);
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextVisible = false;
         // 
         // layoutControlItem4
         // 
         this.layoutControlItem4.Control = this.btnOutputFolder;
         this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
         this.layoutControlItem4.Location = new System.Drawing.Point(0, 24);
         this.layoutControlItem4.Name = "layoutControlItem4";
         this.layoutControlItem4.Size = new System.Drawing.Size(788, 24);
         this.layoutControlItem4.Text = "Output Folder:";
         this.layoutControlItem4.TextSize = new System.Drawing.Size(71, 13);
         // 
         // layoutControlItem5
         // 
         this.layoutControlItem5.Control = this.panelLog;
         this.layoutControlItem5.Location = new System.Drawing.Point(0, 48);
         this.layoutControlItem5.Name = "layoutControlItem5";
         this.layoutControlItem5.Size = new System.Drawing.Size(788, 521);
         this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem5.TextVisible = false;
         // 
         // InputAndOutputBatchView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(808, 641);
         this.Controls.Add(this.layoutControl1);
         this.Name = "InputAndOutputBatchView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnOutputFolder.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnInputFolder.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SimpleButton btnCalculate;
      private DevExpress.XtraEditors.ButtonEdit btnInputFolder;
      private DevExpress.XtraEditors.SimpleButton btnClose;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
      private DevExpress.XtraEditors.ButtonEdit btnOutputFolder;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
      private DevExpress.XtraEditors.PanelControl panelLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
   }
}

