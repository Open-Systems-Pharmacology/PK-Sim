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
         this.layoutControlImage = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemModel = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.pbModel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbModel.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlImage)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemModel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.pbModel);
         this.layoutControl.Controls.Add(this.cbModel);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(788, 203, 442, 494);
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(416, 475);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // pbModel
         // 
         this.pbModel.Location = new System.Drawing.Point(2, 38);
         this.pbModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.pbModel.Name = "pbModel";
         this.pbModel.Size = new System.Drawing.Size(412, 435);
         this.pbModel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.pbModel.TabIndex = 7;
         this.pbModel.TabStop = false;
         // 
         // cbModel
         // 
         this.cbModel.Location = new System.Drawing.Point(2, 2);
         this.cbModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.cbModel.Name = "cbModel";
         this.cbModel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbModel.Size = new System.Drawing.Size(412, 22);
         this.cbModel.StyleController = this.layoutControl;
         this.cbModel.TabIndex = 5;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlImage,
            this.layoutItemModel,
            this.emptySpaceItem1});
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(416, 475);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlImage
         // 
         this.layoutControlImage.Control = this.pbModel;
         this.layoutControlImage.CustomizationFormText = "layoutControlItem1";
         this.layoutControlImage.Location = new System.Drawing.Point(0, 36);
         this.layoutControlImage.Name = "layoutControlImage";
         this.layoutControlImage.Size = new System.Drawing.Size(416, 439);
         this.layoutControlImage.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlImage.TextVisible = false;
         // 
         // layoutItemModel
         // 
         this.layoutItemModel.Control = this.cbModel;
         this.layoutItemModel.CustomizationFormText = "layoutItemModel";
         this.layoutItemModel.Location = new System.Drawing.Point(0, 0);
         this.layoutItemModel.Name = "layoutItemModel";
         this.layoutItemModel.Size = new System.Drawing.Size(416, 26);
         this.layoutItemModel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemModel.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 26);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(416, 10);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // SimulationModelSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
         this.Name = "SimulationModelSelectionView";
         this.Size = new System.Drawing.Size(416, 475);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.pbModel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbModel.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlImage)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemModel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbModel;
      private System.Windows.Forms.PictureBox pbModel;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlImage;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemModel;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}
