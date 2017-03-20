using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Parameters
{
   partial class ParameterGroupsView
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
         _treeView.SelectedNodeChanging -= activateNode;
         _groupModeBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.layoutControlTree = new OSPSuite.UI.Controls.UxLayoutControl();
         this._filterTreeView = new OSPSuite.UI.Controls.FilterTreeView();
         this.cbGroupingMode = new DevExpress.XtraEditors.ComboBoxEdit();
         this.layoutGroupTrees = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemGroupingType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.groupParameters = new DevExpress.XtraEditors.GroupControl();
         this._toolTipController = new DevExpress.Utils.ToolTipController(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlTree)).BeginInit();
         this.layoutControlTree.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbGroupingMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupTrees)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupingType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.groupParameters)).BeginInit();
         this.SuspendLayout();
         // 
         // splitContainer
         // 
         this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer.Location = new System.Drawing.Point(0, 0);
         this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.Controls.Add(this.layoutControlTree);
         this.splitContainer.Panel1.Text = "Panel1";
         this.splitContainer.Panel2.Controls.Add(this.groupParameters);
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(695, 533);
         this.splitContainer.SplitterPosition = 185;
         this.splitContainer.TabIndex = 1;
         this.splitContainer.Text = "splitContainerControl1";
         // 
         // layoutControlTree
         // 
         this.layoutControlTree.AllowCustomization = false;
         this.layoutControlTree.Controls.Add(this._filterTreeView);
         this.layoutControlTree.Controls.Add(this.cbGroupingMode);
         this.layoutControlTree.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlTree.Location = new System.Drawing.Point(0, 0);
         this.layoutControlTree.Name = "layoutControlTree";
         this.layoutControlTree.Root = this.layoutGroupTrees;
         this.layoutControlTree.Size = new System.Drawing.Size(185, 533);
         this.layoutControlTree.TabIndex = 1;
         this.layoutControlTree.Text = "layoutControlTree";
         // 
         // _filterTreeView
         // 
         this._filterTreeView.Location = new System.Drawing.Point(3, 3);
         this._filterTreeView.Name = "_filterTreeView";
         this._filterTreeView.ShowDescendantNode = true;
         this._filterTreeView.Size = new System.Drawing.Size(179, 503);
         this._filterTreeView.TabIndex = 7;
         // 
         // cbGroupingMode
         // 
         this.cbGroupingMode.Location = new System.Drawing.Point(3, 510);
         this.cbGroupingMode.Name = "cbGroupingMode";
         this.cbGroupingMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGroupingMode.Size = new System.Drawing.Size(179, 20);
         this.cbGroupingMode.StyleController = this.layoutControlTree;
         this.cbGroupingMode.TabIndex = 6;
         // 
         // layoutGroupTrees
         // 
         this.layoutGroupTrees.CustomizationFormText = "layoutGroupTrees";
         this.layoutGroupTrees.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemGroupingType,
            this.layoutControlItem1});
         this.layoutGroupTrees.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupTrees.Name = "layoutControlGroup1";
         this.layoutGroupTrees.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutGroupTrees.Size = new System.Drawing.Size(185, 533);
         this.layoutGroupTrees.Text = "layoutGroupTrees";
         this.layoutGroupTrees.TextVisible = false;
         // 
         // layoutItemGroupingType
         // 
         this.layoutItemGroupingType.Control = this.cbGroupingMode;
         this.layoutItemGroupingType.CustomizationFormText = "layoutItemGroupingType";
         this.layoutItemGroupingType.Location = new System.Drawing.Point(0, 507);
         this.layoutItemGroupingType.Name = "layoutItemGroupingType";
         this.layoutItemGroupingType.Size = new System.Drawing.Size(183, 24);
         this.layoutItemGroupingType.Text = "layoutItemGroupingType";
         this.layoutItemGroupingType.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGroupingType.TextToControlDistance = 0;
         this.layoutItemGroupingType.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this._filterTreeView;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(183, 507);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // groupParameters
         // 
         this.groupParameters.Dock = System.Windows.Forms.DockStyle.Fill;
         this.groupParameters.Location = new System.Drawing.Point(0, 0);
         this.groupParameters.Name = "groupParameters";
         this.groupParameters.Padding = new System.Windows.Forms.Padding(10);
         this.groupParameters.Size = new System.Drawing.Size(505, 533);
         this.groupParameters.TabIndex = 3;
         this.groupParameters.Text = "groupParameters";
         // 
         // ParameterGroupsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainer);
         this.Name = "ParameterGroupsView";
         this.Size = new System.Drawing.Size(695, 533);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlTree)).EndInit();
         this.layoutControlTree.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbGroupingMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupTrees)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupingType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.groupParameters)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private DevExpress.XtraEditors.GroupControl groupParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupTrees;
      private DevExpress.XtraEditors.ComboBoxEdit cbGroupingMode;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGroupingType;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControlTree;
      private DevExpress.Utils.ToolTipController _toolTipController;
      private FilterTreeView _filterTreeView;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}


