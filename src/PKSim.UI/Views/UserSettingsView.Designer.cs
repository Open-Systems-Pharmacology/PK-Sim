using OSPSuite.UI.Controls;

namespace PKSim.UI.Views
{
    partial class UserSettingsView
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
         this.layoutControlUserConfig = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tbNumberOfIndividualsPerBin = new DevExpress.XtraEditors.TextEdit();
         this.tbNumberOfBins = new DevExpress.XtraEditors.TextEdit();
         this.cbPreferredChartYScaling = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbPreferredVewLayout = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbDefaultPopulationAnalysis = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.tbNumberOfProcessors = new DevExpress.XtraEditors.TextEdit();
         this.chkShowUpdateNotification = new OSPSuite.UI.Controls.UxCheckEdit();
         this.colorFormula = new OSPSuite.UI.Controls.UxColorPickEditWithHistory();
         this.cbDefaultSolName = new PKSim.UI.Views.Core.UxMRUEdit();
         this.cbDefaultFuName = new PKSim.UI.Views.Core.UxMRUEdit();
         this.cbDefaultLipoName = new PKSim.UI.Views.Core.UxMRUEdit();
         this.tbRelTol = new DevExpress.XtraEditors.TextEdit();
         this.tbAbsTol = new DevExpress.XtraEditors.TextEdit();
         this.cbIconSizeContextMenu = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbDefaultPopulation = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbDefaultSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.chkShouldRestoreWorkspaceLayout = new OSPSuite.UI.Controls.UxCheckEdit();
         this.cbDefaultParameterGroupingMode = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.colorDisabled = new OSPSuite.UI.Controls.UxColorPickEditWithHistory();
         this.colorChartDiagramBack = new OSPSuite.UI.Controls.UxColorPickEditWithHistory();
         this.colorChartBack = new OSPSuite.UI.Controls.UxColorPickEditWithHistory();
         this.tbTemplateDatabase = new DevExpress.XtraEditors.ButtonEdit();
         this.cbIconSizeTab = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.cbIconSizeTreeView = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.tbMRUListItemCount = new DevExpress.XtraEditors.TextEdit();
         this.colorChanged = new OSPSuite.UI.Controls.UxColorPickEditWithHistory();
         this.tbDecimalPlace = new DevExpress.XtraEditors.TextEdit();
         this.cbActiveSkin = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.chkAllowsScientificNotation = new OSPSuite.UI.Controls.UxCheckEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupNumericalProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemAllowsScientificNotation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDecimalPlace = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemAbsTol = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemRelTol = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNumberOfProcessors = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNumberOfBins = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNumberOfIndividualsPerBin = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupUIProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemActiveSkin = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemMRUListItemCount = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPreferredViewLayout = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutGroupIconSizes = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemIconSizeTreeView = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemIconSizeTab = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemIconSizeContextMenu = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupTemplateDatabase = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemTemplateDatabase = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupColors = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemChangedColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemChartBackColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemChartDiagramBackColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDisabledColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemFormulaColor = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupDefaults = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDefaultSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDefaultPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemParameterGroupingMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDefaultLipoName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDefaultFuName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDefaultSolName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDefaultPopulationAnalysis = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDefaultChartYScale = new DevExpress.XtraLayout.LayoutControlItem();
         this.chckColorGroupObservedData = new DevExpress.XtraEditors.CheckEdit();
         this.layoutItemColorGroupObservedData = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlUserConfig)).BeginInit();
         this.layoutControlUserConfig.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividualsPerBin.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfBins.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPreferredChartYScaling.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPreferredVewLayout.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultPopulationAnalysis.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfProcessors.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowUpdateNotification.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorFormula.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultSolName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultFuName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultLipoName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbRelTol.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbAbsTol.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbIconSizeContextMenu.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultPopulation.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShouldRestoreWorkspaceLayout.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultParameterGroupingMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorDisabled.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorChartDiagramBack.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorChartBack.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbTemplateDatabase.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbIconSizeTab.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbIconSizeTreeView.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbMRUListItemCount.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorChanged.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDecimalPlace.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbActiveSkin.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkAllowsScientificNotation.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupNumericalProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAllowsScientificNotation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDecimalPlace)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAbsTol)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRelTol)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfProcessors)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfBins)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividualsPerBin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupUIProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemActiveSkin)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMRUListItemCount)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPreferredViewLayout)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupIconSizes)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIconSizeTreeView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIconSizeTab)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIconSizeContextMenu)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupTemplateDatabase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTemplateDatabase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupColors)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChangedColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChartBackColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChartDiagramBackColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDisabledColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulaColor)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDefaults)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameterGroupingMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultLipoName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultFuName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultSolName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultPopulationAnalysis)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultChartYScale)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.chckColorGroupObservedData.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemColorGroupObservedData)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControlUserConfig
         // 
         this.layoutControlUserConfig.AllowCustomization = false;
         this.layoutControlUserConfig.Controls.Add(this.tbNumberOfIndividualsPerBin);
         this.layoutControlUserConfig.Controls.Add(this.tbNumberOfBins);
         this.layoutControlUserConfig.Controls.Add(this.cbPreferredChartYScaling);
         this.layoutControlUserConfig.Controls.Add(this.cbPreferredVewLayout);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultPopulationAnalysis);
         this.layoutControlUserConfig.Controls.Add(this.tbNumberOfProcessors);
         this.layoutControlUserConfig.Controls.Add(this.chkShowUpdateNotification);
         this.layoutControlUserConfig.Controls.Add(this.colorFormula);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultSolName);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultFuName);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultLipoName);
         this.layoutControlUserConfig.Controls.Add(this.tbRelTol);
         this.layoutControlUserConfig.Controls.Add(this.tbAbsTol);
         this.layoutControlUserConfig.Controls.Add(this.cbIconSizeContextMenu);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultPopulation);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultSpecies);
         this.layoutControlUserConfig.Controls.Add(this.chkShouldRestoreWorkspaceLayout);
         this.layoutControlUserConfig.Controls.Add(this.cbDefaultParameterGroupingMode);
         this.layoutControlUserConfig.Controls.Add(this.colorDisabled);
         this.layoutControlUserConfig.Controls.Add(this.colorChartDiagramBack);
         this.layoutControlUserConfig.Controls.Add(this.colorChartBack);
         this.layoutControlUserConfig.Controls.Add(this.tbTemplateDatabase);
         this.layoutControlUserConfig.Controls.Add(this.cbIconSizeTab);
         this.layoutControlUserConfig.Controls.Add(this.cbIconSizeTreeView);
         this.layoutControlUserConfig.Controls.Add(this.tbMRUListItemCount);
         this.layoutControlUserConfig.Controls.Add(this.colorChanged);
         this.layoutControlUserConfig.Controls.Add(this.tbDecimalPlace);
         this.layoutControlUserConfig.Controls.Add(this.cbActiveSkin);
         this.layoutControlUserConfig.Controls.Add(this.chkAllowsScientificNotation);
         this.layoutControlUserConfig.Controls.Add(this.chckColorGroupObservedData);
         this.layoutControlUserConfig.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlUserConfig.Location = new System.Drawing.Point(0, 0);
         this.layoutControlUserConfig.Name = "layoutControlUserConfig";
         this.layoutControlUserConfig.OptionsView.UseSkinIndents = false;
         this.layoutControlUserConfig.Root = this.layoutControlGroup;
         this.layoutControlUserConfig.Size = new System.Drawing.Size(640, 623);
         this.layoutControlUserConfig.TabIndex = 34;
         this.layoutControlUserConfig.Text = "layoutControl1";
         // 
         // tbNumberOfIndividualsPerBin
         // 
         this.tbNumberOfIndividualsPerBin.Location = new System.Drawing.Point(206, -251);
         this.tbNumberOfIndividualsPerBin.Name = "tbNumberOfIndividualsPerBin";
         this.tbNumberOfIndividualsPerBin.Size = new System.Drawing.Size(398, 20);
         this.tbNumberOfIndividualsPerBin.StyleController = this.layoutControlUserConfig;
         this.tbNumberOfIndividualsPerBin.TabIndex = 35;
         // 
         // tbNumberOfBins
         // 
         this.tbNumberOfBins.Location = new System.Drawing.Point(206, -281);
         this.tbNumberOfBins.Name = "tbNumberOfBins";
         this.tbNumberOfBins.Size = new System.Drawing.Size(398, 20);
         this.tbNumberOfBins.StyleController = this.layoutControlUserConfig;
         this.tbNumberOfBins.TabIndex = 34;
         // 
         // cbPreferredChartYScaling
         // 
         this.cbPreferredChartYScaling.Location = new System.Drawing.Point(206, 193);
         this.cbPreferredChartYScaling.Name = "cbPreferredChartYScaling";
         this.cbPreferredChartYScaling.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbPreferredChartYScaling.Size = new System.Drawing.Size(398, 20);
         this.cbPreferredChartYScaling.StyleController = this.layoutControlUserConfig;
         this.cbPreferredChartYScaling.TabIndex = 33;
         // 
         // cbPreferredVewLayout
         // 
         this.cbPreferredVewLayout.Location = new System.Drawing.Point(206, -134);
         this.cbPreferredVewLayout.Name = "cbPreferredVewLayout";
         this.cbPreferredVewLayout.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbPreferredVewLayout.Size = new System.Drawing.Size(398, 20);
         this.cbPreferredVewLayout.StyleController = this.layoutControlUserConfig;
         this.cbPreferredVewLayout.TabIndex = 32;
         // 
         // cbDefaultPopulationAnalysis
         // 
         this.cbDefaultPopulationAnalysis.Location = new System.Drawing.Point(206, 163);
         this.cbDefaultPopulationAnalysis.Name = "cbDefaultPopulationAnalysis";
         this.cbDefaultPopulationAnalysis.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultPopulationAnalysis.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultPopulationAnalysis.StyleController = this.layoutControlUserConfig;
         this.cbDefaultPopulationAnalysis.TabIndex = 31;
         // 
         // tbNumberOfProcessors
         // 
         this.tbNumberOfProcessors.Location = new System.Drawing.Point(206, -311);
         this.tbNumberOfProcessors.Name = "tbNumberOfProcessors";
         this.tbNumberOfProcessors.Size = new System.Drawing.Size(398, 20);
         this.tbNumberOfProcessors.StyleController = this.layoutControlUserConfig;
         this.tbNumberOfProcessors.TabIndex = 29;
         // 
         // chkShowUpdateNotification
         // 
         this.chkShowUpdateNotification.AllowClicksOutsideControlArea = false;
         this.chkShowUpdateNotification.Location = new System.Drawing.Point(19, -74);
         this.chkShowUpdateNotification.Name = "chkShowUpdateNotification";
         this.chkShowUpdateNotification.Properties.Caption = "chkShowUpdateNotification";
         this.chkShowUpdateNotification.Size = new System.Drawing.Size(585, 20);
         this.chkShowUpdateNotification.StyleController = this.layoutControlUserConfig;
         this.chkShowUpdateNotification.TabIndex = 28;
         // 
         // colorFormula
         // 
         this.colorFormula.EditValue = System.Drawing.Color.Empty;
         this.colorFormula.Location = new System.Drawing.Point(206, 424);
         this.colorFormula.Name = "colorFormula";
         this.colorFormula.Properties.AutomaticColor = System.Drawing.Color.Black;
         this.colorFormula.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.colorFormula.Size = new System.Drawing.Size(398, 20);
         this.colorFormula.StyleController = this.layoutControlUserConfig;
         this.colorFormula.TabIndex = 26;
         // 
         // cbDefaultSolName
         // 
         this.cbDefaultSolName.Location = new System.Drawing.Point(206, 133);
         this.cbDefaultSolName.Name = "cbDefaultSolName";
         this.cbDefaultSolName.Properties.AllowRemoveMRUItems = false;
         this.cbDefaultSolName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultSolName.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultSolName.StyleController = this.layoutControlUserConfig;
         this.cbDefaultSolName.TabIndex = 25;
         // 
         // cbDefaultFuName
         // 
         this.cbDefaultFuName.Location = new System.Drawing.Point(206, 103);
         this.cbDefaultFuName.Name = "cbDefaultFuName";
         this.cbDefaultFuName.Properties.AllowRemoveMRUItems = false;
         this.cbDefaultFuName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultFuName.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultFuName.StyleController = this.layoutControlUserConfig;
         this.cbDefaultFuName.TabIndex = 24;
         // 
         // cbDefaultLipoName
         // 
         this.cbDefaultLipoName.Location = new System.Drawing.Point(206, 73);
         this.cbDefaultLipoName.Name = "cbDefaultLipoName";
         this.cbDefaultLipoName.Properties.AllowRemoveMRUItems = false;
         this.cbDefaultLipoName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultLipoName.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultLipoName.StyleController = this.layoutControlUserConfig;
         this.cbDefaultLipoName.TabIndex = 23;
         // 
         // tbRelTol
         // 
         this.tbRelTol.Location = new System.Drawing.Point(206, -341);
         this.tbRelTol.Name = "tbRelTol";
         this.tbRelTol.Size = new System.Drawing.Size(398, 20);
         this.tbRelTol.StyleController = this.layoutControlUserConfig;
         this.tbRelTol.TabIndex = 22;
         // 
         // tbAbsTol
         // 
         this.tbAbsTol.Location = new System.Drawing.Point(206, -371);
         this.tbAbsTol.Name = "tbAbsTol";
         this.tbAbsTol.Size = new System.Drawing.Size(398, 20);
         this.tbAbsTol.StyleController = this.layoutControlUserConfig;
         this.tbAbsTol.TabIndex = 21;
         // 
         // cbIconSizeContextMenu
         // 
         this.cbIconSizeContextMenu.Location = new System.Drawing.Point(206, 310);
         this.cbIconSizeContextMenu.Name = "cbIconSizeContextMenu";
         this.cbIconSizeContextMenu.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbIconSizeContextMenu.Size = new System.Drawing.Size(398, 20);
         this.cbIconSizeContextMenu.StyleController = this.layoutControlUserConfig;
         this.cbIconSizeContextMenu.TabIndex = 20;
         // 
         // cbDefaultPopulation
         // 
         this.cbDefaultPopulation.Location = new System.Drawing.Point(206, 13);
         this.cbDefaultPopulation.Name = "cbDefaultPopulation";
         this.cbDefaultPopulation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultPopulation.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultPopulation.StyleController = this.layoutControlUserConfig;
         this.cbDefaultPopulation.TabIndex = 18;
         // 
         // cbDefaultSpecies
         // 
         this.cbDefaultSpecies.Location = new System.Drawing.Point(206, -17);
         this.cbDefaultSpecies.Name = "cbDefaultSpecies";
         this.cbDefaultSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultSpecies.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultSpecies.StyleController = this.layoutControlUserConfig;
         this.cbDefaultSpecies.TabIndex = 17;
         // 
         // chkShouldRestoreWorkspaceLayout
         // 
         this.chkShouldRestoreWorkspaceLayout.AllowClicksOutsideControlArea = false;
         this.chkShouldRestoreWorkspaceLayout.Location = new System.Drawing.Point(19, -104);
         this.chkShouldRestoreWorkspaceLayout.Name = "chkShouldRestoreWorkspaceLayout";
         this.chkShouldRestoreWorkspaceLayout.Properties.Caption = "chkShouldRestoreWorkspaceLayout";
         this.chkShouldRestoreWorkspaceLayout.Size = new System.Drawing.Size(585, 20);
         this.chkShouldRestoreWorkspaceLayout.StyleController = this.layoutControlUserConfig;
         this.chkShouldRestoreWorkspaceLayout.TabIndex = 16;
         // 
         // cbDefaultParameterGroupingMode
         // 
         this.cbDefaultParameterGroupingMode.Location = new System.Drawing.Point(206, 43);
         this.cbDefaultParameterGroupingMode.Name = "cbDefaultParameterGroupingMode";
         this.cbDefaultParameterGroupingMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDefaultParameterGroupingMode.Size = new System.Drawing.Size(398, 20);
         this.cbDefaultParameterGroupingMode.StyleController = this.layoutControlUserConfig;
         this.cbDefaultParameterGroupingMode.TabIndex = 19;
         // 
         // colorDisabled
         // 
         this.colorDisabled.EditValue = System.Drawing.Color.Empty;
         this.colorDisabled.Location = new System.Drawing.Point(206, 544);
         this.colorDisabled.Name = "colorDisabled";
         this.colorDisabled.Properties.AutomaticColor = System.Drawing.Color.Black;
         this.colorDisabled.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.colorDisabled.Size = new System.Drawing.Size(398, 20);
         this.colorDisabled.StyleController = this.layoutControlUserConfig;
         this.colorDisabled.TabIndex = 14;
         // 
         // colorChartDiagramBack
         // 
         this.colorChartDiagramBack.EditValue = System.Drawing.Color.Empty;
         this.colorChartDiagramBack.Location = new System.Drawing.Point(206, 514);
         this.colorChartDiagramBack.Name = "colorChartDiagramBack";
         this.colorChartDiagramBack.Properties.AutomaticColor = System.Drawing.Color.Black;
         this.colorChartDiagramBack.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.colorChartDiagramBack.Size = new System.Drawing.Size(398, 20);
         this.colorChartDiagramBack.StyleController = this.layoutControlUserConfig;
         this.colorChartDiagramBack.TabIndex = 13;
         // 
         // colorChartBack
         // 
         this.colorChartBack.EditValue = System.Drawing.Color.Empty;
         this.colorChartBack.Location = new System.Drawing.Point(206, 484);
         this.colorChartBack.Name = "colorChartBack";
         this.colorChartBack.Properties.AutomaticColor = System.Drawing.Color.Black;
         this.colorChartBack.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.colorChartBack.Size = new System.Drawing.Size(398, 20);
         this.colorChartBack.StyleController = this.layoutControlUserConfig;
         this.colorChartBack.TabIndex = 12;
         // 
         // tbTemplateDatabase
         // 
         this.tbTemplateDatabase.Location = new System.Drawing.Point(206, 367);
         this.tbTemplateDatabase.Name = "tbTemplateDatabase";
         this.tbTemplateDatabase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight)});
         this.tbTemplateDatabase.Size = new System.Drawing.Size(398, 20);
         this.tbTemplateDatabase.StyleController = this.layoutControlUserConfig;
         this.tbTemplateDatabase.TabIndex = 10;
         // 
         // cbIconSizeTab
         // 
         this.cbIconSizeTab.Location = new System.Drawing.Point(206, 280);
         this.cbIconSizeTab.Name = "cbIconSizeTab";
         this.cbIconSizeTab.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbIconSizeTab.Size = new System.Drawing.Size(398, 20);
         this.cbIconSizeTab.StyleController = this.layoutControlUserConfig;
         this.cbIconSizeTab.TabIndex = 9;
         // 
         // cbIconSizeTreeView
         // 
         this.cbIconSizeTreeView.Location = new System.Drawing.Point(206, 250);
         this.cbIconSizeTreeView.Name = "cbIconSizeTreeView";
         this.cbIconSizeTreeView.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbIconSizeTreeView.Size = new System.Drawing.Size(398, 20);
         this.cbIconSizeTreeView.StyleController = this.layoutControlUserConfig;
         this.cbIconSizeTreeView.TabIndex = 8;
         // 
         // tbMRUListItemCount
         // 
         this.tbMRUListItemCount.Location = new System.Drawing.Point(206, -164);
         this.tbMRUListItemCount.Name = "tbMRUListItemCount";
         this.tbMRUListItemCount.Size = new System.Drawing.Size(398, 20);
         this.tbMRUListItemCount.StyleController = this.layoutControlUserConfig;
         this.tbMRUListItemCount.TabIndex = 7;
         // 
         // colorChanged
         // 
         this.colorChanged.EditValue = System.Drawing.Color.Empty;
         this.colorChanged.Location = new System.Drawing.Point(206, 454);
         this.colorChanged.Name = "colorChanged";
         this.colorChanged.Properties.AutomaticColor = System.Drawing.Color.Black;
         this.colorChanged.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.colorChanged.Size = new System.Drawing.Size(398, 20);
         this.colorChanged.StyleController = this.layoutControlUserConfig;
         this.colorChanged.TabIndex = 11;
         // 
         // tbDecimalPlace
         // 
         this.tbDecimalPlace.Location = new System.Drawing.Point(206, -401);
         this.tbDecimalPlace.Name = "tbDecimalPlace";
         this.tbDecimalPlace.Size = new System.Drawing.Size(398, 20);
         this.tbDecimalPlace.StyleController = this.layoutControlUserConfig;
         this.tbDecimalPlace.TabIndex = 6;
         // 
         // cbActiveSkin
         // 
         this.cbActiveSkin.Location = new System.Drawing.Point(206, -194);
         this.cbActiveSkin.Name = "cbActiveSkin";
         this.cbActiveSkin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbActiveSkin.Size = new System.Drawing.Size(398, 20);
         this.cbActiveSkin.StyleController = this.layoutControlUserConfig;
         this.cbActiveSkin.TabIndex = 5;
         // 
         // chkAllowsScientificNotation
         // 
         this.chkAllowsScientificNotation.AllowClicksOutsideControlArea = false;
         this.chkAllowsScientificNotation.Location = new System.Drawing.Point(19, -431);
         this.chkAllowsScientificNotation.Name = "chkAllowsScientificNotation";
         this.chkAllowsScientificNotation.Properties.Caption = "chkAllowsScientificNotation";
         this.chkAllowsScientificNotation.Size = new System.Drawing.Size(585, 20);
         this.chkAllowsScientificNotation.StyleController = this.layoutControlUserConfig;
         this.chkAllowsScientificNotation.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutGroupNumericalProperties,
            this.layoutGroupUIProperties,
            this.emptySpaceItem1,
            this.layoutGroupIconSizes,
            this.layoutGroupTemplateDatabase,
            this.layoutGroupColors,
            this.layoutGroupDefaults});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
         this.layoutControlGroup.Size = new System.Drawing.Size(623, 1094);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutGroupNumericalProperties
         // 
         this.layoutGroupNumericalProperties.CustomizationFormText = "layoutGroupNumericalProperties";
         this.layoutGroupNumericalProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemAllowsScientificNotation,
            this.layoutItemDecimalPlace,
            this.layoutItemAbsTol,
            this.layoutItemRelTol,
            this.layoutItemNumberOfProcessors,
            this.layoutItemNumberOfBins,
            this.layoutItemNumberOfIndividualsPerBin});
         this.layoutGroupNumericalProperties.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupNumericalProperties.Name = "layoutGroupNumericalProperties";
         this.layoutGroupNumericalProperties.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupNumericalProperties.Size = new System.Drawing.Size(601, 237);
         // 
         // layoutItemAllowsScientificNotation
         // 
         this.layoutItemAllowsScientificNotation.Control = this.chkAllowsScientificNotation;
         this.layoutItemAllowsScientificNotation.CustomizationFormText = "layoutItemAllowsScientificNotation";
         this.layoutItemAllowsScientificNotation.Location = new System.Drawing.Point(0, 0);
         this.layoutItemAllowsScientificNotation.Name = "layoutItemAllowsScientificNotation";
         this.layoutItemAllowsScientificNotation.Size = new System.Drawing.Size(595, 30);
         this.layoutItemAllowsScientificNotation.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAllowsScientificNotation.TextVisible = false;
         // 
         // layoutItemDecimalPlace
         // 
         this.layoutItemDecimalPlace.Control = this.tbDecimalPlace;
         this.layoutItemDecimalPlace.CustomizationFormText = "layoutItemDecimalPlace";
         this.layoutItemDecimalPlace.Location = new System.Drawing.Point(0, 30);
         this.layoutItemDecimalPlace.Name = "layoutItemDecimalPlace";
         this.layoutItemDecimalPlace.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDecimalPlace.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemAbsTol
         // 
         this.layoutItemAbsTol.Control = this.tbAbsTol;
         this.layoutItemAbsTol.CustomizationFormText = "layoutItemAbsTol";
         this.layoutItemAbsTol.Location = new System.Drawing.Point(0, 60);
         this.layoutItemAbsTol.Name = "layoutItemAbsTol";
         this.layoutItemAbsTol.Size = new System.Drawing.Size(595, 30);
         this.layoutItemAbsTol.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemRelTol
         // 
         this.layoutItemRelTol.Control = this.tbRelTol;
         this.layoutItemRelTol.CustomizationFormText = "layoutItemRelTol";
         this.layoutItemRelTol.Location = new System.Drawing.Point(0, 90);
         this.layoutItemRelTol.Name = "layoutItemRelTol";
         this.layoutItemRelTol.Size = new System.Drawing.Size(595, 30);
         this.layoutItemRelTol.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemNumberOfProcessors
         // 
         this.layoutItemNumberOfProcessors.Control = this.tbNumberOfProcessors;
         this.layoutItemNumberOfProcessors.CustomizationFormText = "layoutItemNumberOfProcessors";
         this.layoutItemNumberOfProcessors.Location = new System.Drawing.Point(0, 120);
         this.layoutItemNumberOfProcessors.Name = "layoutItemNumberOfProcessors";
         this.layoutItemNumberOfProcessors.Size = new System.Drawing.Size(595, 30);
         this.layoutItemNumberOfProcessors.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemNumberOfBins
         // 
         this.layoutItemNumberOfBins.Control = this.tbNumberOfBins;
         this.layoutItemNumberOfBins.Location = new System.Drawing.Point(0, 150);
         this.layoutItemNumberOfBins.Name = "layoutItemNumberOfBins";
         this.layoutItemNumberOfBins.Size = new System.Drawing.Size(595, 30);
         this.layoutItemNumberOfBins.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemNumberOfIndividualsPerBin
         // 
         this.layoutItemNumberOfIndividualsPerBin.Control = this.tbNumberOfIndividualsPerBin;
         this.layoutItemNumberOfIndividualsPerBin.Location = new System.Drawing.Point(0, 180);
         this.layoutItemNumberOfIndividualsPerBin.Name = "layoutItemNumberOfIndividualsPerBin";
         this.layoutItemNumberOfIndividualsPerBin.Size = new System.Drawing.Size(595, 30);
         this.layoutItemNumberOfIndividualsPerBin.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutGroupUIProperties
         // 
         this.layoutGroupUIProperties.CustomizationFormText = "layoutGroupUIProperties";
         this.layoutGroupUIProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemActiveSkin,
            this.layoutItemMRUListItemCount,
            this.layoutControlItem1,
            this.layoutControlItem3,
            this.layoutItemPreferredViewLayout});
         this.layoutGroupUIProperties.Location = new System.Drawing.Point(0, 237);
         this.layoutGroupUIProperties.Name = "layoutGroupUIProperties";
         this.layoutGroupUIProperties.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupUIProperties.Size = new System.Drawing.Size(601, 177);
         // 
         // layoutItemActiveSkin
         // 
         this.layoutItemActiveSkin.Control = this.cbActiveSkin;
         this.layoutItemActiveSkin.CustomizationFormText = "layoutItemActiveSkin";
         this.layoutItemActiveSkin.Location = new System.Drawing.Point(0, 0);
         this.layoutItemActiveSkin.Name = "layoutItemActiveSkin";
         this.layoutItemActiveSkin.Size = new System.Drawing.Size(595, 30);
         this.layoutItemActiveSkin.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemMRUListItemCount
         // 
         this.layoutItemMRUListItemCount.Control = this.tbMRUListItemCount;
         this.layoutItemMRUListItemCount.CustomizationFormText = "layoutItemMRUListItemCount";
         this.layoutItemMRUListItemCount.Location = new System.Drawing.Point(0, 30);
         this.layoutItemMRUListItemCount.Name = "layoutItemMRUListItemCount";
         this.layoutItemMRUListItemCount.Size = new System.Drawing.Size(595, 30);
         this.layoutItemMRUListItemCount.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.chkShouldRestoreWorkspaceLayout;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(0, 90);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(595, 30);
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextVisible = false;
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.chkShowUpdateNotification;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(0, 120);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(595, 30);
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextVisible = false;
         // 
         // layoutItemPreferredViewLayout
         // 
         this.layoutItemPreferredViewLayout.Control = this.cbPreferredVewLayout;
         this.layoutItemPreferredViewLayout.CustomizationFormText = "layoutItemPreferredViewLayout";
         this.layoutItemPreferredViewLayout.Location = new System.Drawing.Point(0, 60);
         this.layoutItemPreferredViewLayout.Name = "layoutItemPreferredViewLayout";
         this.layoutItemPreferredViewLayout.Size = new System.Drawing.Size(595, 30);
         this.layoutItemPreferredViewLayout.TextSize = new System.Drawing.Size(182, 13);
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 1062);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(601, 10);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutGroupIconSizes
         // 
         this.layoutGroupIconSizes.CustomizationFormText = "layoutControlGroup2";
         this.layoutGroupIconSizes.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemIconSizeTreeView,
            this.layoutItemIconSizeTab,
            this.layoutItemIconSizeContextMenu});
         this.layoutGroupIconSizes.Location = new System.Drawing.Point(0, 681);
         this.layoutGroupIconSizes.Name = "layoutGroupIconSizes";
         this.layoutGroupIconSizes.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupIconSizes.Size = new System.Drawing.Size(601, 117);
         // 
         // layoutItemIconSizeTreeView
         // 
         this.layoutItemIconSizeTreeView.Control = this.cbIconSizeTreeView;
         this.layoutItemIconSizeTreeView.CustomizationFormText = "layoutItemIconSize";
         this.layoutItemIconSizeTreeView.Location = new System.Drawing.Point(0, 0);
         this.layoutItemIconSizeTreeView.Name = "layoutItemIconSizeTreeView";
         this.layoutItemIconSizeTreeView.Size = new System.Drawing.Size(595, 30);
         this.layoutItemIconSizeTreeView.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemIconSizeTab
         // 
         this.layoutItemIconSizeTab.Control = this.cbIconSizeTab;
         this.layoutItemIconSizeTab.CustomizationFormText = "layoutItemIconSizeTab";
         this.layoutItemIconSizeTab.Location = new System.Drawing.Point(0, 30);
         this.layoutItemIconSizeTab.Name = "layoutItemIconSizeTab";
         this.layoutItemIconSizeTab.Size = new System.Drawing.Size(595, 30);
         this.layoutItemIconSizeTab.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemIconSizeContextMenu
         // 
         this.layoutItemIconSizeContextMenu.Control = this.cbIconSizeContextMenu;
         this.layoutItemIconSizeContextMenu.CustomizationFormText = "layoutItemIconSizeContextMenu";
         this.layoutItemIconSizeContextMenu.Location = new System.Drawing.Point(0, 60);
         this.layoutItemIconSizeContextMenu.Name = "layoutItemIconSizeContextMenu";
         this.layoutItemIconSizeContextMenu.Size = new System.Drawing.Size(595, 30);
         this.layoutItemIconSizeContextMenu.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutGroupTemplateDatabase
         // 
         this.layoutGroupTemplateDatabase.CustomizationFormText = "layoutGroupTemplateDatabase";
         this.layoutGroupTemplateDatabase.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemTemplateDatabase});
         this.layoutGroupTemplateDatabase.Location = new System.Drawing.Point(0, 798);
         this.layoutGroupTemplateDatabase.Name = "layoutGroupTemplateDatabase";
         this.layoutGroupTemplateDatabase.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupTemplateDatabase.Size = new System.Drawing.Size(601, 57);
         // 
         // layoutItemTemplateDatabase
         // 
         this.layoutItemTemplateDatabase.Control = this.tbTemplateDatabase;
         this.layoutItemTemplateDatabase.CustomizationFormText = "layoutItemTemplateDatabase";
         this.layoutItemTemplateDatabase.Location = new System.Drawing.Point(0, 0);
         this.layoutItemTemplateDatabase.Name = "layoutItemTemplateDatabase";
         this.layoutItemTemplateDatabase.Size = new System.Drawing.Size(595, 30);
         this.layoutItemTemplateDatabase.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutGroupColors
         // 
         this.layoutGroupColors.CustomizationFormText = "layoutGroupColors";
         this.layoutGroupColors.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemChangedColor,
            this.layoutItemChartBackColor,
            this.layoutItemChartDiagramBackColor,
            this.layoutItemDisabledColor,
            this.layoutItemFormulaColor,
            this.layoutItemColorGroupObservedData});
         this.layoutGroupColors.Location = new System.Drawing.Point(0, 855);
         this.layoutGroupColors.Name = "layoutGroupColor";
         this.layoutGroupColors.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupColors.Size = new System.Drawing.Size(601, 207);
         this.layoutGroupColors.Text = "layoutGroupColors";
         // 
         // layoutItemChangedColor
         // 
         this.layoutItemChangedColor.Control = this.colorChanged;
         this.layoutItemChangedColor.CustomizationFormText = "layoutItemChangedColor";
         this.layoutItemChangedColor.Location = new System.Drawing.Point(0, 30);
         this.layoutItemChangedColor.Name = "layoutItemChangedColor";
         this.layoutItemChangedColor.Size = new System.Drawing.Size(595, 30);
         this.layoutItemChangedColor.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemChartBackColor
         // 
         this.layoutItemChartBackColor.Control = this.colorChartBack;
         this.layoutItemChartBackColor.CustomizationFormText = "layoutItemChartBackColor";
         this.layoutItemChartBackColor.Location = new System.Drawing.Point(0, 60);
         this.layoutItemChartBackColor.Name = "layoutItemPlotBackColor";
         this.layoutItemChartBackColor.Size = new System.Drawing.Size(595, 30);
         this.layoutItemChartBackColor.Text = "layoutItemChartBackColor";
         this.layoutItemChartBackColor.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemChartDiagramBackColor
         // 
         this.layoutItemChartDiagramBackColor.Control = this.colorChartDiagramBack;
         this.layoutItemChartDiagramBackColor.CustomizationFormText = "layoutItemChartDiagramBackColor";
         this.layoutItemChartDiagramBackColor.Location = new System.Drawing.Point(0, 90);
         this.layoutItemChartDiagramBackColor.Name = "layoutItemPlotDiagramBackColor";
         this.layoutItemChartDiagramBackColor.Size = new System.Drawing.Size(595, 30);
         this.layoutItemChartDiagramBackColor.Text = "layoutItemChartDiagramBackColor";
         this.layoutItemChartDiagramBackColor.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDisabledColor
         // 
         this.layoutItemDisabledColor.Control = this.colorDisabled;
         this.layoutItemDisabledColor.CustomizationFormText = "layoutItemDisabled1Color";
         this.layoutItemDisabledColor.Location = new System.Drawing.Point(0, 120);
         this.layoutItemDisabledColor.Name = "layoutItemDisabledColor";
         this.layoutItemDisabledColor.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDisabledColor.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemFormulaColor
         // 
         this.layoutItemFormulaColor.Control = this.colorFormula;
         this.layoutItemFormulaColor.CustomizationFormText = "layoutItemFormulaColor";
         this.layoutItemFormulaColor.Location = new System.Drawing.Point(0, 0);
         this.layoutItemFormulaColor.Name = "layoutItemFormulaColor";
         this.layoutItemFormulaColor.Size = new System.Drawing.Size(595, 30);
         this.layoutItemFormulaColor.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutGroupDefaults
         // 
         this.layoutGroupDefaults.CustomizationFormText = "layoutGroupDefaults";
         this.layoutGroupDefaults.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDefaultSpecies,
            this.layoutItemDefaultPopulation,
            this.layoutItemParameterGroupingMode,
            this.layoutItemDefaultLipoName,
            this.layoutItemDefaultFuName,
            this.layoutItemDefaultSolName,
            this.layoutItemDefaultPopulationAnalysis,
            this.layoutItemDefaultChartYScale});
         this.layoutGroupDefaults.Location = new System.Drawing.Point(0, 414);
         this.layoutGroupDefaults.Name = "layoutGroupDefaults";
         this.layoutGroupDefaults.OptionsItemText.TextToControlDistance = 5;
         this.layoutGroupDefaults.Size = new System.Drawing.Size(601, 267);
         // 
         // layoutItemDefaultSpecies
         // 
         this.layoutItemDefaultSpecies.Control = this.cbDefaultSpecies;
         this.layoutItemDefaultSpecies.CustomizationFormText = "layoutItemDefaultSpecies";
         this.layoutItemDefaultSpecies.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDefaultSpecies.Name = "layoutItemDefaultSpecies";
         this.layoutItemDefaultSpecies.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultSpecies.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDefaultPopulation
         // 
         this.layoutItemDefaultPopulation.Control = this.cbDefaultPopulation;
         this.layoutItemDefaultPopulation.CustomizationFormText = "layoutItemDefaultPopulation";
         this.layoutItemDefaultPopulation.Location = new System.Drawing.Point(0, 30);
         this.layoutItemDefaultPopulation.Name = "layoutItemDefaultPopulation";
         this.layoutItemDefaultPopulation.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultPopulation.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemParameterGroupingMode
         // 
         this.layoutItemParameterGroupingMode.Control = this.cbDefaultParameterGroupingMode;
         this.layoutItemParameterGroupingMode.CustomizationFormText = "layoutItemParameterGroupingMode";
         this.layoutItemParameterGroupingMode.Location = new System.Drawing.Point(0, 60);
         this.layoutItemParameterGroupingMode.Name = "layoutItemParameterGroupingMode";
         this.layoutItemParameterGroupingMode.Size = new System.Drawing.Size(595, 30);
         this.layoutItemParameterGroupingMode.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDefaultLipoName
         // 
         this.layoutItemDefaultLipoName.Control = this.cbDefaultLipoName;
         this.layoutItemDefaultLipoName.CustomizationFormText = "layoutItemDefaultLipo";
         this.layoutItemDefaultLipoName.Location = new System.Drawing.Point(0, 90);
         this.layoutItemDefaultLipoName.Name = "layoutItemDefaultLipo";
         this.layoutItemDefaultLipoName.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultLipoName.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDefaultFuName
         // 
         this.layoutItemDefaultFuName.Control = this.cbDefaultFuName;
         this.layoutItemDefaultFuName.CustomizationFormText = "layoutItemDefaultFuName";
         this.layoutItemDefaultFuName.Location = new System.Drawing.Point(0, 120);
         this.layoutItemDefaultFuName.Name = "layoutItemDefaultFuName";
         this.layoutItemDefaultFuName.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultFuName.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDefaultSolName
         // 
         this.layoutItemDefaultSolName.Control = this.cbDefaultSolName;
         this.layoutItemDefaultSolName.CustomizationFormText = "layoutItemDefaultSolName";
         this.layoutItemDefaultSolName.Location = new System.Drawing.Point(0, 150);
         this.layoutItemDefaultSolName.Name = "layoutItemDefaultSolName";
         this.layoutItemDefaultSolName.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultSolName.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDefaultPopulationAnalysis
         // 
         this.layoutItemDefaultPopulationAnalysis.Control = this.cbDefaultPopulationAnalysis;
         this.layoutItemDefaultPopulationAnalysis.CustomizationFormText = "layoutItemDefaultPopulationAnalysis";
         this.layoutItemDefaultPopulationAnalysis.Location = new System.Drawing.Point(0, 180);
         this.layoutItemDefaultPopulationAnalysis.Name = "layoutItemDefaultPopulationAnalysis";
         this.layoutItemDefaultPopulationAnalysis.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultPopulationAnalysis.TextSize = new System.Drawing.Size(182, 13);
         // 
         // layoutItemDefaultChartYScale
         // 
         this.layoutItemDefaultChartYScale.Control = this.cbPreferredChartYScaling;
         this.layoutItemDefaultChartYScale.CustomizationFormText = "layoutItemDefaultChartScale";
         this.layoutItemDefaultChartYScale.Location = new System.Drawing.Point(0, 210);
         this.layoutItemDefaultChartYScale.Name = "layoutItemDefaultChartYScale";
         this.layoutItemDefaultChartYScale.Size = new System.Drawing.Size(595, 30);
         this.layoutItemDefaultChartYScale.Text = "layoutItemDefaultChartScale";
         this.layoutItemDefaultChartYScale.TextSize = new System.Drawing.Size(182, 13);
         // 
         // chckColorGroupObservedData
         // 
         this.chckColorGroupObservedData.Location = new System.Drawing.Point(19, 574);
         this.chckColorGroupObservedData.Name = "chckColorGroupObservedData";
         this.chckColorGroupObservedData.Properties.Caption = "checkEdit1";
         this.chckColorGroupObservedData.Size = new System.Drawing.Size(585, 20);
         this.chckColorGroupObservedData.StyleController = this.layoutControlUserConfig;
         this.chckColorGroupObservedData.TabIndex = 36;
         // 
         // layoutItemColorGroupObservedData
         // 
         this.layoutItemColorGroupObservedData.Control = this.chckColorGroupObservedData;
         this.layoutItemColorGroupObservedData.Location = new System.Drawing.Point(0, 150);
         this.layoutItemColorGroupObservedData.Name = "layoutItemColorGroupObservedData";
         this.layoutItemColorGroupObservedData.Size = new System.Drawing.Size(595, 30);
         this.layoutItemColorGroupObservedData.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemColorGroupObservedData.TextVisible = false;
         // 
         // UserSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "UserSettingsView";
         this.Controls.Add(this.layoutControlUserConfig);
         this.Name = "UserSettingsView";
         this.Size = new System.Drawing.Size(640, 623);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlUserConfig)).EndInit();
         this.layoutControlUserConfig.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividualsPerBin.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfBins.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPreferredChartYScaling.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbPreferredVewLayout.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultPopulationAnalysis.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfProcessors.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShowUpdateNotification.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorFormula.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultSolName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultFuName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultLipoName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbRelTol.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbAbsTol.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbIconSizeContextMenu.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultPopulation.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkShouldRestoreWorkspaceLayout.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbDefaultParameterGroupingMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorDisabled.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorChartDiagramBack.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorChartBack.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbTemplateDatabase.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbIconSizeTab.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbIconSizeTreeView.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbMRUListItemCount.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.colorChanged.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDecimalPlace.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbActiveSkin.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chkAllowsScientificNotation.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupNumericalProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAllowsScientificNotation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDecimalPlace)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAbsTol)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRelTol)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfProcessors)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfBins)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividualsPerBin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupUIProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemActiveSkin)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemMRUListItemCount)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPreferredViewLayout)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupIconSizes)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIconSizeTreeView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIconSizeTab)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIconSizeContextMenu)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupTemplateDatabase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemTemplateDatabase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupColors)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChangedColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChartBackColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemChartDiagramBackColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDisabledColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFormulaColor)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupDefaults)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameterGroupingMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultLipoName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultFuName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultSolName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultPopulationAnalysis)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDefaultChartYScale)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.chckColorGroupObservedData.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemColorGroupObservedData)).EndInit();
         this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbActiveSkin;
        private OSPSuite.UI.Controls.UxCheckEdit chkAllowsScientificNotation;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemActiveSkin;
        private DevExpress.XtraEditors.TextEdit tbMRUListItemCount;
        private DevExpress.XtraEditors.TextEdit tbDecimalPlace;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemMRUListItemCount;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbIconSizeTreeView;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemIconSizeTreeView;
        private DevExpress.XtraLayout.LayoutControlGroup layoutGroupUIProperties;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbIconSizeTab;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemIconSizeTab;
        private OSPSuite.UI.Controls.UxLayoutControl layoutControlUserConfig;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutGroupIconSizes;
        private DevExpress.XtraLayout.LayoutControlGroup layoutGroupNumericalProperties;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemAllowsScientificNotation;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDecimalPlace;
        private DevExpress.XtraEditors.ButtonEdit tbTemplateDatabase;
        private DevExpress.XtraLayout.LayoutControlGroup layoutGroupTemplateDatabase;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemTemplateDatabase;
        private UxColorPickEditWithHistory colorDisabled;
        private UxColorPickEditWithHistory colorChartDiagramBack;
        private UxColorPickEditWithHistory colorChartBack;
        private UxColorPickEditWithHistory colorChanged;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemChangedColor;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemChartBackColor;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemChartDiagramBackColor;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDisabledColor;
        private DevExpress.XtraLayout.LayoutControlGroup layoutGroupColors;
        private OSPSuite.UI.Controls.UxCheckEdit chkShouldRestoreWorkspaceLayout;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbDefaultPopulation;
        private PKSim.UI.Views.Core.UxImageComboBoxEdit cbDefaultSpecies;
        private DevExpress.XtraLayout.LayoutControlGroup layoutGroupDefaults;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultSpecies;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultPopulation;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbDefaultParameterGroupingMode;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemParameterGroupingMode;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbIconSizeContextMenu;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemIconSizeContextMenu;
        private DevExpress.XtraEditors.TextEdit tbRelTol;
        private DevExpress.XtraEditors.TextEdit tbAbsTol;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemAbsTol;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemRelTol;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultLipoName;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultFuName;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultSolName;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemFormulaColor;
        private PKSim.UI.Views.Core.UxMRUEdit cbDefaultSolName;
        private PKSim.UI.Views.Core.UxMRUEdit cbDefaultFuName;
        private PKSim.UI.Views.Core.UxMRUEdit cbDefaultLipoName;
        private OSPSuite.UI.Controls.UxCheckEdit chkShowUpdateNotification;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraEditors.TextEdit tbNumberOfProcessors;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfProcessors;
        private PKSim.UI.Views.Core.UxImageComboBoxEdit cbDefaultPopulationAnalysis;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultPopulationAnalysis;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbPreferredVewLayout;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemPreferredViewLayout;
        private OSPSuite.UI.Controls.UxComboBoxEdit cbPreferredChartYScaling;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemDefaultChartYScale;
        private DevExpress.XtraEditors.TextEdit tbNumberOfIndividualsPerBin;
        private DevExpress.XtraEditors.TextEdit tbNumberOfBins;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfBins;
        private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfIndividualsPerBin;
        private UxColorPickEditWithHistory colorFormula;
      private DevExpress.XtraEditors.CheckEdit chckColorGroupObservedData;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemColorGroupObservedData;
   }
}