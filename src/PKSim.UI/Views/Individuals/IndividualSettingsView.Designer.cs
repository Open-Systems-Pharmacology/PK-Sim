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
         this.cbSpecies = new DevExpress.XtraEditors.ImageComboBoxEdit();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this._panelValueOrigin = new DevExpress.XtraEditors.PanelControl();
         this.uxGestationalAge = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.uxBMI = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.cbPopulation = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbGender = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.uxWeight = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.btnMeanValues = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlGroupPopulationProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemSubPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCalculationMethods = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemGender = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlGroupPopulationParameters = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemWeight = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemHeight = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlMeanButton = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceAge = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemAge = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemBMI = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceBMI = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemGestationalAge = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemValueOrigin = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridCalculationMethods)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewCalculationMethods)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValueVersions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValueVersions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._panelValueOrigin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPopulation.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGender.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSubPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCalculationMethods)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGender)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHeight)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlMeanButton)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAge)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAge)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBMI)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBMI)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGestationalAge)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // gridCalculationMethods
         // 
         this.gridCalculationMethods.Location = new System.Drawing.Point(172, 147);
         this.gridCalculationMethods.MainView = this.gridViewCalculationMethods;
         this.gridCalculationMethods.Name = "gridCalculationMethods";
         this.gridCalculationMethods.Size = new System.Drawing.Size(320, 38);
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
         this.uxHeight.Location = new System.Drawing.Point(172, 303);
         this.uxHeight.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxHeight.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxHeight.Name = "uxHeight";
         this.uxHeight.Size = new System.Drawing.Size(264, 22);
         this.uxHeight.TabIndex = 9;
         this.uxHeight.ToolTip = "";
         // 
         // gridParameterValueVersions
         // 
         this.gridParameterValueVersions.Location = new System.Drawing.Point(172, 114);
         this.gridParameterValueVersions.MainView = this.gridViewParameterValueVersions;
         this.gridParameterValueVersions.Name = "gridParameterValueVersions";
         this.gridParameterValueVersions.Size = new System.Drawing.Size(320, 29);
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
         this.uxAge.Location = new System.Drawing.Point(172, 231);
         this.uxAge.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxAge.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxAge.Name = "uxAge";
         this.uxAge.Size = new System.Drawing.Size(264, 22);
         this.uxAge.TabIndex = 7;
         this.uxAge.ToolTip = "";
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(172, 42);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(320, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 3;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
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
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup2;
         this.layoutControl.Size = new System.Drawing.Size(516, 410);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // _panelValueOrigin
         // 
         this._panelValueOrigin.Location = new System.Drawing.Point(172, 351);
         this._panelValueOrigin.Name = "_panelValueOrigin";
         this._panelValueOrigin.Size = new System.Drawing.Size(264, 13);
         this._panelValueOrigin.TabIndex = 15;
         // 
         // uxGestationalAge
         // 
         this.uxGestationalAge.Caption = "";
         this.uxGestationalAge.Location = new System.Drawing.Point(172, 255);
         this.uxGestationalAge.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxGestationalAge.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxGestationalAge.Name = "uxGestationalAge";
         this.uxGestationalAge.Size = new System.Drawing.Size(264, 22);
         this.uxGestationalAge.TabIndex = 14;
         this.uxGestationalAge.ToolTip = "";
         // 
         // uxBMI
         // 
         this.uxBMI.Caption = "";
         this.uxBMI.Location = new System.Drawing.Point(172, 327);
         this.uxBMI.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxBMI.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxBMI.Name = "uxBMI";
         this.uxBMI.Size = new System.Drawing.Size(264, 22);
         this.uxBMI.TabIndex = 10;
         this.uxBMI.ToolTip = "";
         // 
         // cbPopulation
         // 
         this.cbPopulation.Location = new System.Drawing.Point(172, 66);
         this.cbPopulation.Name = "cbPopulation";
         this.cbPopulation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbPopulation.Size = new System.Drawing.Size(320, 20);
         this.cbPopulation.StyleController = this.layoutControl;
         this.cbPopulation.TabIndex = 4;
         // 
         // cbGender
         // 
         this.cbGender.Location = new System.Drawing.Point(172, 90);
         this.cbGender.Name = "cbGender";
         this.cbGender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbGender.Size = new System.Drawing.Size(320, 20);
         this.cbGender.StyleController = this.layoutControl;
         this.cbGender.TabIndex = 6;
         // 
         // uxWeight
         // 
         this.uxWeight.Caption = "";
         this.uxWeight.Location = new System.Drawing.Point(172, 279);
         this.uxWeight.MaximumSize = new System.Drawing.Size(10000, 22);
         this.uxWeight.MinimumSize = new System.Drawing.Size(0, 22);
         this.uxWeight.Name = "uxWeight";
         this.uxWeight.Size = new System.Drawing.Size(264, 22);
         this.uxWeight.TabIndex = 8;
         this.uxWeight.ToolTip = "";
         // 
         // btnMeanValues
         // 
         this.btnMeanValues.Location = new System.Drawing.Point(440, 279);
         this.btnMeanValues.Name = "btnMeanValues";
         this.btnMeanValues.Size = new System.Drawing.Size(52, 44);
         this.btnMeanValues.StyleController = this.layoutControl;
         this.btnMeanValues.TabIndex = 10;
         this.btnMeanValues.Text = "Mean";
         // 
         // layoutControlGroup2
         // 
         this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
         this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup2.GroupBordersVisible = false;
         this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlGroupPopulationProperties,
            this.layoutControlGroupPopulationParameters,
            this.emptySpaceItem1});
         this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup2.Name = "layoutControlGroup2";
         this.layoutControlGroup2.Size = new System.Drawing.Size(516, 410);
         this.layoutControlGroup2.TextVisible = false;
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
         this.layoutControlGroupPopulationProperties.Size = new System.Drawing.Size(496, 189);
         // 
         // layoutItemSubPopulation
         // 
         this.layoutItemSubPopulation.AllowHide = false;
         this.layoutItemSubPopulation.Control = this.gridParameterValueVersions;
         this.layoutItemSubPopulation.CustomizationFormText = "layoutItemSubPopulation";
         this.layoutItemSubPopulation.Location = new System.Drawing.Point(0, 72);
         this.layoutItemSubPopulation.Name = "layoutItemSubPopulation";
         this.layoutItemSubPopulation.Size = new System.Drawing.Size(472, 33);
         this.layoutItemSubPopulation.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemCalculationMethods
         // 
         this.layoutItemCalculationMethods.AllowHide = false;
         this.layoutItemCalculationMethods.Control = this.gridCalculationMethods;
         this.layoutItemCalculationMethods.CustomizationFormText = "layoutItemCalculationMethods";
         this.layoutItemCalculationMethods.Location = new System.Drawing.Point(0, 105);
         this.layoutItemCalculationMethods.Name = "layoutItemCalculationMethods";
         this.layoutItemCalculationMethods.Size = new System.Drawing.Size(472, 42);
         this.layoutItemCalculationMethods.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.CustomizationFormText = "layoutItemCreature";
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemSpecies.Name = "layoutItemCreature";
         this.layoutItemSpecies.Size = new System.Drawing.Size(472, 24);
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemGender
         // 
         this.layoutItemGender.AllowHide = false;
         this.layoutItemGender.Control = this.cbGender;
         this.layoutItemGender.CustomizationFormText = "layoutItemGender";
         this.layoutItemGender.Location = new System.Drawing.Point(0, 48);
         this.layoutItemGender.Name = "layoutItemGender";
         this.layoutItemGender.Size = new System.Drawing.Size(472, 24);
         this.layoutItemGender.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemPopulation
         // 
         this.layoutItemPopulation.Control = this.cbPopulation;
         this.layoutItemPopulation.CustomizationFormText = "layoutItemPopulation";
         this.layoutItemPopulation.Location = new System.Drawing.Point(0, 24);
         this.layoutItemPopulation.Name = "layoutItemPopulation";
         this.layoutItemPopulation.Size = new System.Drawing.Size(472, 24);
         this.layoutItemPopulation.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutControlGroupPopulationParameters
         // 
         this.layoutControlGroupPopulationParameters.CustomizationFormText = "layoutControlGroup3";
         this.layoutControlGroupPopulationParameters.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemWeight,
            this.layoutItemHeight,
            this.layoutControlMeanButton,
            this.emptySpaceAge,
            this.layoutItemAge,
            this.layoutItemBMI,
            this.emptySpaceBMI,
            this.layoutItemGestationalAge,
            this.layoutItemValueOrigin});
         this.layoutControlGroupPopulationParameters.Location = new System.Drawing.Point(0, 189);
         this.layoutControlGroupPopulationParameters.Name = "layoutControlGroupPopulationParameters";
         this.layoutControlGroupPopulationParameters.Size = new System.Drawing.Size(496, 179);
         // 
         // layoutItemWeight
         // 
         this.layoutItemWeight.Control = this.uxWeight;
         this.layoutItemWeight.CustomizationFormText = "layoutItemWeight";
         this.layoutItemWeight.Location = new System.Drawing.Point(0, 48);
         this.layoutItemWeight.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemWeight.MinSize = new System.Drawing.Size(173, 24);
         this.layoutItemWeight.Name = "layoutItemWeight";
         this.layoutItemWeight.Size = new System.Drawing.Size(416, 24);
         this.layoutItemWeight.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemWeight.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemHeight
         // 
         this.layoutItemHeight.Control = this.uxHeight;
         this.layoutItemHeight.CustomizationFormText = "layoutItemHeight";
         this.layoutItemHeight.Location = new System.Drawing.Point(0, 72);
         this.layoutItemHeight.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemHeight.MinSize = new System.Drawing.Size(173, 24);
         this.layoutItemHeight.Name = "layoutItemHeight";
         this.layoutItemHeight.Size = new System.Drawing.Size(416, 24);
         this.layoutItemHeight.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemHeight.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutControlMeanButton
         // 
         this.layoutControlMeanButton.Control = this.btnMeanValues;
         this.layoutControlMeanButton.CustomizationFormText = "layoutControlMeanButton";
         this.layoutControlMeanButton.Location = new System.Drawing.Point(416, 48);
         this.layoutControlMeanButton.MinSize = new System.Drawing.Size(1, 1);
         this.layoutControlMeanButton.Name = "layoutControlMeanButton";
         this.layoutControlMeanButton.Size = new System.Drawing.Size(56, 48);
         this.layoutControlMeanButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutControlMeanButton.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlMeanButton.TextVisible = false;
         // 
         // emptySpaceAge
         // 
         this.emptySpaceAge.AllowHotTrack = false;
         this.emptySpaceAge.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceAge.Location = new System.Drawing.Point(416, 0);
         this.emptySpaceAge.Name = "emptySpaceAge";
         this.emptySpaceAge.Size = new System.Drawing.Size(56, 48);
         this.emptySpaceAge.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemAge
         // 
         this.layoutItemAge.Control = this.uxAge;
         this.layoutItemAge.CustomizationFormText = "layoutItemAge";
         this.layoutItemAge.Location = new System.Drawing.Point(0, 0);
         this.layoutItemAge.MaxSize = new System.Drawing.Size(0, 24);
         this.layoutItemAge.MinSize = new System.Drawing.Size(173, 24);
         this.layoutItemAge.Name = "layoutItemAge";
         this.layoutItemAge.Size = new System.Drawing.Size(416, 24);
         this.layoutItemAge.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemAge.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemBMI
         // 
         this.layoutItemBMI.Control = this.uxBMI;
         this.layoutItemBMI.CustomizationFormText = "layoutItemBMI";
         this.layoutItemBMI.Location = new System.Drawing.Point(0, 96);
         this.layoutItemBMI.MaxSize = new System.Drawing.Size(10153, 26);
         this.layoutItemBMI.MinSize = new System.Drawing.Size(173, 24);
         this.layoutItemBMI.Name = "layoutItemBMI";
         this.layoutItemBMI.Size = new System.Drawing.Size(416, 24);
         this.layoutItemBMI.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemBMI.TextSize = new System.Drawing.Size(145, 13);
         // 
         // emptySpaceBMI
         // 
         this.emptySpaceBMI.AllowHotTrack = false;
         this.emptySpaceBMI.CustomizationFormText = "emptySpaceBMI";
         this.emptySpaceBMI.Location = new System.Drawing.Point(416, 96);
         this.emptySpaceBMI.Name = "emptySpaceBMI";
         this.emptySpaceBMI.Size = new System.Drawing.Size(56, 41);
         this.emptySpaceBMI.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemGestationalAge
         // 
         this.layoutItemGestationalAge.Control = this.uxGestationalAge;
         this.layoutItemGestationalAge.CustomizationFormText = "layoutItemGestationalAge";
         this.layoutItemGestationalAge.Location = new System.Drawing.Point(0, 24);
         this.layoutItemGestationalAge.MaxSize = new System.Drawing.Size(10153, 24);
         this.layoutItemGestationalAge.MinSize = new System.Drawing.Size(173, 24);
         this.layoutItemGestationalAge.Name = "layoutItemGestationalAge";
         this.layoutItemGestationalAge.Size = new System.Drawing.Size(416, 24);
         this.layoutItemGestationalAge.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemGestationalAge.TextSize = new System.Drawing.Size(145, 13);
         // 
         // layoutItemValueOrigin
         // 
         this.layoutItemValueOrigin.Control = this._panelValueOrigin;
         this.layoutItemValueOrigin.Location = new System.Drawing.Point(0, 120);
         this.layoutItemValueOrigin.Name = "layoutItemValueOrigin";
         this.layoutItemValueOrigin.Size = new System.Drawing.Size(416, 17);
         this.layoutItemValueOrigin.TextSize = new System.Drawing.Size(145, 13);
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 368);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(496, 22);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // IndividualSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "IndividualSettingsView";
         this.Size = new System.Drawing.Size(516, 410);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameterValueVersions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameterValueVersions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._panelValueOrigin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPopulation.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbGender.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSubPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCalculationMethods)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGender)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupPopulationParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemHeight)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlMeanButton)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceAge)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAge)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemBMI)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceBMI)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemGestationalAge)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemValueOrigin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxComboBoxEdit cbPopulation;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbGender;
      private DevExpress.XtraEditors.SimpleButton btnMeanValues;
      private DevExpress.XtraEditors.ImageComboBoxEdit cbSpecies;
      private OSPSuite.UI.Controls.UxGridControl gridParameterValueVersions;
      private UxGridView gridViewParameterValueVersions;
      private UxParameterDTOEdit uxAge;
      private UxParameterDTOEdit uxHeight;
      private UxParameterDTOEdit uxWeight;
      private UxParameterDTOEdit uxGestationalAge;
      private UxGridView gridViewCalculationMethods;
      private OSPSuite.UI.Controls.UxGridControl gridCalculationMethods;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
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
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}


