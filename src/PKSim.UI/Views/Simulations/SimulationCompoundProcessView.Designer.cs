using PKSim.Core.Model;
using PKSim.Presentation.DTO.Simulations;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundProcessView<TPartialProcess, TPartialProcessDTO> 
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
         _gridViewPartialBinder.Dispose();
         _gridViewSystemicBinder.Dispose();
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelWarning = new OSPSuite.UI.Controls.UxHintPanel();
         this.gridSystemicProcesses = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewSystemicProcesses = new PKSim.UI.Views.Core.UxGridView();
         this.gridPartialProcesses = new OSPSuite.UI.Controls.UxGridControl();
         this.gridViewPartialProcesses = new PKSim.UI.Views.Core.UxGridView();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemPartialProcesses = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSystemicProcesses = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemWarning = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.gridSystemicProcesses)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewSystemicProcesses)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridPartialProcesses)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewPartialProcesses)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPartialProcesses)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSystemicProcesses)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.AutoScroll = false;
         this.layoutControl.Controls.Add(this.panelWarning);
         this.layoutControl.Controls.Add(this.gridSystemicProcesses);
         this.layoutControl.Controls.Add(this.gridPartialProcesses);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutMainGroup;
         this.layoutControl.Size = new System.Drawing.Size(352, 325);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // panelWarning
         // 
         this.panelWarning.Location = new System.Drawing.Point(2, 2);
         this.panelWarning.MaximumSize = new System.Drawing.Size(1000000, 40);
         this.panelWarning.MinimumSize = new System.Drawing.Size(200, 40);
         this.panelWarning.Name = "panelWarning";
         this.panelWarning.NoteText = "";
         this.panelWarning.Size = new System.Drawing.Size(348, 40);
         this.panelWarning.TabIndex = 6;
         // 
         // gridSystemicProcesses
         // 
         this.gridSystemicProcesses.Location = new System.Drawing.Point(2, 46);
         this.gridSystemicProcesses.MainView = this.gridViewSystemicProcesses;
         this.gridSystemicProcesses.Name = "gridSystemicProcesses";
         this.gridSystemicProcesses.Size = new System.Drawing.Size(348, 119);
         this.gridSystemicProcesses.TabIndex = 5;
         this.gridSystemicProcesses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewSystemicProcesses});
         // 
         // gridViewSystemicProcesses
         // 
         this.gridViewSystemicProcesses.AllowsFiltering = true;
         this.gridViewSystemicProcesses.EnableColumnContextMenu = true;
         this.gridViewSystemicProcesses.GridControl = this.gridSystemicProcesses;
         this.gridViewSystemicProcesses.MultiSelect = true;
         this.gridViewSystemicProcesses.Name = "gridViewSystemicProcesses";
         this.gridViewSystemicProcesses.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridViewSystemicProcesses.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewSystemicProcesses.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewSystemicProcesses.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // gridPartialProcesses
         // 
         this.gridPartialProcesses.Location = new System.Drawing.Point(2, 169);
         this.gridPartialProcesses.MainView = this.gridViewPartialProcesses;
         this.gridPartialProcesses.Name = "gridPartialProcesses";
         this.gridPartialProcesses.Size = new System.Drawing.Size(348, 154);
         this.gridPartialProcesses.TabIndex = 4;
         this.gridPartialProcesses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewPartialProcesses});
         // 
         // gridViewPartialProcesses
         // 
         this.gridViewPartialProcesses.AllowsFiltering = true;
         this.gridViewPartialProcesses.EnableColumnContextMenu = true;
         this.gridViewPartialProcesses.GridControl = this.gridPartialProcesses;
         this.gridViewPartialProcesses.MultiSelect = true;
         this.gridViewPartialProcesses.Name = "gridViewPartialProcesses";
         this.gridViewPartialProcesses.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
         this.gridViewPartialProcesses.OptionsNavigation.AutoFocusNewRow = true;
         this.gridViewPartialProcesses.OptionsSelection.EnableAppearanceFocusedCell = false;
         this.gridViewPartialProcesses.OptionsSelection.EnableAppearanceFocusedRow = false;
         // 
         // layoutMainGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutMainGroup";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPartialProcesses,
            this.layoutItemSystemicProcesses,
            this.layoutItemWarning});
         this.layoutMainGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutMainGroup.Name = "layoutControlGroup1";
         this.layoutMainGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutMainGroup.Size = new System.Drawing.Size(352, 325);
         this.layoutMainGroup.Text = "layoutMainGroup";
         this.layoutMainGroup.TextVisible = false;
         // 
         // layoutItemPartialProcesses
         // 
         this.layoutItemPartialProcesses.Control = this.gridPartialProcesses;
         this.layoutItemPartialProcesses.CustomizationFormText = "layoutItemPartialProcesses";
         this.layoutItemPartialProcesses.Location = new System.Drawing.Point(0, 167);
         this.layoutItemPartialProcesses.Name = "layoutItemPartialProcesses";
         this.layoutItemPartialProcesses.Size = new System.Drawing.Size(352, 158);
         this.layoutItemPartialProcesses.Text = "layoutItemPartialProcesses";
         this.layoutItemPartialProcesses.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPartialProcesses.TextToControlDistance = 0;
         this.layoutItemPartialProcesses.TextVisible = false;
         // 
         // layoutItemSystemicProcesses
         // 
         this.layoutItemSystemicProcesses.Control = this.gridSystemicProcesses;
         this.layoutItemSystemicProcesses.CustomizationFormText = "layoutItemSystemicProcesses";
         this.layoutItemSystemicProcesses.Location = new System.Drawing.Point(0, 44);
         this.layoutItemSystemicProcesses.Name = "layoutItemTotalProcesses";
         this.layoutItemSystemicProcesses.Size = new System.Drawing.Size(352, 123);
         this.layoutItemSystemicProcesses.Text = "layoutItemSystemicProcesses";
         this.layoutItemSystemicProcesses.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSystemicProcesses.TextToControlDistance = 0;
         this.layoutItemSystemicProcesses.TextVisible = false;
         // 
         // layoutItemWarning
         // 
         this.layoutItemWarning.Control = this.panelWarning;
         this.layoutItemWarning.CustomizationFormText = "layoutItemWarning";
         this.layoutItemWarning.Location = new System.Drawing.Point(0, 0);
         this.layoutItemWarning.Name = "layoutItemWarning";
         this.layoutItemWarning.Size = new System.Drawing.Size(352, 44);
         this.layoutItemWarning.Text = "layoutItemWarning";
         this.layoutItemWarning.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemWarning.TextToControlDistance = 0;
         this.layoutItemWarning.TextVisible = false;
         // 
         // SimulationCompoundProcessView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
         this.Name = "SimulationCompoundProcessView";
         this.Size = new System.Drawing.Size(352, 325);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.gridSystemicProcesses)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewSystemicProcesses)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridPartialProcesses)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.gridViewPartialProcesses)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPartialProcesses)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSystemicProcesses)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemWarning)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private OSPSuite.UI.Controls.UxGridControl gridSystemicProcesses;
      private UxGridView gridViewSystemicProcesses;
      private OSPSuite.UI.Controls.UxGridControl gridPartialProcesses;
      protected UxGridView gridViewPartialProcesses;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPartialProcesses;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSystemicProcesses;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxHintPanel panelWarning;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemWarning;
   }
}
