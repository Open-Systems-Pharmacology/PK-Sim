namespace PKSim.UI.Views.Parameters
{
   partial class UxParameterDTOEdit
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
         _discreteValueElementBinder.Dispose();
         _valueElementBinder.Dispose();
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
         this.cbUnit = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.tbValue = new DevExpress.XtraEditors.TextEdit();
         this.cbDiscreteValue = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItemValue = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItemUnit = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDiscreteValue = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbUnit.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbValue.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDiscreteValue.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemValue)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUnit)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiscreteValue)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.AutoScroll = false;
         this.layoutControl.Controls.Add(this.cbUnit);
         this.layoutControl.Controls.Add(this.tbValue);
         this.layoutControl.Controls.Add(this.cbDiscreteValue);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(691, 238, 250, 350);
         this.layoutControl.OptionsView.UseSkinIndents = false;
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(497, 42);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // cbUnit
         // 
         this.cbUnit.Location = new System.Drawing.Point(376, 0);
         this.cbUnit.Name = "cbUnit";
         this.cbUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbUnit.Size = new System.Drawing.Size(121, 20);
         this.cbUnit.StyleController = this.layoutControl;
         this.cbUnit.TabIndex = 5;
         // 
         // tbValue
         // 
         this.tbValue.Location = new System.Drawing.Point(0, 0);
         this.tbValue.Name = "tbValue";
         this.tbValue.Size = new System.Drawing.Size(372, 20);
         this.tbValue.StyleController = this.layoutControl;
         this.tbValue.TabIndex = 4;
         // 
         // cbDiscreteValue
         // 
         this.cbDiscreteValue.Location = new System.Drawing.Point(0, 20);
         this.cbDiscreteValue.Name = "cbDiscreteValue";
         this.cbDiscreteValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDiscreteValue.Size = new System.Drawing.Size(372, 20);
         this.cbDiscreteValue.StyleController = this.layoutControl;
         this.cbDiscreteValue.TabIndex = 6;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemValue,
            this.layoutControlItemUnit,
            this.layoutItemDiscreteValue});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup.ShowInCustomizationForm = false;
         this.layoutControlGroup.Size = new System.Drawing.Size(497, 42);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutControlItemValue
         // 
         this.layoutControlItemValue.Control = this.tbValue;
         this.layoutControlItemValue.CustomizationFormText = "layoutControlItemValue";
         this.layoutControlItemValue.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItemValue.Name = "layoutControlItemValue";
         this.layoutControlItemValue.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 0, 0);
         this.layoutControlItemValue.Size = new System.Drawing.Size(374, 20);
         this.layoutControlItemValue.Text = "layoutControlItemValue";
         this.layoutControlItemValue.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemValue.TextToControlDistance = 0;
         this.layoutControlItemValue.TextVisible = false;
         // 
         // layoutControlItemUnit
         // 
         this.layoutControlItemUnit.Control = this.cbUnit;
         this.layoutControlItemUnit.CustomizationFormText = "layoutControlItemUnit";
         this.layoutControlItemUnit.Location = new System.Drawing.Point(374, 0);
         this.layoutControlItemUnit.Name = "layoutControlItemUnit";
         this.layoutControlItemUnit.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 0, 0);
         this.layoutControlItemUnit.ShowInCustomizationForm = false;
         this.layoutControlItemUnit.Size = new System.Drawing.Size(123, 42);
         this.layoutControlItemUnit.Text = "layoutControlItemUnit";
         this.layoutControlItemUnit.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemUnit.TextToControlDistance = 0;
         this.layoutControlItemUnit.TextVisible = false;
         // 
         // layoutItemDiscreteValue
         // 
         this.layoutItemDiscreteValue.Control = this.cbDiscreteValue;
         this.layoutItemDiscreteValue.CustomizationFormText = "layoutItemDiscreteValue";
         this.layoutItemDiscreteValue.Location = new System.Drawing.Point(0, 20);
         this.layoutItemDiscreteValue.Name = "layoutItemDiscreteValue";
         this.layoutItemDiscreteValue.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 0, 0);
         this.layoutItemDiscreteValue.Size = new System.Drawing.Size(374, 22);
         this.layoutItemDiscreteValue.Text = "layoutItemDiscreteValue";
         this.layoutItemDiscreteValue.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDiscreteValue.TextToControlDistance = 0;
         this.layoutItemDiscreteValue.TextVisible = false;
         // 
         // UxParameterDTOEdit
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.MinimumSize = new System.Drawing.Size(0, 21);
         this.Name = "UxParameterDTOEdit";
         this.Size = new System.Drawing.Size(497, 42);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbUnit.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbValue.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDiscreteValue.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemValue)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUnit)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiscreteValue)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.TextEdit tbValue;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemValue;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbUnit;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemUnit;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDiscreteValue;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiscreteValue;
   }
}


