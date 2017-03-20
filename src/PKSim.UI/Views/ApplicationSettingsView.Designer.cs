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
         this.gridDatabasePath = new PKSim.UI.Views.Core.UxGridControl();
         this.gridViewDatabasePath = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDatabasePath = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridDatabasePath)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewDatabasePath)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDatabasePath)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.gridDatabasePath);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(443, 401);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // gridDatabasePath
         // 
         this.gridDatabasePath.Location = new System.Drawing.Point(12, 12);
         this.gridDatabasePath.MainView = this.gridViewDatabasePath;
         this.gridDatabasePath.Name = "gridDatabasePath";
         this.gridDatabasePath.Size = new System.Drawing.Size(419, 377);
         this.gridDatabasePath.TabIndex = 4;
         this.gridDatabasePath.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewDatabasePath});
         // 
         // gridViewDatabasePath
         // 
         this.gridViewDatabasePath.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridViewDatabasePath.EnableColumnContextMenu = true;
         this.gridViewDatabasePath.GridControl = this.gridDatabasePath;
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
            this.layoutItemDatabasePath});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(443, 401);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemDatabasePath
         // 
         this.layoutItemDatabasePath.Control = this.gridDatabasePath;
         this.layoutItemDatabasePath.CustomizationFormText = "layoutItemDatabasePath";
         this.layoutItemDatabasePath.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDatabasePath.Name = "layoutItemDatabasePath";
         this.layoutItemDatabasePath.Size = new System.Drawing.Size(423, 381);
         this.layoutItemDatabasePath.Text = "layoutItemDatabasePath";
         this.layoutItemDatabasePath.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDatabasePath.TextToControlDistance = 0;
         this.layoutItemDatabasePath.TextVisible = false;
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
         ((System.ComponentModel.ISupportInitialize)(this.gridDatabasePath)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewDatabasePath)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDatabasePath)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private UxGridControl gridDatabasePath;
      private UxGridView gridViewDatabasePath;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDatabasePath;
   }
}
