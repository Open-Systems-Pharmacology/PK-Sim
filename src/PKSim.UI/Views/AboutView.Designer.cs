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
         this.peImage = new DevExpress.XtraEditors.PictureEdit();
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.linkSite.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.licenseAgreementLink.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).BeginInit();
         this.SuspendLayout();
         // 
         // tablePanel
         // 
         this.tablePanel.Location = new System.Drawing.Point(0, 256);
         this.tablePanel.Size = new System.Drawing.Size(769, 43);
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
         // peImage
         // 
         this.peImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.peImage.EditValue = ((object)(resources.GetObject("peImage.EditValue")));
         this.peImage.Location = new System.Drawing.Point(18, 9);
         this.peImage.Margin = new System.Windows.Forms.Padding(0);
         this.peImage.Name = "peImage";
         this.peImage.Properties.AllowFocused = false;
         this.peImage.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
         this.peImage.Properties.Appearance.Options.UseBackColor = true;
         this.peImage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.peImage.Properties.ShowMenu = false;
         this.peImage.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
         this.peImage.Properties.SvgImageColorizationMode = DevExpress.Utils.SvgImageColorizationMode.None;
         this.peImage.Size = new System.Drawing.Size(728, 142);
         this.peImage.TabIndex = 39;
         // 
         // AboutView
         // 
         this.Appearance.BackColor = System.Drawing.Color.White;
         this.Appearance.Options.UseBackColor = true;
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "AboutView";
         this.ClientSize = new System.Drawing.Size(769, 299);
         this.Controls.Add(this.peImage);
         this.Controls.Add(this.lblProductInfo);
         this.Controls.Add(this.linkSite);
         this.Controls.Add(this.licenseAgreementLink);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "AboutView";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.Text = "AboutView";
         this.Controls.SetChildIndex(this.tablePanel, 0);
         this.Controls.SetChildIndex(this.licenseAgreementLink, 0);
         this.Controls.SetChildIndex(this.linkSite, 0);
         this.Controls.SetChildIndex(this.lblProductInfo, 0);
         this.Controls.SetChildIndex(this.peImage, 0);
         ((System.ComponentModel.ISupportInitialize)(this.tablePanel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.linkSite.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.licenseAgreementLink.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.peImage.Properties)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion
      private DevExpress.XtraEditors.HyperLinkEdit linkSite;
      private DevExpress.XtraEditors.HyperLinkEdit licenseAgreementLink;
      private DevExpress.XtraEditors.LabelControl lblProductInfo;
      private DevExpress.XtraEditors.PictureEdit peImage;
   }
}