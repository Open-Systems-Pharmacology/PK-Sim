namespace PKSim.UI.Views.Simulations
{
   partial class ParameterDebugView
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
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.gridParameterId = new DevExpress.XtraGrid.GridControl();
         this.gridViewParameterId = new PKSim.UI.Views.Core.UxGridView();
         this.gridView2 = new PKSim.UI.Views.Core.UxGridView();
         this.btnExport = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterId)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterId)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(493, 12);
         this.btnCancel.Size = new System.Drawing.Size(102, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(370, 12);
         this.btnOk.Size = new System.Drawing.Size(119, 22);
         // 
         // gridParameterId
         // 
         this.gridParameterId.Location = new System.Drawing.Point(12, 12);
         this.gridParameterId.MainView = this.gridViewParameterId;
         this.gridParameterId.Name = "gridParameterId";
         this.gridParameterId.Size = new System.Drawing.Size(583, 470);
         this.gridParameterId.TabIndex = 34;
         this.gridParameterId.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameterId,
            this.gridView2});
         // 
         // gridViewParameterId
         // 
         this.gridViewParameterId.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridViewParameterId.GridControl = this.gridParameterId;
         this.gridViewParameterId.Name = "gridViewParameterId";
         this.gridViewParameterId.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridViewParameterId.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewParameterId.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // gridView2
         // 
         this.gridView2.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView2.GridControl = this.gridParameterId;
         this.gridView2.Name = "gridView2";
         this.gridView2.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView2.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // btnExport
         // 
         this.btnExport.Location = new System.Drawing.Point(12, 486);
         this.btnExport.Name = "btnExport";
         this.btnExport.Size = new System.Drawing.Size(80, 22);
         this.btnExport.StyleController = this.layoutControl1;
         this.btnExport.TabIndex = 35;
         this.btnExport.Text = "Export";
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.gridParameterId);
         this.layoutControl1.Controls.Add(this.btnExport);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(607, 520);
         this.layoutControl1.TabIndex = 36;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(607, 520);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.btnExport;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 474);
         this.layoutControlItem1.MaxSize = new System.Drawing.Size(84, 26);
         this.layoutControlItem1.MinSize = new System.Drawing.Size(84, 26);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(84, 26);
         this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.gridParameterId;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(587, 474);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(84, 474);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(503, 26);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // ParameterDebugView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ParameterDebugView";
         this.ClientSize = new System.Drawing.Size(607, 566);
         this.Controls.Add(this.layoutControl1);
         this.Name = "ParameterDebugView";
         this.Text = "ParameterDebugView";
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterId)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterId)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraGrid.GridControl gridParameterId;
      private PKSim.UI.Views.Core.UxGridView gridViewParameterId;
      private PKSim.UI.Views.Core.UxGridView gridView2;
      private DevExpress.XtraEditors.SimpleButton btnExport;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;

   }
}