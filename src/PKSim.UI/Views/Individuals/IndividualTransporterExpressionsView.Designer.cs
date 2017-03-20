
namespace PKSim.UI.Views.Individuals
{
   partial class IndividualTransporterExpressionsView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelMoleculeProperties = new DevExpress.XtraEditors.PanelControl();
         this.panelWarning = new OSPSuite.UI.Controls.UxHintPanel();
         this.lblTransporterTypeDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbTransporterType = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.gridControl = new DevExpress.XtraGrid.GridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeProperties = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeLocalization = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTransporterType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWarning = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTransporterType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTransporterType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelMoleculeProperties);
         this.layoutControl.Controls.Add(this.panelWarning);
         this.layoutControl.Controls.Add(this.lblTransporterTypeDescription);
         this.layoutControl.Controls.Add(this.cbTransporterType);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(604, 360, 370, 350);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(623, 570);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelMoleculeProperties
         // 
         this.panelMoleculeProperties.Location = new System.Drawing.Point(160, 43);
         this.panelMoleculeProperties.Name = "panelMoleculeProperties";
         this.panelMoleculeProperties.Size = new System.Drawing.Size(439, 44);
         this.panelMoleculeProperties.TabIndex = 16;
         // 
         // panelWarning
         // 
         this.panelWarning.Location = new System.Drawing.Point(24, 177);
         this.panelWarning.MaximumSize = new System.Drawing.Size(1000000, 40);
         this.panelWarning.MinimumSize = new System.Drawing.Size(200, 40);
         this.panelWarning.Name = "panelWarning";
         this.panelWarning.NoteText = "";
         this.panelWarning.Size = new System.Drawing.Size(575, 40);
         this.panelWarning.TabIndex = 14;
         // 
         // lblTransporterTypeDescription
         // 
         this.lblTransporterTypeDescription.Location = new System.Drawing.Point(24, 160);
         this.lblTransporterTypeDescription.Name = "lblTransporterTypeDescription";
         this.lblTransporterTypeDescription.Size = new System.Drawing.Size(144, 13);
         this.lblTransporterTypeDescription.StyleController = this.layoutControl;
         this.lblTransporterTypeDescription.TabIndex = 13;
         this.lblTransporterTypeDescription.Text = "lblTransporterTypeDescription";
         // 
         // cbTransporterType
         // 
         this.cbTransporterType.Location = new System.Drawing.Point(160, 134);
         this.cbTransporterType.Name = "cbTransporterType";
         this.cbTransporterType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTransporterType.Size = new System.Drawing.Size(439, 20);
         this.cbTransporterType.StyleController = this.layoutControl;
         this.cbTransporterType.TabIndex = 11;
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(12, 233);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(599, 325);
         this.gridControl.TabIndex = 4;
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
         this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
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
            this.layoutControlItem2,
            this.layoutGroupMoleculeProperties,
            this.layoutGroupMoleculeLocalization});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(623, 570);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.gridControl;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 221);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(603, 329);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutGroupMoleculeProperties
         // 
         this.layoutGroupMoleculeProperties.CustomizationFormText = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeProperties});
         this.layoutGroupMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeProperties.Name = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Size = new System.Drawing.Size(603, 91);
         this.layoutGroupMoleculeProperties.Text = "layoutGroupMoleculeProperties";
         // 
         // layoutItemMoleculeProperties
         // 
         this.layoutItemMoleculeProperties.Control = this.panelMoleculeProperties;
         this.layoutItemMoleculeProperties.CustomizationFormText = "layoutMoleculeProperties";
         this.layoutItemMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeProperties.Name = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.Size = new System.Drawing.Size(579, 48);
         this.layoutItemMoleculeProperties.Text = "layoutMoleculeProperties";
         this.layoutItemMoleculeProperties.TextSize = new System.Drawing.Size(133, 13);
         // 
         // layoutGroupMoleculeLocalization
         // 
         this.layoutGroupMoleculeLocalization.CustomizationFormText = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemTransporterType,
            this.layoutControlItem1,
            this.layoutItemWarning});
         this.layoutGroupMoleculeLocalization.Location = new System.Drawing.Point(0, 91);
         this.layoutGroupMoleculeLocalization.Name = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Size = new System.Drawing.Size(603, 130);
         this.layoutGroupMoleculeLocalization.Text = "layoutGroupMoleculeLocalization";
         // 
         // layoutItemTransporterType
         // 
         this.layoutItemTransporterType.Control = this.cbTransporterType;
         this.layoutItemTransporterType.CustomizationFormText = "layoutItemTransporterType";
         this.layoutItemTransporterType.Location = new System.Drawing.Point(0, 0);
         this.layoutItemTransporterType.MaxSize = new System.Drawing.Size(0, 26);
         this.layoutItemTransporterType.MinSize = new System.Drawing.Size(191, 26);
         this.layoutItemTransporterType.Name = "layoutItemTransporterType";
         this.layoutItemTransporterType.Size = new System.Drawing.Size(579, 26);
         this.layoutItemTransporterType.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemTransporterType.Text = "layoutItemTransporterType";
         this.layoutItemTransporterType.TextSize = new System.Drawing.Size(133, 13);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.lblTransporterTypeDescription;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(579, 17);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutItemWarning
         // 
         this.layoutItemWarning.Control = this.panelWarning;
         this.layoutItemWarning.CustomizationFormText = "layoutItemWarning";
         this.layoutItemWarning.Location = new System.Drawing.Point(0, 43);
         this.layoutItemWarning.Name = "layoutItemWarning";
         this.layoutItemWarning.Size = new System.Drawing.Size(579, 44);
         this.layoutItemWarning.Text = "layoutItemWarning";
         this.layoutItemWarning.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemWarning.TextToControlDistance = 0;
         this.layoutItemWarning.TextVisible = false;
         // 
         // IndividualTransporterExpressionsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualTransporterExpressionsView";
         this.Size = new System.Drawing.Size(623, 570);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTransporterType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTransporterType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraGrid.GridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbTransporterType;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.LabelControl lblTransporterTypeDescription;
      private OSPSuite.UI.Controls.UxHintPanel panelWarning;
      private DevExpress.XtraEditors.PanelControl panelMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeLocalization;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTransporterType;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWarning;
   }
}
