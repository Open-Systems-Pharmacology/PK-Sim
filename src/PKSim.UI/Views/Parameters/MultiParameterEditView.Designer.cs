using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Parameters
{
   partial class MultiParameterEditView
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
         this.gridParameters = new PKSim.UI.Views.Core.UxGridControl();
         this.gridViewParameters = new PKSim.UI.Views.Core.UxGridView();
         this.layoutControlParameters = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelScaling = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemScaling = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItemForScaling = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlParameters)).BeginInit();
         this.layoutControlParameters.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelScaling)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemScaling)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemForScaling)).BeginInit();
         this.SuspendLayout();
         // 
         // gridParameters
         // 
         this.gridParameters.Location = new System.Drawing.Point(0, 27);
         this.gridParameters.MainView = this.gridViewParameters;
         this.gridParameters.Name = "gridParameters";
         this.gridParameters.Size = new System.Drawing.Size(526, 347);
         this.gridParameters.TabIndex = 4;
         this.gridParameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewParameters});
         // 
         // gridViewParameters
         // 
         this.gridViewParameters.AllowsFiltering = true;
         this.gridViewParameters.EnableColumnContextMenu = true;
         this.gridViewParameters.GridControl = this.gridParameters;
         this.gridViewParameters.MultiSelect = true;
         this.gridViewParameters.Name = "gridViewParameters";
         this.gridViewParameters.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewParameters.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutControlParameters
         // 
         this.layoutControlParameters.AllowCustomization = false;
         this.layoutControlParameters.Controls.Add(this.gridParameters);
         this.layoutControlParameters.Controls.Add(this.panelScaling);
         this.layoutControlParameters.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutControlParameters.Name = "layoutControlParameters";
         this.layoutControlParameters.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(607, 202, 250, 350);
         this.layoutControlParameters.Root = this.layoutControlGroup1;
         this.layoutControlParameters.Size = new System.Drawing.Size(526, 374);
         this.layoutControlParameters.TabIndex = 5;
         this.layoutControlParameters.Text = "layoutControl1";
         // 
         // panelScaling
         // 
         this.panelScaling.Location = new System.Drawing.Point(275, 2);
         this.panelScaling.Name = "panelScaling";
         this.panelScaling.Size = new System.Drawing.Size(249, 23);
         this.panelScaling.TabIndex = 5;
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.layoutItemScaling,
            this.emptySpaceItemForScaling});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup1.Size = new System.Drawing.Size(526, 374);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.gridParameters;
         this.layoutItemParameters.CustomizationFormText = "layoutItemParameters";
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 27);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemParameters.Size = new System.Drawing.Size(526, 347);
         this.layoutItemParameters.Text = "layoutItemParameters";
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextToControlDistance = 0;
         this.layoutItemParameters.TextVisible = false;
         // 
         // layoutItemScaling
         // 
         this.layoutItemScaling.Control = this.panelScaling;
         this.layoutItemScaling.CustomizationFormText = "layoutItemScaling";
         this.layoutItemScaling.Location = new System.Drawing.Point(273, 0);
         this.layoutItemScaling.MaxSize = new System.Drawing.Size(253, 27);
         this.layoutItemScaling.MinSize = new System.Drawing.Size(253, 27);
         this.layoutItemScaling.Name = "layoutItemScaling";
         this.layoutItemScaling.Size = new System.Drawing.Size(253, 27);
         this.layoutItemScaling.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemScaling.Text = "layoutItemScaling";
         this.layoutItemScaling.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemScaling.TextToControlDistance = 0;
         this.layoutItemScaling.TextVisible = false;
         // 
         // emptySpaceItemForScaling
         // 
         this.emptySpaceItemForScaling.AllowHotTrack = false;
         this.emptySpaceItemForScaling.CustomizationFormText = "emptySpaceItemForScaling";
         this.emptySpaceItemForScaling.Location = new System.Drawing.Point(0, 0);
         this.emptySpaceItemForScaling.Name = "emptySpaceItem1";
         this.emptySpaceItemForScaling.Size = new System.Drawing.Size(273, 27);
         this.emptySpaceItemForScaling.Text = "emptySpaceItemForScaling";
         this.emptySpaceItemForScaling.TextSize = new System.Drawing.Size(0, 0);
         // 
         // MultiParameterEditView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControlParameters);
         this.Margin = new System.Windows.Forms.Padding(0);
         this.Name = "MultiParameterEditView";
         this.Size = new System.Drawing.Size(526, 374);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlParameters)).EndInit();
         this.layoutControlParameters.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelScaling)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemScaling)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemForScaling)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private UxGridControl gridParameters;
      private UxGridView gridViewParameters;
      private DevExpress.XtraEditors.PanelControl panelScaling;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemScaling;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItemForScaling;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControlParameters;
   }
}