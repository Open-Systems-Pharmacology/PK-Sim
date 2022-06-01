using OSPSuite.UI.Controls;

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
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.pivotGrid = new PKAnalysisPivotGridControl();
         this.layoutItemPivotGrid = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivotGrid)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.pivotGrid);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(667, 415);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPivotGrid});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(667, 415);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // pivotGrid
         // 
         this.pivotGrid.ExceptionManager = null;
         this.pivotGrid.Location = new System.Drawing.Point(98, 0);
         this.pivotGrid.Name = "pivotGrid";
         this.pivotGrid.Size = new System.Drawing.Size(569, 415);
         this.pivotGrid.TabIndex = 4;
         // 
         // layoutItemPivotGrid
         // 
         this.layoutItemPivotGrid.Control = this.pivotGrid;
         this.layoutItemPivotGrid.CustomizationFormText = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPivotGrid.Name = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemPivotGrid.Size = new System.Drawing.Size(667, 415);
         this.layoutItemPivotGrid.Text = "layoutItemPivotGrid";
         this.layoutItemPivotGrid.TextSize = new System.Drawing.Size(95, 13);
         // 
         // GlobalPKAnalysisView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "GlobalPKAnalysisView";
         this.Size = new System.Drawing.Size(667, 415);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivotGrid)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private PKAnalysisPivotGridControl pivotGrid;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPivotGrid;
   }
}
