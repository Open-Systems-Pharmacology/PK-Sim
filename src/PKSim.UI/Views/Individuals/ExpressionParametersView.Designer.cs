namespace PKSim.UI.Views.Individuals
{
   partial class ExpressionParametersView<TExpressionParameterDTO>
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
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this._gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItemGridView = new DevExpress.XtraLayout.LayoutControlItem();
         this.chkShowInitialConcentration = new OSPSuite.UI.Controls.UxCheckEdit();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowInitialConcentration.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(2, 26);
         this.gridControl.MainView = this._gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(597, 316);
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
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.chkShowInitialConcentration);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(601, 344);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItemGridView,
            this.layoutControlItem1});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(601, 344);
         this.Root.TextVisible = false;
         // 
         // layoutControlItemGridView
         // 
         this.layoutControlItemGridView.Control = this.gridControl;
         this.layoutControlItemGridView.Location = new System.Drawing.Point(0, 24);
         this.layoutControlItemGridView.Name = "layoutControlItemGridView";
         this.layoutControlItemGridView.Size = new System.Drawing.Size(601, 320);
         this.layoutControlItemGridView.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItemGridView.TextVisible = false;
         // 
         // chkShowInitialConcentration
         // 
         this.chkShowInitialConcentration.AllowClicksOutsideControlArea = false;
         this.chkShowInitialConcentration.Location = new System.Drawing.Point(2, 2);
         this.chkShowInitialConcentration.Name = "chkShowInitialConcentration";
         this.chkShowInitialConcentration.Properties.AllowFocused = false;
         this.chkShowInitialConcentration.Properties.Caption = "chkShowInitialConcentration";
         this.chkShowInitialConcentration.Size = new System.Drawing.Size(597, 20);
         this.chkShowInitialConcentration.StyleController = this.layoutControl;
         this.chkShowInitialConcentration.TabIndex = 4;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.chkShowInitialConcentration;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(601, 24);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // ExpressionParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ExpressionParametersView";
         this.Size = new System.Drawing.Size(601, 344);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItemGridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowInitialConcentration.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxGridControl gridControl;
      protected PKSim.UI.Views.Core.UxGridView _gridView;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxCheckEdit chkShowInitialConcentration;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItemGridView;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}
