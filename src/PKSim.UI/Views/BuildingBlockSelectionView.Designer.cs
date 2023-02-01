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
         this.btnLoadBuildingBlock = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnCreateBuildingBlock = new DevExpress.XtraEditors.SimpleButton();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemLoad = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCreate = new DevExpress.XtraLayout.LayoutControlItem();
         this.cbBuildingBlocks = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.layoutItemBuildingBlock = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbBuildingBlocks.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBuildingBlock)).BeginInit();
         this.SuspendLayout();
         // 
         // btnLoadBuildingBlock
         // 
         this.btnLoadBuildingBlock.Location = new System.Drawing.Point(407, 2);
         this.btnLoadBuildingBlock.Name = "btnLoadBuildingBlock";
         this.btnLoadBuildingBlock.Size = new System.Drawing.Size(174, 22);
         this.btnLoadBuildingBlock.StyleController = this.layoutControl;
         this.btnLoadBuildingBlock.TabIndex = 7;
         this.btnLoadBuildingBlock.Text = "btnLoadBuildingBlock";
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.cbBuildingBlocks);
         this.layoutControl.Controls.Add(this.btnCreateBuildingBlock);
         this.layoutControl.Controls.Add(this.btnLoadBuildingBlock);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(583, 29);
         this.layoutControl.TabIndex = 2;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // btnCreateBuildingBlock
         // 
         this.btnCreateBuildingBlock.Location = new System.Drawing.Point(187, 2);
         this.btnCreateBuildingBlock.Name = "btnCreateBuildingBlock";
         this.btnCreateBuildingBlock.Size = new System.Drawing.Size(216, 22);
         this.btnCreateBuildingBlock.StyleController = this.layoutControl;
         this.btnCreateBuildingBlock.TabIndex = 5;
         this.btnCreateBuildingBlock.Text = "btnCreateBuildingBlock";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemLoad,
            this.layoutItemCreate,
            this.layoutItemBuildingBlock});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(583, 29);
         this.Root.TextVisible = false;
         // 
         // layoutItemLoad
         // 
         this.layoutItemLoad.Control = this.btnLoadBuildingBlock;
         this.layoutItemLoad.Location = new System.Drawing.Point(405, 0);
         this.layoutItemLoad.Name = "layoutItemLoad";
         this.layoutItemLoad.Size = new System.Drawing.Size(178, 29);
         this.layoutItemLoad.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLoad.TextVisible = false;
         // 
         // layoutItemCreate
         // 
         this.layoutItemCreate.Control = this.btnCreateBuildingBlock;
         this.layoutItemCreate.Location = new System.Drawing.Point(185, 0);
         this.layoutItemCreate.Name = "layoutItemCreate";
         this.layoutItemCreate.Size = new System.Drawing.Size(220, 29);
         this.layoutItemCreate.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCreate.TextVisible = false;
         // 
         // cbBuildingBlocks
         // 
         this.cbBuildingBlocks.Location = new System.Drawing.Point(2, 2);
         this.cbBuildingBlocks.Name = "cbBuildingBlocks";
         this.cbBuildingBlocks.Properties.AutoHeight = false;
         this.cbBuildingBlocks.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbBuildingBlocks.Size = new System.Drawing.Size(181, 25);
         this.cbBuildingBlocks.StyleController = this.layoutControl;
         this.cbBuildingBlocks.TabIndex = 6;
         // 
         // layoutItemBuildingBlock
         // 
         this.layoutItemBuildingBlock.Control = this.cbBuildingBlocks;
         this.layoutItemBuildingBlock.Location = new System.Drawing.Point(0, 0);
         this.layoutItemBuildingBlock.Name = "layoutItemBuildingBlock";
         this.layoutItemBuildingBlock.Size = new System.Drawing.Size(185, 29);
         this.layoutItemBuildingBlock.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemBuildingBlock.TextVisible = false;
         // 
         // BuildingBlockSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.Name = "BuildingBlockSelectionView";
         this.Size = new System.Drawing.Size(583, 29);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbBuildingBlocks.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBuildingBlock)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion
      private DevExpress.XtraEditors.SimpleButton btnCreateBuildingBlock;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbBuildingBlocks;
      private DevExpress.XtraEditors.SimpleButton btnLoadBuildingBlock;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLoad;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCreate;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemBuildingBlock;
   }
}


