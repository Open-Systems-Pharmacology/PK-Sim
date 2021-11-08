
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
         this.panelExpressionParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelMoleculeProperties = new DevExpress.XtraEditors.PanelControl();
         this.cbTransporterType = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupMoleculeProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeProperties = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeLocalization = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTransporterDirection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemExpressionParameters = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTransporterType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTransporterDirection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelExpressionParameters);
         this.layoutControl.Controls.Add(this.panelMoleculeProperties);
         this.layoutControl.Controls.Add(this.cbTransporterType);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(604, 360, 370, 350);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(810, 543);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelExpressionParameters
         // 
         this.panelExpressionParameters.Location = new System.Drawing.Point(164, 176);
         this.panelExpressionParameters.Name = "panelExpressionParameters";
         this.panelExpressionParameters.Size = new System.Drawing.Size(644, 365);
         this.panelExpressionParameters.TabIndex = 17;
         // 
         // panelMoleculeProperties
         // 
         this.panelMoleculeProperties.Location = new System.Drawing.Point(174, 33);
         this.panelMoleculeProperties.Name = "panelMoleculeProperties";
         this.panelMoleculeProperties.Size = new System.Drawing.Size(624, 58);
         this.panelMoleculeProperties.TabIndex = 16;
         // 
         // cbTransporterType
         // 
         this.cbTransporterType.Location = new System.Drawing.Point(176, 138);
         this.cbTransporterType.Name = "cbTransporterType";
         this.cbTransporterType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTransporterType.Size = new System.Drawing.Size(620, 20);
         this.cbTransporterType.StyleController = this.layoutControl;
         this.cbTransporterType.TabIndex = 11;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupMoleculeProperties,
            this.layoutGroupMoleculeLocalization,
            this.layoutItemExpressionParameters});
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(810, 543);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutGroupMoleculeProperties
         // 
         this.layoutGroupMoleculeProperties.CustomizationFormText = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeProperties});
         this.layoutGroupMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeProperties.Name = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Size = new System.Drawing.Size(810, 103);
         // 
         // layoutItemMoleculeProperties
         // 
         this.layoutItemMoleculeProperties.Control = this.panelMoleculeProperties;
         this.layoutItemMoleculeProperties.CustomizationFormText = "layoutMoleculeProperties";
         this.layoutItemMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeProperties.Name = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemMoleculeProperties.Size = new System.Drawing.Size(786, 58);
         this.layoutItemMoleculeProperties.TextSize = new System.Drawing.Size(159, 13);
         // 
         // layoutGroupMoleculeLocalization
         // 
         this.layoutGroupMoleculeLocalization.CustomizationFormText = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemTransporterDirection});
         this.layoutGroupMoleculeLocalization.Location = new System.Drawing.Point(0, 103);
         this.layoutGroupMoleculeLocalization.Name = "layoutGroupMoleculeLocalization";
         this.layoutGroupMoleculeLocalization.Size = new System.Drawing.Size(810, 71);
         // 
         // layoutItemTransporterDirection
         // 
         this.layoutItemTransporterDirection.Control = this.cbTransporterType;
         this.layoutItemTransporterDirection.CustomizationFormText = "layoutItemTransporterDirection";
         this.layoutItemTransporterDirection.Location = new System.Drawing.Point(0, 0);
         this.layoutItemTransporterDirection.MaxSize = new System.Drawing.Size(0, 26);
         this.layoutItemTransporterDirection.MinSize = new System.Drawing.Size(191, 26);
         this.layoutItemTransporterDirection.Name = "layoutItemTransporterDirection";
         this.layoutItemTransporterDirection.Size = new System.Drawing.Size(786, 26);
         this.layoutItemTransporterDirection.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemTransporterDirection.TextSize = new System.Drawing.Size(159, 13);
         // 
         // layoutItemExpressionParameters
         // 
         this.layoutItemExpressionParameters.Control = this.panelExpressionParameters;
         this.layoutItemExpressionParameters.Location = new System.Drawing.Point(0, 174);
         this.layoutItemExpressionParameters.Name = "layoutItemExpressionParameters";
         this.layoutItemExpressionParameters.Size = new System.Drawing.Size(810, 369);
         this.layoutItemExpressionParameters.TextSize = new System.Drawing.Size(159, 13);
         // 
         // IndividualTransporterExpressionsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualTransporterExpressionsView";
         this.Size = new System.Drawing.Size(810, 543);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTransporterType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTransporterDirection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbTransporterType;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupMoleculeLocalization;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTransporterDirection;
      private DevExpress.XtraEditors.PanelControl panelExpressionParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExpressionParameters;
   }
}
