namespace PKSim.UI.Views.Protocols
{
   partial class CreateProtocolView
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
         _propertiesBinder.Dispose();         
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutApplicationSchema = new OSPSuite.UI.Controls.UxLayoutControl();
         this.splitContainer = new DevExpress.XtraEditors.SplitContainerControl();
         this.tbName = new DevExpress.XtraEditors.TextEdit();
         this.radioGroupProtocolMode = new DevExpress.XtraEditors.RadioGroup();
         this.layoutProtocol = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutProtocolMode = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemName = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemSplit = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
         this.layoutControlBase.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutApplicationSchema)).BeginInit();
         this.layoutApplicationSchema.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
         this.splitContainer.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupProtocolMode.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutProtocol)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutProtocolMode)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplit)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(563, 12);
         this.btnCancel.Size = new System.Drawing.Size(116, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(421, 12);
         this.btnOk.Size = new System.Drawing.Size(138, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 631);
         this.layoutControlBase.Size = new System.Drawing.Size(691, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(200, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(691, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(409, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(142, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(551, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(120, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(204, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(205, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(204, 26);
         // 
         // layoutApplicationSchema
         // 
         this.layoutApplicationSchema.AllowCustomization = false;
         this.layoutApplicationSchema.Controls.Add(this.splitContainer);
         this.layoutApplicationSchema.Controls.Add(this.tbName);
         this.layoutApplicationSchema.Controls.Add(this.radioGroupProtocolMode);
         this.layoutApplicationSchema.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutApplicationSchema.Location = new System.Drawing.Point(0, 0);
         this.layoutApplicationSchema.Name = "layoutApplicationSchema";
         this.layoutApplicationSchema.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(581, 362, 250, 350);
         this.layoutApplicationSchema.Root = this.layoutProtocol;
         this.layoutApplicationSchema.Size = new System.Drawing.Size(691, 631);
         this.layoutApplicationSchema.TabIndex = 33;
         this.layoutApplicationSchema.Text = "layoutControl1";
         // 
         // splitContainer
         // 
         this.splitContainer.Horizontal = false;
         this.splitContainer.Location = new System.Drawing.Point(10, 63);
         this.splitContainer.Name = "splitContainer";
         this.splitContainer.Panel1.AutoScroll = true;
         this.splitContainer.Panel1.Text = "Panel1";
         this.splitContainer.Panel2.Text = "Panel2";
         this.splitContainer.Size = new System.Drawing.Size(671, 558);
         this.splitContainer.SplitterPosition = 255;
         this.splitContainer.TabIndex = 7;
         this.splitContainer.Text = "splitContainer";
         // 
         // tbName
         // 
         this.tbName.Location = new System.Drawing.Point(94, 41);
         this.tbName.Name = "tbName";
         this.tbName.Size = new System.Drawing.Size(585, 20);
         this.tbName.StyleController = this.layoutApplicationSchema;
         this.tbName.TabIndex = 6;
         // 
         // radioGroupProtocolMode
         // 
         this.radioGroupProtocolMode.Location = new System.Drawing.Point(12, 12);
         this.radioGroupProtocolMode.Name = "radioGroupProtocolMode";
         this.radioGroupProtocolMode.Size = new System.Drawing.Size(667, 25);
         this.radioGroupProtocolMode.StyleController = this.layoutApplicationSchema;
         this.radioGroupProtocolMode.TabIndex = 5;
         // 
         // layoutProtocol
         // 
         this.layoutProtocol.CustomizationFormText = "layoutGroupProperties";
         this.layoutProtocol.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutProtocol.GroupBordersVisible = false;
         this.layoutProtocol.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutProtocolMode,
            this.layoutItemName,
            this.layoutItemSplit});
         this.layoutProtocol.Name = "Root";
         this.layoutProtocol.Size = new System.Drawing.Size(691, 631);
         this.layoutProtocol.TextVisible = false;
         // 
         // layoutProtocolMode
         // 
         this.layoutProtocolMode.Control = this.radioGroupProtocolMode;
         this.layoutProtocolMode.CustomizationFormText = "layoutProtocolMode";
         this.layoutProtocolMode.Location = new System.Drawing.Point(0, 0);
         this.layoutProtocolMode.MaxSize = new System.Drawing.Size(0, 29);
         this.layoutProtocolMode.MinSize = new System.Drawing.Size(54, 29);
         this.layoutProtocolMode.Name = "layoutProtocolMode";
         this.layoutProtocolMode.Size = new System.Drawing.Size(671, 29);
         this.layoutProtocolMode.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutProtocolMode.TextSize = new System.Drawing.Size(0, 0);
         this.layoutProtocolMode.TextVisible = false;
         // 
         // layoutItemName
         // 
         this.layoutItemName.Control = this.tbName;
         this.layoutItemName.CustomizationFormText = "layoutItemName";
         this.layoutItemName.Location = new System.Drawing.Point(0, 29);
         this.layoutItemName.Name = "layoutItemName";
         this.layoutItemName.Size = new System.Drawing.Size(671, 24);
         this.layoutItemName.TextSize = new System.Drawing.Size(79, 13);
         // 
         // layoutItemSplit
         // 
         this.layoutItemSplit.Control = this.splitContainer;
         this.layoutItemSplit.CustomizationFormText = "layoutItemSplit";
         this.layoutItemSplit.Location = new System.Drawing.Point(0, 53);
         this.layoutItemSplit.Name = "layoutItemSplit";
         this.layoutItemSplit.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemSplit.Size = new System.Drawing.Size(671, 558);
         this.layoutItemSplit.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemSplit.TextVisible = false;
         // 
         // CreateProtocolView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "CreateProtocolView";
         this.ClientSize = new System.Drawing.Size(691, 677);
         this.Controls.Add(this.layoutApplicationSchema);
         this.Name = "CreateProtocolView";
         this.Text = "CreateProtocolView";
         this.Controls.SetChildIndex(this.layoutControlBase, 0);
         this.Controls.SetChildIndex(this.layoutApplicationSchema, 0);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
         this.layoutControlBase.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOK)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemBase)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemExtra)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutApplicationSchema)).EndInit();
         this.layoutApplicationSchema.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
         this.splitContainer.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.radioGroupProtocolMode.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutProtocol)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutProtocolMode)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemName)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemSplit)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutApplicationSchema;
      private DevExpress.XtraLayout.LayoutControlGroup layoutProtocol;
      private DevExpress.XtraEditors.RadioGroup radioGroupProtocolMode;
      private DevExpress.XtraLayout.LayoutControlItem layoutProtocolMode;
      private DevExpress.XtraEditors.TextEdit tbName;
      private DevExpress.XtraEditors.SplitContainerControl splitContainer;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemName;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemSplit;
   }
}