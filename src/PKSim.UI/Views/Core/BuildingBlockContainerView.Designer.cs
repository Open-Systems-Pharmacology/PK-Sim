namespace PKSim.UI.Views.Core
{
   partial class BuildingBlockContainerView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panel = new DevExpress.XtraEditors.PanelControl();
         this.tbName = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPanel = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanel)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panel);
         this.layoutControl.Controls.Add(this.tbName);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(457, 514);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panel
         // 
         this.panel.Location = new System.Drawing.Point(10, 34);
         this.panel.Name = "panel";
         this.panel.Size = new System.Drawing.Size(437, 470);
         this.panel.TabIndex = 5;
         // 
         // tbName
         // 
         this.tbName.Location = new System.Drawing.Point(94, 12);
         this.tbName.Name = "tbName";
         this.tbName.Size = new System.Drawing.Size(351, 20);
         this.tbName.StyleController = this.layoutControl;
         this.tbName.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemName,
            this.layoutItemPanel});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(457, 514);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemName
         // 
         this.layoutItemName.Control = this.tbName;
         this.layoutItemName.CustomizationFormText = "layoutItemName";
         this.layoutItemName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemName.Name = "layoutItemName";
         this.layoutItemName.Size = new System.Drawing.Size(437, 24);
         this.layoutItemName.Text = "layoutItemName";
         this.layoutItemName.TextSize = new System.Drawing.Size(79, 13);
         // 
         // layoutItemPanel
         // 
         this.layoutItemPanel.Control = this.panel;
         this.layoutItemPanel.CustomizationFormText = "layoutItemPanel";
         this.layoutItemPanel.Location = new System.Drawing.Point(0, 24);
         this.layoutItemPanel.Name = "layoutItemPanel";
         this.layoutItemPanel.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemPanel.Size = new System.Drawing.Size(437, 470);
         this.layoutItemPanel.Text = "layoutItemPanel";
         this.layoutItemPanel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPanel.TextToControlDistance = 0;
         this.layoutItemPanel.TextVisible = false;
         // 
         // BuildingBlockContainerView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "BuildingBlockContainerView";
         this.ClientSize = new System.Drawing.Size(457, 560);
         this.Controls.Add(this.layoutControl);
         this.Name = "BuildingBlockContainerView";
         this.Text = "BuildingBlockContainerView";
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanel)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.TextEdit tbName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemName;
      private DevExpress.XtraEditors.PanelControl panel;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanel;
   }
}