using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   partial class NumberOfBinsGroupingView
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
         this.ceStartColor = new UxColorPickEditWithHistory();
         this.cbSymbol = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.ceEndColor = new UxColorPickEditWithHistory();
         this.cbGenerationStrategy = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.gridControl = new OSPSuite.UI.Controls.UxGridControl();
         this.gridView = new PKSim.UI.Views.Core.UxGridView();
         this.lblNamingPatternDescription = new DevExpress.XtraEditors.LabelControl();
         this.tbNamingPattern = new DevExpress.XtraEditors.TextEdit();
         this.tbNumberOfBins = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlGroupPattern = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemNumberOfBins = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSymbol = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupPattern = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemStartColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemEndColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGenerationStrategie = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNamingPattern = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroupLabels = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemLabels = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.ceStartColor.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSymbol.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.ceEndColor.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGenerationStrategy.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNamingPattern.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfBins.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPattern)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfBins)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSymbol)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupPattern)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEndColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGenerationStrategie)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNamingPattern)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupLabels)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLabels)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.ceStartColor);
         this.layoutControl.Controls.Add(this.cbSymbol);
         this.layoutControl.Controls.Add(this.ceEndColor);
         this.layoutControl.Controls.Add(this.cbGenerationStrategy);
         this.layoutControl.Controls.Add(this.gridControl);
         this.layoutControl.Controls.Add(this.lblNamingPatternDescription);
         this.layoutControl.Controls.Add(this.tbNamingPattern);
         this.layoutControl.Controls.Add(this.tbNumberOfBins);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(451, 372);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // ceStartColor
         // 
         this.ceStartColor.EditValue = System.Drawing.Color.Empty;
         this.ceStartColor.Location = new System.Drawing.Point(166, 81);
         this.ceStartColor.Name = "ceStartColor";
         this.ceStartColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.ceStartColor.Size = new System.Drawing.Size(271, 20);
         this.ceStartColor.StyleController = this.layoutControl;
         this.ceStartColor.TabIndex = 13;
         // 
         // cbSymbol
         // 
         this.cbSymbol.Location = new System.Drawing.Point(166, 57);
         this.cbSymbol.Name = "cbSymbol";
         this.cbSymbol.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSymbol.Size = new System.Drawing.Size(271, 20);
         this.cbSymbol.StyleController = this.layoutControl;
         this.cbSymbol.TabIndex = 12;
         // 
         // ceEndColor
         // 
         this.ceEndColor.EditValue = System.Drawing.Color.Empty;
         this.ceEndColor.Location = new System.Drawing.Point(166, 105);
         this.ceEndColor.Name = "ceEndColor";
         this.ceEndColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.ceEndColor.Size = new System.Drawing.Size(271, 20);
         this.ceEndColor.StyleController = this.layoutControl;
         this.ceEndColor.TabIndex = 11;
         // 
         // cbGenerationStrategy
         // 
         this.cbGenerationStrategy.Location = new System.Drawing.Point(166, 129);
         this.cbGenerationStrategy.Name = "cbGenerationStrategy";
         this.cbGenerationStrategy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGenerationStrategy.Size = new System.Drawing.Size(271, 20);
         this.cbGenerationStrategy.StyleController = this.layoutControl;
         this.cbGenerationStrategy.TabIndex = 9;
         // 
         // gridControl
         // 
         this.gridControl.Location = new System.Drawing.Point(14, 237);
         this.gridControl.MainView = this.gridView;
         this.gridControl.Name = "gridControl";
         this.gridControl.Size = new System.Drawing.Size(423, 121);
         this.gridControl.TabIndex = 7;
         this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
         // 
         // gridView
         // 
         this.gridView.EnableColumnContextMenu = true;
         this.gridView.GridControl = this.gridControl;
         this.gridView.Name = "gridView";
         // 
         // lblNamingPatternDescription
         // 
         this.lblNamingPatternDescription.Location = new System.Drawing.Point(14, 177);
         this.lblNamingPatternDescription.Name = "lblNamingPatternDescription";
         this.lblNamingPatternDescription.Size = new System.Drawing.Size(134, 13);
         this.lblNamingPatternDescription.StyleController = this.layoutControl;
         this.lblNamingPatternDescription.TabIndex = 6;
         this.lblNamingPatternDescription.Text = "lblNamingPatternDescription";
         // 
         // tbNamingPattern
         // 
         this.tbNamingPattern.Location = new System.Drawing.Point(166, 153);
         this.tbNamingPattern.Name = "tbNamingPattern";
         this.tbNamingPattern.Size = new System.Drawing.Size(271, 20);
         this.tbNamingPattern.StyleController = this.layoutControl;
         this.tbNamingPattern.TabIndex = 5;
         // 
         // tbNumberOfBins
         // 
         this.tbNumberOfBins.Location = new System.Drawing.Point(166, 33);
         this.tbNumberOfBins.Name = "tbNumberOfBins";
         this.tbNumberOfBins.Size = new System.Drawing.Size(271, 20);
         this.tbNumberOfBins.StyleController = this.layoutControl;
         this.tbNumberOfBins.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroupPattern,
            this.layoutControlGroupLabels});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(451, 372);
         this.layoutControlGroup.Text = "layoutControlGroup";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutControlGroupPattern
         // 
         this.layoutControlGroupPattern.CustomizationFormText = "layoutControlGroupPattern";
         this.layoutControlGroupPattern.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemNumberOfBins,
            this.layoutItemSymbol,
            this.layoutGroupPattern});
         this.layoutControlGroupPattern.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroupPattern.Name = "layoutControlGroupPattern";
         this.layoutControlGroupPattern.Size = new System.Drawing.Size(451, 204);
         this.layoutControlGroupPattern.Text = "layoutControlGroupPattern";
         // 
         // layoutItemNumberOfBins
         // 
         this.layoutItemNumberOfBins.Control = this.tbNumberOfBins;
         this.layoutItemNumberOfBins.CustomizationFormText = "layoutItemNumberOfBins";
         this.layoutItemNumberOfBins.Location = new System.Drawing.Point(0, 0);
         this.layoutItemNumberOfBins.Name = "layoutItemNumberOfBins";
         this.layoutItemNumberOfBins.Size = new System.Drawing.Size(427, 24);
         this.layoutItemNumberOfBins.Text = "layoutItemNumberOfBins";
         this.layoutItemNumberOfBins.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutItemSymbol
         // 
         this.layoutItemSymbol.Control = this.cbSymbol;
         this.layoutItemSymbol.CustomizationFormText = "layoutItemSymbol";
         this.layoutItemSymbol.Location = new System.Drawing.Point(0, 24);
         this.layoutItemSymbol.Name = "layoutItemSymbol";
         this.layoutItemSymbol.Size = new System.Drawing.Size(427, 24);
         this.layoutItemSymbol.Text = "layoutItemSymbol";
         this.layoutItemSymbol.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutGroupPattern
         // 
         this.layoutGroupPattern.CustomizationFormText = "layoutGroupPattern";
         this.layoutGroupPattern.GroupBordersVisible = false;
         this.layoutGroupPattern.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemStartColor,
            this.layoutItemEndColor,
            this.layoutItemGenerationStrategie,
            this.layoutItemNamingPattern,
            this.layoutItemDescription});
         this.layoutGroupPattern.Location = new System.Drawing.Point(0, 48);
         this.layoutGroupPattern.Name = "layoutGroupPattern";
         this.layoutGroupPattern.Size = new System.Drawing.Size(427, 113);
         this.layoutGroupPattern.Text = "layoutGroupPattern";
         // 
         // layoutItemStartColor
         // 
         this.layoutItemStartColor.Control = this.ceStartColor;
         this.layoutItemStartColor.CustomizationFormText = "layoutItemStartColor";
         this.layoutItemStartColor.Location = new System.Drawing.Point(0, 0);
         this.layoutItemStartColor.Name = "layoutItemStartColor";
         this.layoutItemStartColor.Size = new System.Drawing.Size(427, 24);
         this.layoutItemStartColor.Text = "layoutItemStartColor";
         this.layoutItemStartColor.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutItemEndColor
         // 
         this.layoutItemEndColor.Control = this.ceEndColor;
         this.layoutItemEndColor.CustomizationFormText = "layoutItemEndColor";
         this.layoutItemEndColor.Location = new System.Drawing.Point(0, 24);
         this.layoutItemEndColor.Name = "layoutItemEndColor";
         this.layoutItemEndColor.Size = new System.Drawing.Size(427, 24);
         this.layoutItemEndColor.Text = "layoutItemEndColor";
         this.layoutItemEndColor.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutItemGenerationStrategie
         // 
         this.layoutItemGenerationStrategie.Control = this.cbGenerationStrategy;
         this.layoutItemGenerationStrategie.CustomizationFormText = "layoutItemGenerationStrategie";
         this.layoutItemGenerationStrategie.Location = new System.Drawing.Point(0, 48);
         this.layoutItemGenerationStrategie.Name = "layoutItemGenerationStrategie";
         this.layoutItemGenerationStrategie.Size = new System.Drawing.Size(427, 24);
         this.layoutItemGenerationStrategie.Text = "layoutItemGenerationStrategie";
         this.layoutItemGenerationStrategie.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutItemNamingPattern
         // 
         this.layoutItemNamingPattern.Control = this.tbNamingPattern;
         this.layoutItemNamingPattern.CustomizationFormText = "layoutItemNamingPattern";
         this.layoutItemNamingPattern.Location = new System.Drawing.Point(0, 72);
         this.layoutItemNamingPattern.Name = "layoutItemNamingPattern";
         this.layoutItemNamingPattern.Size = new System.Drawing.Size(427, 24);
         this.layoutItemNamingPattern.Text = "layoutItemNamingPattern";
         this.layoutItemNamingPattern.TextSize = new System.Drawing.Size(149, 13);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblNamingPatternDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutItemDescription";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 96);
         this.layoutItemDescription.Name = "layoutControlItem1";
         this.layoutItemDescription.Size = new System.Drawing.Size(427, 17);
         this.layoutItemDescription.Text = "layoutControlItem1";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutControlGroupLabels
         // 
         this.layoutControlGroupLabels.CustomizationFormText = "layoutControlGroupLabels";
         this.layoutControlGroupLabels.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemLabels});
         this.layoutControlGroupLabels.Location = new System.Drawing.Point(0, 204);
         this.layoutControlGroupLabels.Name = "layoutControlGroupLabels";
         this.layoutControlGroupLabels.Size = new System.Drawing.Size(451, 168);
         this.layoutControlGroupLabels.Text = "layoutControlGroupLabels";
         // 
         // layoutItemLabels
         // 
         this.layoutItemLabels.Control = this.gridControl;
         this.layoutItemLabels.CustomizationFormText = "layoutItemLabels";
         this.layoutItemLabels.Location = new System.Drawing.Point(0, 0);
         this.layoutItemLabels.Name = "layoutItemLabels";
         this.layoutItemLabels.Size = new System.Drawing.Size(427, 125);
         this.layoutItemLabels.Text = "layoutItemLabels";
         this.layoutItemLabels.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemLabels.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLabels.TextToControlDistance = 0;
         this.layoutItemLabels.TextVisible = false;
         // 
         // NumberOfBinsGroupingView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "NumberOfBinsGroupingView";
         this.Size = new System.Drawing.Size(451, 372);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.ceStartColor.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSymbol.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.ceEndColor.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGenerationStrategy.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNamingPattern.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfBins.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPattern)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfBins)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSymbol)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupPattern)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStartColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemEndColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGenerationStrategie)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNamingPattern)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupLabels)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLabels)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbGenerationStrategy;
      private OSPSuite.UI.Controls.UxGridControl gridControl;
      private PKSim.UI.Views.Core.UxGridView gridView;
      private DevExpress.XtraEditors.LabelControl lblNamingPatternDescription;
      private DevExpress.XtraEditors.TextEdit tbNamingPattern;
      private DevExpress.XtraEditors.TextEdit tbNumberOfBins;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfBins;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNamingPattern;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLabels;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGenerationStrategie;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupPattern;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupLabels;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbSymbol;
      private UxColorPickEditWithHistory ceEndColor;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemEndColor;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSymbol;
      private UxColorPickEditWithHistory ceStartColor;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStartColor;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupPattern;
   }
}
