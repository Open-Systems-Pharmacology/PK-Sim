namespace PKSim.UI.Views.ProteinExpression
{
   partial class TransferView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
         this.grdTransfer = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView1 = new PKSim.UI.Views.Core.UxGridView();
         this.gridView2 = new PKSim.UI.Views.Core.UxGridView();
         this.radioGroup = new DevExpress.XtraEditors.RadioGroup();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.grdTransfer)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroup.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // grdTransfer
         // 
         this.grdTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         gridLevelNode1.RelationName = "Level1";
         this.grdTransfer.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
         this.grdTransfer.Location = new System.Drawing.Point(0, 31);
         this.grdTransfer.MainView = this.gridView1;
         this.grdTransfer.Name = "grdTransfer";
         this.grdTransfer.Size = new System.Drawing.Size(401, 361);
         this.grdTransfer.TabIndex = 1;
         this.grdTransfer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1,
            this.gridView2});
         // 
         // gridView1
         // 
         this.gridView1.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView1.EnableColumnContextMenu = true;
         this.gridView1.GridControl = this.grdTransfer;
         this.gridView1.Name = "gridView1";
         this.gridView1.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView1.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // gridView2
         // 
         this.gridView2.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView2.EnableColumnContextMenu = true;
         this.gridView2.GridControl = this.grdTransfer;
         this.gridView2.Name = "gridView2";
         this.gridView2.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView2.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView2.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // radioGroup
         // 
         this.radioGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.radioGroup.Location = new System.Drawing.Point(0, 0);
         this.radioGroup.Name = "radioGroup";
         this.radioGroup.Size = new System.Drawing.Size(401, 33);
         this.radioGroup.TabIndex = 2;
         // 
         // TransferView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.radioGroup);
         this.Controls.Add(this.grdTransfer);
         this.Name = "TransferView";
         this.Size = new System.Drawing.Size(401, 392);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.grdTransfer)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroup.Properties)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraGrid.GridControl grdTransfer;
      private PKSim.UI.Views.Core.UxGridView gridView2;
      private PKSim.UI.Views.Core.UxGridView gridView1;
      private DevExpress.XtraEditors.RadioGroup radioGroup;
   }
}
