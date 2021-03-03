using PKSim.UI.Views.Parameters;

namespace PKSim.UI.Views.Protocols
{
   partial class SimpleProtocolView 
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
         this.layoutSimpleProtocol = new OSPSuite.UI.Controls.UxLayoutControl();
         this.cbTargetCompartment = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.cbTargetOrgan = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.cbApplicationType = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.panelDynamicParameters = new DevExpress.XtraEditors.PanelControl();
         this.uxEndTime = new UxParameterDTOEdit();
         this.cbDosingType = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.uxDose = new UxParameterDTOEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDose = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDynamicParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDosingInterval = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemEndTime = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemApplicationType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemTargetOrgan = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemTargetCompartment = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutSimpleProtocol)).BeginInit();
         this.layoutSimpleProtocol.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetCompartment.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetOrgan.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbApplicationType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDynamicParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDosingType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDose)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDynamicParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDosingInterval)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEndTime)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemApplicationType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTargetOrgan)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTargetCompartment)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutSimpleProtocol
         // 
         this.layoutSimpleProtocol.AllowCustomization = false;
         this.layoutSimpleProtocol.Controls.Add(this.cbTargetCompartment);
         this.layoutSimpleProtocol.Controls.Add(this.cbTargetOrgan);
         this.layoutSimpleProtocol.Controls.Add(this.cbApplicationType);
         this.layoutSimpleProtocol.Controls.Add(this.panelDynamicParameters);
         this.layoutSimpleProtocol.Controls.Add(this.uxEndTime);
         this.layoutSimpleProtocol.Controls.Add(this.cbDosingType);
         this.layoutSimpleProtocol.Controls.Add(this.uxDose);
         this.layoutSimpleProtocol.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutSimpleProtocol.Location = new System.Drawing.Point(0, 0);
         this.layoutSimpleProtocol.Name = "layoutSimpleProtocol";
         this.layoutSimpleProtocol.OptionsView.UseSkinIndents = false;
         this.layoutSimpleProtocol.Root = this.layoutControlGroup1;
         this.layoutSimpleProtocol.Size = new System.Drawing.Size(486, 393);
         this.layoutSimpleProtocol.TabIndex = 0;
         this.layoutSimpleProtocol.Text = "uxLayoutControl1";
         // 
         // cbTargetCompartment
         // 
         this.cbTargetCompartment.Location = new System.Drawing.Point(162, 182);
         this.cbTargetCompartment.Name = "cbTargetCompartment";
         this.cbTargetCompartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTargetCompartment.Size = new System.Drawing.Size(316, 20);
         this.cbTargetCompartment.StyleController = this.layoutSimpleProtocol;
         this.cbTargetCompartment.TabIndex = 16;
         // 
         // cbTargetOrgan
         // 
         this.cbTargetOrgan.Location = new System.Drawing.Point(162, 152);
         this.cbTargetOrgan.Name = "cbTargetOrgan";
         this.cbTargetOrgan.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTargetOrgan.Size = new System.Drawing.Size(316, 20);
         this.cbTargetOrgan.StyleController = this.layoutSimpleProtocol;
         this.cbTargetOrgan.TabIndex = 15;
         // 
         // cbApplicationType
         // 
         this.cbApplicationType.Location = new System.Drawing.Point(162, 28);
         this.cbApplicationType.Name = "cbApplicationType";
         this.cbApplicationType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbApplicationType.Size = new System.Drawing.Size(316, 20);
         this.cbApplicationType.StyleController = this.layoutSimpleProtocol;
         this.cbApplicationType.TabIndex = 13;
         // 
         // panelDynamicParameters
         // 
         this.panelDynamicParameters.Location = new System.Drawing.Point(8, 212);
         this.panelDynamicParameters.MinimumSize = new System.Drawing.Size(0, 50);
         this.panelDynamicParameters.Name = "panelDynamicParameters";
         this.panelDynamicParameters.Size = new System.Drawing.Size(470, 173);
         this.panelDynamicParameters.TabIndex = 12;
         // 
         // uxEndTime
         // 
         this.uxEndTime.Caption = "";
         this.uxEndTime.Location = new System.Drawing.Point(162, 120);
         this.uxEndTime.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxEndTime.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxEndTime.Name = "uxEndTime";
         this.uxEndTime.Size = new System.Drawing.Size(316, 22);
         this.uxEndTime.TabIndex = 10;
         // 
         // cbDosingType
         // 
         this.cbDosingType.Location = new System.Drawing.Point(162, 90);
         this.cbDosingType.Name = "cbDosingType";
         this.cbDosingType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDosingType.Size = new System.Drawing.Size(316, 20);
         this.cbDosingType.StyleController = this.layoutSimpleProtocol;
         this.cbDosingType.TabIndex = 10;
         // 
         // uxDose
         // 
         this.uxDose.Caption = "";
         this.uxDose.Location = new System.Drawing.Point(162, 58);
         this.uxDose.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxDose.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxDose.Name = "uxDose";
         this.uxDose.Size = new System.Drawing.Size(316, 22);
         this.uxDose.TabIndex = 9;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupProperties});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup1.Size = new System.Drawing.Size(486, 393);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutGroupProperties
         // 
         this.layoutGroupProperties.CustomizationFormText = "layoutGroupProperties";
         this.layoutGroupProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDose,
            this.layoutItemDynamicParameters,
            this.layoutItemDosingInterval,
            this.layoutItemEndTime,
            this.layoutItemApplicationType,
            this.layoutItemTargetOrgan,
            this.layoutItemTargetCompartment});
         this.layoutGroupProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupProperties.Name = "layoutGroupProperties";
         this.layoutGroupProperties.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupProperties.Size = new System.Drawing.Size(486, 393);
         this.layoutGroupProperties.Text = "layoutGroupProperties";
         // 
         // layoutItemDose
         // 
         this.layoutItemDose.Control = this.uxDose;
         this.layoutItemDose.CustomizationFormText = "layoutItemDose";
         this.layoutItemDose.Location = new System.Drawing.Point(0, 30);
         this.layoutItemDose.Name = "layoutItemDose";
         this.layoutItemDose.Size = new System.Drawing.Size(480, 32);
         this.layoutItemDose.Text = "layoutItemDose";
         this.layoutItemDose.TextSize = new System.Drawing.Size(149, 13);
         this.layoutItemDose.TextToControlDistance = 5;
         // 
         // layoutItemDynamicParameters
         // 
         this.layoutItemDynamicParameters.Control = this.panelDynamicParameters;
         this.layoutItemDynamicParameters.CustomizationFormText = "layoutItemDynamicParameters";
         this.layoutItemDynamicParameters.Location = new System.Drawing.Point(0, 184);
         this.layoutItemDynamicParameters.Name = "layoutItemDynamicParameters";
         this.layoutItemDynamicParameters.Size = new System.Drawing.Size(480, 183);
         this.layoutItemDynamicParameters.Text = "layoutItemDynamicParameters";
         this.layoutItemDynamicParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDynamicParameters.TextToControlDistance = 0;
         this.layoutItemDynamicParameters.TextVisible = false;
         // 
         // layoutItemDosingInterval
         // 
         this.layoutItemDosingInterval.Control = this.cbDosingType;
         this.layoutItemDosingInterval.CustomizationFormText = "layoutItemDosingInterval";
         this.layoutItemDosingInterval.Location = new System.Drawing.Point(0, 62);
         this.layoutItemDosingInterval.Name = "layoutItemDosingInterval";
         this.layoutItemDosingInterval.Size = new System.Drawing.Size(480, 30);
         this.layoutItemDosingInterval.Text = "layoutItemDosingInterval";
         this.layoutItemDosingInterval.TextSize = new System.Drawing.Size(149, 13);
         this.layoutItemDosingInterval.TextToControlDistance = 5;
         // 
         // layoutItemEndTime
         // 
         this.layoutItemEndTime.Control = this.uxEndTime;
         this.layoutItemEndTime.CustomizationFormText = "layoutItemEndTime";
         this.layoutItemEndTime.Location = new System.Drawing.Point(0, 92);
         this.layoutItemEndTime.Name = "layoutItemEndTime";
         this.layoutItemEndTime.Size = new System.Drawing.Size(480, 32);
         this.layoutItemEndTime.Text = "layoutItemEndTime";
         this.layoutItemEndTime.TextSize = new System.Drawing.Size(149, 13);
         this.layoutItemEndTime.TextToControlDistance = 5;
         // 
         // layoutItemApplicationType
         // 
         this.layoutItemApplicationType.Control = this.cbApplicationType;
         this.layoutItemApplicationType.CustomizationFormText = "layoutItemApplicationType";
         this.layoutItemApplicationType.Location = new System.Drawing.Point(0, 0);
         this.layoutItemApplicationType.Name = "layoutItemApplicationType";
         this.layoutItemApplicationType.Size = new System.Drawing.Size(480, 30);
         this.layoutItemApplicationType.Text = "layoutItemApplicationType";
         this.layoutItemApplicationType.TextSize = new System.Drawing.Size(149, 13);
         this.layoutItemApplicationType.TextToControlDistance = 5;
         // 
         // layoutItemTargetOrgan
         // 
         this.layoutItemTargetOrgan.Control = this.cbTargetOrgan;
         this.layoutItemTargetOrgan.CustomizationFormText = "layoutItemTargetOrgan";
         this.layoutItemTargetOrgan.Location = new System.Drawing.Point(0, 124);
         this.layoutItemTargetOrgan.Name = "layoutItemTargetOrgan";
         this.layoutItemTargetOrgan.Size = new System.Drawing.Size(480, 30);
         this.layoutItemTargetOrgan.Text = "layoutItemTargetOrgan";
         this.layoutItemTargetOrgan.TextSize = new System.Drawing.Size(149, 13);
         this.layoutItemTargetOrgan.TextToControlDistance = 5;
         // 
         // layoutItemTargetCompartment
         // 
         this.layoutItemTargetCompartment.Control = this.cbTargetCompartment;
         this.layoutItemTargetCompartment.CustomizationFormText = "layoutItemTargetCompartment";
         this.layoutItemTargetCompartment.Location = new System.Drawing.Point(0, 154);
         this.layoutItemTargetCompartment.Name = "layoutItemTargetCompartment";
         this.layoutItemTargetCompartment.Size = new System.Drawing.Size(480, 30);
         this.layoutItemTargetCompartment.Text = "layoutItemTargetCompartment";
         this.layoutItemTargetCompartment.TextSize = new System.Drawing.Size(149, 13);
         this.layoutItemTargetCompartment.TextToControlDistance = 5;
         // 
         // SimpleProtocolView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutSimpleProtocol);
         this.Name = "SimpleProtocolView";
         this.Size = new System.Drawing.Size(486, 393);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutSimpleProtocol)).EndInit();
         this.layoutSimpleProtocol.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetCompartment.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetOrgan.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbApplicationType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDynamicParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDosingType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDose)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDynamicParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDosingInterval)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEndTime)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemApplicationType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTargetOrgan)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTargetCompartment)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private UxParameterDTOEdit uxEndTime;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDosingType;
      private UxParameterDTOEdit uxDose;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDose;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDosingInterval;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemEndTime;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupProperties;
      private DevExpress.XtraEditors.PanelControl panelDynamicParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDynamicParameters;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbApplicationType;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemApplicationType;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbTargetCompartment;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbTargetOrgan;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTargetOrgan;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTargetCompartment;
      private OSPSuite.UI.Controls.UxLayoutControl layoutSimpleProtocol;
   }
}


