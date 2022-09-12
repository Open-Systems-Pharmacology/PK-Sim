namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class PopulationAnalysisParameterSelectionView
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
         this.panelSelectedParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonRemove = new DevExpress.XtraLayout.LayoutControlItem();
         this.panelDistribution = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.splitter = new DevExpress.XtraLayout.SplitterItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDistribution)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.panelDistribution);
         this.layoutControl.Controls.Add(this.btnRemove);
         this.layoutControl.Controls.Add(this.btnAdd);
         this.layoutControl.Controls.Add(this.panelSelectedParameters);
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1419, 502, 650, 400);
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(1203, 758);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnRemove
         // 
         this.btnRemove.Location = new System.Drawing.Point(530, 228);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(147, 22);
         this.btnRemove.StyleController = this.layoutControl;
         this.btnRemove.TabIndex = 7;
         this.btnRemove.Text = "btnRemove";
         // 
         // btnAdd
         // 
         this.btnAdd.Location = new System.Drawing.Point(530, 202);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(147, 22);
         this.btnAdd.StyleController = this.layoutControl;
         this.btnAdd.TabIndex = 6;
         this.btnAdd.Text = "btnAdd";
         // 
         // panelSelectedParameters
         // 
         this.panelSelectedParameters.Location = new System.Drawing.Point(681, 2);
         this.panelSelectedParameters.Name = "panelSelectedParameters";
         this.panelSelectedParameters.Size = new System.Drawing.Size(520, 425);
         this.panelSelectedParameters.TabIndex = 5;
         // 
         // panelParameters
         // 
         this.panelParameters.Location = new System.Drawing.Point(2, 2);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Padding = new System.Windows.Forms.Padding(10);
         this.panelParameters.Size = new System.Drawing.Size(524, 425);
         this.panelParameters.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.CustomizationFormText = "Root";
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutItemButtonAdd,
            this.layoutItemButtonRemove,
            this.layoutControlItem1,
            this.splitter});
         this.Root.Name = "Root";
         this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.Root.Size = new System.Drawing.Size(1203, 758);
         this.Root.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.panelParameters;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(528, 429);
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.panelSelectedParameters;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(679, 0);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(524, 429);
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(528, 0);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(151, 200);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(528, 252);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(151, 177);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.btnAdd;
         this.layoutItemButtonAdd.CustomizationFormText = "layoutControlItem4";
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(528, 200);
         this.layoutItemButtonAdd.Name = "layoutItemButtonAdd";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(151, 26);
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // layoutItemButtonRemove
         // 
         this.layoutItemButtonRemove.Control = this.btnRemove;
         this.layoutItemButtonRemove.CustomizationFormText = "layoutControlItem5";
         this.layoutItemButtonRemove.Location = new System.Drawing.Point(528, 226);
         this.layoutItemButtonRemove.Name = "layoutItemButtonRemove";
         this.layoutItemButtonRemove.Size = new System.Drawing.Size(151, 26);
         this.layoutItemButtonRemove.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonRemove.TextVisible = false;
         // 
         // panelDistribution
         // 
         this.panelDistribution.Location = new System.Drawing.Point(2, 441);
         this.panelDistribution.Name = "panelDistribution";
         this.panelDistribution.Size = new System.Drawing.Size(1199, 315);
         this.panelDistribution.TabIndex = 8;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.panelDistribution;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 439);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(1203, 319);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // splitter
         // 
         this.splitter.AllowHotTrack = true;
         this.splitter.Location = new System.Drawing.Point(0, 429);
         this.splitter.Name = "splitter";
         this.splitter.Size = new System.Drawing.Size(1203, 10);
         // 
         // PopulationAnalysisParameterSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "PopulationAnalysisParameterSelectionView";
         this.Size = new System.Drawing.Size(1203, 758);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelDistribution)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnRemove;
      private DevExpress.XtraEditors.SimpleButton btnAdd;
      private DevExpress.XtraEditors.PanelControl panelSelectedParameters;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonRemove;
      private DevExpress.XtraEditors.PanelControl panelDistribution;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.SplitterItem splitter;
   }
}