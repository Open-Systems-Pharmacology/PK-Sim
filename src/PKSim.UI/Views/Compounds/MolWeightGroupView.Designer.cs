using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Compounds
{
   partial class MolWeightGroupView
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
         _presenter = null;
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
         this._gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this._panelValueOrigin = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemMolWeight = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemValueOrigin = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._panelValueOrigin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMolWeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).BeginInit();
         this.SuspendLayout();
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 2);
         this.gridControl.MainView = this._gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(542, 105);
         this.gridControl.TabIndex = 0;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this._gridView});
         // 
         // _gridView
         // 
         this._gridView.AllowsFiltering = true;
         this._gridView.EnableColumnContextMenu = true;
         this._gridView.GridControl = this.gridControl;
         this._gridView.MultiSelect = false;
         this._gridView.Name = "_gridView";
         this._gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this._gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this._panelValueOrigin);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(546, 155);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // _panelValueOrigin
         // 
         this._panelValueOrigin.Location = new System.Drawing.Point(2, 111);
         this._panelValueOrigin.Name = "_panelValueOrigin";
         this._panelValueOrigin.Size = new System.Drawing.Size(542, 42);
         this._panelValueOrigin.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemMolWeight,
            this.layoutItemValueOrigin});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(546, 155);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemMolWeight
         // 
         this.layoutItemMolWeight.Control = this.gridControl;
         this.layoutItemMolWeight.Location = new System.Drawing.Point(0, 0);
         this.layoutItemMolWeight.Name = "layoutItemMolWeight";
         this.layoutItemMolWeight.Size = new System.Drawing.Size(546, 109);
         this.layoutItemMolWeight.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemMolWeight.TextVisible = false;
         // 
         // layoutItemValueOrigin
         // 
         this.layoutItemValueOrigin.Control = this._panelValueOrigin;
         this.layoutItemValueOrigin.Location = new System.Drawing.Point(0, 109);
         this.layoutItemValueOrigin.Name = "layoutItemValueOrigin";
         this.layoutItemValueOrigin.Size = new System.Drawing.Size(546, 46);
         this.layoutItemValueOrigin.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemValueOrigin.TextVisible = false;
         // 
         // MolWeightGroupView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "MolWeightGroupView";
         this.Size = new System.Drawing.Size(546, 155);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._panelValueOrigin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMolWeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private UxGridView _gridView;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl _panelValueOrigin;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemMolWeight;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemValueOrigin;
   }
}
