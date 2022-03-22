using PKSim.UI.Views.Core;

using PKSim.UI.Views.Parameters;

namespace PKSim.UI.Views.Individuals
{
   partial class IndividualSettingsView
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
         _settingsBinder.Dispose();
         _parameterBinder.Dispose();
         _diseaseStateBinder.Dispose();
         _gridParameterValueVersionsBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.gridCalculationMethods = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewCalculationMethods = new PKSim.UI.Views.Core.UxGridView();
         this.uxHeight = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.gridParameterValueVersions = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewParameterValueVersions = new PKSim.UI.Views.Core.UxGridView();
         this.uxAge = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tablePanel = new DevExpress.Utils.Layout.TablePanel();
         this.labelValueOrigin = new DevExpress.XtraEditors.LabelControl();
         this.labelBMI = new DevExpress.XtraEditors.LabelControl();
         this.labelHeight = new DevExpress.XtraEditors.LabelControl();
         this.labelWeight = new DevExpress.XtraEditors.LabelControl();
         this.labelGestationalAge = new DevExpress.XtraEditors.LabelControl();
         this.labelAge = new DevExpress.XtraEditors.LabelControl();
         this.uxGestationalAge = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.uxWeight = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.uxBMI = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.btnMeanValues = new DevExpress.XtraEditors.SimpleButton();
         this.uxDiseaseParameter = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbDiseaseState = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbPopulation = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbGender = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlGroupPopulationProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSubPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCalculationMethods = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGender = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroupPopulationParameters = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupDiseaseState = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDiseaseState = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDiseaseParameter = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridCalculationMethods)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewCalculationMethods)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValueVersions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValueVersions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         this.tablePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbDiseaseState.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPopulation.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGender.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSubPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCalculationMethods)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGender)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseParameter)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         this.SuspendLayout();
         // 
         // gridCalculationMethods
         // 
         this.gridCalculationMethods.Location = new System.Drawing.Point(181, 141);
         this.gridCalculationMethods.MainView = this.gridViewCalculationMethods;
         this.gridCalculationMethods.Name = "gridCalculationMethods";
         this.gridCalculationMethods.Size = new System.Drawing.Size(417, 20);
         this.gridCalculationMethods.TabIndex = 13;
         this.gridCalculationMethods.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCalculationMethods});
         // 
         // gridViewCalculationMethods
         // 
         this.gridViewCalculationMethods.AllowsFiltering = true;
         this.gridViewCalculationMethods.EnableColumnContextMenu = true;
         this.gridViewCalculationMethods.GridControl = this.gridCalculationMethods;
         this.gridViewCalculationMethods.MultiSelect = false;
         this.gridViewCalculationMethods.Name = "gridViewCalculationMethods";
         this.gridViewCalculationMethods.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridViewCalculationMethods.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewCalculationMethods.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewCalculationMethods.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // uxHeight
         // 
         this.uxHeight.Caption = "";
         this.tablePanel.SetColumn(this.uxHeight, 1);
         this.uxHeight.Location = new System.Drawing.Point(107, 80);
         this.uxHeight.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
         this.uxHeight.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxHeight.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxHeight.Name = "uxHeight";
         this.tablePanel.SetRow(this.uxHeight, 3);
         this.uxHeight.Size = new System.Drawing.Size(393, 22);
         this.uxHeight.TabIndex = 9;
         this.uxHeight.ToolTip = "";
         // 
         // gridParameterValueVersions
         // 
         this.gridParameterValueVersions.Location = new System.Drawing.Point(181, 117);
         this.gridParameterValueVersions.MainView = this.gridViewParameterValueVersions;
         this.gridParameterValueVersions.Name = "gridParameterValueVersions";
         this.gridParameterValueVersions.Size = new System.Drawing.Size(417, 20);
         this.gridParameterValueVersions.TabIndex = 5;
         this.gridParameterValueVersions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameterValueVersions});
         // 
         // gridViewParameterValueVersions
         // 
         this.gridViewParameterValueVersions.AllowsFiltering = true;
         this.gridViewParameterValueVersions.EnableColumnContextMenu = true;
         this.gridViewParameterValueVersions.GridControl = this.gridParameterValueVersions;
         this.gridViewParameterValueVersions.MultiSelect = false;
         this.gridViewParameterValueVersions.Name = "gridViewParameterValueVersions";
         this.gridViewParameterValueVersions.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridViewParameterValueVersions.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewParameterValueVersions.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewParameterValueVersions.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // uxAge
         // 
         this.uxAge.Caption = "";
         this.tablePanel.SetColumn(this.uxAge, 1);
         this.uxAge.Location = new System.Drawing.Point(107, 2);
         this.uxAge.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
         this.uxAge.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxAge.Name = "uxAge";
         this.tablePanel.SetRow(this.uxAge, 0);
         this.uxAge.Size = new System.Drawing.Size(393, 22);
         this.uxAge.TabIndex = 7;
         this.uxAge.ToolTip = "";
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(181, 45);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(417, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 3;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.tablePanel);
         this.layoutControl.Controls.Add(this.uxDiseaseParameter);
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.cbDiseaseState);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Controls.Add(this.cbPopulation);
         this.layoutControl.Controls.Add(this.cbGender);
         this.layoutControl.Controls.Add(this.gridParameterValueVersions);
         this.layoutControl.Controls.Add(this.gridCalculationMethods);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutMainGroup;
         this.layoutControl.Size = new System.Drawing.Size(622, 518);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // tablePanel
         // 
         this.tablePanel.Columns.AddRange(new DevExpress.Utils.Layout.TablePanelColumn[] {
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.AutoSize, 28.91F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 69.39F),
            new DevExpress.Utils.Layout.TablePanelColumn(DevExpress.Utils.Layout.TablePanelEntityStyle.Relative, 11.7F)});
         this.tablePanel.Controls.Add(this.labelValueOrigin);
         this.tablePanel.Controls.Add(this.labelBMI);
         this.tablePanel.Controls.Add(this.labelHeight);
         this.tablePanel.Controls.Add(this.labelWeight);
         this.tablePanel.Controls.Add(this.labelGestationalAge);
         this.tablePanel.Controls.Add(this.labelAge);
         this.tablePanel.Controls.Add(this.uxAge);
         this.tablePanel.Controls.Add(this.uxGestationalAge);
         this.tablePanel.Controls.Add(this.uxWeight);
         this.tablePanel.Controls.Add(this.uxHeight);
         this.tablePanel.Controls.Add(this.uxBMI);
         this.tablePanel.Controls.Add(this.btnMeanValues);
         this.tablePanel.Location = new System.Drawing.Point(24, 210);
         this.tablePanel.Name = "tablePanel";
         this.tablePanel.Rows.AddRange(new DevExpress.Utils.Layout.TablePanelRow[] {
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 24F),
            new DevExpress.Utils.Layout.TablePanelRow(DevExpress.Utils.Layout.TablePanelEntityStyle.Absolute, 26F)});
         this.tablePanel.Size = new System.Drawing.Size(574, 163);
         this.tablePanel.TabIndex = 19;
         // 
         // labelValueOrigin
         // 
         this.tablePanel.SetColumn(this.labelValueOrigin, 0);
         this.labelValueOrigin.Location = new System.Drawing.Point(3, 135);
         this.labelValueOrigin.Name = "labelValueOrigin";
         this.tablePanel.SetRow(this.labelValueOrigin, 5);
         this.labelValueOrigin.Size = new System.Drawing.Size(76, 13);
         this.labelValueOrigin.TabIndex = 21;
         this.labelValueOrigin.Text = "labelValueOrigin";
         // 
         // labelBMI
         // 
         this.tablePanel.SetColumn(this.labelBMI, 0);
         this.labelBMI.Location = new System.Drawing.Point(3, 110);
         this.labelBMI.Name = "labelBMI";
         this.tablePanel.SetRow(this.labelBMI, 4);
         this.labelBMI.Size = new System.Drawing.Size(40, 13);
         this.labelBMI.TabIndex = 20;
         this.labelBMI.Text = "labelBMI";
         // 
         // labelHeight
         // 
         this.tablePanel.SetColumn(this.labelHeight, 0);
         this.labelHeight.Location = new System.Drawing.Point(3, 84);
         this.labelHeight.Name = "labelHeight";
         this.tablePanel.SetRow(this.labelHeight, 3);
         this.labelHeight.Size = new System.Drawing.Size(53, 13);
         this.labelHeight.TabIndex = 19;
         this.labelHeight.Text = "labelHeight";
         // 
         // labelWeight
         // 
         this.tablePanel.SetColumn(this.labelWeight, 0);
         this.labelWeight.Location = new System.Drawing.Point(3, 58);
         this.labelWeight.Name = "labelWeight";
         this.tablePanel.SetRow(this.labelWeight, 2);
         this.labelWeight.Size = new System.Drawing.Size(56, 13);
         this.labelWeight.TabIndex = 18;
         this.labelWeight.Text = "labelWeight";
         // 
         // labelGestationalAge
         // 
         this.tablePanel.SetColumn(this.labelGestationalAge, 0);
         this.labelGestationalAge.Location = new System.Drawing.Point(3, 32);
         this.labelGestationalAge.Name = "labelGestationalAge";
         this.tablePanel.SetRow(this.labelGestationalAge, 1);
         this.labelGestationalAge.Size = new System.Drawing.Size(95, 13);
         this.labelGestationalAge.TabIndex = 17;
         this.labelGestationalAge.Text = "labelGestationalAge";
         // 
         // labelAge
         // 
         this.tablePanel.SetColumn(this.labelAge, 0);
         this.labelAge.Location = new System.Drawing.Point(3, 6);
         this.labelAge.Name = "labelAge";
         this.tablePanel.SetRow(this.labelAge, 0);
         this.labelAge.Size = new System.Drawing.Size(41, 13);
         this.labelAge.TabIndex = 16;
         this.labelAge.Text = "labelAge";
         // 
         // uxGestationalAge
         // 
         this.uxGestationalAge.Caption = "";
         this.tablePanel.SetColumn(this.uxGestationalAge, 1);
         this.uxGestationalAge.Location = new System.Drawing.Point(107, 28);
         this.uxGestationalAge.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
         this.uxGestationalAge.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxGestationalAge.Name = "uxGestationalAge";
         this.tablePanel.SetRow(this.uxGestationalAge, 1);
         this.uxGestationalAge.Size = new System.Drawing.Size(393, 22);
         this.uxGestationalAge.TabIndex = 14;
         this.uxGestationalAge.ToolTip = "";
         // 
         // uxWeight
         // 
         this.uxWeight.Caption = "";
         this.tablePanel.SetColumn(this.uxWeight, 1);
         this.uxWeight.Location = new System.Drawing.Point(107, 54);
         this.uxWeight.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
         this.uxWeight.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxWeight.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxWeight.Name = "uxWeight";
         this.tablePanel.SetRow(this.uxWeight, 2);
         this.uxWeight.Size = new System.Drawing.Size(393, 22);
         this.uxWeight.TabIndex = 8;
         this.uxWeight.ToolTip = "";
         // 
         // uxBMI
         // 
         this.uxBMI.Caption = "";
         this.tablePanel.SetColumn(this.uxBMI, 1);
         this.uxBMI.Location = new System.Drawing.Point(107, 106);
         this.uxBMI.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
         this.uxBMI.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxBMI.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxBMI.Name = "uxBMI";
         this.tablePanel.SetRow(this.uxBMI, 4);
         this.uxBMI.Size = new System.Drawing.Size(393, 22);
         this.uxBMI.TabIndex = 10;
         this.uxBMI.ToolTip = "";
         // 
         // btnMeanValues
         // 
         this.tablePanel.SetColumn(this.btnMeanValues, 2);
         this.btnMeanValues.Location = new System.Drawing.Point(509, 55);
         this.btnMeanValues.Name = "btnMeanValues";
         this.tablePanel.SetRow(this.btnMeanValues, 2);
         this.tablePanel.SetRowSpan(this.btnMeanValues, 2);
         this.btnMeanValues.Size = new System.Drawing.Size(62, 46);
         this.btnMeanValues.TabIndex = 10;
         this.btnMeanValues.Text = "Mean";
         // 
         // uxDiseaseParameter
         // 
         this.uxDiseaseParameter.Caption = "";
         this.uxDiseaseParameter.Location = new System.Drawing.Point(181, 446);
         this.uxDiseaseParameter.Margin = new System.Windows.Forms.Padding(5);
         this.uxDiseaseParameter.MinimumSize = new System.Drawing.Size(0, 21);
         this.uxDiseaseParameter.Name = "uxDiseaseParameter";
         this.uxDiseaseParameter.Size = new System.Drawing.Size(417, 21);
         this.uxDiseaseParameter.TabIndex = 18;
         this.uxDiseaseParameter.ToolTip = "";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(24, 471);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 17;
         this.lblDescription.Text = "lblDescription";
         // 
         // cbDiseaseState
         // 
         this.cbDiseaseState.Location = new System.Drawing.Point(181, 422);
         this.cbDiseaseState.Name = "cbDiseaseState";
         this.cbDiseaseState.Properties.AllowMouseWheel = false;
         this.cbDiseaseState.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDiseaseState.Size = new System.Drawing.Size(417, 20);
         this.cbDiseaseState.StyleController = this.layoutControl;
         this.cbDiseaseState.TabIndex = 16;
         // 
         // cbPopulation
         // 
         this.cbPopulation.Location = new System.Drawing.Point(181, 69);
         this.cbPopulation.Name = "cbPopulation";
         this.cbPopulation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbPopulation.Size = new System.Drawing.Size(417, 20);
         this.cbPopulation.StyleController = this.layoutControl;
         this.cbPopulation.TabIndex = 4;
         // 
         // cbGender
         // 
         this.cbGender.Location = new System.Drawing.Point(181, 93);
         this.cbGender.Name = "cbGender";
         this.cbGender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGender.Size = new System.Drawing.Size(417, 20);
         this.cbGender.StyleController = this.layoutControl;
         this.cbGender.TabIndex = 6;
         // 
         // layoutMainGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutControlGroup2";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroupPopulationProperties,
            this.layoutControlGroupPopulationParameters,
            this.layoutGroupDiseaseState,
            this.emptySpaceItem});
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Size = new System.Drawing.Size(622, 518);
         this.layoutMainGroup.TextVisible = false;
         // 
         // layoutControlGroupPopulationProperties
         // 
         this.layoutControlGroupPopulationProperties.CustomizationFormText = "layoutControlGroup3";
         this.layoutControlGroupPopulationProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemSubPopulation,
            this.layoutItemCalculationMethods,
            this.layoutItemSpecies,
            this.layoutItemGender,
            this.layoutItemPopulation});
         this.layoutControlGroupPopulationProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroupPopulationProperties.Name = "layoutControlGroupPopulationProperties";
         this.layoutControlGroupPopulationProperties.Size = new System.Drawing.Size(602, 165);
         // 
         // layoutItemSubPopulation
         // 
         this.layoutItemSubPopulation.AllowHide = false;
         this.layoutItemSubPopulation.Control = this.gridParameterValueVersions;
         this.layoutItemSubPopulation.CustomizationFormText = "layoutItemSubPopulation";
         this.layoutItemSubPopulation.Location = new System.Drawing.Point(0, 72);
         this.layoutItemSubPopulation.Name = "layoutItemSubPopulation";
         this.layoutItemSubPopulation.Size = new System.Drawing.Size(578, 24);
         this.layoutItemSubPopulation.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemCalculationMethods
         // 
         this.layoutItemCalculationMethods.AllowHide = false;
         this.layoutItemCalculationMethods.Control = this.gridCalculationMethods;
         this.layoutItemCalculationMethods.CustomizationFormText = "layoutItemCalculationMethods";
         this.layoutItemCalculationMethods.Location = new System.Drawing.Point(0, 96);
         this.layoutItemCalculationMethods.Name = "layoutItemCalculationMethods";
         this.layoutItemCalculationMethods.Size = new System.Drawing.Size(578, 24);
         this.layoutItemCalculationMethods.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.CustomizationFormText = "layoutItemSpecies";
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemCreature";
         this.layoutItemSpecies.Size = new System.Drawing.Size(578, 24);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemGender
         // 
         this.layoutItemGender.AllowHide = false;
         this.layoutItemGender.Control = this.cbGender;
         this.layoutItemGender.CustomizationFormText = "layoutItemGender";
         this.layoutItemGender.Location = new System.Drawing.Point(0, 48);
         this.layoutItemGender.Name = "layoutItemGender";
         this.layoutItemGender.Size = new System.Drawing.Size(578, 24);
         this.layoutItemGender.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemPopulation
         // 
         this.layoutItemPopulation.Control = this.cbPopulation;
         this.layoutItemPopulation.CustomizationFormText = "layoutItemPopulation";
         this.layoutItemPopulation.Location = new System.Drawing.Point(0, 24);
         this.layoutItemPopulation.Name = "layoutItemPopulation";
         this.layoutItemPopulation.Size = new System.Drawing.Size(578, 24);
         this.layoutItemPopulation.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutControlGroupPopulationParameters
         // 
         this.layoutControlGroupPopulationParameters.CustomizationFormText = "layoutControlGroup3";
         this.layoutControlGroupPopulationParameters.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1});
         this.layoutControlGroupPopulationParameters.Location = new System.Drawing.Point(0, 165);
         this.layoutControlGroupPopulationParameters.Name = "layoutControlGroupPopulationParameters";
         this.layoutControlGroupPopulationParameters.Size = new System.Drawing.Size(602, 212);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.tablePanel;
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(578, 167);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutGroupDiseaseState
         // 
         this.layoutGroupDiseaseState.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDiseaseState,
            this.layoutItemDescription,
            this.layoutItemDiseaseParameter});
         this.layoutGroupDiseaseState.Location = new System.Drawing.Point(0, 377);
         this.layoutGroupDiseaseState.Name = "layoutGroupDiseaseState";
         this.layoutGroupDiseaseState.Size = new System.Drawing.Size(602, 111);
         // 
         // layoutItemDiseaseState
         // 
         this.layoutItemDiseaseState.Control = this.cbDiseaseState;
         this.layoutItemDiseaseState.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDiseaseState.Name = "layoutItemDiseaseState";
         this.layoutItemDiseaseState.Size = new System.Drawing.Size(578, 24);
         this.layoutItemDiseaseState.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 49);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(578, 17);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutItemDiseaseParameter
         // 
         this.layoutItemDiseaseParameter.Control = this.uxDiseaseParameter;
         this.layoutItemDiseaseParameter.Location = new System.Drawing.Point(0, 24);
         this.layoutItemDiseaseParameter.MaxSize = new System.Drawing.Size(10153, 26);
         this.layoutItemDiseaseParameter.MinSize = new System.Drawing.Size(265, 25);
         this.layoutItemDiseaseParameter.Name = "layoutItemDiseaseParameter";
         this.layoutItemDiseaseParameter.Size = new System.Drawing.Size(578, 25);
         this.layoutItemDiseaseParameter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemDiseaseParameter.TextSize = new System.Drawing.Size(145, 13);
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 488);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(602, 10);
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // IndividualSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(4);
         this.Name = "IndividualSettingsView";
         this.Size = new System.Drawing.Size(622, 518);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValueVersions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValueVersions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         this.tablePanel.ResumeLayout(false);
         this.tablePanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbDiseaseState.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPopulation.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGender.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSubPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGender)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDiseaseState)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseParameter)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxComboBoxEdit cbPopulation;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbGender;
      private DevExpress.XtraEditors.SimpleButton btnMeanValues;
      private PKSim.UI.Views.Core.UxImageComboBoxEdit cbSpecies;
      private OSPSuite.UI.Controls.UxGridControl gridParameterValueVersions;
      private UxGridView gridViewParameterValueVersions;
      private UxParameterDTOEdit uxAge;
      private UxParameterDTOEdit uxHeight;
      private UxParameterDTOEdit uxWeight;
      private UxParameterDTOEdit uxGestationalAge;
      private UxGridView gridViewCalculationMethods;
      private OSPSuite.UI.Controls.UxGridControl gridCalculationMethods;
      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSpecies;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPopulation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGender;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupPopulationProperties;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSubPopulation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCalculationMethods;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroupPopulationParameters;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private UxParameterDTOEdit uxBMI;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupDiseaseState;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDiseaseState;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseState;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private UxParameterDTOEdit uxDiseaseParameter;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseParameter;
      private DevExpress.Utils.Layout.TablePanel tablePanel;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraEditors.LabelControl labelBMI;
      private DevExpress.XtraEditors.LabelControl labelHeight;
      private DevExpress.XtraEditors.LabelControl labelWeight;
      private DevExpress.XtraEditors.LabelControl labelGestationalAge;
      private DevExpress.XtraEditors.LabelControl labelAge;
      private DevExpress.XtraEditors.LabelControl labelValueOrigin;
   }
}


