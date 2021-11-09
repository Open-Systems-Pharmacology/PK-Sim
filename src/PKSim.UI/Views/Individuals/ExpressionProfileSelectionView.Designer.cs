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
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.cbExpressionProfile = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.layoutItemExpressionProfileSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnCreate = new OSPSuite.UI.Controls.UxSimpleButton();
         this.layoutItemCreate = new DevExpress.XtraLayout.LayoutControlItem();
         this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
         this.layoutItemLoad = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
         this.layoutControl1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbExpressionProfile.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionProfileSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(406, 12);
         this.btnCancel.Size = new System.Drawing.Size(83, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(305, 12);
         this.btnOk.Size = new System.Drawing.Size(97, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 77);
         this.layoutControlBase.Size = new System.Drawing.Size(501, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(143, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(501, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(293, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(101, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(394, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(87, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(147, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(146, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(147, 26);
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
         this.layoutControl1.Name = "layoutControl1";
         this.layoutControl1.Root = this.layoutControlGroup1;
         this.layoutControl1.Size = new System.Drawing.Size(501, 77);
         this.layoutControl1.TabIndex = 34;
         this.layoutControl1.Text = "layoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 12);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
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
            this.layoutItemLoad});
         this.layoutControlGroup1.Name = "layoutControlGroup1";
         this.layoutControlGroup1.Size = new System.Drawing.Size(501, 77);
         this.layoutControlGroup1.TextVisible = false;
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(481, 17);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // cbExpressionProfile
         // 
         this.cbExpressionProfile.Location = new System.Drawing.Point(192, 29);
         this.cbExpressionProfile.Name = "cbExpressionProfile";
         this.cbExpressionProfile.Properties.AllowMouseWheel = false;
         this.cbExpressionProfile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbExpressionProfile.Size = new System.Drawing.Size(187, 20);
         this.cbExpressionProfile.StyleController = this.layoutControl1;
         this.cbExpressionProfile.TabIndex = 7;
         // 
         // layoutItemExpressionProfileSelection
         // 
         this.layoutItemExpressionProfileSelection.Control = this.cbExpressionProfile;
         this.layoutItemExpressionProfileSelection.Location = new System.Drawing.Point(0, 17);
         this.layoutItemExpressionProfileSelection.Name = "layoutItemExpressionProfileSelection";
         this.layoutItemExpressionProfileSelection.Size = new System.Drawing.Size(371, 40);
         this.layoutItemExpressionProfileSelection.TextSize = new System.Drawing.Size(177, 13);
         // 
         // btnCreate
         // 
         this.btnCreate.Location = new System.Drawing.Point(383, 29);
         this.btnCreate.Manager = null;
         this.btnCreate.Name = "btnCreate";
         this.btnCreate.Shortcut = System.Windows.Forms.Keys.None;
         this.btnCreate.Size = new System.Drawing.Size(56, 22);
         this.btnCreate.StyleController = this.layoutControl1;
         this.btnCreate.TabIndex = 8;
         this.btnCreate.Text = "btnCreate";
         // 
         // layoutItemCreate
         // 
         this.layoutItemCreate.Control = this.btnCreate;
         this.layoutItemCreate.Location = new System.Drawing.Point(371, 17);
         this.layoutItemCreate.Name = "layoutItemCreate";
         this.layoutItemCreate.Size = new System.Drawing.Size(60, 40);
         this.layoutItemCreate.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemCreate.TextVisible = false;
         // 
         // btnLoad
         // 
         this.btnLoad.Location = new System.Drawing.Point(443, 29);
         this.btnLoad.Name = "btnLoad";
         this.btnLoad.Size = new System.Drawing.Size(46, 22);
         this.btnLoad.StyleController = this.layoutControl1;
         this.btnLoad.TabIndex = 9;
         this.btnLoad.Text = "btnLoad";
         // 
         // layoutItemLoad
         // 
         this.layoutItemLoad.Control = this.btnLoad;
         this.layoutItemLoad.Location = new System.Drawing.Point(431, 17);
         this.layoutItemLoad.Name = "layoutItemLoad";
         this.layoutItemLoad.Size = new System.Drawing.Size(50, 40);
         this.layoutItemLoad.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemLoad.TextVisible = false;
         // 
         // ExpressionProfileSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SimpleProteinView";
         this.ClientSize = new System.Drawing.Size(501, 123);
         this.Controls.Add(this.layoutControl1);
         this.Name = "ExpressionProfileSelectionView";
         this.Text = "SimpleProteinView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl1, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
         this.layoutControl1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.cbExpressionProfile.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExpressionProfileSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCreate)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemLoad)).EndInit();
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
   }
}