namespace PKSim.UI.Views.Simulations
{
   partial class SimulationEventsConfigurationView
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
         this.gridControl = new DevExpress.XtraGrid.GridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.btnAddEvent = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemAddEvent = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddEvent)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Controls.Add(this.btnAddEvent);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(431, 446);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(12, 38);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(407, 396);
         this.gridControl.TabIndex = 5;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView.GridControl = this.gridControl;
         this.gridView.Name = "gridView";
         this.gridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseUp;
         this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // btnAddEvent
         // 
         this.btnAddEvent.Location = new System.Drawing.Point(22, 12);
         this.btnAddEvent.Name = "btnAddEvent";
         this.btnAddEvent.Size = new System.Drawing.Size(397, 22);
         this.btnAddEvent.StyleController = this.layoutControl;
         this.btnAddEvent.TabIndex = 4;
         this.btnAddEvent.Text = "btnCreateEvent";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemAddEvent,
            this.layoutItemGrid,
            this.emptySpaceItem1});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(431, 446);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemAddEvent
         // 
         this.layoutItemAddEvent.Control = this.btnAddEvent;
         this.layoutItemAddEvent.CustomizationFormText = "layoutItemAddEvent";
         this.layoutItemAddEvent.Location = new System.Drawing.Point(10, 0);
         this.layoutItemAddEvent.Name = "layoutItemAddEvent";
         this.layoutItemAddEvent.Size = new System.Drawing.Size(401, 26);
         this.layoutItemAddEvent.Text = "layoutItemAddEvent";
         this.layoutItemAddEvent.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAddEvent.TextToControlDistance = 0;
         this.layoutItemAddEvent.TextVisible = false;
         // 
         // layoutItemGrid
         // 
         this.layoutItemGrid.Control = this.gridControl;
         this.layoutItemGrid.CustomizationFormText = "layoutItemGrid";
         this.layoutItemGrid.Location = new System.Drawing.Point(0, 26);
         this.layoutItemGrid.Name = "layoutItemGrid";
         this.layoutItemGrid.Size = new System.Drawing.Size(411, 400);
         this.layoutItemGrid.Text = "layoutItemGrid";
         this.layoutItemGrid.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemGrid.TextToControlDistance = 0;
         this.layoutItemGrid.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // SimulationEventsConfigurationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationEventsConfigurationView";
         this.Size = new System.Drawing.Size(431, 446);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddEvent)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraGrid.GridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraEditors.SimpleButton btnAddEvent;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAddEvent;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGrid;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
   }
}
