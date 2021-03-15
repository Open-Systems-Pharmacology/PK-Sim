namespace PKSim.UI.Views.Formulations
{
   partial class FormulationSettingsView
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
         this.layoutControlFormulation = new OSPSuite.UI.Controls.UxLayoutControl();
         this.splitControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.cbFormulationType = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.lblFormulationDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemFormulationDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemFormulationType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSplitter = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlFormulation)).BeginInit();
         this.layoutControlFormulation.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitControl)).BeginInit();
         this.splitControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbFormulationType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulationDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulationType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplitter)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControlFormulation
         // 
         this.layoutControlFormulation.AllowCustomization = false;
         this.layoutControlFormulation.Controls.Add(this.splitControl);
         this.layoutControlFormulation.Controls.Add(this.cbFormulationType);
         this.layoutControlFormulation.Controls.Add(this.lblFormulationDescription);
         this.layoutControlFormulation.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlFormulation.Location = new System.Drawing.Point(0, 0);
         this.layoutControlFormulation.Name = "layoutControlFormulation";
         this.layoutControlFormulation.Root = this.layoutControlGroup1;
         this.layoutControlFormulation.Size = new System.Drawing.Size(434, 435);
         this.layoutControlFormulation.TabIndex = 0;
         this.layoutControlFormulation.Text = "layoutControl1";
         // 
         // splitControl
         // 
         this.splitControl.Horizontal = false;
         this.splitControl.Location = new System.Drawing.Point(2, 43);
         this.splitControl.Name = "splitControl";
         this.splitControl.Panel1.Text = "Panel1";
         this.splitControl.Panel2.Text = "Panel2";
         this.splitControl.Size = new System.Drawing.Size(430, 390);
         this.splitControl.SplitterPosition = 180;
         this.splitControl.TabIndex = 7;
         this.splitControl.Text = "splitContainerControl1";
         // 
         // cbFormulationType
         // 
         this.cbFormulationType.Location = new System.Drawing.Point(2, 2);
         this.cbFormulationType.Name = "cbFormulationType";
         this.cbFormulationType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbFormulationType.Size = new System.Drawing.Size(430, 20);
         this.cbFormulationType.StyleController = this.layoutControlFormulation;
         this.cbFormulationType.TabIndex = 6;
         // 
         // lblFormulationDescription
         // 
         this.lblFormulationDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
         this.lblFormulationDescription.Location = new System.Drawing.Point(2, 26);
         this.lblFormulationDescription.Name = "lblFormulationDescription";
         this.lblFormulationDescription.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
         this.lblFormulationDescription.Size = new System.Drawing.Size(430, 13);
         this.lblFormulationDescription.StyleController = this.layoutControlFormulation;
         this.lblFormulationDescription.TabIndex = 5;
         this.lblFormulationDescription.Text = "lblFormulationDescription";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemFormulationDescription,
            this.layoutItemFormulationType,
            this.layoutItemSplitter});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(434, 435);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemFormulationDescription
         // 
         this.layoutItemFormulationDescription.Control = this.lblFormulationDescription;
         this.layoutItemFormulationDescription.CustomizationFormText = "layoutItemFormulationDescription";
         this.layoutItemFormulationDescription.Location = new System.Drawing.Point(0, 24);
         this.layoutItemFormulationDescription.Name = "layoutItemFormulationDescription";
         this.layoutItemFormulationDescription.Size = new System.Drawing.Size(434, 17);
         this.layoutItemFormulationDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemFormulationDescription.TextVisible = false;
         // 
         // layoutItemFormulationType
         // 
         this.layoutItemFormulationType.Control = this.cbFormulationType;
         this.layoutItemFormulationType.CustomizationFormText = "layoutItemFormulationType";
         this.layoutItemFormulationType.Location = new System.Drawing.Point(0, 0);
         this.layoutItemFormulationType.Name = "layoutItemFormulationType";
         this.layoutItemFormulationType.Size = new System.Drawing.Size(434, 24);
         this.layoutItemFormulationType.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemFormulationType.TextVisible = false;
         // 
         // layoutItemSplitter
         // 
         this.layoutItemSplitter.Control = this.splitControl;
         this.layoutItemSplitter.CustomizationFormText = "layoutItemSplitter";
         this.layoutItemSplitter.Location = new System.Drawing.Point(0, 41);
         this.layoutItemSplitter.Name = "layoutItemSplitter";
         this.layoutItemSplitter.Size = new System.Drawing.Size(434, 394);
         this.layoutItemSplitter.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSplitter.TextVisible = false;
         // 
         // FormulationSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControlFormulation);
         this.Name = "FormulationSettingsView";
         this.Size = new System.Drawing.Size(434, 435);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlFormulation)).EndInit();
         this.layoutControlFormulation.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitControl)).EndInit();
         this.splitControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbFormulationType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulationDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulationType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplitter)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbFormulationType;
      private DevExpress.XtraEditors.LabelControl lblFormulationDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemFormulationDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemFormulationType;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControlFormulation;
      private DevExpress.XtraEditors.SplitContainerControl splitControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSplitter;
   }
}
