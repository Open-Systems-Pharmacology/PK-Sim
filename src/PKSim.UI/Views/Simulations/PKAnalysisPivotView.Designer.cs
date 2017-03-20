using PKSim.UI.Views.Core;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   partial class PKAnalysisPivotView
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
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.pivotGrid = new PKAnalysisPivotGridControl();
         this.layoutItemPivot = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivot)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.pivotGrid);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(346, 254);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPivot});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(346, 254);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // pivotGrid
         // 
         this.pivotGrid.ExceptionManager = null;
         this.pivotGrid.Location = new System.Drawing.Point(0, 0);
         this.pivotGrid.Margin = new System.Windows.Forms.Padding(0);
         this.pivotGrid.Name = "pivotGrid";
         this.pivotGrid.Size = new System.Drawing.Size(346, 254);
         this.pivotGrid.TabIndex = 4;
         // 
         // layoutItemPivot
         // 
         this.layoutItemPivot.Control = this.pivotGrid;
         this.layoutItemPivot.CustomizationFormText = "layoutItemPivot";
         this.layoutItemPivot.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPivot.Name = "layoutItemPivot";
         this.layoutItemPivot.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemPivot.Size = new System.Drawing.Size(346, 254);
         this.layoutItemPivot.Text = "layoutItemPivot";
         this.layoutItemPivot.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPivot.TextToControlDistance = 0;
         this.layoutItemPivot.TextVisible = false;
         // 
         // PKAnalysisPivotView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "PKAnalysisPivotView";
         this.Size = new System.Drawing.Size(346, 254);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.pivotGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPivot)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private PKAnalysisPivotGridControl pivotGrid;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPivot;

   }
}
