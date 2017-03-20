using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.ProteinExpression
{
   partial class ProteinExpressionsView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.xtraTabController = new DevExpress.XtraTab.XtraTabControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTabController = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.xtraTabController)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTabController)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.xtraTabController);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(634, 555);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // xtraTabController
         // 
         this.xtraTabController.Location = new System.Drawing.Point(12, 12);
         this.xtraTabController.Name = "xtraTabController";
         this.xtraTabController.Size = new System.Drawing.Size(610, 531);
         this.xtraTabController.TabIndex = 0;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemTabController});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(634, 555);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemTabController
         // 
         this.layoutItemTabController.Control = this.xtraTabController;
         this.layoutItemTabController.CustomizationFormText = "layoutItemTabController";
         this.layoutItemTabController.Location = new System.Drawing.Point(0, 0);
         this.layoutItemTabController.Name = "layoutItemTabController";
         this.layoutItemTabController.Size = new System.Drawing.Size(614, 535);
         this.layoutItemTabController.Text = "layoutItemTabController";
         this.layoutItemTabController.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemTabController.TextToControlDistance = 0;
         this.layoutItemTabController.TextVisible = false;
         // 
         // ProteinExpressionsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ProteinExpressionWizardView";
         this.ClientSize = new System.Drawing.Size(634, 601);
         this.Controls.Add(this.layoutControl);
         this.Name = "ProteinExpressionsView";
         this.Text = "ProteinExpressionWizardView";
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.xtraTabController)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTabController)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraTab.XtraTabControl xtraTabController;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTabController;
      private UxLayoutControl layoutControl;
   }
}