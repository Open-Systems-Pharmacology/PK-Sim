namespace PKSim.UI.Views.AdvancedParameters
{
   partial class AdvancedParametersView
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
         this.panelDistributions = new DevExpress.XtraEditors.PanelControl();
         this.panelConstantParameters = new DevExpress.XtraEditors.PanelControl();
         this.panelAdvancedParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemAddButton = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemRemoveButton = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelDistributions)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelConstantParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelAdvancedParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnRemove);
         this.layoutControl.Controls.Add(this.btnAdd);
         this.layoutControl.Controls.Add(this.panelDistributions);
         this.layoutControl.Controls.Add(this.panelConstantParameters);
         this.layoutControl.Controls.Add(this.panelAdvancedParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup1;
         this.layoutControl.Size = new System.Drawing.Size(635, 516);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnRemove
         // 
         this.btnRemove.Location = new System.Drawing.Point(270, 147);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(81, 22);
         this.btnRemove.StyleController = this.layoutControl;
         this.btnRemove.TabIndex = 8;
         this.btnRemove.Text = "btnRemove";
         // 
         // btnAdd
         // 
         this.btnAdd.Location = new System.Drawing.Point(270, 121);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(81, 22);
         this.btnAdd.StyleController = this.layoutControl;
         this.btnAdd.TabIndex = 7;
         this.btnAdd.Text = "btnAdd";
         // 
         // panelDistributions
         // 
         this.panelDistributions.Location = new System.Drawing.Point(270, 300);
         this.panelDistributions.Margin = new System.Windows.Forms.Padding(0);
         this.panelDistributions.Name = "panelDistributions";
         this.panelDistributions.Size = new System.Drawing.Size(363, 214);
         this.panelDistributions.TabIndex = 6;
         // 
         // panelConstantParameters
         // 
         this.panelConstantParameters.Location = new System.Drawing.Point(2, 2);
         this.panelConstantParameters.Name = "panelConstantParameters";
         this.panelConstantParameters.Size = new System.Drawing.Size(264, 512);
         this.panelConstantParameters.TabIndex = 5;
         // 
         // panelAdvancedParameters
         // 
         this.panelAdvancedParameters.Location = new System.Drawing.Point(355, 2);
         this.panelAdvancedParameters.Name = "panelAdvancedParameters";
         this.panelAdvancedParameters.Size = new System.Drawing.Size(278, 294);
         this.panelAdvancedParameters.TabIndex = 4;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutItemAddButton,
            this.layoutItemRemoveButton,
            this.layoutControlItem1,
            this.emptySpaceItem1,
            this.emptySpaceItem2});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(635, 516);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutControlItem2
         // 
         this.layoutControlItem2.Control = this.panelConstantParameters;
         this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
         this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlItem2.Name = "layoutControlItem2";
         this.layoutControlItem2.Size = new System.Drawing.Size(268, 516);
         this.layoutControlItem2.Text = "layoutControlItem2";
         this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem2.TextToControlDistance = 0;
         this.layoutControlItem2.TextVisible = false;
         // 
         // layoutControlItem3
         // 
         this.layoutControlItem3.Control = this.panelDistributions;
         this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
         this.layoutControlItem3.Location = new System.Drawing.Point(268, 298);
         this.layoutControlItem3.Name = "layoutControlItem3";
         this.layoutControlItem3.Size = new System.Drawing.Size(367, 218);
         this.layoutControlItem3.Text = "layoutControlItem3";
         this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem3.TextToControlDistance = 0;
         this.layoutControlItem3.TextVisible = false;
         // 
         // layoutControlItem4
         // 
         this.layoutItemAddButton.Control = this.btnAdd;
         this.layoutItemAddButton.CustomizationFormText = "layoutControlItem4";
         this.layoutItemAddButton.Location = new System.Drawing.Point(268, 119);
         this.layoutItemAddButton.Name = "layoutControlItem4";
         this.layoutItemAddButton.Size = new System.Drawing.Size(85, 26);
         this.layoutItemAddButton.Text = "layoutControlItem4";
         this.layoutItemAddButton.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAddButton.TextToControlDistance = 0;
         this.layoutItemAddButton.TextVisible = false;
         // 
         // layoutControlItem5
         // 
         this.layoutItemRemoveButton.Control = this.btnRemove;
         this.layoutItemRemoveButton.CustomizationFormText = "layoutControlItem5";
         this.layoutItemRemoveButton.Location = new System.Drawing.Point(268, 145);
         this.layoutItemRemoveButton.Name = "layoutControlItem5";
         this.layoutItemRemoveButton.Size = new System.Drawing.Size(85, 26);
         this.layoutItemRemoveButton.Text = "layoutControlItem5";
         this.layoutItemRemoveButton.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemRemoveButton.TextToControlDistance = 0;
         this.layoutItemRemoveButton.TextVisible = false;
         // 
         // layoutControlItem1
         // 
         this.layoutControlItem1.Control = this.panelAdvancedParameters;
         this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
         this.layoutControlItem1.Location = new System.Drawing.Point(353, 0);
         this.layoutControlItem1.Name = "layoutControlItem1";
         this.layoutControlItem1.Size = new System.Drawing.Size(282, 298);
         this.layoutControlItem1.Text = "layoutControlItem1";
         this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
         this.layoutControlItem1.TextToControlDistance = 0;
         this.layoutControlItem1.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
         this.emptySpaceItem1.Location = new System.Drawing.Point(268, 171);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(85, 127);
         this.emptySpaceItem1.Text = "emptySpaceItem1";
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
         this.emptySpaceItem2.Location = new System.Drawing.Point(268, 0);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(85, 119);
         this.emptySpaceItem2.Text = "emptySpaceItem2";
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // AdvancedParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "AdvancedParametersView";
         this.Size = new System.Drawing.Size(635, 516);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelDistributions)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelConstantParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelAdvancedParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.SimpleButton btnRemove;
      private DevExpress.XtraEditors.SimpleButton btnAdd;
      private DevExpress.XtraEditors.PanelControl panelDistributions;
      private DevExpress.XtraEditors.PanelControl panelConstantParameters;
      private DevExpress.XtraEditors.PanelControl panelAdvancedParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAddButton;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemRemoveButton;
      private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
   }
}
