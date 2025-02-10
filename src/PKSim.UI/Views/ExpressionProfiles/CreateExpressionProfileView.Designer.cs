
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class CreateExpressionProfileView
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.cbCategory = new OSPSuite.UI.Controls.UxMRUEdit();
         this.labelCategoryDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMoleculeName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelDiseaseState = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemDiseaseState = new DevExpress.XtraLayout.LayoutControlItem();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         this.SuspendLayout();
         // 
         // tablePanel
         // 
         this.tablePanel.Location = new System.Drawing.Point(0, 131);
         this.tablePanel.Size = new System.Drawing.Size(565, 43);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelDiseaseState);
         this.layoutControl.Controls.Add(this.cbCategory);
         this.layoutControl.Controls.Add(this.labelCategoryDescription);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(565, 131);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // cbCategory
         // 
         this.cbCategory.Location = new System.Drawing.Point(144, 60);
         this.cbCategory.Name = "cbCategory";
         this.cbCategory.Properties.AllowRemoveMRUItems = false;
         this.cbCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbCategory.Size = new System.Drawing.Size(409, 20);
         this.cbCategory.StyleController = this.layoutControl;
         this.cbCategory.TabIndex = 8;
         // 
         // labelCategoryDescription
         // 
         this.labelCategoryDescription.Location = new System.Drawing.Point(12, 84);
         this.labelCategoryDescription.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.labelCategoryDescription.Name = "labelCategoryDescription";
         this.labelCategoryDescription.Size = new System.Drawing.Size(120, 13);
         this.labelCategoryDescription.StyleController = this.layoutControl;
         this.labelCategoryDescription.TabIndex = 7;
         this.labelCategoryDescription.Text = "labelCategoryDescription";
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(144, 36);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(409, 20);
         this.cbMoleculeName.StyleController = this.layoutControl;
         this.cbMoleculeName.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMoleculeName,
            this.layoutItemSpecies,
            this.layoutControlItem1,
            this.layoutItemCategory,
            this.layoutItemDiseaseState});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(565, 131);
         this.Root.TextVisible = false;
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
         this.layoutItemMoleculeName.CustomizationFormText = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 24);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(545, 24);
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(120, 13);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.labelCategoryDescription;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 72);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(545, 17);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.cbCategory;
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 48);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(545, 24);
         this.layoutItemCategory.TextSize = new System.Drawing.Size(120, 13);
         // 
         // panelDiseaseState
         // 
         this.panelDiseaseState.Location = new System.Drawing.Point(12, 101);
         this.panelDiseaseState.Name = "panelDiseaseState";
         this.panelDiseaseState.Size = new System.Drawing.Size(541, 18);
         this.panelDiseaseState.TabIndex = 9;
         // 
         // layoutItemDiseaseState
         // 
         this.layoutItemDiseaseState.Control = this.panelDiseaseState;
         this.layoutItemDiseaseState.Location = new System.Drawing.Point(0, 89);
         this.layoutItemDiseaseState.Name = "layoutItemDiseaseState";
         this.layoutItemDiseaseState.Size = new System.Drawing.Size(545, 22);
         this.layoutItemDiseaseState.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDiseaseState.TextVisible = false;
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(144, 12);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.AllowMouseWheel = false;
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(409, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 6;
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(545, 24);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(120, 13);
         // 
         // CreateExpressionProfileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "CreateExpressionProfileView";
         this.ClientSize = new System.Drawing.Size(565, 174);
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5);
         this.Name = "CreateExpressionProfileView";
         this.Text = "CreateExpressionProfileView";
         this.Controls.SetChildIndex(this.tablePanel, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         this.layoutControl.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDiseaseState)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private OSPSuite.UI.Controls.UxMRUEdit cbMoleculeName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeName;
      private Core.UxImageComboBoxEdit cbSpecies;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSpecies;
      private DevExpress.XtraEditors.LabelControl labelCategoryDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private OSPSuite.UI.Controls.UxMRUEdit cbCategory;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCategory;
      private DevExpress.XtraEditors.PanelControl panelDiseaseState;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseState;
   }
}