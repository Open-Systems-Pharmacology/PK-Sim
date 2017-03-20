using PKSim.UI.Views.Core;

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
         _gridViewBinder.Dispose();
         _presenter = null;
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControlMetabolism = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelMoleculeProperties = new DevExpress.XtraEditors.PanelControl();
         this.chkIntracellularVascularEndoLocation = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.cbLocationOnVascularEndo = new DevExpress.XtraEditors.ComboBoxEdit();
         this.cbLocalizationInTissue = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.gridControl = new PKSim.UI.Views.Core.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeProperties = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeLocalization = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemLocalizationInTissue = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemIntracellularVascularEndoLocation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLocalizationInVascularEndo = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlMetabolism)).BeginInit();
         this.layoutControlMetabolism.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkIntracellularVascularEndoLocation.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbLocationOnVascularEndo.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbLocalizationInTissue.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLocalizationInTissue)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIntracellularVascularEndoLocation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLocalizationInVascularEndo)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControlMetabolism
         // 
         this.layoutControlMetabolism.AllowCustomization = false;
         this.layoutControlMetabolism.Controls.Add(this.panelMoleculeProperties);
         this.layoutControlMetabolism.Controls.Add(this.chkIntracellularVascularEndoLocation);
         this.layoutControlMetabolism.Controls.Add(this.cbLocationOnVascularEndo);
         this.layoutControlMetabolism.Controls.Add(this.cbLocalizationInTissue);
         this.layoutControlMetabolism.Controls.Add(this.gridControl);
         this.layoutControlMetabolism.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlMetabolism.Location = new System.Drawing.Point(0, 0);
         this.layoutControlMetabolism.Name = "layoutControlMetabolism";
         this.layoutControlMetabolism.Root = this.layoutControlGroup1;
         this.layoutControlMetabolism.Size = new System.Drawing.Size(555, 515);
         this.layoutControlMetabolism.TabIndex = 0;
         this.layoutControlMetabolism.Text = "layoutControl1";
         // 
         // panelMoleculeProperties
         // 
         this.panelMoleculeProperties.Location = new System.Drawing.Point(240, 43);
         this.panelMoleculeProperties.Name = "panelMoleculeProperties";
         this.panelMoleculeProperties.Size = new System.Drawing.Size(291, 44);
         this.panelMoleculeProperties.TabIndex = 12;
         // 
         // chkIntracellularVascularEndoLocation
         // 
         this.chkIntracellularVascularEndoLocation.Location = new System.Drawing.Point(240, 158);
         this.chkIntracellularVascularEndoLocation.Name = "chkIntracellularVascularEndoLocation";
         this.chkIntracellularVascularEndoLocation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.chkIntracellularVascularEndoLocation.Size = new System.Drawing.Size(291, 20);
         this.chkIntracellularVascularEndoLocation.StyleController = this.layoutControlMetabolism;
         this.chkIntracellularVascularEndoLocation.TabIndex = 11;
         // 
         // cbLocationOnVascularEndo
         // 
         this.cbLocationOnVascularEndo.Location = new System.Drawing.Point(240, 182);
         this.cbLocationOnVascularEndo.Name = "cbLocationOnVascularEndo";
         this.cbLocationOnVascularEndo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbLocationOnVascularEndo.Size = new System.Drawing.Size(291, 20);
         this.cbLocationOnVascularEndo.StyleController = this.layoutControlMetabolism;
         this.cbLocationOnVascularEndo.TabIndex = 10;
         // 
         // cbLocalizationInTissue
         // 
         this.cbLocalizationInTissue.Location = new System.Drawing.Point(240, 134);
         this.cbLocalizationInTissue.Name = "cbLocalizationInTissue";
         this.cbLocalizationInTissue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbLocalizationInTissue.Size = new System.Drawing.Size(291, 20);
         this.cbLocalizationInTissue.StyleController = this.layoutControlMetabolism;
         this.cbLocalizationInTissue.TabIndex = 8;
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(12, 218);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(531, 285);
         this.gridControl.TabIndex = 7;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = true;
         this.gridView.Name = "gridView";
         this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridView.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutGroupMoleculeProperties,
            this.layoutGroupMoleculeLocalization});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(555, 515);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.gridControl;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 206);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(535, 289);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutGroupMoleculeProperties
         // 
         this.layoutGroupMoleculeProperties.CustomizationFormText = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeProperties});
         this.layoutGroupMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeProperties.Name = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Size = new System.Drawing.Size(535, 91);
         this.layoutGroupMoleculeProperties.Text = "layoutGroupMoleculeProperties";
         // 
         // layoutItemMoleculeProperties
         // 
         this.layoutItemMoleculeProperties.Control = this.panelMoleculeProperties;
         this.layoutItemMoleculeProperties.CustomizationFormText = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeProperties.Name = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.Size = new System.Drawing.Size(511, 48);
         this.layoutItemMoleculeProperties.Text = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.TextSize = new System.Drawing.Size(213, 13);
         // 
         // layoutGroupMoleculeLocalization
         // 
         this.layoutGroupMoleculeLocalization.CustomizationFormText = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemLocalizationInTissue,
            this.layoutItemIntracellularVascularEndoLocation,
            this.layoutItemLocalizationInVascularEndo});
         this.layoutGroupMoleculeLocalization.Location = new System.Drawing.Point(0, 91);
         this.layoutGroupMoleculeLocalization.Name = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Size = new System.Drawing.Size(535, 115);
         this.layoutGroupMoleculeLocalization.Text = "layoutGroupMoleculeLocalization";
         // 
         // layoutItemLocalizationInTissue
         // 
         this.layoutItemLocalizationInTissue.Control = this.cbLocalizationInTissue;
         this.layoutItemLocalizationInTissue.CustomizationFormText = "layoutItemCompartmentType";
         this.layoutItemLocalizationInTissue.Location = new System.Drawing.Point(0, 0);
         this.layoutItemLocalizationInTissue.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemLocalizationInTissue.MinSize = new System.Drawing.Size(199, 24);
         this.layoutItemLocalizationInTissue.Name = "layoutItemLocalizationInTissue";
         this.layoutItemLocalizationInTissue.Size = new System.Drawing.Size(511, 24);
         this.layoutItemLocalizationInTissue.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemLocalizationInTissue.Text = "layoutItemLocalizationInTissue";
         this.layoutItemLocalizationInTissue.TextSize = new System.Drawing.Size(213, 13);
         // 
         // layoutItemIntracellularVascularEndoLocation
         // 
         this.layoutItemIntracellularVascularEndoLocation.Control = this.chkIntracellularVascularEndoLocation;
         this.layoutItemIntracellularVascularEndoLocation.CustomizationFormText = "layoutItemIntracellularVascularEndoLocation";
         this.layoutItemIntracellularVascularEndoLocation.Location = new System.Drawing.Point(0, 24);
         this.layoutItemIntracellularVascularEndoLocation.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemIntracellularVascularEndoLocation.MinSize = new System.Drawing.Size(271, 24);
         this.layoutItemIntracellularVascularEndoLocation.Name = "layoutItemIntracellularVascularEndoLocation";
         this.layoutItemIntracellularVascularEndoLocation.Size = new System.Drawing.Size(511, 24);
         this.layoutItemIntracellularVascularEndoLocation.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemIntracellularVascularEndoLocation.Text = "layoutItemIntracellularVascularEndoLocation";
         this.layoutItemIntracellularVascularEndoLocation.TextSize = new System.Drawing.Size(213, 13);
         // 
         // layoutItemLocalizationInVascularEndo
         // 
         this.layoutItemLocalizationInVascularEndo.Control = this.cbLocationOnVascularEndo;
         this.layoutItemLocalizationInVascularEndo.CustomizationFormText = "layoutItemLocalizationOnVascularEndo";
         this.layoutItemLocalizationInVascularEndo.Location = new System.Drawing.Point(0, 48);
         this.layoutItemLocalizationInVascularEndo.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemLocalizationInVascularEndo.MinSize = new System.Drawing.Size(271, 24);
         this.layoutItemLocalizationInVascularEndo.Name = "layoutItemLocalizationInVascularEndo";
         this.layoutItemLocalizationInVascularEndo.Size = new System.Drawing.Size(511, 24);
         this.layoutItemLocalizationInVascularEndo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemLocalizationInVascularEndo.Text = "layoutItemLocalizationOnVascularEndo";
         this.layoutItemLocalizationInVascularEndo.TextSize = new System.Drawing.Size(213, 13);
         // 
         // IndividualProteinExpressionsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControlMetabolism);
         this.Name = "IndividualProteinExpressionsView";
         this.Size = new System.Drawing.Size(555, 515);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlMetabolism)).EndInit();
         this.layoutControlMetabolism.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkIntracellularVascularEndoLocation.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbLocationOnVascularEndo.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbLocalizationInTissue.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLocalizationInTissue)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIntracellularVascularEndoLocation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLocalizationInVascularEndo)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControlMetabolism;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private UxGridControl gridControl;
      protected UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbLocalizationInTissue;
      private DevExpress.XtraEditors.ComboBoxEdit cbLocationOnVascularEndo;
      private DevExpress.XtraEditors.ImageComboBoxEdit chkIntracellularVascularEndoLocation;
      private DevExpress.XtraEditors.PanelControl panelMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeLocalization;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLocalizationInTissue;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIntracellularVascularEndoLocation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLocalizationInVascularEndo;
   }
}
