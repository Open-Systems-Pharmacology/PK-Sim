namespace PKSim.UI.Views.Simulations
{
   partial class SimulationSubjectConfigurationView
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
         this.chkAllowAging = new OSPSuite.UI.Controls.UxCheckEdit();
         this.panelIndividualSelection = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemIndividual = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemAllowAging = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.chkAllowAging.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelIndividualSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividual)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAllowAging)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.chkAllowAging);
         this.layoutControl.Controls.Add(this.panelIndividualSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(595, 280, 612, 461);
         this.layoutControl.OptionsView.UseSkinIndents = false;
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(419, 60);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // chkAllowAging
         // 
         this.chkAllowAging.AllowClicksOutsideControlArea = false;
         this.chkAllowAging.AutoSizeInLayoutControl = true;
         this.chkAllowAging.Location = new System.Drawing.Point(3, 32);
         this.chkAllowAging.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.chkAllowAging.Name = "chkAllowAging";
         this.chkAllowAging.Properties.Caption = "chkAllowAging";
         this.chkAllowAging.Size = new System.Drawing.Size(110, 24);
         this.chkAllowAging.StyleController = this.layoutControl;
         this.chkAllowAging.TabIndex = 9;
         // 
         // panelIndividualSelection
         // 
         this.panelIndividualSelection.Location = new System.Drawing.Point(120, 0);
         this.panelIndividualSelection.Margin = new System.Windows.Forms.Padding(0);
         this.panelIndividualSelection.Name = "panelIndividualSelection";
         this.panelIndividualSelection.Size = new System.Drawing.Size(299, 28);
         this.panelIndividualSelection.TabIndex = 7;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemIndividual,
            this.layoutItemAllowAging});
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup.Size = new System.Drawing.Size(419, 60);
         this.layoutControlGroup.Text = "layoutControlGroup1";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemIndividual
         // 
         this.layoutItemIndividual.Control = this.panelIndividualSelection;
         this.layoutItemIndividual.CustomizationFormText = "layoutItemIndividualSelection";
         this.layoutItemIndividual.Location = new System.Drawing.Point(0, 0);
         this.layoutItemIndividual.Name = "layoutItemIndividual";
         this.layoutItemIndividual.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemIndividual.Size = new System.Drawing.Size(419, 28);
         this.layoutItemIndividual.TextSize = new System.Drawing.Size(114, 16);
         // 
         // layoutItemAllowAging
         // 
         this.layoutItemAllowAging.Control = this.chkAllowAging;
         this.layoutItemAllowAging.CustomizationFormText = "layoutItemAllowAging";
         this.layoutItemAllowAging.Location = new System.Drawing.Point(0, 28);
         this.layoutItemAllowAging.Name = "layoutItemAllowAging";
         this.layoutItemAllowAging.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 4, 4);
         this.layoutItemAllowAging.Size = new System.Drawing.Size(419, 32);
         this.layoutItemAllowAging.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemAllowAging.TextVisible = false;
         // 
         // SimulationSubjectConfigurationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
         this.Name = "SimulationSubjectConfigurationView";
         this.Size = new System.Drawing.Size(419, 60);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.chkAllowAging.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelIndividualSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividual)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemAllowAging)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.PanelControl panelIndividualSelection;
      private OSPSuite.UI.Controls.UxCheckEdit chkAllowAging;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividual;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemAllowAging;
   }
}


