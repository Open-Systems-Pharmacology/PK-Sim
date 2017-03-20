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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.panelHalfLifeIntestine = new DevExpress.XtraEditors.PanelControl();
         this.panelOntogeny = new DevExpress.XtraEditors.PanelControl();
         this.panelHalfLifeLiver = new DevExpress.XtraEditors.PanelControl();
         this.panelReferenceConcentration = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemReferenceConcentration = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemHalfLifeLiver = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemHalfLifeIntestine = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeIntestine)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeLiver)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelReferenceConcentration)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemReferenceConcentration)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeLiver)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeIntestine)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelHalfLifeIntestine);
         this.layoutControl.Controls.Add(this.panelOntogeny);
         this.layoutControl.Controls.Add(this.panelHalfLifeLiver);
         this.layoutControl.Controls.Add(this.panelReferenceConcentration);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(348, 186, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(549, 98);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelHalfLifeIntestine
         // 
         this.panelHalfLifeIntestine.Location = new System.Drawing.Point(175, 50);
         this.panelHalfLifeIntestine.Name = "panelHalfLifeIntestine";
         this.panelHalfLifeIntestine.Size = new System.Drawing.Size(372, 20);
         this.panelHalfLifeIntestine.TabIndex = 7;
         // 
         // panelOntogeny
         // 
         this.panelOntogeny.Location = new System.Drawing.Point(173, 72);
         this.panelOntogeny.Name = "panelOntogeny";
         this.panelOntogeny.Size = new System.Drawing.Size(376, 26);
         this.panelOntogeny.TabIndex = 6;
         // 
         // panelHalfLifeLiver
         // 
         this.panelHalfLifeLiver.Location = new System.Drawing.Point(175, 26);
         this.panelHalfLifeLiver.Name = "panelHalfLifeLiver";
         this.panelHalfLifeLiver.Size = new System.Drawing.Size(372, 20);
         this.panelHalfLifeLiver.TabIndex = 5;
         // 
         // panelReferenceConcentration
         // 
         this.panelReferenceConcentration.Location = new System.Drawing.Point(175, 2);
         this.panelReferenceConcentration.Name = "panelReferenceConcentration";
         this.panelReferenceConcentration.Size = new System.Drawing.Size(372, 20);
         this.panelReferenceConcentration.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemReferenceConcentration,
            this.layoutItemHalfLifeLiver,
            this.layoutItemOntogeny,
            this.layoutItemHalfLifeIntestine});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(549, 98);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemReferenceConcentration
         // 
         this.layoutItemReferenceConcentration.Control = this.panelReferenceConcentration;
         this.layoutItemReferenceConcentration.CustomizationFormText = "layoutItemReferenceConcentration";
         this.layoutItemReferenceConcentration.Location = new System.Drawing.Point(0, 0);
         this.layoutItemReferenceConcentration.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemReferenceConcentration.MinSize = new System.Drawing.Size(278, 24);
         this.layoutItemReferenceConcentration.Name = "layoutItemReferenceConcentration";
         this.layoutItemReferenceConcentration.Size = new System.Drawing.Size(549, 24);
         this.layoutItemReferenceConcentration.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemReferenceConcentration.TextSize = new System.Drawing.Size(170, 13);
         // 
         // layoutItemHalfLifeLiver
         // 
         this.layoutItemHalfLifeLiver.Control = this.panelHalfLifeLiver;
         this.layoutItemHalfLifeLiver.CustomizationFormText = "layoutItemHalfTime";
         this.layoutItemHalfLifeLiver.Location = new System.Drawing.Point(0, 24);
         this.layoutItemHalfLifeLiver.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemHalfLifeLiver.MinSize = new System.Drawing.Size(278, 24);
         this.layoutItemHalfLifeLiver.Name = "layoutItemHalfLifeLiver";
         this.layoutItemHalfLifeLiver.Size = new System.Drawing.Size(549, 24);
         this.layoutItemHalfLifeLiver.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemHalfLifeLiver.TextSize = new System.Drawing.Size(170, 13);
         // 
         // layoutItemOntogeny
         // 
         this.layoutItemOntogeny.Control = this.panelOntogeny;
         this.layoutItemOntogeny.CustomizationFormText = "layoutItemOntogeny";
         this.layoutItemOntogeny.Location = new System.Drawing.Point(0, 72);
         this.layoutItemOntogeny.MaxSize = new System.Drawing.Size(0, 26);
         this.layoutItemOntogeny.MinSize = new System.Drawing.Size(278, 26);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.layoutItemOntogeny.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemOntogeny.Size = new System.Drawing.Size(549, 26);
         this.layoutItemOntogeny.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemOntogeny.TextSize = new System.Drawing.Size(170, 13);
         // 
         // layoutItemHalfLifeIntestine
         // 
         this.layoutItemHalfLifeIntestine.Control = this.panelHalfLifeIntestine;
         this.layoutItemHalfLifeIntestine.CustomizationFormText = "layoutItemHalfLifeIntestine";
         this.layoutItemHalfLifeIntestine.Location = new System.Drawing.Point(0, 48);
         this.layoutItemHalfLifeIntestine.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemHalfLifeIntestine.MinSize = new System.Drawing.Size(277, 24);
         this.layoutItemHalfLifeIntestine.Name = "layoutItemHalfLifeIntestine";
         this.layoutItemHalfLifeIntestine.Size = new System.Drawing.Size(549, 24);
         this.layoutItemHalfLifeIntestine.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemHalfLifeIntestine.TextSize = new System.Drawing.Size(170, 13);
         // 
         // IndividualMoleculePropertiesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualMoleculePropertiesView";
         this.Size = new System.Drawing.Size(549, 98);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeIntestine)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeLiver)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelReferenceConcentration)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemReferenceConcentration)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeLiver)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeIntestine)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.PanelControl panelHalfLifeLiver;
      private DevExpress.XtraEditors.PanelControl panelReferenceConcentration;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemReferenceConcentration;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemHalfLifeLiver;
      private DevExpress.XtraEditors.PanelControl panelOntogeny;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOntogeny;
      private DevExpress.XtraEditors.PanelControl panelHalfLifeIntestine;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemHalfLifeIntestine;
   }
}
