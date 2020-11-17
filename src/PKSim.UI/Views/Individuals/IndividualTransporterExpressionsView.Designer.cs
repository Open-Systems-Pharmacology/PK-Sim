
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
         this.panelMoleculeProperties = new DevExpress.XtraEditors.PanelControl();
         this.panelWarning = new OSPSuite.UI.Controls.UxHintPanel();
         this.lblTransporterTypeDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbTransporterType = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupMoleculeProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeProperties = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupMoleculeLocalization = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTransporterType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWarning = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelExpressionParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemExpressionParameters = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTransporterType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTransporterType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelExpressionParameters);
         this.layoutControl.Controls.Add(this.panelMoleculeProperties);
         this.layoutControl.Controls.Add(this.panelWarning);
         this.layoutControl.Controls.Add(this.lblTransporterTypeDescription);
         this.layoutControl.Controls.Add(this.cbTransporterType);
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
         this.panelMoleculeProperties.Location = new System.Drawing.Point(186, 45);
         this.panelMoleculeProperties.Name = "panelMoleculeProperties";
         this.panelMoleculeProperties.Size = new System.Drawing.Size(413, 42);
         this.panelMoleculeProperties.TabIndex = 16;
         // 
         // panelWarning
         // 
         this.panelWarning.Location = new System.Drawing.Point(24, 179);
         this.panelWarning.MaximumSize = new System.Drawing.Size(1000000, 40);
         this.panelWarning.MaxLines = 3;
         this.panelWarning.MinimumSize = new System.Drawing.Size(200, 40);
         this.panelWarning.Name = "panelWarning";
         this.panelWarning.NoteText = "";
         this.panelWarning.Size = new System.Drawing.Size(575, 40);
         this.panelWarning.TabIndex = 14;
         // 
         // lblTransporterTypeDescription
         // 
         this.lblTransporterTypeDescription.Location = new System.Drawing.Point(24, 162);
         this.lblTransporterTypeDescription.Name = "lblTransporterTypeDescription";
         this.lblTransporterTypeDescription.Size = new System.Drawing.Size(144, 13);
         this.lblTransporterTypeDescription.StyleController = this.layoutControl;
         this.lblTransporterTypeDescription.TabIndex = 13;
         this.lblTransporterTypeDescription.Text = "lblTransporterTypeDescription";
         // 
         // cbTransporterType
         // 
         this.cbTransporterType.Location = new System.Drawing.Point(186, 136);
         this.cbTransporterType.Name = "cbTransporterType";
         this.cbTransporterType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTransporterType.Size = new System.Drawing.Size(413, 20);
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
         this.layoutControlGroup1.Size = new System.Drawing.Size(623, 570);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutGroupMoleculeProperties
         // 
         this.layoutGroupMoleculeProperties.CustomizationFormText = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeProperties});
         this.layoutGroupMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupMoleculeProperties.Name = "layoutGroupMoleculeProperties";
         this.layoutGroupMoleculeProperties.Size = new System.Drawing.Size(603, 91);
         // 
         // layoutItemMoleculeProperties
         // 
         this.layoutItemMoleculeProperties.Control = this.panelMoleculeProperties;
         this.layoutItemMoleculeProperties.CustomizationFormText = "layoutMoleculeProperties";
         this.layoutItemMoleculeProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoleculeProperties.Name = "layoutItemMoleculeProperties";
         this.layoutItemMoleculeProperties.Size = new System.Drawing.Size(579, 46);
         this.layoutItemMoleculeProperties.Text = "layoutMoleculeProperties";
         this.layoutItemMoleculeProperties.TextSize = new System.Drawing.Size(159, 13);
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
         this.layoutGroupMoleculeLocalization.Size = new System.Drawing.Size(603, 146);
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
         this.layoutItemTransporterType.TextSize = new System.Drawing.Size(159, 13);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.lblTransporterTypeDescription;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(579, 17);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutItemWarning
         // 
         this.layoutItemWarning.Control = this.panelWarning;
         this.layoutItemWarning.CustomizationFormText = "layoutItemWarning";
         this.layoutItemWarning.Location = new System.Drawing.Point(0, 43);
         this.layoutItemWarning.Name = "layoutItemWarning";
         this.layoutItemWarning.Size = new System.Drawing.Size(579, 58);
         this.layoutItemWarning.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemWarning.TextVisible = false;
         // 
         // panelExpressionParameters
         // 
         this.panelExpressionParameters.Location = new System.Drawing.Point(174, 249);
         this.panelExpressionParameters.Name = "panelExpressionParameters";
         this.panelExpressionParameters.Size = new System.Drawing.Size(437, 309);
         this.panelExpressionParameters.TabIndex = 17;
         // 
         // layoutItemExpressionParameters
         // 
         this.layoutItemExpressionParameters.Control = this.panelExpressionParameters;
         this.layoutItemExpressionParameters.Location = new System.Drawing.Point(0, 237);
         this.layoutItemExpressionParameters.Name = "layoutItemExpressionParameters";
         this.layoutItemExpressionParameters.Size = new System.Drawing.Size(603, 313);
         this.layoutItemExpressionParameters.TextSize = new System.Drawing.Size(159, 13);
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
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupMoleculeLocalization)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTransporterType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpressionParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbTransporterType;
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
      private DevExpress.XtraEditors.PanelControl panelExpressionParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExpressionParameters;
   }
}
