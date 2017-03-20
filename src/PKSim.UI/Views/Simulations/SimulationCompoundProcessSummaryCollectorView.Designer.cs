namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundProcessSummaryCollectorView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl(); 
         this.panelViewCollector = new DevExpress.XtraEditors.PanelControl();
         this.panelInhibitionSelection = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemViewCollector = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupInhibitionSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemInhibitionSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.showDiagramButton = new DevExpress.XtraEditors.SimpleButton();
         this.diagramButtonItem = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelViewCollector)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelInhibitionSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemViewCollector)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupInhibitionSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemInhibitionSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.diagramButtonItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.showDiagramButton);
         this.layoutControl.Controls.Add(this.panelViewCollector);
         this.layoutControl.Controls.Add(this.panelInhibitionSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(407, 432);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelViewCollector
         // 
         this.panelViewCollector.Location = new System.Drawing.Point(152, 98);
         this.panelViewCollector.Name = "panelViewCollector";
         this.panelViewCollector.Size = new System.Drawing.Size(245, 298);
         this.panelViewCollector.TabIndex = 5;
         // 
         // panelInhibitionSelection
         // 
         this.panelInhibitionSelection.Location = new System.Drawing.Point(166, 43);
         this.panelInhibitionSelection.Name = "panelInhibitionSelection";
         this.panelInhibitionSelection.Size = new System.Drawing.Size(217, 41);
         this.panelInhibitionSelection.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemViewCollector,
            this.layoutGroupInhibitionSelection,
            this.diagramButtonItem,
            this.emptySpaceItem1});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Size = new System.Drawing.Size(407, 432);
         this.layoutControlGroup.Text = "Root";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemViewCollector
         // 
         this.layoutItemViewCollector.Control = this.panelViewCollector;
         this.layoutItemViewCollector.CustomizationFormText = "layoutItemViewCollector";
         this.layoutItemViewCollector.Location = new System.Drawing.Point(0, 88);
         this.layoutItemViewCollector.Name = "layoutItemViewCollector";
         this.layoutItemViewCollector.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemViewCollector.Size = new System.Drawing.Size(387, 298);
         this.layoutItemViewCollector.Text = "layoutItemViewCollector";
         this.layoutItemViewCollector.TextSize = new System.Drawing.Size(139, 13);
         // 
         // layoutGroupInhibitionSelection
         // 
         this.layoutGroupInhibitionSelection.CustomizationFormText = "layoutGroupInhibitionSelection";
         this.layoutGroupInhibitionSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemInhibitionSelection});
         this.layoutGroupInhibitionSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupInhibitionSelection.Name = "layoutGroupInhibitionSelection";
         this.layoutGroupInhibitionSelection.Size = new System.Drawing.Size(387, 88);
         this.layoutGroupInhibitionSelection.Text = "layoutGroupInhibitionSelection";
         // 
         // layoutItemInhibitionSelection
         // 
         this.layoutItemInhibitionSelection.Control = this.panelInhibitionSelection;
         this.layoutItemInhibitionSelection.CustomizationFormText = "layoutItemInhibitionSelection";
         this.layoutItemInhibitionSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutItemInhibitionSelection.Name = "layoutItemInhibitionSelection";
         this.layoutItemInhibitionSelection.Size = new System.Drawing.Size(363, 45);
         this.layoutItemInhibitionSelection.Text = "layoutItemInhibitionSelection";
         this.layoutItemInhibitionSelection.TextSize = new System.Drawing.Size(139, 13);
         // 
         // showDiagramButton
         // 
         this.showDiagramButton.Location = new System.Drawing.Point(12, 398);
         this.showDiagramButton.Name = "showDiagramButton";
         this.showDiagramButton.Size = new System.Drawing.Size(127, 22);
         this.showDiagramButton.StyleController = this.layoutControl;
         this.showDiagramButton.TabIndex = 6;
         this.showDiagramButton.Text = "simpleButton1";
         // 
         // diagramButtonItem
         // 
         this.diagramButtonItem.Control = this.showDiagramButton;
         this.diagramButtonItem.CustomizationFormText = "diagramButtonItem";
         this.diagramButtonItem.Location = new System.Drawing.Point(0, 386);
         this.diagramButtonItem.Name = "diagramButtonItem";
         this.diagramButtonItem.Size = new System.Drawing.Size(131, 26);
         this.diagramButtonItem.Text = "diagramButtonItem";
         this.diagramButtonItem.TextSize = new System.Drawing.Size(0, 0);
         this.diagramButtonItem.TextToControlDistance = 0;
         this.diagramButtonItem.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(131, 386);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(256, 26);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // SimulationProcessSummaryCollectorView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationCompoundProcessSummaryCollectorView";
         this.Size = new System.Drawing.Size(407, 432);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelViewCollector)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelInhibitionSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemViewCollector)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupInhibitionSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemInhibitionSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.diagramButtonItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelViewCollector;
      private DevExpress.XtraEditors.PanelControl panelInhibitionSelection;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemInhibitionSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemViewCollector;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupInhibitionSelection;
      private DevExpress.XtraEditors.SimpleButton showDiagramButton;
      private DevExpress.XtraLayout.LayoutControlItem diagramButtonItem;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}
