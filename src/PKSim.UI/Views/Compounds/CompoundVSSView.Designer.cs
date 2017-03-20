namespace PKSim.UI.Views.Compounds
{
   partial class CompoundVSSView
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
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.btnCalculateVSS = new DevExpress.XtraEditors.SimpleButton();
         this.layoutItemCalculateVSS = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.gridControl = new DevExpress.XtraGrid.GridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCalculateVSS)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Controls.Add(this.btnCalculateVSS);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(372, 191);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemCalculateVSS,
            this.emptySpaceItem,
            this.layoutItemGrid});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(372, 191);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // btnCalculateVSS
         // 
         this.btnCalculateVSS.Location = new System.Drawing.Point(188, 2);
         this.btnCalculateVSS.Name = "btnCalculateVSS";
         this.btnCalculateVSS.Size = new System.Drawing.Size(182, 22);
         this.btnCalculateVSS.StyleController = this.layoutControl;
         this.btnCalculateVSS.TabIndex = 4;
         this.btnCalculateVSS.Text = "simpleButton1";
         // 
         // layoutItemCalculateVSS
         // 
         this.layoutItemCalculateVSS.Control = this.btnCalculateVSS;
         this.layoutItemCalculateVSS.CustomizationFormText = "layoutItemCalculateVSS";
         this.layoutItemCalculateVSS.Location = new System.Drawing.Point(186, 0);
         this.layoutItemCalculateVSS.Name = "layoutItemCalculateVSS";
         this.layoutItemCalculateVSS.Size = new System.Drawing.Size(186, 26);
         this.layoutItemCalculateVSS.Text = "layoutItemCalculateVSS";
         this.layoutItemCalculateVSS.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCalculateVSS.TextToControlDistance = 0;
         this.layoutItemCalculateVSS.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.CustomizationFormText = "emptySpaceItem";
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(186, 26);
         this.emptySpaceItem.Text = "emptySpaceItem";
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // gridControl
         // 
         this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
         this.gridControl.Location = new System.Drawing.Point(77, 28);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(293, 161);
         this.gridControl.TabIndex = 5;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.GridControl = this.gridControl;
         this.gridView.Name = "gridView";
         // 
         // layoutItemGrid
         // 
         this.layoutItemGrid.Control = this.gridControl;
         this.layoutItemGrid.CustomizationFormText = "layoutItemGrid";
         this.layoutItemGrid.Location = new System.Drawing.Point(0, 26);
         this.layoutItemGrid.Name = "layoutItemGrid";
         this.layoutItemGrid.Size = new System.Drawing.Size(372, 165);
         this.layoutItemGrid.Text = "layoutItemGrid";
         this.layoutItemGrid.TextSize = new System.Drawing.Size(71, 13);
         // 
         // CompoundVSSView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "CompoundVSSView";
         this.Size = new System.Drawing.Size(372, 191);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCalculateVSS)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraGrid.GridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraEditors.SimpleButton btnCalculateVSS;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCalculateVSS;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGrid;
   }
}
