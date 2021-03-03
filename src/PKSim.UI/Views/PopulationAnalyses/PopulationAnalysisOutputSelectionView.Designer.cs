namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class PopulationAnalysisOutputSelectionView
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
         this.cbTimeUnit = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.panelStatistics = new DevExpress.XtraEditors.PanelControl();
         this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
         this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
         this.panelOutputs = new DevExpress.XtraEditors.PanelControl();
         this.panelSelectedOutputs = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSelectedOutput = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemOutput = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonAdd = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonRemove = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemStatisticsSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemTimeUnit = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbTimeUnit.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelStatistics)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelOutputs)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedOutputs)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectedOutput)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutput)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStatisticsSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTimeUnit)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.cbTimeUnit);
         this.layoutControl.Controls.Add(this.panelStatistics);
         this.layoutControl.Controls.Add(this.btnRemove);
         this.layoutControl.Controls.Add(this.btnAdd);
         this.layoutControl.Controls.Add(this.panelOutputs);
         this.layoutControl.Controls.Add(this.panelSelectedOutputs);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(777, 493);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // cbTimeUnit
         // 
         this.cbTimeUnit.Location = new System.Drawing.Point(543, 10);
         this.cbTimeUnit.Name = "cbTimeUnit";
         this.cbTimeUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbTimeUnit.Size = new System.Drawing.Size(220, 20);
         this.cbTimeUnit.StyleController = this.layoutControl;
         this.cbTimeUnit.TabIndex = 8;
         // 
         // panelStatistics
         // 
         this.panelStatistics.Location = new System.Drawing.Point(435, 281);
         this.panelStatistics.Name = "panelStatistics";
         this.panelStatistics.Size = new System.Drawing.Size(340, 210);
         this.panelStatistics.TabIndex = 0;
         // 
         // btnRemove
         // 
         this.btnRemove.Location = new System.Drawing.Point(341, 281);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(90, 22);
         this.btnRemove.StyleController = this.layoutControl;
         this.btnRemove.TabIndex = 7;
         this.btnRemove.Text = "btnRemove";
         // 
         // btnAdd
         // 
         this.btnAdd.Location = new System.Drawing.Point(341, 255);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(90, 22);
         this.btnAdd.StyleController = this.layoutControl;
         this.btnAdd.TabIndex = 6;
         this.btnAdd.Text = "btnAdd";
         // 
         // panelOutputs
         // 
         this.panelOutputs.Location = new System.Drawing.Point(2, 2);
         this.panelOutputs.Name = "panelOutputs";
         this.panelOutputs.Padding = new System.Windows.Forms.Padding(10);
         this.panelOutputs.Size = new System.Drawing.Size(335, 489);
         this.panelOutputs.TabIndex = 5;
         // 
         // panelSelectedOutputs
         // 
         this.panelSelectedOutputs.Location = new System.Drawing.Point(435, 32);
         this.panelSelectedOutputs.Name = "panelSelectedOutputs";
         this.panelSelectedOutputs.Size = new System.Drawing.Size(340, 245);
         this.panelSelectedOutputs.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSelectedOutput,
            this.layoutItemOutput,
            this.layoutItemButtonAdd,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutItemButtonRemove,
            this.layoutItemStatisticsSelection,
            this.layoutItemTimeUnit});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(777, 493);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemSelectedOutput
         // 
         this.layoutItemSelectedOutput.Control = this.panelSelectedOutputs;
         this.layoutItemSelectedOutput.CustomizationFormText = "layoutItemSelectedOutput";
         this.layoutItemSelectedOutput.Location = new System.Drawing.Point(433, 30);
         this.layoutItemSelectedOutput.Name = "layoutItemSelectedOutput";
         this.layoutItemSelectedOutput.Size = new System.Drawing.Size(344, 249);
         this.layoutItemSelectedOutput.Text = "layoutItemSelectedOutput";
         this.layoutItemSelectedOutput.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSelectedOutput.TextToControlDistance = 0;
         this.layoutItemSelectedOutput.TextVisible = false;
         // 
         // layoutItemOutput
         // 
         this.layoutItemOutput.Control = this.panelOutputs;
         this.layoutItemOutput.CustomizationFormText = "layoutItemOutput";
         this.layoutItemOutput.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOutput.Name = "layoutItemOutput";
         this.layoutItemOutput.Size = new System.Drawing.Size(339, 493);
         this.layoutItemOutput.Text = "layoutItemOutput";
         this.layoutItemOutput.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemOutput.TextToControlDistance = 0;
         this.layoutItemOutput.TextVisible = false;
         // 
         // layoutItemButtonAdd
         // 
         this.layoutItemButtonAdd.Control = this.btnAdd;
         this.layoutItemButtonAdd.CustomizationFormText = "layoutControlItem3";
         this.layoutItemButtonAdd.Location = new System.Drawing.Point(339, 253);
         this.layoutItemButtonAdd.Name = "layoutItemButtonAdd";
         this.layoutItemButtonAdd.Size = new System.Drawing.Size(94, 26);
         this.layoutItemButtonAdd.Text = "layoutControlItem3";
         this.layoutItemButtonAdd.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonAdd.TextToControlDistance = 0;
         this.layoutItemButtonAdd.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(339, 0);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(94, 253);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(339, 305);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(94, 188);
         this.emptySpaceItem2.Text = "emptySpaceItem2";
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonRemove
         // 
         this.layoutItemButtonRemove.Control = this.btnRemove;
         this.layoutItemButtonRemove.CustomizationFormText = "layoutControlItem4";
         this.layoutItemButtonRemove.Location = new System.Drawing.Point(339, 279);
         this.layoutItemButtonRemove.Name = "layoutItemButtonRemove";
         this.layoutItemButtonRemove.Size = new System.Drawing.Size(94, 26);
         this.layoutItemButtonRemove.Text = "layoutControlItem4";
         this.layoutItemButtonRemove.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonRemove.TextToControlDistance = 0;
         this.layoutItemButtonRemove.TextVisible = false;
         // 
         // layoutItemStatisticsSelection
         // 
         this.layoutItemStatisticsSelection.Control = this.panelStatistics;
         this.layoutItemStatisticsSelection.CustomizationFormText = "layoutItemStatisticsSelection";
         this.layoutItemStatisticsSelection.Location = new System.Drawing.Point(433, 279);
         this.layoutItemStatisticsSelection.Name = "layoutItemStatisticsSelection";
         this.layoutItemStatisticsSelection.Size = new System.Drawing.Size(344, 214);
         this.layoutItemStatisticsSelection.Text = "layoutItemStatisticsSelection";
         this.layoutItemStatisticsSelection.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStatisticsSelection.TextToControlDistance = 0;
         this.layoutItemStatisticsSelection.TextVisible = false;
         // 
         // layoutItemTimeUnit
         // 
         this.layoutItemTimeUnit.Control = this.cbTimeUnit;
         this.layoutItemTimeUnit.CustomizationFormText = "layoutItemTimeUnit";
         this.layoutItemTimeUnit.Location = new System.Drawing.Point(433, 0);
         this.layoutItemTimeUnit.MaxSize = new System.Drawing.Size(0, 30);
         this.layoutItemTimeUnit.MinSize = new System.Drawing.Size(174, 30);
         this.layoutItemTimeUnit.Name = "layoutItemTimeUnit";
         this.layoutItemTimeUnit.Padding = new DevExpress.XtraLayout.Utils.Padding(14, 14, 10, 0);
         this.layoutItemTimeUnit.Size = new System.Drawing.Size(344, 30);
         this.layoutItemTimeUnit.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemTimeUnit.Text = "layoutItemTimeUnit";
         this.layoutItemTimeUnit.TextSize = new System.Drawing.Size(93, 13);
         // 
         // PopulationAnalysisOutputSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "PopulationAnalysisOutputSelectionView";
         this.Size = new System.Drawing.Size(777, 493);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbTimeUnit.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelStatistics)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelOutputs)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelSelectedOutputs)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSelectedOutput)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutput)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonAdd)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonRemove)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStatisticsSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTimeUnit)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SimpleButton btnRemove;
      private DevExpress.XtraEditors.SimpleButton btnAdd;
      private DevExpress.XtraEditors.PanelControl panelOutputs;
      private DevExpress.XtraEditors.PanelControl panelSelectedOutputs;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSelectedOutput;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOutput;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonAdd;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonRemove;
      private DevExpress.XtraEditors.PanelControl panelStatistics;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStatisticsSelection;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbTimeUnit;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemTimeUnit;
   }
}
