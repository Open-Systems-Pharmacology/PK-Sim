namespace PKSim.UI.Views.Simulations
{
   partial class PopulationSimulationSettingsView
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.panel = new DevExpress.XtraEditors.PanelControl();
         this.layoutItemPanel = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanel)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(467, 12);
         this.btnCancel.Size = new System.Drawing.Size(96, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(350, 12);
         this.btnOk.Size = new System.Drawing.Size(113, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 513);
         this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(874, 332, 250, 350);
         this.layoutControlBase.Size = new System.Drawing.Size(575, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(165, 22);
         // 
         // layoutControl1
         // 
         this.layoutControl1.Controls.Add(this.panel);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(575, 513);
         this.layoutControl1.TabIndex = 34;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemPanel});
         this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(575, 513);
         this.layoutControlGroup1.Text = "layoutControlGroup1";
         this.layoutControlGroup1.TextVisible = false;
         // 
         // panel
         // 
         this.panel.Location = new System.Drawing.Point(12, 12);
         this.panel.Name = "panel";
         this.panel.Size = new System.Drawing.Size(551, 489);
         this.panel.TabIndex = 4;
         // 
         // layoutItemPanel
         // 
         this.layoutItemPanel.Control = this.panel;
         this.layoutItemPanel.CustomizationFormText = "layoutItemPanel";
         this.layoutItemPanel.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPanel.Name = "layoutItemPanel";
         this.layoutItemPanel.Size = new System.Drawing.Size(555, 493);
         this.layoutItemPanel.Text = "layoutItemPanel";
         this.layoutItemPanel.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemPanel.TextToControlDistance = 0;
         this.layoutItemPanel.TextVisible = false;
         // 
         // PopulationSimulationSettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "PopulationSimulationSettingsView";
         this.ClientSize = new System.Drawing.Size(575, 559);
         this.Controls.Add(this.layoutControl1);
         this.Name = "PopulationSimulationSettingsView";
         this.Text = "PopulationSimulationSettingsView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPanel)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraEditors.PanelControl panel;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPanel;

   }
}