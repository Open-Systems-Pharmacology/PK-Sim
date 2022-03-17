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
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.labelTargetCompartment = new DevExpress.XtraEditors.LabelControl();
         this.labelTargetOrgan = new DevExpress.XtraEditors.LabelControl();
         this.labelEndTime = new DevExpress.XtraEditors.LabelControl();
         this.labelDosingInterval = new DevExpress.XtraEditors.LabelControl();
         this.labelDose = new DevExpress.XtraEditors.LabelControl();
         this.labelApplicationType = new DevExpress.XtraEditors.LabelControl();
         this.cbTargetCompartment = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.cbTargetOrgan = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.uxEndTime = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.cbDosingType = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.uxDose = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.cbApplicationType = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.panelDynamicParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDynamicParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutSimpleProtocol)).BeginInit();
         this.layoutSimpleProtocol.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetCompartment.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetOrgan.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDosingType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbApplicationType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDynamicParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDynamicParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutSimpleProtocol
         // 
         this.layoutSimpleProtocol.AllowCustomization = false;
         this.layoutSimpleProtocol.Controls.Add(this.tablePanel);
         this.layoutSimpleProtocol.Controls.Add(this.panelDynamicParameters);
         this.layoutSimpleProtocol.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutSimpleProtocol.Location = new System.Drawing.Point(0, 0);
         this.layoutSimpleProtocol.Name = "layoutSimpleProtocol";
         this.layoutSimpleProtocol.OptionsView.UseSkinIndents = false;
         this.layoutSimpleProtocol.Root = this.layoutControlGroup1;
         this.layoutSimpleProtocol.Size = new System.Drawing.Size(486, 393);
         this.layoutSimpleProtocol.TabIndex = 0;
         this.layoutSimpleProtocol.Text = "uxLayoutControl1";
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 5F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 55F)});
         this.tablePanel.Controls.Add(this.labelTargetCompartment);
         this.tablePanel.Controls.Add(this.labelTargetOrgan);
         this.tablePanel.Controls.Add(this.labelEndTime);
         this.tablePanel.Controls.Add(this.labelDosingInterval);
         this.tablePanel.Controls.Add(this.labelDose);
         this.tablePanel.Controls.Add(this.labelApplicationType);
         this.tablePanel.Controls.Add(this.cbTargetCompartment);
         this.tablePanel.Controls.Add(this.cbTargetOrgan);
         this.tablePanel.Controls.Add(this.uxEndTime);
         this.tablePanel.Controls.Add(this.cbDosingType);
         this.tablePanel.Controls.Add(this.uxDose);
         this.tablePanel.Controls.Add(this.cbApplicationType);
         this.tablePanel.Location = new System.Drawing.Point(8, 29);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 30F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 28F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(470, 183);
         this.tablePanel.TabIndex = 17;
         // 
         // labelTargetCompartment
         // 
         this.tablePanel.SetColumn(this.labelTargetCompartment, 0);
         this.labelTargetCompartment.Location = new System.Drawing.Point(3, 141);
         this.labelTargetCompartment.Name = "labelTargetCompartment";
         this.tablePanel.SetRow(this.labelTargetCompartment, 5);
         this.labelTargetCompartment.Size = new System.Drawing.Size(119, 13);
         this.labelTargetCompartment.TabIndex = 22;
         this.labelTargetCompartment.Text = "labelTargetCompartment";
         // 
         // labelTargetOrgan
         // 
         this.tablePanel.SetColumn(this.labelTargetOrgan, 0);
         this.labelTargetOrgan.Location = new System.Drawing.Point(3, 112);
         this.labelTargetOrgan.Name = "labelTargetOrgan";
         this.tablePanel.SetRow(this.labelTargetOrgan, 4);
         this.labelTargetOrgan.Size = new System.Drawing.Size(84, 13);
         this.labelTargetOrgan.TabIndex = 21;
         this.labelTargetOrgan.Text = "labelTargetOrgan";
         // 
         // labelEndTime
         // 
         this.tablePanel.SetColumn(this.labelEndTime, 0);
         this.labelEndTime.Location = new System.Drawing.Point(3, 84);
         this.labelEndTime.Name = "labelEndTime";
         this.tablePanel.SetRow(this.labelEndTime, 3);
         this.labelEndTime.Size = new System.Drawing.Size(62, 13);
         this.labelEndTime.TabIndex = 20;
         this.labelEndTime.Text = "labelEndTime";
         // 
         // labelDosingInterval
         // 
         this.tablePanel.SetColumn(this.labelDosingInterval, 0);
         this.labelDosingInterval.Location = new System.Drawing.Point(3, 58);
         this.labelDosingInterval.Name = "labelDosingInterval";
         this.tablePanel.SetRow(this.labelDosingInterval, 2);
         this.labelDosingInterval.Size = new System.Drawing.Size(78, 13);
         this.labelDosingInterval.TabIndex = 19;
         this.labelDosingInterval.Text = "labelDosingType";
         // 
         // labelDose
         // 
         this.tablePanel.SetColumn(this.labelDose, 0);
         this.labelDose.Location = new System.Drawing.Point(3, 32);
         this.labelDose.Name = "labelDose";
         this.tablePanel.SetRow(this.labelDose, 1);
         this.labelDose.Size = new System.Drawing.Size(46, 13);
         this.labelDose.TabIndex = 18;
         this.labelDose.Text = "labelDose";
         // 
         // labelApplicationType
         // 
         this.tablePanel.SetColumn(this.labelApplicationType, 0);
         this.labelApplicationType.Location = new System.Drawing.Point(3, 6);
         this.labelApplicationType.Name = "labelApplicationType";
         this.tablePanel.SetRow(this.labelApplicationType, 0);
         this.labelApplicationType.Size = new System.Drawing.Size(98, 13);
         this.labelApplicationType.TabIndex = 17;
         this.labelApplicationType.Text = "labelApplicationType";
         // 
         // cbTargetCompartment
         // 
         this.tablePanel.SetColumn(this.cbTargetCompartment, 1);
         this.cbTargetCompartment.Location = new System.Drawing.Point(128, 138);
         this.cbTargetCompartment.Name = "cbTargetCompartment";
         this.cbTargetCompartment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbTargetCompartment, 5);
         this.cbTargetCompartment.Size = new System.Drawing.Size(339, 20);
         this.cbTargetCompartment.TabIndex = 16;
         // 
         // cbTargetOrgan
         // 
         this.tablePanel.SetColumn(this.cbTargetOrgan, 1);
         this.cbTargetOrgan.Location = new System.Drawing.Point(128, 109);
         this.cbTargetOrgan.Name = "cbTargetOrgan";
         this.cbTargetOrgan.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbTargetOrgan, 4);
         this.cbTargetOrgan.Size = new System.Drawing.Size(339, 20);
         this.cbTargetOrgan.TabIndex = 15;
         // 
         // uxEndTime
         // 
         this.uxEndTime.Caption = "";
         this.tablePanel.SetColumn(this.uxEndTime, 1);
         this.uxEndTime.Location = new System.Drawing.Point(128, 81);
         this.uxEndTime.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxEndTime.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxEndTime.Name = "uxEndTime";
         this.tablePanel.SetRow(this.uxEndTime, 3);
         this.uxEndTime.Size = new System.Drawing.Size(339, 22);
         this.uxEndTime.TabIndex = 10;
         this.uxEndTime.ToolTip = "";
         // 
         // cbDosingType
         // 
         this.tablePanel.SetColumn(this.cbDosingType, 1);
         this.cbDosingType.Location = new System.Drawing.Point(128, 55);
         this.cbDosingType.Name = "cbDosingType";
         this.cbDosingType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbDosingType, 2);
         this.cbDosingType.Size = new System.Drawing.Size(339, 20);
         this.cbDosingType.TabIndex = 10;
         // 
         // uxDose
         // 
         this.uxDose.Caption = "";
         this.tablePanel.SetColumn(this.uxDose, 1);
         this.uxDose.Location = new System.Drawing.Point(128, 29);
         this.uxDose.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxDose.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxDose.Name = "uxDose";
         this.tablePanel.SetRow(this.uxDose, 1);
         this.uxDose.Size = new System.Drawing.Size(339, 22);
         this.uxDose.TabIndex = 9;
         this.uxDose.ToolTip = "";
         // 
         // cbApplicationType
         // 
         this.tablePanel.SetColumn(this.cbApplicationType, 1);
         this.cbApplicationType.Location = new System.Drawing.Point(128, 3);
         this.cbApplicationType.Name = "cbApplicationType";
         this.cbApplicationType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.tablePanel.SetRow(this.cbApplicationType, 0);
         this.cbApplicationType.Size = new System.Drawing.Size(339, 20);
         this.cbApplicationType.TabIndex = 13;
         // 
         // panelDynamicParameters
         // 
         this.panelDynamicParameters.Location = new System.Drawing.Point(8, 222);
         this.panelDynamicParameters.MinimumSize = new System.Drawing.Size(0, 50);
         this.panelDynamicParameters.Name = "panelDynamicParameters";
         this.panelDynamicParameters.Size = new System.Drawing.Size(470, 163);
         this.panelDynamicParameters.TabIndex = 12;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupProperties});
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup1.Size = new System.Drawing.Size(486, 393);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutGroupProperties
         // 
         this.layoutGroupProperties.CustomizationFormText = "layoutGroupProperties";
         this.layoutGroupProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDynamicParameters,
            this.layoutControlItem1});
         this.layoutGroupProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupProperties.Name = "layoutGroupProperties";
         this.layoutGroupProperties.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupProperties.Size = new System.Drawing.Size(486, 393);
         // 
         // layoutItemDynamicParameters
         // 
         this.layoutItemDynamicParameters.Control = this.panelDynamicParameters;
         this.layoutItemDynamicParameters.CustomizationFormText = "layoutItemDynamicParameters";
         this.layoutItemDynamicParameters.Location = new System.Drawing.Point(0, 193);
         this.layoutItemDynamicParameters.Name = "layoutItemDynamicParameters";
         this.layoutItemDynamicParameters.Size = new System.Drawing.Size(480, 173);
         this.layoutItemDynamicParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDynamicParameters.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.tablePanel;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(480, 193);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
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
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.tablePanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetCompartment.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbTargetOrgan.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDosingType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbApplicationType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDynamicParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDynamicParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private UxParameterDTOEdit uxEndTime;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDosingType;
      private UxParameterDTOEdit uxDose;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupProperties;
      private DevExpress.XtraEditors.PanelControl panelDynamicParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDynamicParameters;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbApplicationType;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbTargetCompartment;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbTargetOrgan;
      private OSPSuite.UI.Controls.UxLayoutControl layoutSimpleProtocol;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraEditors.LabelControl labelTargetCompartment;
      private DevExpress.XtraEditors.LabelControl labelTargetOrgan;
      private DevExpress.XtraEditors.LabelControl labelEndTime;
      private DevExpress.XtraEditors.LabelControl labelDosingInterval;
      private DevExpress.XtraEditors.LabelControl labelDose;
      private DevExpress.XtraEditors.LabelControl labelApplicationType;
   }
}


