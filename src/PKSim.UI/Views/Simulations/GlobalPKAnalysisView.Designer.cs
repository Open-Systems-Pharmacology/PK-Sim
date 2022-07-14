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
         this.labelControl = new DevExpress.XtraEditors.LabelControl();
         this.pivotGrid = new OSPSuite.UI.Controls.PKAnalysisPivotGridControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemPivotGrid = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItemLabel = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivotGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemLabel)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.labelControl);
         this.layoutControl.Controls.Add(this.pivotGrid);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3407, 27, 812, 500);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(778, 511);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // labelControl
         // 
         this.labelControl.Location = new System.Drawing.Point(2, 493);
         this.labelControl.Name = "labelControl";
         this.labelControl.Size = new System.Drawing.Size(319, 16);
         this.labelControl.StyleController = this.layoutControl;
         this.labelControl.TabIndex = 5;
         this.labelControl.Text = "labelControl";
         // 
         // pivotGrid
         // 
         this.pivotGrid.ExceptionManager = null;
         this.pivotGrid.Location = new System.Drawing.Point(122, 0);
         this.pivotGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.pivotGrid.Name = "pivotGrid";
         this.pivotGrid.OptionsDataField.RowHeaderWidth = 117;
         this.pivotGrid.OptionsView.RowTreeOffset = 24;
         this.pivotGrid.OptionsView.RowTreeWidth = 117;
         this.pivotGrid.Size = new System.Drawing.Size(656, 491);
         this.pivotGrid.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPivotGrid,
            this.layoutControlItemLabel});
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(778, 511);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemPivotGrid
         // 
         this.layoutItemPivotGrid.Control = this.pivotGrid;
         this.layoutItemPivotGrid.CustomizationFormText = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPivotGrid.Name = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemPivotGrid.Size = new System.Drawing.Size(778, 491);
         this.layoutItemPivotGrid.TextSize = new System.Drawing.Size(110, 16);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItemLabel.Control = this.labelControl;
         this.layoutControlItemLabel.Location = new System.Drawing.Point(0, 491);
         this.layoutControlItemLabel.Name = "layoutControlItem1";
         this.layoutControlItemLabel.Size = new System.Drawing.Size(778, 20);
         this.layoutControlItemLabel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemLabel.TextVisible = false;
         // 
         // GlobalPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
         this.Name = "GlobalPKAnalysisView";
         this.Size = new System.Drawing.Size(778, 511);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivotGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemLabel)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private PKAnalysisPivotGridControl pivotGrid;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPivotGrid;
      private DevExpress.XtraEditors.LabelControl labelControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemLabel;
   }
}
