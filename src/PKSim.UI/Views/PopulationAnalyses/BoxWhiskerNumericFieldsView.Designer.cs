namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class BoxWhiskerNumericFieldsView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.panelNumericFields = new DevExpress.XtraEditors.PanelControl();
         this.chkShowOutliers = new DevExpress.XtraEditors.CheckEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemShowOutliers = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPanelNumericFields = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelNumericFields)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowOutliers.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOutliers)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelNumericFields)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.panelNumericFields);
         this.layoutControl.Controls.Add(this.chkShowOutliers);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(519, 469);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelNumericFields
         // 
         this.panelNumericFields.Location = new System.Drawing.Point(146, 23);
         this.panelNumericFields.Name = "panelNumericFields";
         this.panelNumericFields.Size = new System.Drawing.Size(373, 446);
         this.panelNumericFields.TabIndex = 5;
         // 
         // chkShowOutliers
         // 
         this.chkShowOutliers.Location = new System.Drawing.Point(2, 2);
         this.chkShowOutliers.Name = "chkShowOutliers";
         this.chkShowOutliers.Properties.Caption = "chkShowOutliers";
         this.chkShowOutliers.Size = new System.Drawing.Size(515, 19);
         this.chkShowOutliers.StyleController = this.layoutControl;
         this.chkShowOutliers.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemShowOutliers,
            this.layoutItemPanelNumericFields});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(519, 469);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemShowOutliers
         // 
         this.layoutItemShowOutliers.Control = this.chkShowOutliers;
         this.layoutItemShowOutliers.CustomizationFormText = "layoutItemShowOutliers";
         this.layoutItemShowOutliers.Location = new System.Drawing.Point(0, 0);
         this.layoutItemShowOutliers.Name = "layoutItemShowOutliers";
         this.layoutItemShowOutliers.Size = new System.Drawing.Size(519, 23);
         this.layoutItemShowOutliers.Text = "layoutItemShowOutliers";
         this.layoutItemShowOutliers.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemShowOutliers.TextToControlDistance = 0;
         this.layoutItemShowOutliers.TextVisible = false;
         // 
         // layoutItemPanelNumericFields
         // 
         this.layoutItemPanelNumericFields.Control = this.panelNumericFields;
         this.layoutItemPanelNumericFields.CustomizationFormText = "layoutItemPanelNumericFields";
         this.layoutItemPanelNumericFields.Location = new System.Drawing.Point(0, 23);
         this.layoutItemPanelNumericFields.Name = "layoutItemPanelNumericFields";
         this.layoutItemPanelNumericFields.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemPanelNumericFields.Size = new System.Drawing.Size(519, 446);
         this.layoutItemPanelNumericFields.Text = "layoutItemPanelNumericFields";
         this.layoutItemPanelNumericFields.TextSize = new System.Drawing.Size(143, 13);
         // 
         // BoxWhiskerNumericFieldsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "BoxWhiskerNumericFieldsView";
         this.Size = new System.Drawing.Size(519, 469);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelNumericFields)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowOutliers.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOutliers)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanelNumericFields)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.CheckEdit chkShowOutliers;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemShowOutliers;
      private DevExpress.XtraEditors.PanelControl panelNumericFields;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanelNumericFields;
   }
}
