using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.AdvancedParameters
{
   internal partial class AdvancedParameterDistributionView
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
         _screenBinder.Dispose();
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.layoutControlTree = new OSPSuite.UI.Controls.UxLayoutControl();
         this.chkUseInReport = new OSPSuite.UI.Controls.UxCheckEdit();
         this.panelTreeGroup = new DevExpress.XtraEditors.PanelControl();
         this.cbScalingMode = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbBarType = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbGroupBy = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupSettings = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemGender = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemBarType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemScalingMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemUseInReport = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlTree)).BeginInit();
         this.layoutControlTree.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.chkUseInReport.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelTreeGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbScalingMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbBarType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGroupBy.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupSettings)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGender)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBarType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemScalingMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemUseInReport)).BeginInit();
         this.SuspendLayout();
         // 
         // splitContainer
         // 
         this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer.Location = new System.Drawing.Point(0, 0);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.Controls.Add(this.layoutControlTree);
         this.splitContainer.Panel1.Text = "Panel1";
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(771, 548);
         this.splitContainer.SplitterPosition = 167;
         this.splitContainer.TabIndex = 0;
         this.splitContainer.Text = "splitContainerControl1";
         // 
         // layoutControlTree
         // 
         this.layoutControlTree.AllowCustomization = false;
         this.layoutControlTree.Controls.Add(this.chkUseInReport);
         this.layoutControlTree.Controls.Add(this.panelTreeGroup);
         this.layoutControlTree.Controls.Add(this.cbScalingMode);
         this.layoutControlTree.Controls.Add(this.cbBarType);
         this.layoutControlTree.Controls.Add(this.cbGroupBy);
         this.layoutControlTree.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlTree.Location = new System.Drawing.Point(0, 0);
         this.layoutControlTree.Name = "layoutControlTree";
         this.layoutControlTree.Root = this.layoutControlGroup1;
         this.layoutControlTree.Size = new System.Drawing.Size(167, 548);
         this.layoutControlTree.TabIndex = 1;
         this.layoutControlTree.Text = "layoutControl1";
         // 
         // chkUseInReport
         // 
         this.chkUseInReport.Location = new System.Drawing.Point(2, 527);
         this.chkUseInReport.Name = "chkUseInReport";
         this.chkUseInReport.Properties.Caption = "chkUseForReport";
         this.chkUseInReport.Size = new System.Drawing.Size(163, 19);
         this.chkUseInReport.StyleController = this.layoutControlTree;
         this.chkUseInReport.TabIndex = 8;
         // 
         // panelTreeGroup
         // 
         this.panelTreeGroup.Location = new System.Drawing.Point(2, 2);
         this.panelTreeGroup.Name = "panelTreeGroup";
         this.panelTreeGroup.Size = new System.Drawing.Size(163, 449);
         this.panelTreeGroup.TabIndex = 7;
         // 
         // cbScalingMode
         // 
         this.cbScalingMode.Location = new System.Drawing.Point(2, 503);
         this.cbScalingMode.Name = "cbScalingMode";
         this.cbScalingMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbScalingMode.Size = new System.Drawing.Size(163, 20);
         this.cbScalingMode.StyleController = this.layoutControlTree;
         this.cbScalingMode.TabIndex = 6;
         // 
         // cbBarType
         // 
         this.cbBarType.Location = new System.Drawing.Point(2, 479);
         this.cbBarType.Name = "cbBarType";
         this.cbBarType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbBarType.Size = new System.Drawing.Size(163, 20);
         this.cbBarType.StyleController = this.layoutControlTree;
         this.cbBarType.TabIndex = 5;
         // 
         // cbGroupBy
         // 
         this.cbGroupBy.Location = new System.Drawing.Point(2, 455);
         this.cbGroupBy.Name = "cbGroupBy";
         this.cbGroupBy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGroupBy.Size = new System.Drawing.Size(163, 20);
         this.cbGroupBy.StyleController = this.layoutControlTree;
         this.cbGroupBy.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutGroupSettings});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(167, 548);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.panelTreeGroup;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(167, 453);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutGroupSettings
         // 
         this.layoutGroupSettings.CustomizationFormText = "layoutGroupSettings";
         this.layoutGroupSettings.GroupBordersVisible = false;
         this.layoutGroupSettings.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemGender,
            this.layoutItemBarType,
            this.layoutItemScalingMode,
            this.layoutItemUseInReport});
         this.layoutGroupSettings.Location = new System.Drawing.Point(0, 453);
         this.layoutGroupSettings.Name = "layoutGroupSettings";
         this.layoutGroupSettings.Size = new System.Drawing.Size(167, 95);
         this.layoutGroupSettings.Text = "layoutGroupSettings";
         // 
         // layoutItemGender
         // 
         this.layoutItemGender.Control = this.cbGroupBy;
         this.layoutItemGender.CustomizationFormText = "layoutItemGender";
         this.layoutItemGender.Location = new System.Drawing.Point(0, 0);
         this.layoutItemGender.Name = "layoutItemGender";
         this.layoutItemGender.Size = new System.Drawing.Size(167, 24);
         this.layoutItemGender.Text = "layoutItemGender";
         this.layoutItemGender.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGender.TextToControlDistance = 0;
         this.layoutItemGender.TextVisible = false;
         // 
         // layoutItemBarType
         // 
         this.layoutItemBarType.Control = this.cbBarType;
         this.layoutItemBarType.CustomizationFormText = "layoutItemBarType";
         this.layoutItemBarType.Location = new System.Drawing.Point(0, 24);
         this.layoutItemBarType.Name = "layoutItemBarType";
         this.layoutItemBarType.Size = new System.Drawing.Size(167, 24);
         this.layoutItemBarType.Text = "layoutItemBarType";
         this.layoutItemBarType.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemBarType.TextToControlDistance = 0;
         this.layoutItemBarType.TextVisible = false;
         // 
         // layoutItemScalingMode
         // 
         this.layoutItemScalingMode.Control = this.cbScalingMode;
         this.layoutItemScalingMode.CustomizationFormText = "layoutItemScalingMode";
         this.layoutItemScalingMode.Location = new System.Drawing.Point(0, 48);
         this.layoutItemScalingMode.Name = "layoutItemScalingMode";
         this.layoutItemScalingMode.Size = new System.Drawing.Size(167, 24);
         this.layoutItemScalingMode.Text = "layoutItemScalingMode";
         this.layoutItemScalingMode.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemScalingMode.TextToControlDistance = 0;
         this.layoutItemScalingMode.TextVisible = false;
         // 
         // layoutItemUseInReport
         // 
         this.layoutItemUseInReport.Control = this.chkUseInReport;
         this.layoutItemUseInReport.CustomizationFormText = "layoutItemUseInReport";
         this.layoutItemUseInReport.Location = new System.Drawing.Point(0, 72);
         this.layoutItemUseInReport.Name = "layoutItemUseInReport";
         this.layoutItemUseInReport.Size = new System.Drawing.Size(167, 23);
         this.layoutItemUseInReport.Text = "layoutItemUseInReport";
         this.layoutItemUseInReport.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemUseInReport.TextToControlDistance = 0;
         this.layoutItemUseInReport.TextVisible = false;
         // 
         // AdvancedParameterDistributionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainer);
         this.Name = "AdvancedParameterDistributionView";
         this.Size = new System.Drawing.Size(771, 548);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlTree)).EndInit();
         this.layoutControlTree.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.chkUseInReport.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelTreeGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbScalingMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbBarType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGroupBy.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupSettings)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGender)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBarType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemScalingMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemUseInReport)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbScalingMode;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbBarType;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbGroupBy;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGender;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemBarType;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemScalingMode;
      private DevExpress.XtraEditors.PanelControl panelTreeGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControlTree;
      private OSPSuite.UI.Controls.UxCheckEdit chkUseInReport;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemUseInReport;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupSettings;
   }
}
