namespace PKSim.UI.Views.Simulations
{
   partial class SimulationCompoundOverwriteParameterSetSelectionView
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
         this.cbOverwriteParameterSet = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemOverwriteParameterSet = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbOverwriteParameterSet.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOverwriteParameterSet)).BeginInit();
         this.SuspendLayout();
         //
         // layoutControl
         //
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.cbOverwriteParameterSet);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsView.UseSkinIndents = false;
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(419, 32);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl";
         //
         // cbOverwriteParameterSet
         //
         this.cbOverwriteParameterSet.Location = new System.Drawing.Point(120, 4);
         this.cbOverwriteParameterSet.Name = "cbOverwriteParameterSet";
         this.cbOverwriteParameterSet.Properties.AllowMouseWheel = false;
         this.cbOverwriteParameterSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbOverwriteParameterSet.Size = new System.Drawing.Size(295, 24);
         this.cbOverwriteParameterSet.StyleController = this.layoutControl;
         this.cbOverwriteParameterSet.TabIndex = 0;
         //
         // layoutControlGroup
         //
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemOverwriteParameterSet});
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.OptionsItemText.TextToControlDistance = 5;
         this.layoutControlGroup.Size = new System.Drawing.Size(419, 32);
         this.layoutControlGroup.TextVisible = false;
         //
         // layoutItemOverwriteParameterSet
         //
         this.layoutItemOverwriteParameterSet.Control = this.cbOverwriteParameterSet;
         this.layoutItemOverwriteParameterSet.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOverwriteParameterSet.Name = "layoutItemOverwriteParameterSet";
         this.layoutItemOverwriteParameterSet.Size = new System.Drawing.Size(419, 32);
         this.layoutItemOverwriteParameterSet.TextSize = new System.Drawing.Size(114, 16);
         //
         // SimulationCompoundOverwriteParameterSetSelectionView
         //
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "SimulationCompoundOverwriteParameterSetSelectionView";
         this.Size = new System.Drawing.Size(419, 32);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbOverwriteParameterSet.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOverwriteParameterSet)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbOverwriteParameterSet;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOverwriteParameterSet;
   }
}
