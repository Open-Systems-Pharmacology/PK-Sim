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
         this.panelOntogeny = new DevExpress.XtraEditors.PanelControl();
         this.panelMoleculeParameters = new DevExpress.XtraEditors.PanelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelOntogeny);
         this.layoutControl.Controls.Add(this.panelMoleculeParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(685, 150);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // panelOntogeny
         // 
         this.panelOntogeny.Location = new System.Drawing.Point(153, 117);
         this.panelOntogeny.Name = "panelOntogeny";
         this.panelOntogeny.Size = new System.Drawing.Size(530, 31);
         this.panelOntogeny.TabIndex = 5;
         // 
         // panelMoleculeParameters
         // 
         this.panelMoleculeParameters.Location = new System.Drawing.Point(153, 2);
         this.panelMoleculeParameters.Name = "panelMoleculeParameters";
         this.panelMoleculeParameters.Size = new System.Drawing.Size(530, 111);
         this.panelMoleculeParameters.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeParameters,
            this.layoutItemOntogeny});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(685, 150);
         this.Root.TextVisible = false;
         // 
         // layoutItemMoleculeParameters
         // 
         this.layoutItemMoleculeParameters.Control = this.panelMoleculeParameters;
         this.layoutItemMoleculeParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeParameters.Name = "layoutItemMoleculeParameters";
         this.layoutItemMoleculeParameters.Size = new System.Drawing.Size(685, 115);
         this.layoutItemMoleculeParameters.TextSize = new System.Drawing.Size(148, 13);
         // 
         // layoutItemOntogeny
         // 
         this.layoutItemOntogeny.Control = this.panelOntogeny;
         this.layoutItemOntogeny.Location = new System.Drawing.Point(0, 115);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.layoutItemOntogeny.Size = new System.Drawing.Size(685, 35);
         this.layoutItemOntogeny.TextSize = new System.Drawing.Size(148, 13);
         // 
         // IndividualMoleculePropertiesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualMoleculePropertiesView";
         this.Size = new System.Drawing.Size(685, 150);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelOntogeny;
      private DevExpress.XtraEditors.PanelControl panelMoleculeParameters;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOntogeny;
   }
}
