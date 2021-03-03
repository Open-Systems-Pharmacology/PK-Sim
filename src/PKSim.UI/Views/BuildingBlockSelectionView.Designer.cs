namespace PKSim.UI.Views
{
   partial class BuildingBlockSelectionView
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
         cleanup();
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
         this.btnLoadBuildingBlock = new DevExpress.XtraEditors.SimpleButton();
         this.cbBuildingBlocks = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.btnCreateBuildingBlock = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemCreate = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemComboBox = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLoad = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbBuildingBlocks.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemComboBox)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnLoadBuildingBlock);
         this.layoutControl.Controls.Add(this.cbBuildingBlocks);
         this.layoutControl.Controls.Add(this.btnCreateBuildingBlock);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(587, 233);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnLoadBuildingBlock
         // 
         this.btnLoadBuildingBlock.Location = new System.Drawing.Point(475, 2);
         this.btnLoadBuildingBlock.Name = "btnLoadBuildingBlock";
         this.btnLoadBuildingBlock.Size = new System.Drawing.Size(112, 22);
         this.btnLoadBuildingBlock.StyleController = this.layoutControl;
         this.btnLoadBuildingBlock.TabIndex = 7;
         this.btnLoadBuildingBlock.Text = "btnLoadBuildingBlock";
         // 
         // cbBuildingBlocks
         // 
         this.cbBuildingBlocks.Location = new System.Drawing.Point(0, 2);
         this.cbBuildingBlocks.Name = "cbBuildingBlocks";
         this.cbBuildingBlocks.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbBuildingBlocks.Size = new System.Drawing.Size(347, 20);
         this.cbBuildingBlocks.StyleController = this.layoutControl;
         this.cbBuildingBlocks.TabIndex = 6;
         // 
         // btnCreateBuildingBlock
         // 
         this.btnCreateBuildingBlock.Location = new System.Drawing.Point(351, 2);
         this.btnCreateBuildingBlock.Name = "btnCreateBuildingBlock";
         this.btnCreateBuildingBlock.Size = new System.Drawing.Size(120, 22);
         this.btnCreateBuildingBlock.StyleController = this.layoutControl;
         this.btnCreateBuildingBlock.TabIndex = 5;
         this.btnCreateBuildingBlock.Text = "btnCreateBuildingBlock";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCreate,
            this.layoutItemComboBox,
            this.layoutItemLoad});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(587, 233);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemCreate
         // 
         this.layoutItemCreate.Control = this.btnCreateBuildingBlock;
         this.layoutItemCreate.CustomizationFormText = "layoutControlItem2";
         this.layoutItemCreate.Location = new System.Drawing.Point(349, 0);
         this.layoutItemCreate.Name = "layoutItemCreate";
         this.layoutItemCreate.Size = new System.Drawing.Size(124, 233);
         this.layoutItemCreate.Text = "layoutItemCreate";
         this.layoutItemCreate.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCreate.TextToControlDistance = 0;
         this.layoutItemCreate.TextVisible = false;
         // 
         // layoutItemComboBox
         // 
         this.layoutItemComboBox.Control = this.cbBuildingBlocks;
         this.layoutItemComboBox.CustomizationFormText = "layoutItemComboBox";
         this.layoutItemComboBox.Location = new System.Drawing.Point(0, 0);
         this.layoutItemComboBox.Name = "layoutItemComboBox";
         this.layoutItemComboBox.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
         this.layoutItemComboBox.Size = new System.Drawing.Size(349, 233);
         this.layoutItemComboBox.Text = "layoutItemComboBox";
         this.layoutItemComboBox.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemComboBox.TextToControlDistance = 0;
         this.layoutItemComboBox.TextVisible = false;
         // 
         // layoutItemLoad
         // 
         this.layoutItemLoad.Control = this.btnLoadBuildingBlock;
         this.layoutItemLoad.CustomizationFormText = "layoutItemLoad";
         this.layoutItemLoad.Location = new System.Drawing.Point(473, 0);
         this.layoutItemLoad.Name = "layoutItemLoad";
         this.layoutItemLoad.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
         this.layoutItemLoad.Size = new System.Drawing.Size(114, 233);
         this.layoutItemLoad.Text = "layoutItemLoad";
         this.layoutItemLoad.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLoad.TextToControlDistance = 0;
         this.layoutItemLoad.TextVisible = false;
         // 
         // BuildingBlockSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "BuildingBlockSelectionView";
         this.Size = new System.Drawing.Size(587, 233);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbBuildingBlocks.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemComboBox)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnCreateBuildingBlock;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCreate;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbBuildingBlocks;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemComboBox;
      private DevExpress.XtraEditors.SimpleButton btnLoadBuildingBlock;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLoad;


   }
}


