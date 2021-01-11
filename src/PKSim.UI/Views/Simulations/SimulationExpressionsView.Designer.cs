namespace PKSim.UI.Views.Simulations
{
   partial class SimulationExpressionsView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.mainLayout = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelExpressionParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelMoleculeParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupMoleculeParameters = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemExpressionParameters = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainLayout)).BeginInit();
         this.mainLayout.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // mainLayout
         // 
         this.mainLayout.AllowCustomization = false;
         this.mainLayout.Controls.Add(this.panelExpressionParameters);
         this.mainLayout.Controls.Add(this.panelMoleculeParameters);
         this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
         this.mainLayout.Location = new System.Drawing.Point(0, 0);
         this.mainLayout.Name = "mainLayout";
         this.mainLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(727, 141, 450, 400);
         this.mainLayout.Root = this.layoutControlGroup1;
         this.mainLayout.Size = new System.Drawing.Size(424, 420);
         this.mainLayout.TabIndex = 0;
         this.mainLayout.Text = "layoutControl1";
         // 
         // panelExpressionParameters
         // 
         this.panelExpressionParameters.Location = new System.Drawing.Point(164, 98);
         this.panelExpressionParameters.Name = "panelExpressionParameters";
         this.panelExpressionParameters.Size = new System.Drawing.Size(258, 320);
         this.panelExpressionParameters.TabIndex = 10;
         // 
         // panelMoleculeParameters
         // 
         this.panelMoleculeParameters.Location = new System.Drawing.Point(176, 35);
         this.panelMoleculeParameters.Name = "panelMoleculeParameters";
         this.panelMoleculeParameters.Size = new System.Drawing.Size(234, 47);
         this.panelMoleculeParameters.TabIndex = 9;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupMoleculeParameters,
            this.layoutItemExpressionParameters});
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(424, 420);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutGroupMoleculeParameters
         // 
         this.layoutGroupMoleculeParameters.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeParameters});
         this.layoutGroupMoleculeParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeParameters.Name = "layoutGroupMoleculeParameters";
         this.layoutGroupMoleculeParameters.Size = new System.Drawing.Size(424, 96);
         // 
         // layoutItemMoleculeParameters
         // 
         this.layoutItemMoleculeParameters.Control = this.panelMoleculeParameters;
         this.layoutItemMoleculeParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeParameters.Name = "layoutItemMoleculeParameters";
         this.layoutItemMoleculeParameters.Size = new System.Drawing.Size(400, 51);
         this.layoutItemMoleculeParameters.TextSize = new System.Drawing.Size(159, 13);
         // 
         // layoutItemExpressionParameters
         // 
         this.layoutItemExpressionParameters.Control = this.panelExpressionParameters;
         this.layoutItemExpressionParameters.Location = new System.Drawing.Point(0, 96);
         this.layoutItemExpressionParameters.Name = "layoutItemExpressionParameters";
         this.layoutItemExpressionParameters.Size = new System.Drawing.Size(424, 324);
         this.layoutItemExpressionParameters.TextSize = new System.Drawing.Size(159, 13);
         // 
         // SimulationExpressionsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.mainLayout);
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "SimulationExpressionsView";
         this.Size = new System.Drawing.Size(424, 420);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainLayout)).EndInit();
         this.mainLayout.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxLayoutControl mainLayout;
      private DevExpress.XtraEditors.PanelControl panelMoleculeParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeParameters;
      private DevExpress.XtraEditors.PanelControl panelExpressionParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExpressionParameters;
   }
}
