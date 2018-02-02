namespace PKSim.UI.Views.Individuals
{
   partial class IndividualMoleculePropertiesView
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
         this.panelMoleculeParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelOntogeny = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemMoleculeParameters = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelMoleculeParameters);
         this.layoutControl.Controls.Add(this.panelOntogeny);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(348, 186, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(685, 260);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelMoleculeParameters
         // 
         this.panelMoleculeParameters.Location = new System.Drawing.Point(153, 2);
         this.panelMoleculeParameters.Name = "panelMoleculeParameters";
         this.panelMoleculeParameters.Size = new System.Drawing.Size(530, 230);
         this.panelMoleculeParameters.TabIndex = 7;
         // 
         // panelOntogeny
         // 
         this.panelOntogeny.Location = new System.Drawing.Point(151, 234);
         this.panelOntogeny.Name = "panelOntogeny";
         this.panelOntogeny.Size = new System.Drawing.Size(534, 26);
         this.panelOntogeny.TabIndex = 6;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemOntogeny,
            this.layoutItemMoleculeParameters});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(685, 260);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemOntogeny
         // 
         this.layoutItemOntogeny.Control = this.panelOntogeny;
         this.layoutItemOntogeny.CustomizationFormText = "layoutItemOntogeny";
         this.layoutItemOntogeny.Location = new System.Drawing.Point(0, 234);
         this.layoutItemOntogeny.MaxSize = new System.Drawing.Size(0, 26);
         this.layoutItemOntogeny.MinSize = new System.Drawing.Size(278, 26);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.layoutItemOntogeny.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemOntogeny.Size = new System.Drawing.Size(685, 26);
         this.layoutItemOntogeny.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemOntogeny.TextSize = new System.Drawing.Size(148, 13);
         // 
         // layoutItemMoleculeParameters
         // 
         this.layoutItemMoleculeParameters.Control = this.panelMoleculeParameters;
         this.layoutItemMoleculeParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeParameters.Name = "layoutItemMoleculeParameters";
         this.layoutItemMoleculeParameters.Size = new System.Drawing.Size(685, 234);
         this.layoutItemMoleculeParameters.TextSize = new System.Drawing.Size(148, 13);
         // 
         // IndividualMoleculePropertiesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualMoleculePropertiesView";
         this.Size = new System.Drawing.Size(685, 260);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.PanelControl panelOntogeny;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOntogeny;
        private DevExpress.XtraEditors.PanelControl panelMoleculeParameters;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeParameters;
    }
}
