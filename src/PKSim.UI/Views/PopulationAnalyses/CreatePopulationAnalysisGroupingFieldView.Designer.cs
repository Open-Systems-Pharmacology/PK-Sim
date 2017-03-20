namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class CreatePopulationAnalysisGroupingFieldView
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
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.cbGroupingDefinition = new DevExpress.XtraEditors.ComboBoxEdit();
         this.panelGroupingView = new DevExpress.XtraEditors.PanelControl();
         this.tbName = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPanel = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGroupingDefinition = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbGroupingDefinition.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelGroupingView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupingDefinition)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(463, 12);
         this.btnCancel.Size = new System.Drawing.Size(95, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(347, 12);
         this.btnOk.Size = new System.Drawing.Size(112, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 460);
         this.layoutControlBase.Size = new System.Drawing.Size(570, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(163, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(570, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(335, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(116, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(451, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(99, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(167, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(168, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(167, 26);
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.cbGroupingDefinition);
         this.layoutControl1.Controls.Add(this.panelGroupingView);
         this.layoutControl1.Controls.Add(this.tbName);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(570, 460);
         this.layoutControl1.TabIndex = 38;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // cbGroupingDefinition
         // 
         this.cbGroupingDefinition.Location = new System.Drawing.Point(155, 36);
         this.cbGroupingDefinition.Name = "cbGroupingDefinition";
         this.cbGroupingDefinition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGroupingDefinition.Size = new System.Drawing.Size(403, 20);
         this.cbGroupingDefinition.StyleController = this.layoutControl1;
         this.cbGroupingDefinition.TabIndex = 6;
         // 
         // panelGroupingView
         // 
         this.panelGroupingView.Location = new System.Drawing.Point(12, 60);
         this.panelGroupingView.Margin = new System.Windows.Forms.Padding(0);
         this.panelGroupingView.Name = "panelGroupingView";
         this.panelGroupingView.Size = new System.Drawing.Size(546, 388);
         this.panelGroupingView.TabIndex = 5;
         // 
         // tbName
         // 
         this.tbName.Location = new System.Drawing.Point(155, 12);
         this.tbName.Name = "tbName";
         this.tbName.Size = new System.Drawing.Size(403, 20);
         this.tbName.StyleController = this.layoutControl1;
         this.tbName.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemName,
            this.layoutItemPanel,
            this.layoutItemGroupingDefinition});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(570, 460);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemName
         // 
         this.layoutItemName.Control = this.tbName;
         this.layoutItemName.CustomizationFormText = "layoutItemName";
         this.layoutItemName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemName.Name = "layoutItemName";
         this.layoutItemName.Size = new System.Drawing.Size(550, 24);
         this.layoutItemName.Text = "layoutItemName";
         this.layoutItemName.TextSize = new System.Drawing.Size(140, 13);
         // 
         // layoutItemPanel
         // 
         this.layoutItemPanel.Control = this.panelGroupingView;
         this.layoutItemPanel.CustomizationFormText = "layoutItemPanel";
         this.layoutItemPanel.Location = new System.Drawing.Point(0, 48);
         this.layoutItemPanel.Name = "layoutItemPanel";
         this.layoutItemPanel.Size = new System.Drawing.Size(550, 392);
         this.layoutItemPanel.Text = "layoutItemPanel";
         this.layoutItemPanel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPanel.TextToControlDistance = 0;
         this.layoutItemPanel.TextVisible = false;
         // 
         // layoutItemGroupingDefinition
         // 
         this.layoutItemGroupingDefinition.Control = this.cbGroupingDefinition;
         this.layoutItemGroupingDefinition.CustomizationFormText = "layoutItemGroupingDefinition";
         this.layoutItemGroupingDefinition.Location = new System.Drawing.Point(0, 24);
         this.layoutItemGroupingDefinition.Name = "layoutItemGroupingDefinition";
         this.layoutItemGroupingDefinition.Size = new System.Drawing.Size(550, 24);
         this.layoutItemGroupingDefinition.Text = "layoutItemGroupingDefinition";
         this.layoutItemGroupingDefinition.TextSize = new System.Drawing.Size(140, 13);
         // 
         // CreatePopulationAnalysisGroupingFieldView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "PopulationAnalysisGroupingFieldView";
         this.ClientSize = new System.Drawing.Size(570, 506);
         this.Controls.Add(this.layoutControl1);
         this.Name = "CreatePopulationAnalysisGroupingFieldView";
         this.Text = "PopulationAnalysisGroupingFieldView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbGroupingDefinition.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelGroupingView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupingDefinition)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.ComboBoxEdit cbGroupingDefinition;
      private DevExpress.XtraEditors.PanelControl panelGroupingView;
      private DevExpress.XtraEditors.TextEdit tbName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGroupingDefinition;
   }
}