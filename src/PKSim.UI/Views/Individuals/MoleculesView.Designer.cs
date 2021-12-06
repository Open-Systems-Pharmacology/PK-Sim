using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Individuals
{
   partial class MoleculesView
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
         treeView.NodeClick -= nodeClick;
         treeView.NodeDoubleClick -= nodeDoubleClicked;
         treeView.SelectedNodeChanged -= nodeSelected;
         treeView.Dispose();
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
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.treeView = new OSPSuite.UI.Controls.UxImageTreeView();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelExpression = new DevExpress.XtraEditors.PanelControl();
         this.lblLinkedExpressionProfile = new DevExpress.XtraEditors.LabelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemLinkedExpressionProfile = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel1)).BeginInit();
         this.splitContainerControl.Panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel2)).BeginInit();
         this.splitContainerControl.Panel2.SuspendLayout();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLinkedExpressionProfile)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // splitContainerControl
         // 
         this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
         this.splitContainerControl.Name = "splitContainerControl";
         // 
         // splitContainerControl.Panel1
         // 
         this.splitContainerControl.Panel1.Controls.Add(this.treeView);
         this.splitContainerControl.Panel1.Text = "PanelEnzyme";
         // 
         // splitContainerControl.Panel2
         // 
         this.splitContainerControl.Panel2.Controls.Add(this.layoutControl);
         this.splitContainerControl.Panel2.Text = "PanelContent";
         this.splitContainerControl.Size = new System.Drawing.Size(735, 486);
         this.splitContainerControl.SplitterPosition = 184;
         this.splitContainerControl.TabIndex = 3;
         this.splitContainerControl.Text = "splitContainerControl1";
         // 
         // treeView
         // 
         this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.treeView.IsLatched = false;
         this.treeView.Location = new System.Drawing.Point(0, 0);
         this.treeView.Name = "treeView";
         this.treeView.OptionsBehavior.Editable = false;
         this.treeView.OptionsView.ShowColumns = false;
         this.treeView.OptionsView.ShowHorzLines = false;
         this.treeView.OptionsView.ShowIndicator = false;
         this.treeView.OptionsView.ShowVertLines = false;
         this.treeView.Size = new System.Drawing.Size(184, 486);
         this.treeView.TabIndex = 8;
         this.treeView.ToolTipForNode = null;
         this.treeView.UseLazyLoading = false;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelExpression);
         this.layoutControl.Controls.Add(this.lblLinkedExpressionProfile);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(541, 486);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // panelExpression
         // 
         this.panelExpression.Location = new System.Drawing.Point(4, 31);
         this.panelExpression.Name = "panelExpression";
         this.panelExpression.Size = new System.Drawing.Size(523, 451);
         this.panelExpression.TabIndex = 5;
         // 
         // lblLinkedExpressionProfile
         // 
         this.lblLinkedExpressionProfile.Location = new System.Drawing.Point(4, 14);
         this.lblLinkedExpressionProfile.Name = "lblLinkedExpressionProfile";
         this.lblLinkedExpressionProfile.Size = new System.Drawing.Size(122, 13);
         this.lblLinkedExpressionProfile.StyleController = this.layoutControl;
         this.lblLinkedExpressionProfile.TabIndex = 4;
         this.lblLinkedExpressionProfile.Text = "lblLinkedExpressionProfile";
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroup});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(541, 486);
         this.Root.TextVisible = false;
         // 
         // layoutItemLinkedExpressionProfile
         // 
         this.layoutItemLinkedExpressionProfile.Control = this.lblLinkedExpressionProfile;
         this.layoutItemLinkedExpressionProfile.Location = new System.Drawing.Point(0, 0);
         this.layoutItemLinkedExpressionProfile.Name = "layoutItemLinkedExpressionProfile";
         this.layoutItemLinkedExpressionProfile.Size = new System.Drawing.Size(527, 17);
         this.layoutItemLinkedExpressionProfile.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLinkedExpressionProfile.TextVisible = false;
         // 
         // layoutGroup
         // 
         this.layoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutGroup.GroupBordersVisible = false;
         this.layoutGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemLinkedExpressionProfile,
            this.layoutControlItem1});
         this.layoutGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutGroup.Name = "layoutGroup";
         this.layoutGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 10, 10, 0);
         this.layoutGroup.Size = new System.Drawing.Size(541, 486);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.panelExpression;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 17);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(527, 455);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // MoleculesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainerControl);
         this.Name = "MoleculesView";
         this.Size = new System.Drawing.Size(735, 486);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel1)).EndInit();
         this.splitContainerControl.Panel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel2)).EndInit();
         this.splitContainerControl.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLinkedExpressionProfile)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      protected DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private UxImageTreeView treeView;
      private UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelExpression;
      private DevExpress.XtraEditors.LabelControl lblLinkedExpressionProfile;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLinkedExpressionProfile;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
   }
}


