using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundConfigurationView
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
         this.layoutSimulationCompound = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutMainGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutSimulationCompound)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutSimulationCompound
         // 
         this.layoutSimulationCompound.AllowCustomization = false;
         this.layoutSimulationCompound.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutSimulationCompound.Location = new System.Drawing.Point(0, 0);
         this.layoutSimulationCompound.Name = "layoutSimulationCompound";
         this.layoutSimulationCompound.Root = this.layoutMainGroup;
         this.layoutSimulationCompound.Size = new System.Drawing.Size(622, 367);
         this.layoutSimulationCompound.TabIndex = 3;
         this.layoutSimulationCompound.Text = "layoutControl1";
         // 
         // layoutControlGroup
         // 
         this.layoutMainGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutMainGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutMainGroup.GroupBordersVisible = false;
         this.layoutMainGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutMainGroup.Name = "layoutMainGroup";
         this.layoutMainGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutMainGroup.Size = new System.Drawing.Size(622, 367);
         this.layoutMainGroup.Text = "layoutControlGroup1";
         this.layoutMainGroup.TextVisible = false;
         // 
         // SimulationCompoundConfigurationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutSimulationCompound);
         this.Name = "SimulationCompoundConfigurationView";
         this.Size = new System.Drawing.Size(622, 367);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutSimulationCompound)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutMainGroup)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutMainGroup;
      private OSPSuite.UI.Controls.UxLayoutControl layoutSimulationCompound;
   }
}


