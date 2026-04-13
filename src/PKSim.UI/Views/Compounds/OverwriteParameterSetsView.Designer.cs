namespace PKSim.UI.Views.Compounds
{
   partial class OverwriteParameterSetsView
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
         _gridViewBinderSets?.Dispose();
         _gridViewBinderParameterValues?.Dispose();
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
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.gridSets = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewSets = new PKSim.UI.Views.Core.UxGridView();
         this.gridParameterValues = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameterValues = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridSets)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewSets)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValues)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValues)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem)).BeginInit();
         this.SuspendLayout();
         //
         // layoutControl
         //
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.splitContainerControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(700, 450);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl";
         //
         // splitContainerControl
         //
         this.splitContainerControl.Location = new System.Drawing.Point(12, 12);
         this.splitContainerControl.Name = "splitContainerControl";
         this.splitContainerControl.Panel1.Controls.Add(this.gridSets);
         this.splitContainerControl.Panel1.Text = "Panel1";
         this.splitContainerControl.Panel2.Controls.Add(this.gridParameterValues);
         this.splitContainerControl.Panel2.Text = "Panel2";
         this.splitContainerControl.Size = new System.Drawing.Size(676, 426);
         this.splitContainerControl.SplitterPosition = 300;
         this.splitContainerControl.TabIndex = 0;
         this.splitContainerControl.Text = "splitContainerControl";
         //
         // gridSets
         //
         this.gridSets.Dock = System.Windows.Forms.DockStyle.Fill;
         this.gridSets.Location = new System.Drawing.Point(0, 0);
         this.gridSets.MainView = this.gridViewSets;
         this.gridSets.Name = "gridSets";
         this.gridSets.Size = new System.Drawing.Size(300, 426);
         this.gridSets.TabIndex = 0;
         this.gridSets.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gridViewSets });
         //
         // gridViewSets
         //
         this.gridViewSets.AllowsFiltering = true;
         this.gridViewSets.EnableColumnContextMenu = true;
         this.gridViewSets.GridControl = this.gridSets;
         this.gridViewSets.Name = "gridViewSets";
         this.gridViewSets.OptionsBehavior.Editable = false;
         this.gridViewSets.OptionsSelection.EnableAppearanceFocusedRow = true;
         //
         // gridParameterValues
         //
         this.gridParameterValues.Dock = System.Windows.Forms.DockStyle.Fill;
         this.gridParameterValues.Location = new System.Drawing.Point(0, 0);
         this.gridParameterValues.MainView = this.gridViewParameterValues;
         this.gridParameterValues.Name = "gridParameterValues";
         this.gridParameterValues.Size = new System.Drawing.Size(371, 426);
         this.gridParameterValues.TabIndex = 0;
         this.gridParameterValues.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gridViewParameterValues });
         //
         // gridViewParameterValues
         //
         this.gridViewParameterValues.AllowsFiltering = true;
         this.gridViewParameterValues.EnableColumnContextMenu = true;
         this.gridViewParameterValues.GridControl = this.gridParameterValues;
         this.gridViewParameterValues.Name = "gridViewParameterValues";
         this.gridViewParameterValues.OptionsBehavior.Editable = false;
         //
         // layoutControlGroup
         //
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(700, 450);
         this.layoutControlGroup.TextVisible = false;
         //
         // layoutControlItem
         //
         this.layoutControlItem.Control = this.splitContainerControl;
         this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem.Name = "layoutControlItem";
         this.layoutControlItem.Size = new System.Drawing.Size(680, 430);
         this.layoutControlItem.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem.TextToControlDistance = 0;
         this.layoutControlItem.TextVisible = false;
         //
         // OverwriteParameterSetsView
         //
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "OverwriteParameterSetsView";
         this.Size = new System.Drawing.Size(700, 450);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridSets)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewSets)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValues)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValues)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem)).EndInit();
         this.ResumeLayout(false);
      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private OSPSuite.UI.Controls.UxGridControl gridSets;
      private PKSim.UI.Views.Core.UxGridView gridViewSets;
      private OSPSuite.UI.Controls.UxGridControl gridParameterValues;
      private PKSim.UI.Views.Core.UxGridView gridViewParameterValues;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem;
   }
}
