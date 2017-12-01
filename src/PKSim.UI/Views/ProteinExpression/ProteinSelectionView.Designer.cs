namespace PKSim.UI.Views.ProteinExpression
{
   partial class ProteinSelectionView
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
         this.txtSearchCriteria = new DevExpress.XtraEditors.TextEdit();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.grdSelection = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView3 = new PKSim.UI.Views.Core.UxGridView();
         this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSearch = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSearchCriteria = new DevExpress.XtraLayout.LayoutControlItem();
         this.gridView1 = new PKSim.UI.Views.Core.UxGridView();
         this.gridView2 = new PKSim.UI.Views.Core.UxGridView();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.txtSearchCriteria.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.grdSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSearch)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSearchCriteria)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView2)).BeginInit();
         this.SuspendLayout();
         // 
         // txtSearchCriteria
         // 
         this.txtSearchCriteria.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtSearchCriteria.Location = new System.Drawing.Point(126, 2);
         this.txtSearchCriteria.Name = "txtSearchCriteria";
         this.txtSearchCriteria.Size = new System.Drawing.Size(307, 20);
         this.txtSearchCriteria.StyleController = this.layoutControl;
         this.txtSearchCriteria.TabIndex = 0;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.grdSelection);
         this.layoutControl.Controls.Add(this.btnSearch);
         this.layoutControl.Controls.Add(this.txtSearchCriteria);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(873, 555);
         this.layoutControl.TabIndex = 5;
         this.layoutControl.Text = "layoutControl1";
         // 
         // grdSelection
         // 
         this.grdSelection.Anchor = System.Windows.Forms.AnchorStyles.None;
         this.grdSelection.Location = new System.Drawing.Point(2, 28);
         this.grdSelection.MainView = this.gridView3;
         this.grdSelection.Name = "grdSelection";
         this.grdSelection.Size = new System.Drawing.Size(869, 525);
         this.grdSelection.TabIndex = 4;
         this.grdSelection.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView3});
         // 
         // gridView3
         // 
         this.gridView3.EnableColumnContextMenu = true;
         this.gridView3.GridControl = this.grdSelection;
         this.gridView3.Name = "gridView3";
         this.gridView3.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView3.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView3.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // btnSearch
         // 
         this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.btnSearch.Location = new System.Drawing.Point(437, 2);
         this.btnSearch.Name = "btnSearch";
         this.btnSearch.Size = new System.Drawing.Size(434, 22);
         this.btnSearch.StyleController = this.layoutControl;
         this.btnSearch.TabIndex = 2;
         this.btnSearch.Text = "#Search";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSelection,
            this.layoutItemSearch,
            this.layoutItemSearchCriteria});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(873, 555);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemSelection
         // 
         this.layoutItemSelection.Control = this.grdSelection;
         this.layoutItemSelection.CustomizationFormText = "layoutControlItem1";
         this.layoutItemSelection.Location = new System.Drawing.Point(0, 26);
         this.layoutItemSelection.Name = "layoutItemSelection";
         this.layoutItemSelection.Size = new System.Drawing.Size(873, 529);
         this.layoutItemSelection.Text = "layoutItemSelection";
         this.layoutItemSelection.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSelection.TextToControlDistance = 0;
         this.layoutItemSelection.TextVisible = false;
         // 
         // layoutItemSearch
         // 
         this.layoutItemSearch.Control = this.btnSearch;
         this.layoutItemSearch.CustomizationFormText = "layoutItemSearch";
         this.layoutItemSearch.Location = new System.Drawing.Point(435, 0);
         this.layoutItemSearch.Name = "layoutItemSearch";
         this.layoutItemSearch.Size = new System.Drawing.Size(438, 26);
         this.layoutItemSearch.Text = "layoutItemSearch";
         this.layoutItemSearch.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSearch.TextToControlDistance = 0;
         this.layoutItemSearch.TextVisible = false;
         // 
         // layoutItemSearchCriteria
         // 
         this.layoutItemSearchCriteria.Control = this.txtSearchCriteria;
         this.layoutItemSearchCriteria.CustomizationFormText = "layoutItemSearchCriteria";
         this.layoutItemSearchCriteria.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSearchCriteria.Name = "layoutItemSearchCriteria";
         this.layoutItemSearchCriteria.Size = new System.Drawing.Size(435, 26);
         this.layoutItemSearchCriteria.Text = "layoutItemSearchCriteria";
         this.layoutItemSearchCriteria.TextSize = new System.Drawing.Size(120, 13);
         // 
         // gridView1
         // 
         this.gridView1.EnableColumnContextMenu = true;
         this.gridView1.Name = "gridView1";
         this.gridView1.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView1.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // gridView2
         // 
         this.gridView2.EnableColumnContextMenu = true;
         this.gridView2.Name = "gridView2";
         this.gridView2.OptionsNavigation.AutoFocusNewRow = true;
         this.gridView2.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView2.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // ProteinSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "ProteinSelectionView";
         this.Size = new System.Drawing.Size(873, 555);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.txtSearchCriteria.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.grdSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSearch)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSearchCriteria)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView2)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.TextEdit txtSearchCriteria;
      private DevExpress.XtraEditors.SimpleButton btnSearch;
      private DevExpress.XtraGrid.GridControl grdSelection;
      private PKSim.UI.Views.Core.UxGridView gridView1;
      private PKSim.UI.Views.Core.UxGridView gridView2;
      private PKSim.UI.Views.Core.UxGridView gridView3;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSearch;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSearchCriteria;
   }
}
