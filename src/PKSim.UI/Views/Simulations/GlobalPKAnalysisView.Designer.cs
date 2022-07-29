using OSPSuite.UI.Controls;
using PKSim.Assets;

namespace PKSim.UI.Views.Simulations
{
   partial class GlobalPKAnalysisView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblWarning = new DevExpress.XtraEditors.LabelControl();
         this.pivotGrid = new OSPSuite.UI.Controls.PKAnalysisPivotGridControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemPivotGrid = new DevExpress.XtraLayout.LayoutControlItem();
         this.groupWarning = new DevExpress.XtraLayout.LayoutControlGroup();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutControlItemLabel = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivotGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.groupWarning)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemLabel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.lblWarning);
         this.layoutControl.Controls.Add(this.pivotGrid);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3407, 27, 812, 500);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(667, 415);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblWarning
         // 
         this.lblWarning.Location = new System.Drawing.Point(305, 308);
         this.lblWarning.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.lblWarning.Name = "lblWarning";
         this.lblWarning.Size = new System.Drawing.Size(57, 13);
         this.lblWarning.StyleController = this.layoutControl;
         this.lblWarning.TabIndex = 5;
         this.lblWarning.Text = "labelControl";
         // 
         // pivotGrid
         // 
         this.pivotGrid.ExceptionManager = null;
         this.pivotGrid.Location = new System.Drawing.Point(107, 0);
         this.pivotGrid.Name = "pivotGrid";
         this.pivotGrid.Size = new System.Drawing.Size(560, 217);
         this.pivotGrid.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPivotGrid,
            this.groupWarning});
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(667, 415);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemPivotGrid
         // 
         this.layoutItemPivotGrid.Control = this.pivotGrid;
         this.layoutItemPivotGrid.CustomizationFormText = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPivotGrid.Name = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemPivotGrid.Size = new System.Drawing.Size(667, 217);
         this.layoutItemPivotGrid.TextSize = new System.Drawing.Size(95, 13);
         // 
         // groupWarning
         // 
         this.groupWarning.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem2,
            this.layoutControlItemLabel,
            this.emptySpaceItem1});
         this.groupWarning.Location = new System.Drawing.Point(0, 217);
         this.groupWarning.Name = "groupWarning";
         this.groupWarning.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.groupWarning.Size = new System.Drawing.Size(667, 198);
         this.groupWarning.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.groupWarning.TextVisible = false;
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(665, 88);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutControlItemLabel
         // 
         this.layoutControlItemLabel.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
         this.layoutControlItemLabel.Control = this.lblWarning;
         this.layoutControlItemLabel.Location = new System.Drawing.Point(0, 88);
         this.layoutControlItemLabel.Name = "layoutControlItem1";
         this.layoutControlItemLabel.Size = new System.Drawing.Size(665, 17);
         this.layoutControlItemLabel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemLabel.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 105);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(665, 91);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // GlobalPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "GlobalPKAnalysisView";
         this.Size = new System.Drawing.Size(667, 415);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivotGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.groupWarning)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemLabel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private PKAnalysisPivotGridControl pivotGrid;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPivotGrid;
      private DevExpress.XtraEditors.LabelControl lblWarning;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemLabel;
      private DevExpress.XtraLayout.LayoutControlGroup groupWarning;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}
