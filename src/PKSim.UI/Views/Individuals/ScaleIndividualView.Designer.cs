namespace PKSim.UI.Views.Individuals
{
   partial class ScaleIndividualView
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
         _scaleIndividualPropertiesBinder.Dispose();
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.tabScaleIndividual = new DevExpress.XtraTab.XtraTabControl();
         this.layoutControlScaleIndividual = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tbIndividualName = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemIndividualTabs = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemIndividualName = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabScaleIndividual)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlScaleIndividual)).BeginInit();
         this.layoutControlScaleIndividual.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbIndividualName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualTabs)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualName)).BeginInit();
         this.SuspendLayout();
         // 
         // tabScaleIndividual
         // 
         this.tabScaleIndividual.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
         this.tabScaleIndividual.Location = new System.Drawing.Point(12, 36);
         this.tabScaleIndividual.Name = "tabScaleIndividual";
         this.tabScaleIndividual.Size = new System.Drawing.Size(691, 427);
         this.tabScaleIndividual.TabIndex = 4;
         // 
         // layoutControlScaleIndividual
         // 
         this.layoutControlScaleIndividual.AllowCustomization = false;
         this.layoutControlScaleIndividual.Controls.Add(this.tbIndividualName);
         this.layoutControlScaleIndividual.Controls.Add(this.tabScaleIndividual);
         this.layoutControlScaleIndividual.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControlScaleIndividual.Location = new System.Drawing.Point(0, 0);
         this.layoutControlScaleIndividual.Name = "layoutControlScaleIndividual";
         this.layoutControlScaleIndividual.Root = this.layoutControlGroup2;
         this.layoutControlScaleIndividual.Size = new System.Drawing.Size(715, 475);
         this.layoutControlScaleIndividual.TabIndex = 34;
         this.layoutControlScaleIndividual.Text = "layoutControl2";
         // 
         // tbIndividualName
         // 
         this.tbIndividualName.Location = new System.Drawing.Point(140, 12);
         this.tbIndividualName.Name = "tbIndividualName";
         this.tbIndividualName.Size = new System.Drawing.Size(563, 20);
         this.tbIndividualName.StyleController = this.layoutControlScaleIndividual;
         this.tbIndividualName.TabIndex = 5;
         // 
         // layoutControlGroup2
         // 
         this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
         this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup2.GroupBordersVisible = false;
         this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemIndividualTabs,
            this.layoutItemIndividualName});
         this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup2.Name = "layoutControlGroup2";
         this.layoutControlGroup2.Size = new System.Drawing.Size(715, 475);
         this.layoutControlGroup2.Text = "layoutControlGroup2";
         this.layoutControlGroup2.TextVisible = false;
         // 
         // layoutItemIndividualTabs
         // 
         this.layoutItemIndividualTabs.Control = this.tabScaleIndividual;
         this.layoutItemIndividualTabs.CustomizationFormText = "layoutItemIndividualTabs";
         this.layoutItemIndividualTabs.Location = new System.Drawing.Point(0, 24);
         this.layoutItemIndividualTabs.Name = "layoutItemIndividualTabs";
         this.layoutItemIndividualTabs.Size = new System.Drawing.Size(695, 431);
         this.layoutItemIndividualTabs.Text = "layoutItemIndividualTabs";
         this.layoutItemIndividualTabs.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemIndividualTabs.TextToControlDistance = 0;
         this.layoutItemIndividualTabs.TextVisible = false;
         // 
         // layoutItemIndividualName
         // 
         this.layoutItemIndividualName.Control = this.tbIndividualName;
         this.layoutItemIndividualName.CustomizationFormText = "layoutItemIndividualName";
         this.layoutItemIndividualName.Location = new System.Drawing.Point(0, 0);
         this.layoutItemIndividualName.Name = "layoutItemIndividualName";
         this.layoutItemIndividualName.Size = new System.Drawing.Size(695, 24);
         this.layoutItemIndividualName.Text = "layoutItemIndividualName";
         this.layoutItemIndividualName.TextSize = new System.Drawing.Size(125, 13);
         // 
         // ScaleIndividualView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ScaleIndividualView";
         this.ClientSize = new System.Drawing.Size(715, 521);
         this.Controls.Add(this.layoutControlScaleIndividual);
         this.Name = "ScaleIndividualView";
         this.Text = "ScaleIndividualView";
         this.Controls.SetChildIndex(this.layoutControlScaleIndividual, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tabScaleIndividual)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlScaleIndividual)).EndInit();
         this.layoutControlScaleIndividual.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbIndividualName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualTabs)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualName)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private DevExpress.XtraTab.XtraTabControl tabScaleIndividual;
      private DevExpress.XtraEditors.TextEdit tbIndividualName;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividualTabs;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividualName;
      private OSPSuite.UI.Controls.UxLayoutControl layoutControlScaleIndividual;

   }
}