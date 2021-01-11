namespace PKSim.UI.Views.Individuals
{
   partial class OntogenySelectionView
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
         _presenter = null;
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.cbOntogey = new DevExpress.XtraEditors.ComboBoxEdit();
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnLoadOntogenyFromFile = new DevExpress.XtraEditors.SimpleButton();
         this.btnShowOntogeny = new DevExpress.XtraEditors.SimpleButton();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLoadOntogeny = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogey.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOntogeny)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoadOntogeny)).BeginInit();
         this.SuspendLayout();
         // 
         // cbOntogey
         // 
         this.cbOntogey.Location = new System.Drawing.Point(2, 2);
         this.cbOntogey.Name = "cbOntogey";
         this.cbOntogey.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbOntogey.Size = new System.Drawing.Size(98, 20);
         this.cbOntogey.StyleController = this.layoutControl;
         this.cbOntogey.TabIndex = 0;
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.btnLoadOntogenyFromFile);
         this.layoutControl.Controls.Add(this.cbOntogey);
         this.layoutControl.Controls.Add(this.btnShowOntogeny);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(349, 26);
         this.layoutControl.TabIndex = 1;
         this.layoutControl.Text = "layoutControl1";
         // 
         // btnLoadOntogenyFromFile
         // 
         this.btnLoadOntogenyFromFile.Location = new System.Drawing.Point(209, 2);
         this.btnLoadOntogenyFromFile.Name = "btnLoadOntogenyFromFile";
         this.btnLoadOntogenyFromFile.Size = new System.Drawing.Size(138, 22);
         this.btnLoadOntogenyFromFile.StyleController = this.layoutControl;
         this.btnLoadOntogenyFromFile.TabIndex = 5;
         this.btnLoadOntogenyFromFile.Text = "btnLoadOntogenyFromFile";
         // 
         // btnShowOntogeny
         // 
         this.btnShowOntogeny.Location = new System.Drawing.Point(104, 2);
         this.btnShowOntogeny.Name = "btnShowOntogeny";
         this.btnShowOntogeny.Size = new System.Drawing.Size(101, 22);
         this.btnShowOntogeny.StyleController = this.layoutControl;
         this.btnShowOntogeny.TabIndex = 4;
         this.btnShowOntogeny.Text = "btnShowOntogeny";
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemOntogeny,
            this.layoutItemButtonOntogeny,
            this.layoutItemLoadOntogeny});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(349, 26);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemOntogeny
         // 
         this.layoutItemOntogeny.Control = this.cbOntogey;
         this.layoutItemOntogeny.CustomizationFormText = "layoutItemOntogeny";
         this.layoutItemOntogeny.Location = new System.Drawing.Point(0, 0);
         this.layoutItemOntogeny.Name = "layoutItemOntogeny";
         this.layoutItemOntogeny.Size = new System.Drawing.Size(102, 26);
         this.layoutItemOntogeny.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemOntogeny.TextVisible = false;
         // 
         // layoutItemButtonOntogeny
         // 
         this.layoutItemButtonOntogeny.Control = this.btnShowOntogeny;
         this.layoutItemButtonOntogeny.CustomizationFormText = "layoutItemButtonOntogeny";
         this.layoutItemButtonOntogeny.Location = new System.Drawing.Point(102, 0);
         this.layoutItemButtonOntogeny.Name = "layoutItemButtonOntogeny";
         this.layoutItemButtonOntogeny.Size = new System.Drawing.Size(105, 26);
         this.layoutItemButtonOntogeny.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonOntogeny.TextVisible = false;
         // 
         // layoutItemLoadOntogeny
         // 
         this.layoutItemLoadOntogeny.Control = this.btnLoadOntogenyFromFile;
         this.layoutItemLoadOntogeny.CustomizationFormText = "layoutControlItem1";
         this.layoutItemLoadOntogeny.Location = new System.Drawing.Point(207, 0);
         this.layoutItemLoadOntogeny.Name = "layoutItemLoadOntogeny";
         this.layoutItemLoadOntogeny.Size = new System.Drawing.Size(142, 26);
         this.layoutItemLoadOntogeny.Text = "layoutControlItem1";
         this.layoutItemLoadOntogeny.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLoadOntogeny.TextVisible = false;
         // 
         // OntogenySelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "OntogenySelectionView";
         this.Size = new System.Drawing.Size(349, 26);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbOntogey.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOntogeny)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoadOntogeny)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraEditors.ComboBoxEdit cbOntogey;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraEditors.SimpleButton btnShowOntogeny;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOntogeny;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonOntogeny;
      private DevExpress.XtraEditors.SimpleButton btnLoadOntogenyFromFile;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLoadOntogeny;
   }
}
