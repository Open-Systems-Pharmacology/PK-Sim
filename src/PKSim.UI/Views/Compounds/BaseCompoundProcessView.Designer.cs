namespace PKSim.UI.Views.Compounds
{
   partial class BaseCompoundProcessView<TProcess,TProcessDTO>
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
         _presenter = null;
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
         this.lblProcessType = new DevExpress.XtraEditors.LabelControl();
         this.cbSpecies = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.tbDescription = new DevExpress.XtraEditors.MemoEdit();
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemProcessType = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProcessType)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.lblProcessType);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Controls.Add(this.tbDescription);
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(777, 204, 725, 524);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(474, 433);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblProcessType
         // 
         this.lblProcessType.Location = new System.Drawing.Point(118, 26);
         this.lblProcessType.Name = "lblProcessType";
         this.lblProcessType.Size = new System.Drawing.Size(71, 13);
         this.lblProcessType.StyleController = this.layoutControl;
         this.lblProcessType.TabIndex = 7;
         this.lblProcessType.Text = "lblProcessType";
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(118, 2);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(354, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 6;
         // 
         // tbDescription
         // 
         this.tbDescription.Location = new System.Drawing.Point(2, 299);
         this.tbDescription.MinimumSize = new System.Drawing.Size(20, 20);
         this.tbDescription.Name = "tbDescription";
         this.tbDescription.Size = new System.Drawing.Size(470, 132);
         this.tbDescription.StyleController = this.layoutControl;
         this.tbDescription.TabIndex = 5;
         // 
         // panelParameters
         // 
         this.panelParameters.Location = new System.Drawing.Point(0, 41);
         this.panelParameters.Margin = new System.Windows.Forms.Padding(0);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Size = new System.Drawing.Size(474, 240);
         this.panelParameters.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutItemDescription,
            this.layoutItemSpecies,
            this.layoutItemProcessType});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(474, 433);
         this.layoutControlGroup.Text = "layoutControlGroup1";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.panelParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 41);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemParameters.Size = new System.Drawing.Size(474, 240);
         this.layoutItemParameters.Text = "layoutItemParameters";
         this.layoutItemParameters.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.tbDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutItemDescription";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 281);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(474, 152);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemDescription.TextSize = new System.Drawing.Size(113, 13);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.CustomizationFormText = "layoutItemSpecies";
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(474, 24);
         this.layoutItemSpecies.Text = "layoutItemSpecies";
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(113, 13);
         // 
         // layoutItemProcessType
         // 
         this.layoutItemProcessType.Control = this.lblProcessType;
         this.layoutItemProcessType.CustomizationFormText = "layoutControlItem1";
         this.layoutItemProcessType.Location = new System.Drawing.Point(0, 24);
         this.layoutItemProcessType.Name = "layoutItemProcessType";
         this.layoutItemProcessType.Size = new System.Drawing.Size(474, 17);
         this.layoutItemProcessType.Text = "layoutItemProcessType";
         this.layoutItemProcessType.TextSize = new System.Drawing.Size(113, 13);
         // 
         // CompoundProcessView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "BaseCompoundProcessView";
         this.Size = new System.Drawing.Size(474, 433);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProcessType)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.MemoEdit tbDescription;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraEditors.LabelControl lblProcessType;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbSpecies;
      protected DevExpress.XtraLayout.LayoutControlItem layoutItemSpecies;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProcessType;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
   }
}
