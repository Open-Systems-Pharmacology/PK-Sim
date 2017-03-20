namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundProtocolView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.uxProtocolSelection = new PKSim.UI.Views.UxBuildingBlockSelection();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemProtocol = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtocol)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.uxProtocolSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(374, 118);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // uxProtocolSelection
         // 
         this.uxProtocolSelection.AllowEmptySelection = false;
         this.uxProtocolSelection.Caption = "";
         this.uxProtocolSelection.Location = new System.Drawing.Point(98, 2);
         this.uxProtocolSelection.MaximumSize = new System.Drawing.Size(10000, 26);
         this.uxProtocolSelection.MinimumSize = new System.Drawing.Size(0, 26);
         this.uxProtocolSelection.Name = "uxProtocolSelection";
         this.uxProtocolSelection.Size = new System.Drawing.Size(273, 26);
         this.uxProtocolSelection.TabIndex = 13;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemProtocol});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(374, 118);
         this.layoutControlGroup.Text = "layoutControlGroup1";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemProtocol
         // 
         this.layoutItemProtocol.Control = this.uxProtocolSelection;
         this.layoutItemProtocol.CustomizationFormText = "layoutItemProtocol";
         this.layoutItemProtocol.Location = new System.Drawing.Point(0, 0);
         this.layoutItemProtocol.Name = "layoutItemProtocol";
         this.layoutItemProtocol.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 3, 2, 2);
         this.layoutItemProtocol.Size = new System.Drawing.Size(374, 118);
         this.layoutItemProtocol.Text = "layoutItemProtocol";
         this.layoutItemProtocol.TextSize = new System.Drawing.Size(91, 13);
         // 
         // SimulationCompoundProtocolView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationCompoundProtocolView";
         this.Size = new System.Drawing.Size(374, 118);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemProtocol)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private UxBuildingBlockSelection uxProtocolSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemProtocol;
   }
}
