using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Populations
{
   partial class RandomPopulationSettingsView
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
         _settingsBinder.Dispose();
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
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.uxBuildingBlockSelection = new PKSim.UI.Views.UxBuildingBlockSelection();
         this.lblIndividual = new DevExpress.XtraEditors.LabelControl();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.lblPopulation = new DevExpress.XtraEditors.LabelControl();
         this.lblDiseaseState = new DevExpress.XtraEditors.LabelControl();
         this.btnStop = new DevExpress.XtraEditors.SimpleButton();
         this.tbProportionsOfFemales = new DevExpress.XtraEditors.TextEdit();
         this.tbNumberOfIndividuals = new DevExpress.XtraEditors.TextEdit();
         this.gridParameters = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupPopulationProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemProportionOfFemales = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNumberOfIndividuals = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupParameterRanges = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemStop = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutGroupIndividualSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbProportionsOfFemales.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividuals.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupPopulationProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProportionOfFemales)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividuals)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupParameterRanges)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStop)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupIndividualSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.tablePanel);
         this.layoutControl.Controls.Add(this.btnStop);
         this.layoutControl.Controls.Add(this.tbProportionsOfFemales);
         this.layoutControl.Controls.Add(this.tbNumberOfIndividuals);
         this.layoutControl.Controls.Add(this.gridParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(642, 336, 250, 350);
         this.layoutControl.Root = this.layoutMainGroup;
         this.layoutControl.Size = new System.Drawing.Size(451, 546);
         this.layoutControl.TabIndex = 9;
         this.layoutControl.Text = "layoutControl1";
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 5F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 55F)});
         this.tablePanel.Controls.Add(this.uxBuildingBlockSelection);
         this.tablePanel.Controls.Add(this.lblIndividual);
         this.tablePanel.Controls.Add(this.lblDescription);
         this.tablePanel.Controls.Add(this.lblPopulation);
         this.tablePanel.Controls.Add(this.lblDiseaseState);
         this.tablePanel.Location = new System.Drawing.Point(12, 12);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(427, 110);
         this.tablePanel.TabIndex = 10;
         // 
         // uxBuildingBlockSelection
         // 
         this.uxBuildingBlockSelection.AllowEmptySelection = false;
         this.uxBuildingBlockSelection.Caption = "";
         this.tablePanel.SetColumn(this.uxBuildingBlockSelection, 1);
         this.uxBuildingBlockSelection.Location = new System.Drawing.Point(65, 3);
         this.uxBuildingBlockSelection.MinimumSize = new System.Drawing.Size(0, 26);
         this.uxBuildingBlockSelection.Name = "uxBuildingBlockSelection";
         this.tablePanel.SetRow(this.uxBuildingBlockSelection, 0);
         this.uxBuildingBlockSelection.Size = new System.Drawing.Size(359, 26);
         this.uxBuildingBlockSelection.TabIndex = 23;
         // 
         // lblIndividual
         // 
         this.tablePanel.SetColumn(this.lblIndividual, 0);
         this.lblIndividual.Location = new System.Drawing.Point(3, 6);
         this.lblIndividual.Name = "lblIndividual";
         this.tablePanel.SetRow(this.lblIndividual, 0);
         this.lblIndividual.Size = new System.Drawing.Size(56, 13);
         this.lblIndividual.TabIndex = 22;
         this.lblIndividual.Text = "lblIndividual";
         // 
         // lblDescription
         // 
         this.tablePanel.SetColumn(this.lblDescription, 0);
         this.tablePanel.SetColumnSpan(this.lblDescription, 2);
         this.lblDescription.Location = new System.Drawing.Point(3, 32);
         this.lblDescription.Name = "lblDescription";
         this.tablePanel.SetRow(this.lblDescription, 1);
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.TabIndex = 20;
         this.lblDescription.Text = "lblDescription";
         // 
         // lblPopulation
         // 
         this.tablePanel.SetColumn(this.lblPopulation, 0);
         this.tablePanel.SetColumnSpan(this.lblPopulation, 2);
         this.lblPopulation.Location = new System.Drawing.Point(3, 58);
         this.lblPopulation.Name = "lblPopulation";
         this.tablePanel.SetRow(this.lblPopulation, 2);
         this.lblPopulation.Size = new System.Drawing.Size(60, 13);
         this.lblPopulation.TabIndex = 18;
         this.lblPopulation.Text = "lblPopulation";
         // 
         // lblDiseaseState
         // 
         this.tablePanel.SetColumn(this.lblDiseaseState, 0);
         this.tablePanel.SetColumnSpan(this.lblDiseaseState, 2);
         this.lblDiseaseState.Location = new System.Drawing.Point(3, 84);
         this.lblDiseaseState.Name = "lblDiseaseState";
         this.tablePanel.SetRow(this.lblDiseaseState, 3);
         this.lblDiseaseState.Size = new System.Drawing.Size(73, 13);
         this.lblDiseaseState.TabIndex = 21;
         this.lblDiseaseState.Text = "lblDiseaseState";
         // 
         // btnStop
         // 
         this.btnStop.Location = new System.Drawing.Point(227, 394);
         this.btnStop.Name = "btnStop";
         this.btnStop.Size = new System.Drawing.Size(212, 22);
         this.btnStop.StyleController = this.layoutControl;
         this.btnStop.TabIndex = 19;
         this.btnStop.Text = "btnStop";
         // 
         // tbProportionsOfFemales
         // 
         this.tbProportionsOfFemales.Location = new System.Drawing.Point(189, 183);
         this.tbProportionsOfFemales.Name = "tbProportionsOfFemales";
         this.tbProportionsOfFemales.Size = new System.Drawing.Size(238, 20);
         this.tbProportionsOfFemales.StyleController = this.layoutControl;
         this.tbProportionsOfFemales.TabIndex = 17;
         // 
         // tbNumberOfIndividuals
         // 
         this.tbNumberOfIndividuals.Location = new System.Drawing.Point(189, 159);
         this.tbNumberOfIndividuals.Name = "tbNumberOfIndividuals";
         this.tbNumberOfIndividuals.Size = new System.Drawing.Size(238, 20);
         this.tbNumberOfIndividuals.StyleController = this.layoutControl;
         this.tbNumberOfIndividuals.TabIndex = 14;
         // 
         // gridParameters
         // 
         this.gridParameters.Location = new System.Drawing.Point(24, 252);
         this.gridParameters.MainView = this.gridViewParameters;
         this.gridParameters.Name = "gridParameters";
         this.gridParameters.Size = new System.Drawing.Size(403, 71);
         this.gridParameters.TabIndex = 10;
         this.gridParameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters});
         // 
         // gridViewParameters
         // 
         this.gridViewParameters.AllowsFiltering = true;
         this.gridViewParameters.EnableColumnContextMenu = true;
         this.gridViewParameters.GridControl = this.gridParameters;
         this.gridViewParameters.MultiSelect = false;
         this.gridViewParameters.Name = "gridViewParameters";
         this.gridViewParameters.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridViewParameters.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutMainGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupPopulationProperties,
            this.layoutGroupParameterRanges,
            this.layoutItemStop,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutGroupIndividualSelection});
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Size = new System.Drawing.Size(451, 546);
         this.layoutMainGroup.TextVisible = false;
         // 
         // layoutGroupPopulationProperties
         // 
         this.layoutGroupPopulationProperties.CustomizationFormText = "layoutGroupPopulationProperties";
         this.layoutGroupPopulationProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemProportionOfFemales,
            this.layoutItemNumberOfIndividuals});
         this.layoutGroupPopulationProperties.Location = new System.Drawing.Point(0, 114);
         this.layoutGroupPopulationProperties.Name = "layoutGroupPopulationProperties";
         this.layoutGroupPopulationProperties.Size = new System.Drawing.Size(431, 93);
         // 
         // layoutItemProportionOfFemales
         // 
         this.layoutItemProportionOfFemales.Control = this.tbProportionsOfFemales;
         this.layoutItemProportionOfFemales.CustomizationFormText = "layoutItemProportionOfFemales";
         this.layoutItemProportionOfFemales.Location = new System.Drawing.Point(0, 24);
         this.layoutItemProportionOfFemales.Name = "layoutItemProportionOfFemales";
         this.layoutItemProportionOfFemales.Size = new System.Drawing.Size(407, 24);
         this.layoutItemProportionOfFemales.TextSize = new System.Drawing.Size(153, 13);
         // 
         // layoutItemNumberOfIndividuals
         // 
         this.layoutItemNumberOfIndividuals.Control = this.tbNumberOfIndividuals;
         this.layoutItemNumberOfIndividuals.CustomizationFormText = "layoutItemNumberOfIndividuals";
         this.layoutItemNumberOfIndividuals.Location = new System.Drawing.Point(0, 0);
         this.layoutItemNumberOfIndividuals.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemNumberOfIndividuals.MinSize = new System.Drawing.Size(210, 24);
         this.layoutItemNumberOfIndividuals.Name = "layoutItemNumberOfIndividuals";
         this.layoutItemNumberOfIndividuals.Size = new System.Drawing.Size(407, 24);
         this.layoutItemNumberOfIndividuals.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemNumberOfIndividuals.TextSize = new System.Drawing.Size(153, 13);
         // 
         // layoutGroupParameterRanges
         // 
         this.layoutGroupParameterRanges.CustomizationFormText = "layoutGroupParameterRanges";
         this.layoutGroupParameterRanges.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.emptySpaceItem3});
         this.layoutGroupParameterRanges.Location = new System.Drawing.Point(0, 207);
         this.layoutGroupParameterRanges.Name = "layoutGroupParameterRanges";
         this.layoutGroupParameterRanges.Size = new System.Drawing.Size(431, 175);
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.gridParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Size = new System.Drawing.Size(407, 75);
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextVisible = false;
         // 
         // emptySpaceItem3
         // 
         this.emptySpaceItem3.AllowHotTrack = false;
         this.emptySpaceItem3.Location = new System.Drawing.Point(0, 75);
         this.emptySpaceItem3.Name = "emptySpaceItem3";
         this.emptySpaceItem3.Size = new System.Drawing.Size(407, 55);
         this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemStop
         // 
         this.layoutItemStop.Control = this.btnStop;
         this.layoutItemStop.CustomizationFormText = "layoutItemStop";
         this.layoutItemStop.Location = new System.Drawing.Point(215, 382);
         this.layoutItemStop.Name = "layoutItemStop";
         this.layoutItemStop.Size = new System.Drawing.Size(216, 26);
         this.layoutItemStop.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStop.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 408);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(431, 118);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 382);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(215, 26);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutGroupIndividualSelection
         // 
         this.layoutGroupIndividualSelection.CustomizationFormText = "layoutGroupIndividualSelection";
         this.layoutGroupIndividualSelection.GroupBordersVisible = false;
         this.layoutGroupIndividualSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
         this.layoutGroupIndividualSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupIndividualSelection.Name = "layoutGroupIndividualSelection";
         this.layoutGroupIndividualSelection.Size = new System.Drawing.Size(431, 114);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.tablePanel;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(431, 114);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // RandomPopulationSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(4);
         this.Name = "RandomPopulationSettingsView";
         this.Size = new System.Drawing.Size(451, 546);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.tablePanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbProportionsOfFemales.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividuals.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupPopulationProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProportionOfFemales)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividuals)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupParameterRanges)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStop)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupIndividualSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private DevExpress.XtraEditors.TextEdit tbNumberOfIndividuals;
      private OSPSuite.UI.Controls.UxGridControl gridParameters;
      private UxGridView gridViewParameters;
      private DevExpress.XtraEditors.TextEdit tbProportionsOfFemales;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupPopulationProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupParameterRanges;
      private DevExpress.XtraEditors.LabelControl lblPopulation;
      private DevExpress.XtraEditors.SimpleButton btnStop;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStop;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupIndividualSelection;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraEditors.LabelControl lblDiseaseState;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
      private DevExpress.XtraEditors.LabelControl lblIndividual;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProportionOfFemales;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfIndividuals;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private UxBuildingBlockSelection uxBuildingBlockSelection;
   }
}
