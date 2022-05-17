using PKSim.UI.Views.Core;
using OSPSuite.UI.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views
{
   partial class TemplateView
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
         this.chkShowQualifiedTemplate = new DevExpress.XtraEditors.CheckEdit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowQualifiedTemplate.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // tablePanel
         // 
         this.tablePanel.Controls.Add(this.chkShowQualifiedTemplate);
         this.tablePanel.Location = new System.Drawing.Point(0, 724);
         this.tablePanel.Size = new System.Drawing.Size(867, 43);
         this.tablePanel.Controls.SetChildIndex(this.chkShowQualifiedTemplate, 0);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutMainGroup;
         this.layoutControl.Size = new System.Drawing.Size(867, 724);
         this.layoutControl.TabIndex = 34;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(11, 10);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 5;
         this.lblDescription.Text = "lblDescription";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(11, 27);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(845, 687);
         this.gridControl.TabIndex = 4;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.AllowsFiltering = true;
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.MultiSelect = false;
         this.gridView.Name = "gridView";
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutMainGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutMainGroup";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutItemDescription});
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Size = new System.Drawing.Size(867, 724);
         this.layoutMainGroup.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.gridControl;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 17);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(849, 691);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(849, 17);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // chkShowQualifiedTemplate
         // 
         this.tablePanel.SetColumn(this.chkShowQualifiedTemplate, 1);
         this.chkShowQualifiedTemplate.Location = new System.Drawing.Point(236, 11);
         this.chkShowQualifiedTemplate.Name = "chkShowQualifiedTemplate";
         this.chkShowQualifiedTemplate.Properties.Caption = "chkShowQualifiedTemplate";
         this.tablePanel.SetRow(this.chkShowQualifiedTemplate, 0);
         this.chkShowQualifiedTemplate.Size = new System.Drawing.Size(205, 20);
         this.chkShowQualifiedTemplate.TabIndex = 3;
         // 
         // TemplateView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "BuildingBlockFromTemplateView";
         this.ClientSize = new System.Drawing.Size(867, 767);
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.Name = "TemplateView";
         this.Text = "BuildingBlockFromTemplateView";
         this.Controls.SetChildIndex(this.tablePanel, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowQualifiedTemplate.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private DevExpress.Utils.ToolTipController toolTipController;
      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraEditors.CheckEdit chkShowQualifiedTemplate;
   }
}