namespace PKSim.UI.Views.Simulations
{
   partial class ImportSimulationPKAnalysesView
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
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.btnImport = new DevExpress.XtraEditors.SimpleButton();
         this.tbLog = new DevExpress.XtraEditors.MemoEdit();
         this.tbFileToImport = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSelectFileToImport = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLog = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonImport = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
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
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbFileToImport.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectFileToImport)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImport)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(431, 12);
         this.btnCancel.Size = new System.Drawing.Size(89, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(324, 12);
         this.btnOk.Size = new System.Drawing.Size(103, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 272);
         this.layoutControlBase.Size = new System.Drawing.Size(532, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(151, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(532, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(312, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(107, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(419, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(93, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(155, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(157, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(155, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.btnImport);
         this.layoutControl.Controls.Add(this.tbLog);
         this.layoutControl.Controls.Add(this.tbFileToImport);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(532, 272);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 36);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 7;
         this.lblDescription.Text = "lblDescription";
         // 
         // btnImport
         // 
         this.btnImport.Location = new System.Drawing.Point(267, 238);
         this.btnImport.Name = "btnImport";
         this.btnImport.Size = new System.Drawing.Size(253, 22);
         this.btnImport.StyleController = this.layoutControl;
         this.btnImport.TabIndex = 6;
         this.btnImport.Text = "btnImport";
         // 
         // tbLog
         // 
         this.tbLog.Location = new System.Drawing.Point(12, 53);
         this.tbLog.Name = "tbLog";
         this.tbLog.Size = new System.Drawing.Size(508, 181);
         this.tbLog.StyleController = this.layoutControl;
         this.tbLog.TabIndex = 5;
         // 
         // tbFileToImport
         // 
         this.tbFileToImport.Location = new System.Drawing.Point(156, 12);
         this.tbFileToImport.Name = "tbFileToImport";
         this.tbFileToImport.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.tbFileToImport.Size = new System.Drawing.Size(364, 20);
         this.tbFileToImport.StyleController = this.layoutControl;
         this.tbFileToImport.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSelectFileToImport,
            this.layoutItemLog,
            this.layoutItemButtonImport,
            this.emptySpaceItem1,
            this.layoutControlItem1});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(532, 272);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemSelectFileToImport
         // 
         this.layoutItemSelectFileToImport.Control = this.tbFileToImport;
         this.layoutItemSelectFileToImport.CustomizationFormText = "layoutItemSelectFileToImport";
         this.layoutItemSelectFileToImport.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSelectFileToImport.Name = "layoutItemSelectFileToImport";
         this.layoutItemSelectFileToImport.Size = new System.Drawing.Size(512, 24);
         this.layoutItemSelectFileToImport.Text = "layoutItemSelectFileToImport";
         this.layoutItemSelectFileToImport.TextSize = new System.Drawing.Size(141, 13);
         // 
         // layoutItemLog
         // 
         this.layoutItemLog.Control = this.tbLog;
         this.layoutItemLog.CustomizationFormText = "layoutItemLog";
         this.layoutItemLog.Location = new System.Drawing.Point(0, 41);
         this.layoutItemLog.Name = "layoutItemLog";
         this.layoutItemLog.Size = new System.Drawing.Size(512, 185);
         this.layoutItemLog.Text = "layoutItemLog";
         this.layoutItemLog.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLog.TextToControlDistance = 0;
         this.layoutItemLog.TextVisible = false;
         // 
         // layoutItemButtonImport
         // 
         this.layoutItemButtonImport.Control = this.btnImport;
         this.layoutItemButtonImport.CustomizationFormText = "layoutItemButtonImport";
         this.layoutItemButtonImport.Location = new System.Drawing.Point(255, 226);
         this.layoutItemButtonImport.Name = "layoutItemButtonImport";
         this.layoutItemButtonImport.Size = new System.Drawing.Size(257, 26);
         this.layoutItemButtonImport.Text = "layoutItemButtonImport";
         this.layoutItemButtonImport.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonImport.TextToControlDistance = 0;
         this.layoutItemButtonImport.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 226);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(255, 26);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.lblDescription;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 24);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(512, 17);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // ImportSimulationPKAnalysesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ImportSimulationPKAnalysesView";
         this.ClientSize = new System.Drawing.Size(532, 318);
         this.Controls.Add(this.layoutControl);
         this.Name = "ImportSimulationPKAnalysesView";
         this.Text = "ImportSimulationPKAnalysesView";
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
         ((System.ComponentModel.ISupportInitialize)(this.tbLog.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbFileToImport.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectFileToImport)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLog)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonImport)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.ButtonEdit tbFileToImport;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSelectFileToImport;
      private DevExpress.XtraEditors.MemoEdit tbLog;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLog;
      private DevExpress.XtraEditors.SimpleButton btnImport;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonImport;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}