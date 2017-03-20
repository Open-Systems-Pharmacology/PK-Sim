using PKSim.UI.Views.Core;
using OSPSuite.UI.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views
{
   partial class BuildingBlockFromTemplateView
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
         this.components = new System.ComponentModel.Container();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.treeView = new OSPSuite.UI.Controls.UxImageTreeView();
         this.tbDescription = new DevExpress.XtraEditors.MemoEdit();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(340, 12);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(255, 12);
         this.btnOk.Size = new System.Drawing.Size(81, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 402);
         this.layoutControlBase.Size = new System.Drawing.Size(419, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(118, 22);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.splitContainerControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutMainGroup;
         this.layoutControl.Size = new System.Drawing.Size(419, 402);
         this.layoutControl.TabIndex = 34;
         this.layoutControl.Text = "layoutControl1";
         // 
         // splitContainerControl
         // 
         this.splitContainerControl.Horizontal = false;
         this.splitContainerControl.Location = new System.Drawing.Point(12, 12);
         this.splitContainerControl.Name = "splitContainerControl";
         this.splitContainerControl.Panel1.Controls.Add(this.treeView);
         this.splitContainerControl.Panel1.Text = "Panel1";
         this.splitContainerControl.Panel2.Controls.Add(this.tbDescription);
         this.splitContainerControl.Panel2.Text = "Panel2";
         this.splitContainerControl.Size = new System.Drawing.Size(395, 378);
         this.splitContainerControl.SplitterPosition = 192;
         this.splitContainerControl.TabIndex = 5;
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
         this.treeView.Size = new System.Drawing.Size(395, 192);
         this.treeView.TabIndex = 1;
         this.treeView.ToolTipForNode = null;
         this.treeView.UseLazyLoading = false;
         // 
         // tbDescription
         // 
         this.tbDescription.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tbDescription.Location = new System.Drawing.Point(0, 0);
         this.tbDescription.Name = "tbDescription";
         this.tbDescription.Size = new System.Drawing.Size(395, 181);
         this.tbDescription.TabIndex = 0;
         // 
         // layoutMainGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutMainGroup";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2});
         this.layoutMainGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Size = new System.Drawing.Size(419, 402);
         this.layoutMainGroup.Text = "layoutMainGroup";
         this.layoutMainGroup.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.splitContainerControl;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(399, 382);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // BuildingBlockFromTemplateView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "BuildingBlockFromTemplateView";
         this.ClientSize = new System.Drawing.Size(419, 448);
         this.Controls.Add(this.layoutControl);
         this.Name = "BuildingBlockFromTemplateView";
         this.Text = "BuildingBlockFromTemplateView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private DevExpress.Utils.ToolTipController toolTipController;
      private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private UxImageTreeView treeView;
      private DevExpress.XtraEditors.MemoEdit tbDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
   }
}