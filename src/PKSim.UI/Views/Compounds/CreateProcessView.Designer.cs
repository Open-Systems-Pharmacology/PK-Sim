using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Compounds
{
   partial class CreateProcessView
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
         _templateBinder.Dispose();
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.tbDataSource = new DevExpress.XtraEditors.TextEdit();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblProcessDescription = new DevExpress.XtraEditors.LabelControl();
         this.lblSpeciesDescription = new DevExpress.XtraEditors.LabelControl();
         this.lblDataSourceDescription = new DevExpress.XtraEditors.LabelControl();
         this.lbProcessType = new DevExpress.XtraEditors.ListBoxControl();
         this.cbSpecies = new PKSim.UI.Views.Core.UxImageComboBoxEdit();
         this.tbSystemicProcessType = new DevExpress.XtraEditors.TextEdit();
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDataSource = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpecies = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemProcessType = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDataSourceDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSpeciesDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemProcessDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceDataSource = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceSpecies = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceProcess = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemSystemicProcessType = new DevExpress.XtraLayout.LayoutControlItem();
         this.cbProteinName = new PKSim.UI.Views.Core.UxMRUEdit();
         this.layoutItemProtein = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDataSource.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.lbProcessType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbSystemicProcessType.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDataSource)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProcessType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDataSourceDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpeciesDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProcessDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceDataSource)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceSpecies)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceProcess)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSystemicProcessType)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbProteinName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtein)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(431, 12);
         this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.btnCancel.Size = new System.Drawing.Size(88, 22);
         this.btnCancel.TabIndex = 1;
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(323, 12);
         this.btnOk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.btnOk.Size = new System.Drawing.Size(104, 22);
         this.btnOk.TabIndex = 0;
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 596);
         this.layoutControlBase.Size = new System.Drawing.Size(531, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(151, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(531, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(311, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(108, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(419, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(92, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(155, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(156, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(155, 26);
         // 
         // tbDataSource
         // 
         this.tbDataSource.Location = new System.Drawing.Point(170, 60);
         this.tbDataSource.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.tbDataSource.Name = "tbDataSource";
         this.tbDataSource.Size = new System.Drawing.Size(349, 20);
         this.tbDataSource.StyleController = this.layoutControl;
         this.tbDataSource.TabIndex = 1;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.lblProcessDescription);
         this.layoutControl.Controls.Add(this.lblSpeciesDescription);
         this.layoutControl.Controls.Add(this.lblDataSourceDescription);
         this.layoutControl.Controls.Add(this.lbProcessType);
         this.layoutControl.Controls.Add(this.cbSpecies);
         this.layoutControl.Controls.Add(this.tbSystemicProcessType);
         this.layoutControl.Controls.Add(this.cbProteinName);
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Controls.Add(this.tbDataSource);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(531, 596);
         this.layoutControl.TabIndex = 41;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblProcessDescription
         // 
         this.lblProcessDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
         this.lblProcessDescription.Location = new System.Drawing.Point(12, 279);
         this.lblProcessDescription.Name = "lblProcessDescription";
         this.lblProcessDescription.Size = new System.Drawing.Size(507, 13);
         this.lblProcessDescription.StyleController = this.layoutControl;
         this.lblProcessDescription.TabIndex = 11;
         this.lblProcessDescription.Text = "lblProcessDescription";
         // 
         // lblSpeciesDescription
         // 
         this.lblSpeciesDescription.Location = new System.Drawing.Point(12, 137);
         this.lblSpeciesDescription.Name = "lblSpeciesDescription";
         this.lblSpeciesDescription.Size = new System.Drawing.Size(99, 13);
         this.lblSpeciesDescription.StyleController = this.layoutControl;
         this.lblSpeciesDescription.TabIndex = 10;
         this.lblSpeciesDescription.Text = "lblSpeciesDescription";
         // 
         // lblDataSourceDescription
         // 
         this.lblDataSourceDescription.Location = new System.Drawing.Point(12, 84);
         this.lblDataSourceDescription.Name = "lblDataSourceDescription";
         this.lblDataSourceDescription.Size = new System.Drawing.Size(119, 13);
         this.lblDataSourceDescription.StyleController = this.layoutControl;
         this.lblDataSourceDescription.TabIndex = 9;
         this.lblDataSourceDescription.Text = "lblDataSourceDescription";
         // 
         // lbProcessType
         // 
         this.lbProcessType.Location = new System.Drawing.Point(12, 183);
         this.lbProcessType.Name = "lbProcessType";
         this.lbProcessType.Size = new System.Drawing.Size(507, 92);
         this.lbProcessType.StyleController = this.layoutControl;
         this.lbProcessType.TabIndex = 6;
         // 
         // cbSpecies
         // 
         this.cbSpecies.Location = new System.Drawing.Point(170, 113);
         this.cbSpecies.Name = "cbSpecies";
         this.cbSpecies.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbSpecies.Size = new System.Drawing.Size(349, 20);
         this.cbSpecies.StyleController = this.layoutControl;
         this.cbSpecies.TabIndex = 5;
         // 
         // tbSystemicProcessType
         // 
         this.tbSystemicProcessType.Location = new System.Drawing.Point(170, 36);
         this.tbSystemicProcessType.Name = "tbSystemicProcessType";
         this.tbSystemicProcessType.Size = new System.Drawing.Size(349, 20);
         this.tbSystemicProcessType.StyleController = this.layoutControl;
         this.tbSystemicProcessType.TabIndex = 4;
         // 
         // panelParameters
         // 
         this.panelParameters.Anchor = System.Windows.Forms.AnchorStyles.None;
         this.panelParameters.Location = new System.Drawing.Point(12, 312);
         this.panelParameters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Size = new System.Drawing.Size(507, 240);
         this.panelParameters.TabIndex = 3;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutItemDataSource,
            this.layoutItemProtein,
            this.layoutItemSpecies,
            this.layoutItemProcessType,
            this.layoutItemDataSourceDescription,
            this.layoutItemSpeciesDescription,
            this.layoutItemProcessDescription,
            this.emptySpaceDataSource,
            this.emptySpaceSpecies,
            this.emptySpaceItem4,
            this.emptySpaceProcess,
            this.layoutItemSystemicProcessType});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(531, 596);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.panelParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 300);
         this.layoutItemParameters.Name = "layoutItemParameter";
         this.layoutItemParameters.Size = new System.Drawing.Size(511, 244);
         this.layoutItemParameters.Text = "layoutItemParameters";
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         // 
         // layoutItemDataSource
         // 
         this.layoutItemDataSource.Control = this.tbDataSource;
         this.layoutItemDataSource.CustomizationFormText = "layoutItemProcessName";
         this.layoutItemDataSource.Location = new System.Drawing.Point(0, 48);
         this.layoutItemDataSource.Name = "layoutItemDataSource";
         this.layoutItemDataSource.Size = new System.Drawing.Size(511, 24);
         this.layoutItemDataSource.Text = "layoutItemDataSource";
         this.layoutItemDataSource.TextSize = new System.Drawing.Size(155, 13);
         // 
         // layoutItemSpecies
         // 
         this.layoutItemSpecies.Control = this.cbSpecies;
         this.layoutItemSpecies.CustomizationFormText = "layoutItemSpecies";
         this.layoutItemSpecies.Location = new System.Drawing.Point(0, 101);
         this.layoutItemSpecies.Name = "layoutItemSpecies";
         this.layoutItemSpecies.Size = new System.Drawing.Size(511, 24);
         this.layoutItemSpecies.Text = "layoutItemSpecies";
         this.layoutItemSpecies.TextSize = new System.Drawing.Size(155, 13);
         // 
         // layoutItemProcessType
         // 
         this.layoutItemProcessType.Control = this.lbProcessType;
         this.layoutItemProcessType.CustomizationFormText = "layoutItemProcessType";
         this.layoutItemProcessType.Location = new System.Drawing.Point(0, 155);
         this.layoutItemProcessType.MaxSize = new System.Drawing.Size(0, 112);
         this.layoutItemProcessType.MinSize = new System.Drawing.Size(159, 112);
         this.layoutItemProcessType.Name = "layoutItemProcessType";
         this.layoutItemProcessType.Size = new System.Drawing.Size(511, 112);
         this.layoutItemProcessType.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemProcessType.Text = "layoutItemProcessType";
         this.layoutItemProcessType.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemProcessType.TextSize = new System.Drawing.Size(155, 13);
         // 
         // layoutItemDataSourceDescription
         // 
         this.layoutItemDataSourceDescription.Control = this.lblDataSourceDescription;
         this.layoutItemDataSourceDescription.CustomizationFormText = "layoutItemDataSourceDescription";
         this.layoutItemDataSourceDescription.Location = new System.Drawing.Point(0, 72);
         this.layoutItemDataSourceDescription.Name = "layoutItemDataSourceDescription";
         this.layoutItemDataSourceDescription.Size = new System.Drawing.Size(511, 17);
         this.layoutItemDataSourceDescription.Text = "layoutItemDataSourceDescription";
         this.layoutItemDataSourceDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDataSourceDescription.TextToControlDistance = 0;
         this.layoutItemDataSourceDescription.TextVisible = false;
         // 
         // layoutItemSpeciesDescription
         // 
         this.layoutItemSpeciesDescription.Control = this.lblSpeciesDescription;
         this.layoutItemSpeciesDescription.CustomizationFormText = "layoutItemSpeciesDescription";
         this.layoutItemSpeciesDescription.Location = new System.Drawing.Point(0, 125);
         this.layoutItemSpeciesDescription.Name = "layoutItemSpeciesDescription";
         this.layoutItemSpeciesDescription.Size = new System.Drawing.Size(511, 17);
         this.layoutItemSpeciesDescription.Text = "layoutItemSpeciesDescription";
         this.layoutItemSpeciesDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSpeciesDescription.TextToControlDistance = 0;
         this.layoutItemSpeciesDescription.TextVisible = false;
         // 
         // layoutItemProcessDescription
         // 
         this.layoutItemProcessDescription.Control = this.lblProcessDescription;
         this.layoutItemProcessDescription.CustomizationFormText = "layoutItemProcessDescription";
         this.layoutItemProcessDescription.Location = new System.Drawing.Point(0, 267);
         this.layoutItemProcessDescription.Name = "layoutItemProcessDescription";
         this.layoutItemProcessDescription.Size = new System.Drawing.Size(511, 17);
         this.layoutItemProcessDescription.Text = "layoutItemProcessDescription";
         this.layoutItemProcessDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemProcessDescription.TextToControlDistance = 0;
         this.layoutItemProcessDescription.TextVisible = false;
         // 
         // emptySpaceDataSource
         // 
         this.emptySpaceDataSource.AllowHotTrack = false;
         this.emptySpaceDataSource.CustomizationFormText = "emptySpaceDataSource";
         this.emptySpaceDataSource.Location = new System.Drawing.Point(0, 89);
         this.emptySpaceDataSource.Name = "emptySpaceDataSource";
         this.emptySpaceDataSource.Size = new System.Drawing.Size(511, 12);
         this.emptySpaceDataSource.Text = "emptySpaceDataSource";
         this.emptySpaceDataSource.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceSpecies
         // 
         this.emptySpaceSpecies.AllowHotTrack = false;
         this.emptySpaceSpecies.CustomizationFormText = "emptySpaceSpecies";
         this.emptySpaceSpecies.Location = new System.Drawing.Point(0, 142);
         this.emptySpaceSpecies.Name = "emptySpaceSpecies";
         this.emptySpaceSpecies.Size = new System.Drawing.Size(511, 13);
         this.emptySpaceSpecies.Text = "emptySpaceSpecies";
         this.emptySpaceSpecies.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem4
         // 
         this.emptySpaceItem4.AllowHotTrack = false;
         this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem4";
         this.emptySpaceItem4.Location = new System.Drawing.Point(0, 544);
         this.emptySpaceItem4.Name = "emptySpaceItem4";
         this.emptySpaceItem4.Size = new System.Drawing.Size(511, 32);
         this.emptySpaceItem4.Text = "emptySpaceItem4";
         this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceProcess
         // 
         this.emptySpaceProcess.AllowHotTrack = false;
         this.emptySpaceProcess.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceProcess.Location = new System.Drawing.Point(0, 284);
         this.emptySpaceProcess.Name = "emptySpaceProcess";
         this.emptySpaceProcess.Size = new System.Drawing.Size(511, 16);
         this.emptySpaceProcess.Text = "emptySpaceProcess";
         this.emptySpaceProcess.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemSystemicProcessType
         // 
         this.layoutItemSystemicProcessType.Control = this.tbSystemicProcessType;
         this.layoutItemSystemicProcessType.CustomizationFormText = "layoutItemProcessName";
         this.layoutItemSystemicProcessType.Location = new System.Drawing.Point(0, 24);
         this.layoutItemSystemicProcessType.Name = "layoutItemSystemicProcessType";
         this.layoutItemSystemicProcessType.Size = new System.Drawing.Size(511, 24);
         this.layoutItemSystemicProcessType.Text = "layoutItemSystemicProcessType";
         this.layoutItemSystemicProcessType.TextSize = new System.Drawing.Size(155, 13);
         // 
         // cbProteinName
         // 
         this.cbProteinName.Location = new System.Drawing.Point(170, 12);
         this.cbProteinName.Name = "cbProteinName";
         this.cbProteinName.Properties.AllowRemoveMRUItems = false;
         this.cbProteinName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbProteinName.Size = new System.Drawing.Size(349, 20);
         this.cbProteinName.StyleController = this.layoutControl;
         this.cbProteinName.TabIndex = 0;
         // 
         // layoutItemProtein
         // 
         this.layoutItemProtein.Control = this.cbProteinName;
         this.layoutItemProtein.CustomizationFormText = "layoutItemProtein";
         this.layoutItemProtein.Location = new System.Drawing.Point(0, 0);
         this.layoutItemProtein.Name = "layoutItemProtein";
         this.layoutItemProtein.Size = new System.Drawing.Size(511, 24);
         this.layoutItemProtein.Text = "layoutItemProtein";
         this.layoutItemProtein.TextSize = new System.Drawing.Size(155, 13);
         // 
         // CreateProcessView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "CreateProcessView";
         this.ClientSize = new System.Drawing.Size(531, 642);
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
         this.Name = "CreateProcessView";
         this.Text = "CreateProcessView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbDataSource.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.lbProcessType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbSpecies.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbSystemicProcessType.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDataSource)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProcessType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDataSourceDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSpeciesDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProcessDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceDataSource)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceSpecies)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceProcess)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSystemicProcessType)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbProteinName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtein)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      protected DevExpress.XtraEditors.TextEdit tbDataSource;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      protected DevExpress.XtraLayout.LayoutControlItem layoutItemDataSource;
      protected DevExpress.XtraLayout.LayoutControlItem layoutItemProtein;
      protected PKSim.UI.Views.Core.UxImageComboBoxEdit cbSpecies;
      protected DevExpress.XtraEditors.TextEdit tbSystemicProcessType;
      protected DevExpress.XtraLayout.LayoutControlItem layoutItemSystemicProcessType;
      protected DevExpress.XtraLayout.LayoutControlItem layoutItemSpecies;
      private DevExpress.XtraEditors.ListBoxControl lbProcessType;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceProcess;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProcessType;
      protected OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.LabelControl lblSpeciesDescription;
      private DevExpress.XtraEditors.LabelControl lblDataSourceDescription;
      protected DevExpress.XtraLayout.LayoutControlItem layoutItemDataSourceDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSpeciesDescription;
      private DevExpress.XtraEditors.LabelControl lblProcessDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProcessDescription;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceDataSource;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceSpecies;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
      protected PKSim.UI.Views.Core.UxMRUEdit cbProteinName;
   }
}