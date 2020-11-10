namespace PKSim.UI.Views.Individuals
{
   partial class IndividualProteinExpressionsView
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
         this.uxLayoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelExpressionParameters = new DevExpress.XtraEditors.PanelControl();
         this.chkShowInitialConcentration = new OSPSuite.UI.Controls.UxCheckEdit();
         this.panelMoleculeLocalization = new DevExpress.XtraEditors.PanelControl();
         this.panelMoleculeProperties = new DevExpress.XtraEditors.PanelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupMoleculeProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeProperties = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeLocalization = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemPanelLocalization = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemShowInitialConcentration = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPanelExpressionParameters = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.uxLayoutControl1)).BeginInit();
         this.uxLayoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowInitialConcentration.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemShowInitialConcentration)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelExpressionParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // uxLayoutControl1
         // 
         this.uxLayoutControl1.AllowCustomization = false;
         this.uxLayoutControl1.Controls.Add(this.panelExpressionParameters);
         this.uxLayoutControl1.Controls.Add(this.chkShowInitialConcentration);
         this.uxLayoutControl1.Controls.Add(this.panelMoleculeLocalization);
         this.uxLayoutControl1.Controls.Add(this.panelMoleculeProperties);
         this.uxLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.uxLayoutControl1.Location = new System.Drawing.Point(0, 0);
         this.uxLayoutControl1.Name = "uxLayoutControl1";
         this.uxLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(857, 251, 650, 400);
         this.uxLayoutControl1.Root = this.Root;
         this.uxLayoutControl1.Size = new System.Drawing.Size(810, 543);
         this.uxLayoutControl1.TabIndex = 0;
         this.uxLayoutControl1.Text = "uxLayoutControl1";
         // 
         // panelExpressionParameters
         // 
         this.panelExpressionParameters.Location = new System.Drawing.Point(200, 276);
         this.panelExpressionParameters.Name = "panelExpressionParameters";
         this.panelExpressionParameters.Size = new System.Drawing.Size(598, 255);
         this.panelExpressionParameters.TabIndex = 8;
         // 
         // chkShowInitialConcentration
         // 
         this.chkShowInitialConcentration.AllowClicksOutsideControlArea = false;
         this.chkShowInitialConcentration.Location = new System.Drawing.Point(12, 252);
         this.chkShowInitialConcentration.Name = "chkShowInitialConcentration";
         this.chkShowInitialConcentration.Properties.AllowFocused = false;
         this.chkShowInitialConcentration.Properties.Caption = "chkShowInitialConcentration";
         this.chkShowInitialConcentration.Size = new System.Drawing.Size(786, 20);
         this.chkShowInitialConcentration.StyleController = this.uxLayoutControl1;
         this.chkShowInitialConcentration.TabIndex = 7;
         // 
         // panelMoleculeLocalization
         // 
         this.panelMoleculeLocalization.Location = new System.Drawing.Point(212, 144);
         this.panelMoleculeLocalization.Name = "panelMoleculeLocalization";
         this.panelMoleculeLocalization.Size = new System.Drawing.Size(574, 92);
         this.panelMoleculeLocalization.TabIndex = 6;
         // 
         // panelMoleculeProperties
         // 
         this.panelMoleculeProperties.Location = new System.Drawing.Point(212, 45);
         this.panelMoleculeProperties.Name = "panelMoleculeProperties";
         this.panelMoleculeProperties.Size = new System.Drawing.Size(574, 50);
         this.panelMoleculeProperties.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupMoleculeProperties,
            this.layoutGroupMoleculeLocalization,
            this.layoutItemShowInitialConcentration,
            this.layoutItemPanelExpressionParameters});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(810, 543);
         this.Root.TextVisible = false;
         // 
         // layoutGroupMoleculeProperties
         // 
         this.layoutGroupMoleculeProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeProperties});
         this.layoutGroupMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeProperties.Name = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Size = new System.Drawing.Size(790, 99);
         // 
         // layoutItemMoleculeProperties
         // 
         this.layoutItemMoleculeProperties.Control = this.panelMoleculeProperties;
         this.layoutItemMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeProperties.Name = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.Size = new System.Drawing.Size(766, 54);
         this.layoutItemMoleculeProperties.TextSize = new System.Drawing.Size(185, 13);
         // 
         // layoutGroupMoleculeLocalization
         // 
         this.layoutGroupMoleculeLocalization.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPanelLocalization});
         this.layoutGroupMoleculeLocalization.Location = new System.Drawing.Point(0, 99);
         this.layoutGroupMoleculeLocalization.Name = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Size = new System.Drawing.Size(790, 141);
         // 
         // layoutItemPanelLocalization
         // 
         this.layoutItemPanelLocalization.Control = this.panelMoleculeLocalization;
         this.layoutItemPanelLocalization.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPanelLocalization.Name = "layoutItemPanelLocalization";
         this.layoutItemPanelLocalization.Size = new System.Drawing.Size(766, 96);
         this.layoutItemPanelLocalization.TextSize = new System.Drawing.Size(185, 13);
         // 
         // layoutItemShowInitialConcentration
         // 
         this.layoutItemShowInitialConcentration.Control = this.chkShowInitialConcentration;
         this.layoutItemShowInitialConcentration.Location = new System.Drawing.Point(0, 240);
         this.layoutItemShowInitialConcentration.Name = "layoutItemShowInitialConcentration";
         this.layoutItemShowInitialConcentration.Size = new System.Drawing.Size(790, 24);
         this.layoutItemShowInitialConcentration.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemShowInitialConcentration.TextVisible = false;
         // 
         // layoutItemPanelExpressionParameters
         // 
         this.layoutItemPanelExpressionParameters.Control = this.panelExpressionParameters;
         this.layoutItemPanelExpressionParameters.Location = new System.Drawing.Point(0, 264);
         this.layoutItemPanelExpressionParameters.Name = "layoutItemPanelExpressionParameters";
         this.layoutItemPanelExpressionParameters.Size = new System.Drawing.Size(790, 259);
         this.layoutItemPanelExpressionParameters.TextSize = new System.Drawing.Size(185, 13);
         // 
         // IndividualProteinExpressionsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.uxLayoutControl1);
         this.Name = "IndividualProteinExpressionsView";
         this.Size = new System.Drawing.Size(810, 543);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.uxLayoutControl1)).EndInit();
         this.uxLayoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowInitialConcentration.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemShowInitialConcentration)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelExpressionParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl uxLayoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraEditors.PanelControl panelMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeProperties;
      private DevExpress.XtraEditors.PanelControl panelMoleculeLocalization;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanelLocalization;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeLocalization;
      private OSPSuite.UI.Controls.UxCheckEdit chkShowInitialConcentration;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemShowInitialConcentration;
      private DevExpress.XtraEditors.PanelControl panelExpressionParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanelExpressionParameters;
   }
}
