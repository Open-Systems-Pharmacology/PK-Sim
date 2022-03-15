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
         this.uxDiseaseParameter = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbDiseaseState = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this._panelValueOrigin = new DevExpress.XtraEditors.PanelControl();
         this.uxGestationalAge = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.uxBMI = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.cbPopulation = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbGender = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.uxWeight = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.btnMeanValues = new DevExpress.XtraEditors.SimpleButton();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlGroupPopulationProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSubPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCalculationMethods = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGender = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroupPopulationParameters = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemHeight = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlMeanButton = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceAge = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemAge = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemBMI = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceBMI = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemGestationalAge = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemValueOrigin = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWeight = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
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
         ((System.ComponentModel.ISupportInitialize)(this.cbDiseaseState.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._panelValueOrigin)).BeginInit();
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
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlMeanButton)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAge)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAge)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBMI)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBMI)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGestationalAge)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseParameter)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         this.SuspendLayout();
         // 
         // gridCalculationMethods
         // 
         this.gridCalculationMethods.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(5);
         this.gridCalculationMethods.Location = new System.Drawing.Point(206, 181);
         this.gridCalculationMethods.MainView = this.gridViewCalculationMethods;
         this.gridCalculationMethods.Margin = new System.Windows.Forms.Padding(4);
         this.gridCalculationMethods.Name = "gridCalculationMethods";
         this.gridCalculationMethods.Size = new System.Drawing.Size(496, 50);
         this.gridCalculationMethods.TabIndex = 13;
         this.gridCalculationMethods.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewCalculationMethods});
         // 
         // gridViewCalculationMethods
         // 
         this.gridViewCalculationMethods.AllowsFiltering = true;
         this.gridViewCalculationMethods.DetailHeight = 431;
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
         this.uxHeight.Location = new System.Drawing.Point(206, 378);
         this.uxHeight.Margin = new System.Windows.Forms.Padding(6);
         this.uxHeight.MaximumSize = new System.Drawing.Size(11667, 27);
         this.uxHeight.MinimumSize = new System.Drawing.Size(0, 27);
         this.uxHeight.Name = "uxHeight";
         this.uxHeight.Size = new System.Drawing.Size(414, 27);
         this.uxHeight.TabIndex = 9;
         this.uxHeight.ToolTip = "";
         // 
         // gridParameterValueVersions
         // 
         this.gridParameterValueVersions.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(5);
         this.gridParameterValueVersions.Location = new System.Drawing.Point(206, 128);
         this.gridParameterValueVersions.MainView = this.gridViewParameterValueVersions;
         this.gridParameterValueVersions.Margin = new System.Windows.Forms.Padding(4);
         this.gridParameterValueVersions.Name = "gridParameterValueVersions";
         this.gridParameterValueVersions.Size = new System.Drawing.Size(496, 49);
         this.gridParameterValueVersions.TabIndex = 5;
         this.gridParameterValueVersions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameterValueVersions});
         // 
         // gridViewParameterValueVersions
         // 
         this.gridViewParameterValueVersions.AllowsFiltering = true;
         this.gridViewParameterValueVersions.DetailHeight = 431;
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
         this.uxAge.Location = new System.Drawing.Point(206, 285);
         this.uxAge.Margin = new System.Windows.Forms.Padding(6);
         this.uxAge.MinimumSize = new System.Drawing.Size(0, 27);
         this.uxAge.Name = "uxAge";
         this.uxAge.Size = new System.Drawing.Size(414, 27);
         this.uxAge.TabIndex = 7;
         this.uxAge.ToolTip = "";
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(206, 50);
         this.cbSpecies.Margin = new System.Windows.Forms.Padding(4);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(496, 22);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 3;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.uxDiseaseParameter);
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.cbDiseaseState);
         this.layoutControl.Controls.Add(this._panelValueOrigin);
         this.layoutControl.Controls.Add(this.uxGestationalAge);
         this.layoutControl.Controls.Add(this.uxBMI);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Controls.Add(this.cbPopulation);
         this.layoutControl.Controls.Add(this.cbGender);
         this.layoutControl.Controls.Add(this.gridParameterValueVersions);
         this.layoutControl.Controls.Add(this.uxHeight);
         this.layoutControl.Controls.Add(this.gridCalculationMethods);
         this.layoutControl.Controls.Add(this.uxWeight);
         this.layoutControl.Controls.Add(this.uxAge);
         this.layoutControl.Controls.Add(this.btnMeanValues);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutMainGroup;
         this.layoutControl.Size = new System.Drawing.Size(726, 637);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // uxDiseaseParameter
         // 
         this.uxDiseaseParameter.Caption = "";
         this.uxDiseaseParameter.Location = new System.Drawing.Point(206, 556);
         this.uxDiseaseParameter.Margin = new System.Windows.Forms.Padding(6);
         this.uxDiseaseParameter.MinimumSize = new System.Drawing.Size(0, 26);
         this.uxDiseaseParameter.Name = "uxDiseaseParameter";
         this.uxDiseaseParameter.Size = new System.Drawing.Size(496, 27);
         this.uxDiseaseParameter.TabIndex = 18;
         this.uxDiseaseParameter.ToolTip = "";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(24, 587);
         this.lblDescription.Margin = new System.Windows.Forms.Padding(4);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(76, 16);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 17;
         this.lblDescription.Text = "lblDescription";
         // 
         // cbDiseaseState
         // 
         this.cbDiseaseState.Location = new System.Drawing.Point(206, 530);
         this.cbDiseaseState.Margin = new System.Windows.Forms.Padding(4);
         this.cbDiseaseState.Name = "cbDiseaseState";
         this.cbDiseaseState.Properties.AllowMouseWheel = false;
         this.cbDiseaseState.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDiseaseState.Size = new System.Drawing.Size(496, 22);
         this.cbDiseaseState.StyleController = this.layoutControl;
         this.cbDiseaseState.TabIndex = 16;
         // 
         // _panelValueOrigin
         // 
         this._panelValueOrigin.Location = new System.Drawing.Point(206, 440);
         this._panelValueOrigin.Margin = new System.Windows.Forms.Padding(4);
         this._panelValueOrigin.Name = "_panelValueOrigin";
         this._panelValueOrigin.Size = new System.Drawing.Size(414, 26);
         this._panelValueOrigin.TabIndex = 15;
         // 
         // uxGestationalAge
         // 
         this.uxGestationalAge.Caption = "";
         this.uxGestationalAge.Location = new System.Drawing.Point(206, 316);
         this.uxGestationalAge.Margin = new System.Windows.Forms.Padding(6);
         this.uxGestationalAge.MinimumSize = new System.Drawing.Size(0, 27);
         this.uxGestationalAge.Name = "uxGestationalAge";
         this.uxGestationalAge.Size = new System.Drawing.Size(414, 27);
         this.uxGestationalAge.TabIndex = 14;
         this.uxGestationalAge.ToolTip = "";
         // 
         // uxBMI
         // 
         this.uxBMI.Caption = "";
         this.uxBMI.Location = new System.Drawing.Point(206, 409);
         this.uxBMI.Margin = new System.Windows.Forms.Padding(6);
         this.uxBMI.MaximumSize = new System.Drawing.Size(11667, 27);
         this.uxBMI.MinimumSize = new System.Drawing.Size(0, 27);
         this.uxBMI.Name = "uxBMI";
         this.uxBMI.Size = new System.Drawing.Size(414, 27);
         this.uxBMI.TabIndex = 10;
         this.uxBMI.ToolTip = "";
         // 
         // cbPopulation
         // 
         this.cbPopulation.Location = new System.Drawing.Point(206, 76);
         this.cbPopulation.Margin = new System.Windows.Forms.Padding(4);
         this.cbPopulation.Name = "cbPopulation";
         this.cbPopulation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbPopulation.Size = new System.Drawing.Size(496, 22);
         this.cbPopulation.StyleController = this.layoutControl;
         this.cbPopulation.TabIndex = 4;
         // 
         // cbGender
         // 
         this.cbGender.Location = new System.Drawing.Point(206, 102);
         this.cbGender.Margin = new System.Windows.Forms.Padding(4);
         this.cbGender.Name = "cbGender";
         this.cbGender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGender.Size = new System.Drawing.Size(496, 22);
         this.cbGender.StyleController = this.layoutControl;
         this.cbGender.TabIndex = 6;
         // 
         // uxWeight
         // 
         this.uxWeight.Caption = "";
         this.uxWeight.Location = new System.Drawing.Point(206, 347);
         this.uxWeight.Margin = new System.Windows.Forms.Padding(6);
         this.uxWeight.MaximumSize = new System.Drawing.Size(11667, 27);
         this.uxWeight.MinimumSize = new System.Drawing.Size(0, 27);
         this.uxWeight.Name = "uxWeight";
         this.uxWeight.Size = new System.Drawing.Size(414, 27);
         this.uxWeight.TabIndex = 8;
         this.uxWeight.ToolTip = "";
         // 
         // btnMeanValues
         // 
         this.btnMeanValues.Location = new System.Drawing.Point(624, 347);
         this.btnMeanValues.Margin = new System.Windows.Forms.Padding(4);
         this.btnMeanValues.Name = "btnMeanValues";
         this.btnMeanValues.Size = new System.Drawing.Size(78, 58);
         this.btnMeanValues.StyleController = this.layoutControl;
         this.btnMeanValues.TabIndex = 10;
         this.btnMeanValues.Text = "Mean";
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
         this.layoutMainGroup.Size = new System.Drawing.Size(726, 637);
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
         this.layoutControlGroupPopulationProperties.Size = new System.Drawing.Size(706, 235);
         // 
         // layoutItemSubPopulation
         // 
         this.layoutItemSubPopulation.AllowHide = false;
         this.layoutItemSubPopulation.Control = this.gridParameterValueVersions;
         this.layoutItemSubPopulation.CustomizationFormText = "layoutItemSubPopulation";
         this.layoutItemSubPopulation.Location = new System.Drawing.Point(0, 78);
         this.layoutItemSubPopulation.Name = "layoutItemSubPopulation";
         this.layoutItemSubPopulation.Size = new System.Drawing.Size(682, 53);
         this.layoutItemSubPopulation.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemCalculationMethods
         // 
         this.layoutItemCalculationMethods.AllowHide = false;
         this.layoutItemCalculationMethods.Control = this.gridCalculationMethods;
         this.layoutItemCalculationMethods.CustomizationFormText = "layoutItemCalculationMethods";
         this.layoutItemCalculationMethods.Location = new System.Drawing.Point(0, 131);
         this.layoutItemCalculationMethods.Name = "layoutItemCalculationMethods";
         this.layoutItemCalculationMethods.Size = new System.Drawing.Size(682, 54);
         this.layoutItemCalculationMethods.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.CustomizationFormText = "layoutItemSpecies";
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemCreature";
         this.layoutItemSpecies.Size = new System.Drawing.Size(682, 26);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemGender
         // 
         this.layoutItemGender.AllowHide = false;
         this.layoutItemGender.Control = this.cbGender;
         this.layoutItemGender.CustomizationFormText = "layoutItemGender";
         this.layoutItemGender.Location = new System.Drawing.Point(0, 52);
         this.layoutItemGender.Name = "layoutItemGender";
         this.layoutItemGender.Size = new System.Drawing.Size(682, 26);
         this.layoutItemGender.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemPopulation
         // 
         this.layoutItemPopulation.Control = this.cbPopulation;
         this.layoutItemPopulation.CustomizationFormText = "layoutItemPopulation";
         this.layoutItemPopulation.Location = new System.Drawing.Point(0, 26);
         this.layoutItemPopulation.Name = "layoutItemPopulation";
         this.layoutItemPopulation.Size = new System.Drawing.Size(682, 26);
         this.layoutItemPopulation.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutControlGroupPopulationParameters
         // 
         this.layoutControlGroupPopulationParameters.CustomizationFormText = "layoutControlGroup3";
         this.layoutControlGroupPopulationParameters.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemHeight,
            this.layoutControlMeanButton,
            this.emptySpaceAge,
            this.layoutItemAge,
            this.layoutItemBMI,
            this.emptySpaceBMI,
            this.layoutItemGestationalAge,
            this.layoutItemValueOrigin,
            this.layoutItemWeight,
            this.emptySpaceItem2});
         this.layoutControlGroupPopulationParameters.Location = new System.Drawing.Point(0, 235);
         this.layoutControlGroupPopulationParameters.Name = "layoutControlGroupPopulationParameters";
         this.layoutControlGroupPopulationParameters.Size = new System.Drawing.Size(706, 245);
         // 
         // layoutItemHeight
         // 
         this.layoutItemHeight.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
         this.layoutItemHeight.Control = this.uxHeight;
         this.layoutItemHeight.CustomizationFormText = "layoutItemHeight";
         this.layoutItemHeight.Location = new System.Drawing.Point(0, 93);
         this.layoutItemHeight.MinSize = new System.Drawing.Size(314, 31);
         this.layoutItemHeight.Name = "layoutItemHeight";
         this.layoutItemHeight.Size = new System.Drawing.Size(600, 31);
         this.layoutItemHeight.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemHeight.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutControlMeanButton
         // 
         this.layoutControlMeanButton.Control = this.btnMeanValues;
         this.layoutControlMeanButton.CustomizationFormText = "layoutControlMeanButton";
         this.layoutControlMeanButton.Location = new System.Drawing.Point(600, 62);
         this.layoutControlMeanButton.MinSize = new System.Drawing.Size(1, 1);
         this.layoutControlMeanButton.Name = "layoutControlMeanButton";
         this.layoutControlMeanButton.Size = new System.Drawing.Size(82, 62);
         this.layoutControlMeanButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutControlMeanButton.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlMeanButton.TextVisible = false;
         // 
         // emptySpaceAge
         // 
         this.emptySpaceAge.AllowHotTrack = false;
         this.emptySpaceAge.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceAge.Location = new System.Drawing.Point(600, 0);
         this.emptySpaceAge.Name = "emptySpaceAge";
         this.emptySpaceAge.Size = new System.Drawing.Size(82, 62);
         this.emptySpaceAge.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemAge
         // 
         this.layoutItemAge.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
         this.layoutItemAge.Control = this.uxAge;
         this.layoutItemAge.CustomizationFormText = "layoutItemAge";
         this.layoutItemAge.Location = new System.Drawing.Point(0, 0);
         this.layoutItemAge.Name = "layoutItemAge";
         this.layoutItemAge.Size = new System.Drawing.Size(600, 31);
         this.layoutItemAge.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemBMI
         // 
         this.layoutItemBMI.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
         this.layoutItemBMI.Control = this.uxBMI;
         this.layoutItemBMI.CustomizationFormText = "layoutItemBMI";
         this.layoutItemBMI.Location = new System.Drawing.Point(0, 124);
         this.layoutItemBMI.MinSize = new System.Drawing.Size(314, 31);
         this.layoutItemBMI.Name = "layoutItemBMI";
         this.layoutItemBMI.Size = new System.Drawing.Size(600, 31);
         this.layoutItemBMI.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemBMI.TextSize = new System.Drawing.Size(170, 16);
         // 
         // emptySpaceBMI
         // 
         this.emptySpaceBMI.AllowHotTrack = false;
         this.emptySpaceBMI.CustomizationFormText = "emptySpaceBMI";
         this.emptySpaceBMI.Location = new System.Drawing.Point(600, 124);
         this.emptySpaceBMI.Name = "emptySpaceBMI";
         this.emptySpaceBMI.Size = new System.Drawing.Size(82, 61);
         this.emptySpaceBMI.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemGestationalAge
         // 
         this.layoutItemGestationalAge.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
         this.layoutItemGestationalAge.Control = this.uxGestationalAge;
         this.layoutItemGestationalAge.CustomizationFormText = "layoutItemGestationalAge";
         this.layoutItemGestationalAge.Location = new System.Drawing.Point(0, 31);
         this.layoutItemGestationalAge.Name = "layoutItemGestationalAge";
         this.layoutItemGestationalAge.Size = new System.Drawing.Size(600, 31);
         this.layoutItemGestationalAge.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemValueOrigin
         // 
         this.layoutItemValueOrigin.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
         this.layoutItemValueOrigin.Control = this._panelValueOrigin;
         this.layoutItemValueOrigin.Location = new System.Drawing.Point(0, 155);
         this.layoutItemValueOrigin.Name = "layoutItemValueOrigin";
         this.layoutItemValueOrigin.Size = new System.Drawing.Size(600, 30);
         this.layoutItemValueOrigin.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemWeight
         // 
         this.layoutItemWeight.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
         this.layoutItemWeight.Control = this.uxWeight;
         this.layoutItemWeight.CustomizationFormText = "layoutItemWeight";
         this.layoutItemWeight.Location = new System.Drawing.Point(0, 62);
         this.layoutItemWeight.Name = "layoutItemWeight";
         this.layoutItemWeight.Size = new System.Drawing.Size(600, 31);
         this.layoutItemWeight.TextSize = new System.Drawing.Size(170, 16);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 185);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(682, 10);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutGroupDiseaseState
         // 
         this.layoutGroupDiseaseState.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDiseaseState,
            this.layoutItemDescription,
            this.layoutItemDiseaseParameter});
         this.layoutGroupDiseaseState.Location = new System.Drawing.Point(0, 480);
         this.layoutGroupDiseaseState.Name = "layoutGroupDiseaseState";
         this.layoutGroupDiseaseState.Size = new System.Drawing.Size(706, 127);
         // 
         // layoutItemDiseaseState
         // 
         this.layoutItemDiseaseState.Control = this.cbDiseaseState;
         this.layoutItemDiseaseState.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDiseaseState.Name = "layoutItemDiseaseState";
         this.layoutItemDiseaseState.Size = new System.Drawing.Size(682, 26);
         this.layoutItemDiseaseState.TextSize = new System.Drawing.Size(170, 16);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 57);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(682, 20);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutItemDiseaseParameter
         // 
         this.layoutItemDiseaseParameter.Control = this.uxDiseaseParameter;
         this.layoutItemDiseaseParameter.Location = new System.Drawing.Point(0, 26);
         this.layoutItemDiseaseParameter.MaxSize = new System.Drawing.Size(11845, 32);
         this.layoutItemDiseaseParameter.MinSize = new System.Drawing.Size(309, 31);
         this.layoutItemDiseaseParameter.Name = "layoutItemDiseaseParameter";
         this.layoutItemDiseaseParameter.Size = new System.Drawing.Size(682, 31);
         this.layoutItemDiseaseParameter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemDiseaseParameter.TextSize = new System.Drawing.Size(170, 16);
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 607);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(706, 10);
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // IndividualSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5);
         this.Name = "IndividualSettingsView";
         this.Size = new System.Drawing.Size(726, 637);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValueVersions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValueVersions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbDiseaseState.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._panelValueOrigin)).EndInit();
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
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlMeanButton)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAge)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAge)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBMI)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBMI)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGestationalAge)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
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
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWeight;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemHeight;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlMeanButton;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAge;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceAge;
      private UxParameterDTOEdit uxBMI;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemBMI;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceBMI;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemGestationalAge;
      private DevExpress.XtraEditors.PanelControl _panelValueOrigin;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemValueOrigin;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupDiseaseState;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDiseaseState;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseState;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private UxParameterDTOEdit uxDiseaseParameter;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseParameter;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
   }
}


