namespace PKSim.BatchTool.Views
{
   partial class GenerateProjectOverviewView
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
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.btnInputFolder = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutItemOutputFolder = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelLog = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnGenerate = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnInputFolder.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutputFolder)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.btnGenerate);
         this.layoutControl1.Controls.Add(this.panelLog);
         this.layoutControl1.Controls.Add(this.btnInputFolder);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1009, 304, 450, 400);
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(683, 552);
         this.layoutControl1.TabIndex = 0;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemOutputFolder,
            this.layoutControlItem1,
            this.layoutControlItem2});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Size = new System.Drawing.Size(683, 552);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // buttonEdit1
         // 
         this.btnInputFolder.Location = new System.Drawing.Point(132, 12);
         this.btnInputFolder.Name = "btnInputFolder";
         this.btnInputFolder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnInputFolder.Size = new System.Drawing.Size(539, 20);
         this.btnInputFolder.StyleController = this.layoutControl1;
         this.btnInputFolder.TabIndex = 7;
         // 
         // layoutItemOutputFolder
         // 
         this.layoutItemOutputFolder.Control = this.btnInputFolder;
         this.layoutItemOutputFolder.CustomizationFormText = "layoutItemOutputFolder";
         this.layoutItemOutputFolder.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOutputFolder.Name = "layoutItemOutputFolder";
         this.layoutItemOutputFolder.Size = new System.Drawing.Size(663, 24);
         this.layoutItemOutputFolder.Text = "layoutItemOutputFolder";
         this.layoutItemOutputFolder.TextSize = new System.Drawing.Size(116, 13);
         // 
         // panelLog
         // 
         this.panelLog.Location = new System.Drawing.Point(12, 36);
         this.panelLog.Name = "panelLog";
         this.panelLog.Size = new System.Drawing.Size(659, 478);
         this.panelLog.TabIndex = 8;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.panelLog;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(663, 482);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // btnGo
         // 
         this.btnGenerate.Location = new System.Drawing.Point(12, 518);
         this.btnGenerate.Name = "btnGenerate";
         this.btnGenerate.Size = new System.Drawing.Size(659, 22);
         this.btnGenerate.StyleController = this.layoutControl1;
         this.btnGenerate.TabIndex = 0;
         this.btnGenerate.Text = "btnGo";
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.btnGenerate;
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 506);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(663, 26);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // GenerateProjectOverviewView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "GenerateProjectOverviewView";
         this.ClientSize = new System.Drawing.Size(683, 552);
         this.Controls.Add(this.layoutControl1);
         this.Name = "GenerateProjectOverviewView";
         this.Text = "GenerateProjectOverviewView";
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnInputFolder.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutputFolder)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.ButtonEdit btnInputFolder;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOutputFolder;
      private DevExpress.XtraEditors.PanelControl panelLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraEditors.SimpleButton btnGenerate;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
   }
}