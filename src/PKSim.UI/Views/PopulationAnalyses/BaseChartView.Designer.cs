using PKSim.Presentation.Presenters.PopulationAnalyses;

namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class BaseChartView<TX, TY> 
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
         this.components = new System.ComponentModel.Container();
         this._dockManager = new DevExpress.XtraBars.Docking.DockManager(this.components);
         this._pnlChartSettings = new DevExpress.XtraBars.Docking.DockPanel();
         this.dockPanel1_Container = new DevExpress.XtraBars.Docking.ControlContainer();
         this._pnlChart = new DevExpress.XtraEditors.PanelControl();
         this.hideContainerRight = new DevExpress.XtraBars.Docking.AutoHideContainer();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._dockManager)).BeginInit();
         this._pnlChartSettings.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._pnlChart)).BeginInit();
         this.hideContainerRight.SuspendLayout();
         this.SuspendLayout();
         // 
         // _dockManager
         // 
         this._dockManager.AutoHideContainers.AddRange(new DevExpress.XtraBars.Docking.AutoHideContainer[] {
            this.hideContainerRight});
         this._dockManager.Form = this;
         this._dockManager.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
         // 
         // _pnlChartSettings
         // 
         this._pnlChartSettings.Controls.Add(this.dockPanel1_Container);
         this._pnlChartSettings.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
         this._pnlChartSettings.ID = new System.Guid("0d911eec-8d7f-4c3d-b5dc-bde300ec5ef6");
         this._pnlChartSettings.Location = new System.Drawing.Point(0, 0);
         this._pnlChartSettings.Name = "_pnlChartSettings";
         this._pnlChartSettings.OriginalSize = new System.Drawing.Size(141, 200);
         this._pnlChartSettings.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
         this._pnlChartSettings.SavedIndex = 0;
         this._pnlChartSettings.Size = new System.Drawing.Size(141, 300);
         this._pnlChartSettings.Text = "dockPanel1";
         this._pnlChartSettings.Visibility = DevExpress.XtraBars.Docking.DockVisibility.AutoHide;
         // 
         // dockPanel1_Container
         // 
         this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
         this.dockPanel1_Container.Name = "dockPanel1_Container";
         this.dockPanel1_Container.Size = new System.Drawing.Size(133, 273);
         this.dockPanel1_Container.TabIndex = 0;
         // 
         // _pnlChart
         // 
         this._pnlChart.Dock = System.Windows.Forms.DockStyle.Fill;
         this._pnlChart.Location = new System.Drawing.Point(0, 0);
         this._pnlChart.Name = "_pnlChart";
         this._pnlChart.Size = new System.Drawing.Size(378, 300);
         this._pnlChart.TabIndex = 1;
         // 
         // hideContainerRight
         // 
         this.hideContainerRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
         this.hideContainerRight.Controls.Add(this._pnlChartSettings);
         this.hideContainerRight.Dock = System.Windows.Forms.DockStyle.Right;
         this.hideContainerRight.Location = new System.Drawing.Point(378, 0);
         this.hideContainerRight.Name = "hideContainerRight";
         this.hideContainerRight.Size = new System.Drawing.Size(19, 300);
         // 
         // BaseChartView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this._pnlChart);
         this.Controls.Add(this.hideContainerRight);
         this.Name = "BaseChartView";
         this.Size = new System.Drawing.Size(397, 300);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._dockManager)).EndInit();
         this._pnlChartSettings.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._pnlChart)).EndInit();
         this.hideContainerRight.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraBars.Docking.DockManager _dockManager;
      private DevExpress.XtraBars.Docking.DockPanel _pnlChartSettings;
      private DevExpress.XtraBars.Docking.ControlContainer dockPanel1_Container;
      private DevExpress.XtraEditors.PanelControl _pnlChart;
      private DevExpress.XtraBars.Docking.AutoHideContainer hideContainerRight;
   }
}
