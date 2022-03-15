namespace PKSim.UI.Views
{
   partial class AboutView
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutView));
         this.linkSite = new DevExpress.XtraEditors.HyperLinkEdit();
         this.licenseAgreementLink = new DevExpress.XtraEditors.HyperLinkEdit();
         this.lblProductInfo = new DevExpress.XtraEditors.LabelControl();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.linkSite.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.licenseAgreementLink.Properties)).BeginInit();
         this.SuspendLayout();
       
         // 
         // linkSite
         // 
         this.linkSite.EditValue = "www.open-systems-pharmacology.org";
         this.linkSite.Location = new System.Drawing.Point(531, 179);
         this.linkSite.Name = "linkSite";
         this.linkSite.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.linkSite.Properties.Appearance.Options.UseBackColor = true;
         this.linkSite.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.linkSite.Size = new System.Drawing.Size(194, 18);
         this.linkSite.TabIndex = 7;
         // 
         // licenseAgreementLink
         // 
         this.licenseAgreementLink.EditValue = "licenseAgreementLink";
         this.licenseAgreementLink.Location = new System.Drawing.Point(531, 203);
         this.licenseAgreementLink.Name = "licenseAgreementLink";
         this.licenseAgreementLink.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.licenseAgreementLink.Properties.Appearance.Options.UseBackColor = true;
         this.licenseAgreementLink.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.licenseAgreementLink.Size = new System.Drawing.Size(194, 18);
         this.licenseAgreementLink.TabIndex = 9;
         // 
         // lblProductInfo
         // 
         this.lblProductInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
         this.lblProductInfo.Location = new System.Drawing.Point(18, 181);
         this.lblProductInfo.Name = "lblProductInfo";
         this.lblProductInfo.Size = new System.Drawing.Size(300, 13);
         this.lblProductInfo.TabIndex = 6;
         this.lblProductInfo.Text = "lblProductInfo";
         // 
         // AboutView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Tile;
         this.BackgroundImageStore = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImageStore")));
         this.Caption = "AboutView";
         this.ClientSize = new System.Drawing.Size(769, 299);
         this.Controls.Add(this.lblProductInfo);
         this.Controls.Add(this.linkSite);
         this.Controls.Add(this.licenseAgreementLink);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "AboutView";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.Text = "AboutView";
         this.Controls.SetChildIndex(this.licenseAgreementLink, 0);
         this.Controls.SetChildIndex(this.linkSite, 0);
         this.Controls.SetChildIndex(this.lblProductInfo, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.linkSite.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.licenseAgreementLink.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private DevExpress.XtraEditors.HyperLinkEdit linkSite;
      private DevExpress.XtraEditors.HyperLinkEdit licenseAgreementLink;
      private DevExpress.XtraEditors.LabelControl lblProductInfo;
   }
}