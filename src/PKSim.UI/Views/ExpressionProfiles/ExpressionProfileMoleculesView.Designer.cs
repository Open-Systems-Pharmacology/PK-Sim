
namespace PKSim.UI.Views.ExpressionProfiles
{
   partial class ExpressionProfileMoleculesView
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
         this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
         this.tbCategory = new DevExpress.XtraEditors.TextEdit();
         this.cbMoleculeName = new OSPSuite.UI.Controls.UxMRUEdit();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemMoleculeName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCategory = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelExpression = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemExpression = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpression)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelExpression);
         this.layoutControl.Controls.Add(this.tbCategory);
         this.layoutControl.Controls.Add(this.cbMoleculeName);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(843, 689);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // tbCategory
         // 
         this.tbCategory.Location = new System.Drawing.Point(137, 83);
         this.tbCategory.Name = "tbCategory";
         this.tbCategory.Size = new System.Drawing.Size(692, 20);
         this.tbCategory.StyleController = this.layoutControl;
         this.tbCategory.TabIndex = 6;
         // 
         // cbMoleculeName
         // 
         this.cbMoleculeName.Location = new System.Drawing.Point(137, 59);
         this.cbMoleculeName.Name = "cbMoleculeName";
         this.cbMoleculeName.Properties.AllowRemoveMRUItems = false;
         this.cbMoleculeName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbMoleculeName.Size = new System.Drawing.Size(692, 20);
         this.cbMoleculeName.StyleController = this.layoutControl;
         this.cbMoleculeName.TabIndex = 5;
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(137, 35);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.AllowMouseWheel = false;
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(692, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupProperties,
            this.layoutItemExpression});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(843, 689);
         this.Root.TextVisible = false;
         // 
         // layoutGroupProperties
         // 
         this.layoutGroupProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSpecies,
            this.layoutItemMoleculeName,
            this.layoutItemCategory});
         this.layoutGroupProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupProperties.Name = "layoutGroupProperties";
         this.layoutGroupProperties.Size = new System.Drawing.Size(843, 117);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(819, 24);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(120, 13);
         // 
         // layoutItemMoleculeName
         // 
         this.layoutItemMoleculeName.Control = this.cbMoleculeName;
         this.layoutItemMoleculeName.Location = new System.Drawing.Point(0, 24);
         this.layoutItemMoleculeName.Name = "layoutItemMoleculeName";
         this.layoutItemMoleculeName.Size = new System.Drawing.Size(819, 24);
         this.layoutItemMoleculeName.TextSize = new System.Drawing.Size(120, 13);
         // 
         // layoutItemCategory
         // 
         this.layoutItemCategory.Control = this.tbCategory;
         this.layoutItemCategory.Location = new System.Drawing.Point(0, 48);
         this.layoutItemCategory.Name = "layoutItemCategory";
         this.layoutItemCategory.Size = new System.Drawing.Size(819, 24);
         this.layoutItemCategory.TextSize = new System.Drawing.Size(120, 13);
         // 
         // panelExpression
         // 
         this.panelExpression.Location = new System.Drawing.Point(2, 119);
         this.panelExpression.Name = "panelExpression";
         this.panelExpression.Size = new System.Drawing.Size(839, 568);
         this.panelExpression.TabIndex = 7;
         // 
         // layoutItemExpression
         // 
         this.layoutItemExpression.Control = this.panelExpression;
         this.layoutItemExpression.Location = new System.Drawing.Point(0, 117);
         this.layoutItemExpression.Name = "layoutItemExpression";
         this.layoutItemExpression.Size = new System.Drawing.Size(843, 572);
         this.layoutItemExpression.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemExpression.TextVisible = false;
         // 
         // ExpressionProfileMoleculesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ExpressionProfileMoleculesView";
         this.Size = new System.Drawing.Size(843, 689);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbCategory.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbMoleculeName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoleculeName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCategory)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpression)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private Core.UxImageComboBoxEdit cbSpecies;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSpecies;
      private DevExpress.XtraEditors.TextEdit tbCategory;
      private OSPSuite.UI.Controls.UxMRUEdit cbMoleculeName;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupProperties;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoleculeName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCategory;
      private DevExpress.XtraEditors.PanelControl panelExpression;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExpression;
   }
}
