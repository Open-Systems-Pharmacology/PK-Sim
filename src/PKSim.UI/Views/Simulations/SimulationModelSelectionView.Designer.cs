using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   partial class SimulationModelSelectionView
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
         _modelBinder.Dispose();

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
         this.pbModel = new System.Windows.Forms.PictureBox();
         this.cbModel = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupModelSettings = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemModel = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pbModel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbModel.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupModelSettings)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemModel)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.pbModel);
         this.layoutControl.Controls.Add(this.cbModel);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(788, 203, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(357, 386);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // pbModel
         // 
         this.pbModel.Location = new System.Drawing.Point(2, 69);
         this.pbModel.Name = "pbModel";
         this.pbModel.Size = new System.Drawing.Size(353, 315);
         this.pbModel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pbModel.TabIndex = 7;
         this.pbModel.TabStop = false;
         // 
         // cbModel
         // 
         this.cbModel.Location = new System.Drawing.Point(14, 33);
         this.cbModel.Name = "cbModel";
         this.cbModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbModel.Size = new System.Drawing.Size(329, 20);
         this.cbModel.StyleController = this.layoutControl;
         this.cbModel.TabIndex = 5;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutGroupModelSettings});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(357, 386);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.pbModel;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 67);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(357, 319);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutGroupModelSettings
         // 
         this.layoutGroupModelSettings.CustomizationFormText = "layoutGroupModelSettings";
         this.layoutGroupModelSettings.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemModel});
         this.layoutGroupModelSettings.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupModelSettings.Name = "layoutGroupModelSettings";
         this.layoutGroupModelSettings.Size = new System.Drawing.Size(357, 67);
         this.layoutGroupModelSettings.Text = "layoutGroupModelSettings";
         // 
         // layoutItemModel
         // 
         this.layoutItemModel.Control = this.cbModel;
         this.layoutItemModel.CustomizationFormText = "layoutItemModel";
         this.layoutItemModel.Location = new System.Drawing.Point(0, 0);
         this.layoutItemModel.Name = "layoutItemModel";
         this.layoutItemModel.Size = new System.Drawing.Size(333, 24);
         this.layoutItemModel.Text = "layoutItemModel";
         this.layoutItemModel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemModel.TextToControlDistance = 0;
         this.layoutItemModel.TextVisible = false;
         // 
         // SimulationModelSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "SimulationModelSelectionView";
         this.Size = new System.Drawing.Size(357, 386);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pbModel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbModel.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupModelSettings)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemModel)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbModel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemModel;
      private System.Windows.Forms.PictureBox pbModel;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupModelSettings;
   }
}
