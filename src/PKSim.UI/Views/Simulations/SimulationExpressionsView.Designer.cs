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
         _gridViewBinder.Dispose();
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
         this.panelHalfLifeIntestine = new DevExpress.XtraEditors.PanelControl();
         this.panelHalfLifeLiver = new DevExpress.XtraEditors.PanelControl();
         this.gridParameters = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.panelReferenceConcentration = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemReferenceConcentration = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemHalfLifeLiver = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemHalfLifeIntestine = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainLayout)).BeginInit();
         this.mainLayout.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeIntestine)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeLiver)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelReferenceConcentration)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemReferenceConcentration)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeLiver)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeIntestine)).BeginInit();
         this.SuspendLayout();
         // 
         // mainLayout
         // 
         this.mainLayout.AllowCustomization = false;
         this.mainLayout.Controls.Add(this.panelHalfLifeIntestine);
         this.mainLayout.Controls.Add(this.panelHalfLifeLiver);
         this.mainLayout.Controls.Add(this.gridParameters);
         this.mainLayout.Controls.Add(this.panelReferenceConcentration);
         this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
         this.mainLayout.Location = new System.Drawing.Point(0, 0);
         this.mainLayout.Name = "mainLayout";
         this.mainLayout.Root = this.layoutControlGroup1;
         this.mainLayout.Size = new System.Drawing.Size(424, 420);
         this.mainLayout.TabIndex = 0;
         this.mainLayout.Text = "layoutControl1";
         // 
         // panelHalfLifeIntestine
         // 
         this.panelHalfLifeIntestine.Location = new System.Drawing.Point(148, 62);
         this.panelHalfLifeIntestine.Name = "panelHalfLifeIntestine";
         this.panelHalfLifeIntestine.Size = new System.Drawing.Size(262, 20);
         this.panelHalfLifeIntestine.TabIndex = 8;
         // 
         // panelHalfLifeLiver
         // 
         this.panelHalfLifeLiver.Location = new System.Drawing.Point(148, 38);
         this.panelHalfLifeLiver.Name = "panelHalfLifeLiver";
         this.panelHalfLifeLiver.Size = new System.Drawing.Size(262, 20);
         this.panelHalfLifeLiver.TabIndex = 7;
         // 
         // gridParameters
         // 
         this.gridParameters.Location = new System.Drawing.Point(2, 98);
         this.gridParameters.MainView = this.gridViewParameters;
         this.gridParameters.Name = "gridParameters";
         this.gridParameters.Size = new System.Drawing.Size(420, 320);
         this.gridParameters.TabIndex = 6;
         this.gridParameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters});
         // 
         // gridViewParameters
         // 
         this.gridViewParameters.AllowsFiltering = true;
         this.gridViewParameters.EnableColumnContextMenu = true;
         this.gridViewParameters.GridControl = this.gridParameters;
         this.gridViewParameters.MultiSelect = true;
         this.gridViewParameters.Name = "gridViewParameters";
         this.gridViewParameters.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridViewParameters.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // panelReferenceConcentration
         // 
         this.panelReferenceConcentration.Location = new System.Drawing.Point(148, 14);
         this.panelReferenceConcentration.Name = "panelReferenceConcentration";
         this.panelReferenceConcentration.Size = new System.Drawing.Size(262, 20);
         this.panelReferenceConcentration.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutControlGroup2});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(424, 420);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.gridParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 96);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Size = new System.Drawing.Size(424, 324);
         this.layoutItemParameters.Text = "layoutItemParameters";
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         // 
         // layoutControlGroup2
         // 
         this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
         this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemReferenceConcentration,
            this.layoutItemHalfLifeLiver,
            this.layoutItemHalfLifeIntestine});
         this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup2.Name = "layoutControlGroup2";
         this.layoutControlGroup2.Size = new System.Drawing.Size(424, 96);
         this.layoutControlGroup2.Text = "layoutControlGroup2";
         this.layoutControlGroup2.TextVisible = false;
         // 
         // layoutItemReferenceConcentration
         // 
         this.layoutItemReferenceConcentration.Control = this.panelReferenceConcentration;
         this.layoutItemReferenceConcentration.CustomizationFormText = "layoutItemProteinContent";
         this.layoutItemReferenceConcentration.Location = new System.Drawing.Point(0, 0);
         this.layoutItemReferenceConcentration.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemReferenceConcentration.MinSize = new System.Drawing.Size(233, 24);
         this.layoutItemReferenceConcentration.Name = "layoutItemReferenceConcentration";
         this.layoutItemReferenceConcentration.Size = new System.Drawing.Size(400, 24);
         this.layoutItemReferenceConcentration.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemReferenceConcentration.Text = "layoutItemProteinContent";
         this.layoutItemReferenceConcentration.TextSize = new System.Drawing.Size(131, 13);
         // 
         // layoutItemHalfLifeLiver
         // 
         this.layoutItemHalfLifeLiver.Control = this.panelHalfLifeLiver;
         this.layoutItemHalfLifeLiver.CustomizationFormText = "layoutItemHalfLifeLiver";
         this.layoutItemHalfLifeLiver.Location = new System.Drawing.Point(0, 24);
         this.layoutItemHalfLifeLiver.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemHalfLifeLiver.MinSize = new System.Drawing.Size(232, 24);
         this.layoutItemHalfLifeLiver.Name = "layoutItemHalfLifeLiver";
         this.layoutItemHalfLifeLiver.Size = new System.Drawing.Size(400, 24);
         this.layoutItemHalfLifeLiver.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemHalfLifeLiver.Text = "layoutItemHalfLife";
         this.layoutItemHalfLifeLiver.TextSize = new System.Drawing.Size(131, 13);
         // 
         // layoutItemHalfLifeIntestine
         // 
         this.layoutItemHalfLifeIntestine.Control = this.panelHalfLifeIntestine;
         this.layoutItemHalfLifeIntestine.CustomizationFormText = "layoutItemHalfLifeIntestine";
         this.layoutItemHalfLifeIntestine.Location = new System.Drawing.Point(0, 48);
         this.layoutItemHalfLifeIntestine.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemHalfLifeIntestine.MinSize = new System.Drawing.Size(238, 24);
         this.layoutItemHalfLifeIntestine.Name = "layoutItemHalfLifeIntestine";
         this.layoutItemHalfLifeIntestine.Size = new System.Drawing.Size(400, 24);
         this.layoutItemHalfLifeIntestine.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemHalfLifeIntestine.Text = "layoutItemHalfLifeIntestine";
         this.layoutItemHalfLifeIntestine.TextSize = new System.Drawing.Size(131, 13);
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
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeIntestine)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelHalfLifeLiver)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelReferenceConcentration)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemReferenceConcentration)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeLiver)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHalfLifeIntestine)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxGridControl gridParameters;
      private PKSim.UI.Views.Core.UxGridView gridViewParameters;
      private DevExpress.XtraEditors.PanelControl panelReferenceConcentration;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemReferenceConcentration;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
      private OSPSuite.UI.Controls.UxLayoutControl mainLayout;
      private DevExpress.XtraEditors.PanelControl panelHalfLifeLiver;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemHalfLifeLiver;
      private DevExpress.XtraEditors.PanelControl panelHalfLifeIntestine;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemHalfLifeIntestine;
   }
}
