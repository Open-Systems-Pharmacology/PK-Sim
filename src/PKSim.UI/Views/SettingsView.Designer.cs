namespace PKSim.UI.Views
{
   partial class SettingsView
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
         this.tabSettings = new DevExpress.XtraTab.XtraTabControl();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabSettings)).BeginInit();
         this.SuspendLayout();
         // 
         // tabSettings
         // 
         this.tabSettings.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tabSettings.Location = new System.Drawing.Point(0, 0);
         this.tabSettings.Name = "tabSettings";
         this.tabSettings.Size = new System.Drawing.Size(460, 558);
         this.tabSettings.TabIndex = 34;
         // 
         // SettingsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SettingsView";
         this.ClientSize = new System.Drawing.Size(550, 650);
         this.Controls.Add(this.tabSettings);
         this.Name = "SettingsView";
         this.Text = "SettingsView";
         this.Controls.SetChildIndex(this.tabSettings, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabSettings)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraTab.XtraTabControl tabSettings;
   }
}