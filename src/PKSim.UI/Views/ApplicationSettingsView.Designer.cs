using PKSim.UI.Views.Core;

namespace PKSim.UI.Views
{
   partial class ApplicationSettingsView
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
         _gridViewBinder.Dispose();  
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
         this.chkUseWatermark = new DevExpress.XtraEditors.CheckEdit();
         this.textWatermark = new DevExpress.XtraEditors.TextEdit();
         this.buttonMoBiPath = new DevExpress.XtraEditors.ButtonEdit();
         this.gridDatabasePath = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewDatabasePath = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDatabasePath = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemMoBiPath = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupWatermark = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemUseWatermark = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWatermarkText = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.chkUseWatermark.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.textWatermark.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonMoBiPath.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridDatabasePath)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewDatabasePath)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDatabasePath)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoBiPath)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupWatermark)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemUseWatermark)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWatermarkText)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.chkUseWatermark);
         this.layoutControl.Controls.Add(this.textWatermark);
         this.layoutControl.Controls.Add(this.buttonMoBiPath);
         this.layoutControl.Controls.Add(this.gridDatabasePath);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(443, 401);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // chkUseWatermark
         // 
         this.chkUseWatermark.Location = new System.Drawing.Point(24, 82);
         this.chkUseWatermark.Name = "chkUseWatermark";
         this.chkUseWatermark.Properties.Caption = "chkUseWatermark";
         this.chkUseWatermark.Size = new System.Drawing.Size(395, 19);
         this.chkUseWatermark.StyleController = this.layoutControl;
         this.chkUseWatermark.TabIndex = 7;
         // 
         // textWatermark
         // 
         this.textWatermark.Location = new System.Drawing.Point(156, 105);
         this.textWatermark.Name = "textWatermark";
         this.textWatermark.Size = new System.Drawing.Size(263, 20);
         this.textWatermark.StyleController = this.layoutControl;
         this.textWatermark.TabIndex = 6;
         // 
         // buttonMoBiPath
         // 
         this.buttonMoBiPath.Location = new System.Drawing.Point(12, 28);
         this.buttonMoBiPath.Name = "buttonMoBiPath";
         this.buttonMoBiPath.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.buttonMoBiPath.Size = new System.Drawing.Size(419, 20);
         this.buttonMoBiPath.StyleController = this.layoutControl;
         this.buttonMoBiPath.TabIndex = 5;
         // 
         // gridDatabasePath
         // 
         this.gridDatabasePath.Location = new System.Drawing.Point(12, 141);
         this.gridDatabasePath.MainView = this.gridViewDatabasePath;
         this.gridDatabasePath.Name = "gridDatabasePath";
         this.gridDatabasePath.Size = new System.Drawing.Size(419, 248);
         this.gridDatabasePath.TabIndex = 4;
         this.gridDatabasePath.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDatabasePath});
         // 
         // gridViewDatabasePath
         // 
         this.gridViewDatabasePath.AllowsFiltering = true;
         this.gridViewDatabasePath.EnableColumnContextMenu = true;
         this.gridViewDatabasePath.GridControl = this.gridDatabasePath;
         this.gridViewDatabasePath.MultiSelect = false;
         this.gridViewDatabasePath.Name = "gridViewDatabasePath";
         this.gridViewDatabasePath.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridViewDatabasePath.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewDatabasePath.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewDatabasePath.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDatabasePath,
            this.layoutItemMoBiPath,
            this.layoutGroupWatermark});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "Root";
         this.layoutControlGroup1.Size = new System.Drawing.Size(443, 401);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemDatabasePath
         // 
         this.layoutItemDatabasePath.Control = this.gridDatabasePath;
         this.layoutItemDatabasePath.CustomizationFormText = "layoutItemDatabasePath";
         this.layoutItemDatabasePath.Location = new System.Drawing.Point(0, 129);
         this.layoutItemDatabasePath.Name = "layoutItemDatabasePath";
         this.layoutItemDatabasePath.Size = new System.Drawing.Size(423, 252);
         this.layoutItemDatabasePath.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDatabasePath.TextVisible = false;
         // 
         // layoutItemMoBiPath
         // 
         this.layoutItemMoBiPath.Control = this.buttonMoBiPath;
         this.layoutItemMoBiPath.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMoBiPath.Name = "layoutItemMoBiPath";
         this.layoutItemMoBiPath.Size = new System.Drawing.Size(423, 40);
         this.layoutItemMoBiPath.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemMoBiPath.TextSize = new System.Drawing.Size(96, 13);
         // 
         // layoutGroupWatermark
         // 
         this.layoutGroupWatermark.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemUseWatermark,
            this.layoutItemWatermarkText});
         this.layoutGroupWatermark.Location = new System.Drawing.Point(0, 40);
         this.layoutGroupWatermark.Name = "layoutGroupWatermark";
         this.layoutGroupWatermark.Size = new System.Drawing.Size(423, 89);
         // 
         // layoutItemUseWatermark
         // 
         this.layoutItemUseWatermark.Control = this.chkUseWatermark;
         this.layoutItemUseWatermark.Location = new System.Drawing.Point(0, 0);
         this.layoutItemUseWatermark.Name = "layoutItemUseWatermark";
         this.layoutItemUseWatermark.Size = new System.Drawing.Size(399, 23);
         this.layoutItemUseWatermark.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemUseWatermark.TextVisible = false;
         // 
         // layoutItemWatermarkText
         // 
         this.layoutItemWatermarkText.Control = this.textWatermark;
         this.layoutItemWatermarkText.Location = new System.Drawing.Point(0, 23);
         this.layoutItemWatermarkText.Name = "layoutItemWatermarkText";
         this.layoutItemWatermarkText.Size = new System.Drawing.Size(399, 24);
         this.layoutItemWatermarkText.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
         this.layoutItemWatermarkText.TextSize = new System.Drawing.Size(127, 13);
         this.layoutItemWatermarkText.TextToControlDistance = 5;
         // 
         // ApplicationSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ApplicationSettingsView";
         this.Size = new System.Drawing.Size(443, 401);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.chkUseWatermark.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.textWatermark.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.buttonMoBiPath.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridDatabasePath)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewDatabasePath)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDatabasePath)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMoBiPath)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupWatermark)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemUseWatermark)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWatermarkText)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxGridControl gridDatabasePath;
      private UxGridView gridViewDatabasePath;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDatabasePath;
      private DevExpress.XtraEditors.ButtonEdit buttonMoBiPath;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMoBiPath;
      private DevExpress.XtraEditors.CheckEdit chkUseWatermark;
      private DevExpress.XtraEditors.TextEdit textWatermark;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWatermarkText;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemUseWatermark;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupWatermark;
   }
}
