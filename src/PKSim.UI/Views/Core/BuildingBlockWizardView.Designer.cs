namespace PKSim.UI.Views.Core
{
   partial class BuildingBlockWizardView
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
         _buildingBlockBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tabControl = new DevExpress.XtraTab.XtraTabControl();
         this.tbName = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemTabControl = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTabControl)).BeginInit();
         this.SuspendLayout();
         // 
         // btnPrevious
         // 
         this.btnPrevious.Location = new System.Drawing.Point(22, 12);
         this.btnPrevious.Size = new System.Drawing.Size(380, 22);
         // 
         // btnNext
         // 
         this.btnNext.Location = new System.Drawing.Point(406, 12);
         this.btnNext.Size = new System.Drawing.Size(63, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(473, 12);
         this.btnOk.Size = new System.Drawing.Size(51, 22);
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(528, 12);
         this.btnCancel.Size = new System.Drawing.Size(119, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 504);
         this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(574, 236, 650, 400);
         this.layoutControlBase.Size = new System.Drawing.Size(659, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnPrevious, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnNext, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Size = new System.Drawing.Size(10, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.tabControl);
         this.layoutControl.Controls.Add(this.tbName);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(659, 504);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // tabControl
         // 
         this.tabControl.Location = new System.Drawing.Point(10, 34);
         this.tabControl.Name = "tabControl";
         this.tabControl.Size = new System.Drawing.Size(639, 460);
         this.tabControl.TabIndex = 5;
         // 
         // tbName
         // 
         this.tbName.Location = new System.Drawing.Point(103, 12);
         this.tbName.Name = "tbName";
         this.tbName.Size = new System.Drawing.Size(544, 20);
         this.tbName.StyleController = this.layoutControl;
         this.tbName.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemName,
            this.layoutItemTabControl});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(659, 504);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemName
         // 
         this.layoutItemName.Control = this.tbName;
         this.layoutItemName.CustomizationFormText = "layoutItemName";
         this.layoutItemName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemName.Name = "layoutItemName";
         this.layoutItemName.Size = new System.Drawing.Size(639, 24);
         this.layoutItemName.TextSize = new System.Drawing.Size(79, 13);
         // 
         // layoutItemTabControl
         // 
         this.layoutItemTabControl.Control = this.tabControl;
         this.layoutItemTabControl.CustomizationFormText = "layoutItemTabControl";
         this.layoutItemTabControl.Location = new System.Drawing.Point(0, 24);
         this.layoutItemTabControl.Name = "layoutItemTabControl";
         this.layoutItemTabControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemTabControl.Size = new System.Drawing.Size(639, 460);
         this.layoutItemTabControl.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemTabControl.TextVisible = false;
         // 
         // BuildingBlockWizardView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "BuildingBlockWizardView";
         this.ClientSize = new System.Drawing.Size(659, 550);
         this.Controls.Add(this.layoutControl);
         this.Name = "BuildingBlockWizardView";
         this.Text = "BuildingBlockWizardView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTabControl)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraTab.XtraTabControl tabControl;
      private DevExpress.XtraEditors.TextEdit tbName;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTabControl;
   }
}