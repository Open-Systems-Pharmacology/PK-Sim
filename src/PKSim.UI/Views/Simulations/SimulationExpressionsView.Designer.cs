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
         this.panelMoleculeParameters = new DevExpress.XtraEditors.PanelControl();
         this.gridParameters = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemRelativeExpressions = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemMoleculeParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeParameters = new DevExpress.XtraLayout.LayoutControlGroup();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.mainLayout)).BeginInit();
         this.mainLayout.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRelativeExpressions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // mainLayout
         // 
         this.mainLayout.AllowCustomization = false;
         this.mainLayout.Controls.Add(this.panelMoleculeParameters);
         this.mainLayout.Controls.Add(this.gridParameters);
         this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
         this.mainLayout.Location = new System.Drawing.Point(0, 0);
         this.mainLayout.Name = "mainLayout";
         this.mainLayout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(727, 141, 450, 400);
         this.mainLayout.Root = this.layoutControlGroup1;
         this.mainLayout.Size = new System.Drawing.Size(424, 420);
         this.mainLayout.TabIndex = 0;
         this.mainLayout.Text = "layoutControl1";
         // 
         // panelMoleculeParameters
         // 
         this.panelMoleculeParameters.Location = new System.Drawing.Point(14, 32);
         this.panelMoleculeParameters.Name = "panelMoleculeParameters";
         this.panelMoleculeParameters.Size = new System.Drawing.Size(396, 50);
         this.panelMoleculeParameters.TabIndex = 9;
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
         this.gridViewParameters.OptionsSelection.MultiSelect = true;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemRelativeExpressions,
            this.layoutGroupMoleculeParameters});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(424, 420);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemRelativeExpressions
         // 
         this.layoutItemRelativeExpressions.Control = this.gridParameters;
         this.layoutItemRelativeExpressions.CustomizationFormText = "layoutItemParameters";
         this.layoutItemRelativeExpressions.Location = new System.Drawing.Point(0, 96);
         this.layoutItemRelativeExpressions.Name = "layoutItemRelativeExpressions";
         this.layoutItemRelativeExpressions.Size = new System.Drawing.Size(424, 324);
         this.layoutItemRelativeExpressions.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemRelativeExpressions.TextVisible = false;
         // 
         // layoutItemMoleculeParameters
         // 
         this.layoutItemMoleculeParameters.Control = this.panelMoleculeParameters;
         this.layoutItemMoleculeParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeParameters.Name = "layoutItemMoleculeParameters";
         this.layoutItemMoleculeParameters.Size = new System.Drawing.Size(400, 54);
         this.layoutItemMoleculeParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemMoleculeParameters.TextVisible = false;
         // 
         // layoutGroupMoleculeParameters
         // 
         this.layoutGroupMoleculeParameters.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeParameters});
         this.layoutGroupMoleculeParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeParameters.Name = "layoutGroupMoleculeParameters";
         this.layoutGroupMoleculeParameters.Size = new System.Drawing.Size(424, 96);
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
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRelativeExpressions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxGridControl gridParameters;
      private PKSim.UI.Views.Core.UxGridView gridViewParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemRelativeExpressions;
      private OSPSuite.UI.Controls.UxLayoutControl mainLayout;
      private DevExpress.XtraEditors.PanelControl panelMoleculeParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeParameters;
   }
}
