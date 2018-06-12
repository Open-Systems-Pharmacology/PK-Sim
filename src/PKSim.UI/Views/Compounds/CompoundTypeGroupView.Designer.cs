using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Compounds
{
   partial class CompoundTypeGroupView
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
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemCompoundTypes = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelValueOrigin = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemValueOrigin = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCompoundTypes)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelValueOrigin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).BeginInit();
         this.SuspendLayout();
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 2);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(409, 104);
         this.gridControl.TabIndex = 0;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = false;
         this.gridView.Name = "gridView";
         this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelValueOrigin);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(413, 138);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCompoundTypes,
            this.layoutItemValueOrigin});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(413, 138);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutItemCompoundTypes.Control = this.gridControl;
         this.layoutItemCompoundTypes.Location = new System.Drawing.Point(0, 0);
         this.layoutItemCompoundTypes.Name = "layoutItemCompoundTypes";
         this.layoutItemCompoundTypes.Size = new System.Drawing.Size(413, 108);
         this.layoutItemCompoundTypes.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCompoundTypes.TextVisible = false;
         // 
         // panelValueOrigin
         // 
         this.panelValueOrigin.Location = new System.Drawing.Point(2, 110);
         this.panelValueOrigin.Name = "panelValueOrigin";
         this.panelValueOrigin.Size = new System.Drawing.Size(409, 26);
         this.panelValueOrigin.TabIndex = 3;
         // 
         // layoutControlItem2
         // 
         this.layoutItemValueOrigin.Control = this.panelValueOrigin;
         this.layoutItemValueOrigin.Location = new System.Drawing.Point(0, 108);
         this.layoutItemValueOrigin.Name = "layoutItemValueOrigin";
         this.layoutItemValueOrigin.Size = new System.Drawing.Size(413, 30);
         this.layoutItemValueOrigin.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemValueOrigin.TextVisible = false;
         // 
         // CompoundTypeGroupView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "CompoundTypeGroupView";
         this.Size = new System.Drawing.Size(413, 138);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCompoundTypes)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelValueOrigin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private UxGridView gridView;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelValueOrigin;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCompoundTypes;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemValueOrigin;
   }
}
