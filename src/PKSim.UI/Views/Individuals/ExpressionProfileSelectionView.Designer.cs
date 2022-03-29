namespace PKSim.UI.Views.Individuals
{
   partial class ExpressionProfileSelectionView
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl1 = new OSPSuite.UI.Controls.UxLayoutControl();
         this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
         this.btnCreate = new OSPSuite.UI.Controls.UxSimpleButton();
         this.cbExpressionProfile = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemExpressionProfileSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemCreate = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemLoad = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbExpressionProfile.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionProfileSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl1
         // 
         this.layoutControl1.AllowCustomization = false;
         this.layoutControl1.Controls.Add(this.btnLoad);
         this.layoutControl1.Controls.Add(this.btnCreate);
         this.layoutControl1.Controls.Add(this.cbExpressionProfile);
         this.layoutControl1.Controls.Add(this.lblDescription);
         this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl1.Location = new System.Drawing.Point(0, 0);
         this.layoutControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(584, 94);
         this.layoutControl1.TabIndex = 34;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // btnLoad
         // 
         this.btnLoad.Location = new System.Drawing.Point(516, 34);
         this.btnLoad.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.btnLoad.Name = "btnLoad";
         this.btnLoad.Size = new System.Drawing.Size(54, 27);
         this.btnLoad.StyleController = this.layoutControl1;
         this.btnLoad.TabIndex = 9;
         this.btnLoad.Text = "btnLoad";
         // 
         // btnCreate
         // 
         this.btnCreate.Location = new System.Drawing.Point(446, 34);
         this.btnCreate.Manager = null;
         this.btnCreate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.btnCreate.Name = "btnCreate";
         this.btnCreate.Shortcut = System.Windows.Forms.Keys.None;
         this.btnCreate.Size = new System.Drawing.Size(66, 27);
         this.btnCreate.StyleController = this.layoutControl1;
         this.btnCreate.TabIndex = 8;
         this.btnCreate.Text = "btnCreate";
         // 
         // cbExpressionProfile
         // 
         this.cbExpressionProfile.Location = new System.Drawing.Point(237, 34);
         this.cbExpressionProfile.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.cbExpressionProfile.Name = "cbExpressionProfile";
         this.cbExpressionProfile.Properties.AllowMouseWheel = false;
         this.cbExpressionProfile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbExpressionProfile.Size = new System.Drawing.Size(205, 22);
         this.cbExpressionProfile.StyleController = this.layoutControl1;
         this.cbExpressionProfile.TabIndex = 7;
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(14, 14);
         this.lblDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(76, 16);
         this.lblDescription.StyleController = this.layoutControl1;
         this.lblDescription.TabIndex = 6;
         this.lblDescription.Text = "lblDescription";
         // 
         // layoutControlGroup1
         // 
         this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup1.GroupBordersVisible = false;
         this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDescription,
            this.layoutItemExpressionProfileSelection,
            this.layoutItemCreate,
            this.layoutItemLoad,
            this.emptySpaceItem1});
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(584, 94);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(560, 20);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // layoutItemExpressionProfileSelection
         // 
         this.layoutItemExpressionProfileSelection.Control = this.cbExpressionProfile;
         this.layoutItemExpressionProfileSelection.Location = new System.Drawing.Point(0, 20);
         this.layoutItemExpressionProfileSelection.Name = "layoutItemExpressionProfileSelection";
         this.layoutItemExpressionProfileSelection.Size = new System.Drawing.Size(432, 31);
         this.layoutItemExpressionProfileSelection.TextSize = new System.Drawing.Size(209, 16);
         // 
         // layoutItemCreate
         // 
         this.layoutItemCreate.Control = this.btnCreate;
         this.layoutItemCreate.Location = new System.Drawing.Point(432, 20);
         this.layoutItemCreate.Name = "layoutItemCreate";
         this.layoutItemCreate.Size = new System.Drawing.Size(70, 31);
         this.layoutItemCreate.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCreate.TextVisible = false;
         // 
         // layoutItemLoad
         // 
         this.layoutItemLoad.Control = this.btnLoad;
         this.layoutItemLoad.Location = new System.Drawing.Point(502, 20);
         this.layoutItemLoad.Name = "layoutItemLoad";
         this.layoutItemLoad.Size = new System.Drawing.Size(58, 31);
         this.layoutItemLoad.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLoad.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 51);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(560, 19);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // ExpressionProfileSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SimpleProteinView";
         this.ClientSize = new System.Drawing.Size(584, 151);
         this.Controls.Add(this.layoutControl1);
         this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
         this.Name = "ExpressionProfileSelectionView";
         this.Text = "SimpleProteinView";
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.cbExpressionProfile.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionProfileSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl1;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private DevExpress.XtraEditors.SimpleButton btnLoad;
      private OSPSuite.UI.Controls.UxSimpleButton btnCreate;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbExpressionProfile;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemExpressionProfileSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemCreate;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemLoad;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}