namespace PKSim.UI.Views.Core
{
   partial class SelectFileView
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
         _screenBinder.Dispose();
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.tbDescription = new DevExpress.XtraEditors.MemoEdit();
         this.btnFileSelection = new DevExpress.XtraEditors.ButtonEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemFileSelection = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnFileSelection.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFileSelection)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(455, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(265, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(92, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(357, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(78, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(132, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(133, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(132, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.Controls.Add(this.tbDescription);
         this.layoutControl.Controls.Add(this.btnFileSelection);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(455, 269);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // tbDescription
         // 
         this.tbDescription.Location = new System.Drawing.Point(12, 68);
         this.tbDescription.Name = "tbDescription";
         this.tbDescription.Size = new System.Drawing.Size(431, 189);
         this.tbDescription.StyleController = this.layoutControl;
         this.tbDescription.TabIndex = 5;
         // 
         // btnFileSelection
         // 
         this.btnFileSelection.Location = new System.Drawing.Point(12, 28);
         this.btnFileSelection.Name = "btnFileSelection";
         this.btnFileSelection.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
         this.btnFileSelection.Size = new System.Drawing.Size(431, 20);
         this.btnFileSelection.StyleController = this.layoutControl;
         this.btnFileSelection.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemFileSelection,
            this.layoutItemDescription});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup1";
         this.layoutControlGroup.Size = new System.Drawing.Size(455, 269);
         this.layoutControlGroup.Text = "layoutControlGroup1";
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemFileSelection
         // 
         this.layoutItemFileSelection.Control = this.btnFileSelection;
         this.layoutItemFileSelection.CustomizationFormText = "layoutItemFileSelection";
         this.layoutItemFileSelection.Location = new System.Drawing.Point(0, 0);
         this.layoutItemFileSelection.Name = "layoutItemFileSelection";
         this.layoutItemFileSelection.Size = new System.Drawing.Size(435, 40);
         this.layoutItemFileSelection.Text = "layoutItemFileSelection";
         this.layoutItemFileSelection.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemFileSelection.TextSize = new System.Drawing.Size(111, 13);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.tbDescription;
         this.layoutItemDescription.CustomizationFormText = "layoutItemDescription";
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 40);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(435, 209);
         this.layoutItemDescription.Text = "layoutItemDescription";
         this.layoutItemDescription.TextLocation = DevExpress.Utils.Locations.Top;
         this.layoutItemDescription.TextSize = new System.Drawing.Size(111, 13);
         // 
         // SelectFileView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "SelectFileView";
         this.ClientSize = new System.Drawing.Size(455, 315);
         this.Controls.Add(this.layoutControl);
         this.Name = "SelectFileView";
         this.Text = "SelectFileView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutControl, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbDescription.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.btnFileSelection.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemFileSelection)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.ButtonEdit btnFileSelection;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemFileSelection;
      private DevExpress.XtraEditors.MemoEdit tbDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
   }
}