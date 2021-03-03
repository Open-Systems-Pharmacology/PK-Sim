namespace PKSim.UI.Views.AdvancedParameters
{
   partial class AdvancedParameterView
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
         _screenBinder.DeleteBinding();
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
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.cbDistributionType = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupDistributionType = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDistributionType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDistributionType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDistributionType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDistributionType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Controls.Add(this.cbDistributionType);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(548, 463);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelParameters
         // 
         this.panelParameters.Location = new System.Drawing.Point(14, 57);
         this.panelParameters.Margin = new System.Windows.Forms.Padding(0);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Size = new System.Drawing.Size(520, 392);
         this.panelParameters.TabIndex = 5;
         // 
         // cbDistributionType
         // 
         this.cbDistributionType.Location = new System.Drawing.Point(14, 33);
         this.cbDistributionType.Name = "cbDistributionType";
         this.cbDistributionType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDistributionType.Size = new System.Drawing.Size(520, 20);
         this.cbDistributionType.StyleController = this.layoutControl;
         this.cbDistributionType.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupDistributionType});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(548, 463);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutGroupDistributionType
         // 
         this.layoutGroupDistributionType.CustomizationFormText = "layoutControlGroup2";
         this.layoutGroupDistributionType.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDistributionType,
            this.layoutItemParameters});
         this.layoutGroupDistributionType.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupDistributionType.Name = "layoutGroupDistributionType";
         this.layoutGroupDistributionType.Size = new System.Drawing.Size(548, 463);
         this.layoutGroupDistributionType.Text = "layoutGroupDistributionType";
         // 
         // layoutItemDistributionType
         // 
         this.layoutItemDistributionType.Control = this.cbDistributionType;
         this.layoutItemDistributionType.CustomizationFormText = "layoutItemDistributionType";
         this.layoutItemDistributionType.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDistributionType.Name = "layoutItemDistributionType";
         this.layoutItemDistributionType.Size = new System.Drawing.Size(524, 24);
         this.layoutItemDistributionType.Text = "layoutItemDistributionType";
         this.layoutItemDistributionType.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDistributionType.TextToControlDistance = 0;
         this.layoutItemDistributionType.TextVisible = false;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.panelParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 24);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Size = new System.Drawing.Size(524, 396);
         this.layoutItemParameters.Text = "layoutItemParameters";
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         // 
         // AdvancedParameterView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "AdvancedParameterView";
         this.Size = new System.Drawing.Size(548, 463);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDistributionType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDistributionType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDistributionType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDistributionType;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDistributionType;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupDistributionType;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
   }
}
