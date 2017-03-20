namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class PopulationAnalysisChartSettingsView
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
         this._layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this._pnlChartSettings = new DevExpress.XtraEditors.PanelControl();
         this._btnEdit = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this._layoutControlItemEdit = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.tabbedControlGroup1 = new DevExpress.XtraLayout.TabbedControlGroup();
         this.chartSettingsLayoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this._layoutControlItemChartSettings = new DevExpress.XtraLayout.LayoutControlItem();
         this.chartExportSettingsLayoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this._pnlChartExportSettings = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._layoutControl)).BeginInit();
         this._layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pnlChartSettings)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._layoutControlItemEdit)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartSettingsLayoutGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._layoutControlItemChartSettings)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartExportSettingsLayoutGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._pnlChartExportSettings)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // _layoutControl
         // 
         this._layoutControl.AllowCustomization = false;
         this._layoutControl.Controls.Add(this._pnlChartExportSettings);
         this._layoutControl.Controls.Add(this._pnlChartSettings);
         this._layoutControl.Controls.Add(this._btnEdit);
         this._layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this._layoutControl.Location = new System.Drawing.Point(0, 0);
         this._layoutControl.Name = "_layoutControl";
         this._layoutControl.Root = this.layoutControlGroup1;
         this._layoutControl.Size = new System.Drawing.Size(360, 267);
         this._layoutControl.TabIndex = 0;
         this._layoutControl.Text = "_layoutControl";
         // 
         // _pnlChartSettings
         // 
         this._pnlChartSettings.Location = new System.Drawing.Point(24, 46);
         this._pnlChartSettings.Name = "_pnlChartSettings";
         this._pnlChartSettings.Size = new System.Drawing.Size(312, 171);
         this._pnlChartSettings.TabIndex = 5;
         // 
         // _btnEdit
         // 
         this._btnEdit.Location = new System.Drawing.Point(12, 233);
         this._btnEdit.Name = "_btnEdit";
         this._btnEdit.Size = new System.Drawing.Size(166, 22);
         this._btnEdit.StyleController = this._layoutControl;
         this._btnEdit.TabIndex = 4;
         this._btnEdit.Text = "_btnEdit";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this._layoutControlItemEdit,
            this.emptySpaceItem1,
            this.tabbedControlGroup1});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(360, 267);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // _layoutControlItemEdit
         // 
         this._layoutControlItemEdit.Control = this._btnEdit;
         this._layoutControlItemEdit.CustomizationFormText = "layoutControlItem1";
         this._layoutControlItemEdit.Location = new System.Drawing.Point(0, 221);
         this._layoutControlItemEdit.Name = "_layoutControlItemEdit";
         this._layoutControlItemEdit.Size = new System.Drawing.Size(170, 26);
         this._layoutControlItemEdit.TextSize = new System.Drawing.Size(0, 0);
         this._layoutControlItemEdit.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(170, 221);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(170, 26);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // tabbedControlGroup1
         // 
         this.tabbedControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.tabbedControlGroup1.Name = "tabbedControlGroup1";
         this.tabbedControlGroup1.SelectedTabPage = this.chartSettingsLayoutGroup;
         this.tabbedControlGroup1.SelectedTabPageIndex = 0;
         this.tabbedControlGroup1.Size = new System.Drawing.Size(340, 221);
         this.tabbedControlGroup1.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.chartSettingsLayoutGroup,
            this.chartExportSettingsLayoutGroup});
         // 
         // chartSettingsLayoutGroup
         // 
         this.chartSettingsLayoutGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this._layoutControlItemChartSettings});
         this.chartSettingsLayoutGroup.Location = new System.Drawing.Point(0, 0);
         this.chartSettingsLayoutGroup.Name = "chartSettingsLayoutGroup";
         this.chartSettingsLayoutGroup.Size = new System.Drawing.Size(316, 175);
         // 
         // _layoutControlItemChartSettings
         // 
         this._layoutControlItemChartSettings.Control = this._pnlChartSettings;
         this._layoutControlItemChartSettings.CustomizationFormText = "layoutControlItem2";
         this._layoutControlItemChartSettings.Location = new System.Drawing.Point(0, 0);
         this._layoutControlItemChartSettings.Name = "_layoutControlItemChartSettings";
         this._layoutControlItemChartSettings.Size = new System.Drawing.Size(316, 175);
         this._layoutControlItemChartSettings.TextSize = new System.Drawing.Size(0, 0);
         this._layoutControlItemChartSettings.TextVisible = false;
         // 
         // chartExportSettingsLayoutGroup
         // 
         this.chartExportSettingsLayoutGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
         this.chartExportSettingsLayoutGroup.Location = new System.Drawing.Point(0, 0);
         this.chartExportSettingsLayoutGroup.Name = "chartExportSettingsLayoutGroup";
         this.chartExportSettingsLayoutGroup.Size = new System.Drawing.Size(316, 175);
         this.chartExportSettingsLayoutGroup.Text = "chartExportSettingsLayoutGroup";
         // 
         // _pnlChartExportSettings
         // 
         this._pnlChartExportSettings.Location = new System.Drawing.Point(24, 46);
         this._pnlChartExportSettings.Name = "_pnlChartExportSettings";
         this._pnlChartExportSettings.Size = new System.Drawing.Size(312, 171);
         this._pnlChartExportSettings.TabIndex = 6;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this._pnlChartExportSettings;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(316, 175);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // PopulationAnalysisChartSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this._layoutControl);
         this.Name = "PopulationAnalysisChartSettingsView";
         this.Size = new System.Drawing.Size(360, 267);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._layoutControl)).EndInit();
         this._layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._pnlChartSettings)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._layoutControlItemEdit)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabbedControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartSettingsLayoutGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._layoutControlItemChartSettings)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chartExportSettingsLayoutGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._pnlChartExportSettings)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl _layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.PanelControl _pnlChartSettings;
      private DevExpress.XtraEditors.SimpleButton _btnEdit;
      private DevExpress.XtraLayout.LayoutControlItem _layoutControlItemEdit;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraEditors.PanelControl _pnlChartExportSettings;
      private DevExpress.XtraLayout.TabbedControlGroup tabbedControlGroup1;
      private DevExpress.XtraLayout.LayoutControlGroup chartExportSettingsLayoutGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.LayoutControlGroup chartSettingsLayoutGroup;
      private DevExpress.XtraLayout.LayoutControlItem _layoutControlItemChartSettings;
   }
}
