namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class PopulationAnalysisPKParameterSelectionView
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
         this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
         this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
         this.panelSelectedPKParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelAvailablePKParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonRemove = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelDistributionView = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemDistributionView = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitter = new DevExpress.XtraLayout.SplitterItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedPKParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelAvailablePKParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDistributionView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDistributionView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelDistributionView);
         this.layoutControl.Controls.Add(this.btnRemove);
         this.layoutControl.Controls.Add(this.btnAdd);
         this.layoutControl.Controls.Add(this.panelSelectedPKParameters);
         this.layoutControl.Controls.Add(this.panelAvailablePKParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup2;
         this.layoutControl.Size = new System.Drawing.Size(1253, 702);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnRemove
         // 
         this.btnRemove.Location = new System.Drawing.Point(551, 200);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(155, 22);
         this.btnRemove.StyleController = this.layoutControl;
         this.btnRemove.TabIndex = 7;
         this.btnRemove.Text = "btnRemove";
         // 
         // btnAdd
         // 
         this.btnAdd.Location = new System.Drawing.Point(551, 174);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(155, 22);
         this.btnAdd.StyleController = this.layoutControl;
         this.btnAdd.TabIndex = 6;
         this.btnAdd.Text = "btnAdd";
         // 
         // panelSelectedPKParameters
         // 
         this.panelSelectedPKParameters.Location = new System.Drawing.Point(710, 2);
         this.panelSelectedPKParameters.Name = "panelSelectedPKParameters";
         this.panelSelectedPKParameters.Size = new System.Drawing.Size(541, 406);
         this.panelSelectedPKParameters.TabIndex = 5;
         // 
         // panelAvailablePKParameters
         // 
         this.panelAvailablePKParameters.Location = new System.Drawing.Point(2, 2);
         this.panelAvailablePKParameters.Name = "panelAvailablePKParameters";
         this.panelAvailablePKParameters.Padding = new System.Windows.Forms.Padding(10);
         this.panelAvailablePKParameters.Size = new System.Drawing.Size(545, 406);
         this.panelAvailablePKParameters.TabIndex = 4;
         // 
         // layoutControlGroup2
         // 
         this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
         this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup2.GroupBordersVisible = false;
         this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutItemButtonAdd,
            this.layoutItemButtonRemove,
            this.layoutItemDistributionView,
            this.splitter});
         this.layoutControlGroup2.Name = "Root";
         this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup2.Size = new System.Drawing.Size(1253, 702);
         this.layoutControlGroup2.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.panelAvailablePKParameters;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(549, 410);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.panelSelectedPKParameters;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(708, 0);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(545, 410);
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(549, 0);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(159, 172);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(549, 224);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
         this.emptySpaceItem2.Size = new System.Drawing.Size(159, 186);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.btnAdd;
         this.layoutItemButtonAdd.CustomizationFormText = "layoutItemButtonAdd";
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(549, 172);
         this.layoutItemButtonAdd.Name = "layoutItemButtonAdd";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(159, 26);
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // layoutItemButtonRemove
         // 
         this.layoutItemButtonRemove.Control = this.btnRemove;
         this.layoutItemButtonRemove.CustomizationFormText = "layoutItemButtonRemove";
         this.layoutItemButtonRemove.Location = new System.Drawing.Point(549, 198);
         this.layoutItemButtonRemove.Name = "layoutItemButtonRemove";
         this.layoutItemButtonRemove.Size = new System.Drawing.Size(159, 26);
         this.layoutItemButtonRemove.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonRemove.TextVisible = false;
         // 
         // panelControl1
         // 
         this.panelDistributionView.Location = new System.Drawing.Point(2, 422);
         this.panelDistributionView.Name = "panelDistributionView";
         this.panelDistributionView.Size = new System.Drawing.Size(1249, 278);
         this.panelDistributionView.TabIndex = 8;
         // 
         // layoutItemChart
         // 
         this.layoutItemDistributionView.Control = this.panelDistributionView;
         this.layoutItemDistributionView.Location = new System.Drawing.Point(0, 420);
         this.layoutItemDistributionView.Name = "layoutItemDistributionView";
         this.layoutItemDistributionView.Size = new System.Drawing.Size(1253, 282);
         this.layoutItemDistributionView.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDistributionView.TextVisible = false;
         // 
         // splitter
         // 
         this.splitter.AllowHotTrack = true;
         this.splitter.Location = new System.Drawing.Point(0, 410);
         this.splitter.Name = "splitter";
         this.splitter.Size = new System.Drawing.Size(1253, 10);
         // 
         // PopulationAnalysisPKParameterSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "PopulationAnalysisPKParameterSelectionView";
         this.Size = new System.Drawing.Size(1253, 702);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedPKParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelAvailablePKParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDistributionView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDistributionView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelDistributionView;
      private DevExpress.XtraEditors.SimpleButton btnRemove;
      private DevExpress.XtraEditors.SimpleButton btnAdd;
      private DevExpress.XtraEditors.PanelControl panelSelectedPKParameters;
      private DevExpress.XtraEditors.PanelControl panelAvailablePKParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonRemove;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDistributionView;
      private DevExpress.XtraLayout.SplitterItem splitter;
   }
}
