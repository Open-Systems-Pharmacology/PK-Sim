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
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.btnStop = new DevExpress.XtraEditors.SimpleButton();
         this.lblPopulation = new DevExpress.XtraEditors.LabelControl();
         this.tbProportionsOfFemales = new DevExpress.XtraEditors.TextEdit();
         this.tbNumberOfIndividuals = new DevExpress.XtraEditors.TextEdit();
         this.panelIndividualSelection = new DevExpress.XtraEditors.PanelControl();
         this.gridParameters = new PKSim.UI.Views.Core.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutGroupPopulationProperties = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemProportionOfFemales = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNumberOfIndividuals = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutGroupParameterRanges = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemStop = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutGroupIndividualSelection = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemIndividual = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemPopulation = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbProportionsOfFemales.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividuals.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelIndividualSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupPopulationProperties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProportionOfFemales)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividuals)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupParameterRanges)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStop)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupIndividualSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividual)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulation)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.btnStop);
         this.layoutControl.Controls.Add(this.lblPopulation);
         this.layoutControl.Controls.Add(this.tbProportionsOfFemales);
         this.layoutControl.Controls.Add(this.tbNumberOfIndividuals);
         this.layoutControl.Controls.Add(this.panelIndividualSelection);
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
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 53);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 20;
         this.lblDescription.Text = "lblDescription";
         // 
         // btnStop
         // 
         this.btnStop.Location = new System.Drawing.Point(227, 392);
         this.btnStop.Name = "btnStop";
         this.btnStop.Size = new System.Drawing.Size(212, 22);
         this.btnStop.StyleController = this.layoutControl;
         this.btnStop.TabIndex = 19;
         this.btnStop.Text = "btnStop";
         // 
         // lblPopulation
         // 
         this.lblPopulation.Location = new System.Drawing.Point(12, 36);
         this.lblPopulation.Name = "lblPopulation";
         this.lblPopulation.Size = new System.Drawing.Size(60, 13);
         this.lblPopulation.StyleController = this.layoutControl;
         this.lblPopulation.TabIndex = 18;
         this.lblPopulation.Text = "lblPopulation";
         // 
         // tbProportionsOfFemales
         // 
         this.tbProportionsOfFemales.Location = new System.Drawing.Point(180, 135);
         this.tbProportionsOfFemales.Name = "tbProportionsOfFemales";
         this.tbProportionsOfFemales.Size = new System.Drawing.Size(247, 20);
         this.tbProportionsOfFemales.StyleController = this.layoutControl;
         this.tbProportionsOfFemales.TabIndex = 17;
         // 
         // tbNumberOfIndividuals
         // 
         this.tbNumberOfIndividuals.Location = new System.Drawing.Point(180, 111);
         this.tbNumberOfIndividuals.Name = "tbNumberOfIndividuals";
         this.tbNumberOfIndividuals.Size = new System.Drawing.Size(247, 20);
         this.tbNumberOfIndividuals.StyleController = this.layoutControl;
         this.tbNumberOfIndividuals.TabIndex = 14;
         // 
         // panelIndividualSelection
         // 
         this.panelIndividualSelection.Location = new System.Drawing.Point(115, 12);
         this.panelIndividualSelection.Name = "panelIndividualSelection";
         this.panelIndividualSelection.Size = new System.Drawing.Size(324, 20);
         this.panelIndividualSelection.TabIndex = 16;
         // 
         // gridParameters
         // 
         this.gridParameters.Location = new System.Drawing.Point(24, 202);
         this.gridParameters.MainView = this.gridViewParameters;
         this.gridParameters.Name = "gridParameters";
         this.gridParameters.Size = new System.Drawing.Size(403, 174);
         this.gridParameters.TabIndex = 10;
         this.gridParameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters});
         // 
         // gridViewParameters
         // 
         this.gridViewParameters.EnableColumnContextMenu = true;
         this.gridViewParameters.GridControl = this.gridParameters;
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
            this.layoutGroupIndividualSelection,
            this.emptySpaceItem});
         this.layoutMainGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Size = new System.Drawing.Size(451, 546);
         this.layoutMainGroup.Text = "layoutMainGroup";
         this.layoutMainGroup.TextVisible = false;
         // 
         // layoutGroupPopulationProperties
         // 
         this.layoutGroupPopulationProperties.CustomizationFormText = "layoutGroupPopulationProperties";
         this.layoutGroupPopulationProperties.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemProportionOfFemales,
            this.layoutItemNumberOfIndividuals});
         this.layoutGroupPopulationProperties.Location = new System.Drawing.Point(0, 68);
         this.layoutGroupPopulationProperties.Name = "layoutGroupPopulationProperties";
         this.layoutGroupPopulationProperties.Size = new System.Drawing.Size(431, 91);
         this.layoutGroupPopulationProperties.Text = "layoutGroupPopulationProperties";
         // 
         // layoutItemProportionOfFemales
         // 
         this.layoutItemProportionOfFemales.Control = this.tbProportionsOfFemales;
         this.layoutItemProportionOfFemales.CustomizationFormText = "layoutItemProportionOfFemales";
         this.layoutItemProportionOfFemales.Location = new System.Drawing.Point(0, 24);
         this.layoutItemProportionOfFemales.Name = "layoutItemProportionOfFemales";
         this.layoutItemProportionOfFemales.Size = new System.Drawing.Size(407, 24);
         this.layoutItemProportionOfFemales.Text = "layoutItemProportionOfFemales";
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
         this.layoutItemNumberOfIndividuals.Text = "layoutItemNumberOfIndividuals";
         this.layoutItemNumberOfIndividuals.TextSize = new System.Drawing.Size(153, 13);
         // 
         // layoutGroupParameterRanges
         // 
         this.layoutGroupParameterRanges.CustomizationFormText = "layoutGroupParameterRanges";
         this.layoutGroupParameterRanges.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters});
         this.layoutGroupParameterRanges.Location = new System.Drawing.Point(0, 159);
         this.layoutGroupParameterRanges.Name = "layoutGroupParameterRanges";
         this.layoutGroupParameterRanges.Size = new System.Drawing.Size(431, 221);
         this.layoutGroupParameterRanges.Text = "layoutGroupParameterRanges";
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.gridParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Size = new System.Drawing.Size(407, 178);
         this.layoutItemParameters.Text = "layoutItemParameters";
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         // 
         // layoutItemStop
         // 
         this.layoutItemStop.Control = this.btnStop;
         this.layoutItemStop.CustomizationFormText = "layoutItemStop";
         this.layoutItemStop.Location = new System.Drawing.Point(215, 380);
         this.layoutItemStop.Name = "layoutItemStop";
         this.layoutItemStop.Size = new System.Drawing.Size(216, 26);
         this.layoutItemStop.Text = "layoutItemStop";
         this.layoutItemStop.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemStop.TextToControlDistance = 0;
         this.layoutItemStop.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 406);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(431, 120);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 380);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(215, 26);
         this.emptySpaceItem2.Text = "emptySpaceItem2";
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutGroupIndividualSelection
         // 
         this.layoutGroupIndividualSelection.CustomizationFormText = "layoutGroupIndividualSelection";
         this.layoutGroupIndividualSelection.GroupBordersVisible = false;
         this.layoutGroupIndividualSelection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemIndividual,
            this.layoutItemPopulation,
            this.layoutItemDescription});
         this.layoutGroupIndividualSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutGroupIndividualSelection.Name = "layoutGroupIndividualSelection";
         this.layoutGroupIndividualSelection.Size = new System.Drawing.Size(431, 58);
         this.layoutGroupIndividualSelection.Text = "layoutGroupIndividualSelection";
         // 
         // layoutItemIndividual
         // 
         this.layoutItemIndividual.Control = this.panelIndividualSelection;
         this.layoutItemIndividual.CustomizationFormText = "layoutItemIndividual";
         this.layoutItemIndividual.Location = new System.Drawing.Point(0, 0);
         this.layoutItemIndividual.Name = "layoutItemIndividual";
         this.layoutItemIndividual.Size = new System.Drawing.Size(431, 24);
         this.layoutItemIndividual.Text = "layoutItemIndividual";
         this.layoutItemIndividual.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
         this.layoutItemIndividual.TextSize = new System.Drawing.Size(98, 13);
         this.layoutItemIndividual.TextToControlDistance = 5;
         // 
         // layoutItemPopulation
         // 
         this.layoutItemPopulation.Control = this.lblPopulation;
         this.layoutItemPopulation.CustomizationFormText = "layoutControlItem1";
         this.layoutItemPopulation.Location = new System.Drawing.Point(0, 24);
         this.layoutItemPopulation.Name = "layoutItemPopulation";
         this.layoutItemPopulation.Size = new System.Drawing.Size(431, 17);
         this.layoutItemPopulation.Text = "layoutItemPopulation";
         this.layoutItemPopulation.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPopulation.TextToControlDistance = 0;
         this.layoutItemPopulation.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutItemDescription";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 41);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(431, 17);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextToControlDistance = 0;
         this.layoutItemDescription.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.CustomizationFormText = "emptySpaceItem3";
         this.emptySpaceItem.Location = new System.Drawing.Point(0, 58);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(431, 10);
         this.emptySpaceItem.Text = "emptySpaceItem";
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // RandomPopulationSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "RandomPopulationSettingsView";
         this.Size = new System.Drawing.Size(451, 546);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbProportionsOfFemales.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbNumberOfIndividuals.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelIndividualSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupPopulationProperties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProportionOfFemales)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNumberOfIndividuals)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupParameterRanges)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemStop)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutGroupIndividualSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividual)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulation)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private DevExpress.XtraEditors.TextEdit tbNumberOfIndividuals;
      private UxGridControl gridParameters;
      private UxGridView gridViewParameters;
      private DevExpress.XtraEditors.PanelControl panelIndividualSelection;
      private DevExpress.XtraEditors.TextEdit tbProportionsOfFemales;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividual;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNumberOfIndividuals;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProportionOfFemales;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupPopulationProperties;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupParameterRanges;
      private DevExpress.XtraEditors.LabelControl lblPopulation;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPopulation;
      private DevExpress.XtraEditors.SimpleButton btnStop;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemStop;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlGroup layoutGroupIndividualSelection;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
   }
}
