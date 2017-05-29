namespace PKSim.UI.Views.Populations
{
   partial class ExtractIndividualsFromPopulationView
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
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblNamingPatternDescription = new DevExpress.XtraEditors.LabelControl();
         this.lblIdividualIdsDescription = new DevExpress.XtraEditors.LabelControl();
         this.tbNamingPattern = new DevExpress.XtraEditors.TextEdit();
         this.tbIndividualIdsExpression = new DevExpress.XtraEditors.TextEdit();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemIndividualIds = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNamingPatternDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemNamingPattern = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemIndividualIdsDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.lblPopulationDescription = new DevExpress.XtraEditors.LabelControl();
         this.layoutItemPopulationDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.tbOutput = new DevExpress.XtraEditors.MemoEdit();
         this.layoutItemOutput = new DevExpress.XtraLayout.LayoutControlItem();
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
         ((System.ComponentModel.ISupportInitialize)(this.tbNamingPattern.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbIndividualIdsExpression.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualIds)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNamingPatternDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNamingPattern)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualIdsDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulationDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbOutput.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutput)).BeginInit();
         this.SuspendLayout();
         // 
         // btnCancel
         // 
         this.btnCancel.Location = new System.Drawing.Point(445, 12);
         this.btnCancel.Size = new System.Drawing.Size(91, 22);
         // 
         // btnOk
         // 
         this.btnOk.Location = new System.Drawing.Point(334, 12);
         this.btnOk.Size = new System.Drawing.Size(107, 22);
         // 
         // layoutControlBase
         // 
         this.layoutControlBase.Location = new System.Drawing.Point(0, 470);
         this.layoutControlBase.Size = new System.Drawing.Size(548, 46);
         this.layoutControlBase.Controls.SetChildIndex(this.btnCancel, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnOk, 0);
         this.layoutControlBase.Controls.SetChildIndex(this.btnExtra, 0);
         // 
         // btnExtra
         // 
         this.btnExtra.Size = new System.Drawing.Size(157, 22);
         // 
         // layoutControlGroupBase
         // 
         this.layoutControlGroupBase.Size = new System.Drawing.Size(444, 46);
         // 
         // layoutItemOK
         // 
         this.layoutItemOK.Location = new System.Drawing.Point(258, 0);
         this.layoutItemOK.Size = new System.Drawing.Size(89, 26);
         // 
         // layoutItemCancel
         // 
         this.layoutItemCancel.Location = new System.Drawing.Point(347, 0);
         this.layoutItemCancel.Size = new System.Drawing.Size(77, 26);
         // 
         // emptySpaceItemBase
         // 
         this.emptySpaceItemBase.Location = new System.Drawing.Point(129, 0);
         this.emptySpaceItemBase.Size = new System.Drawing.Size(129, 26);
         // 
         // layoutItemExtra
         // 
         this.layoutItemExtra.Size = new System.Drawing.Size(129, 26);
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.tbOutput);
         this.layoutControl.Controls.Add(this.lblPopulationDescription);
         this.layoutControl.Controls.Add(this.lblNamingPatternDescription);
         this.layoutControl.Controls.Add(this.lblIdividualIdsDescription);
         this.layoutControl.Controls.Add(this.tbNamingPattern);
         this.layoutControl.Controls.Add(this.tbIndividualIdsExpression);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(548, 470);
         this.layoutControl.TabIndex = 38;
         this.layoutControl.Text = "layoutControl1";
         // 
         // lblNamingPatternDescription
         // 
         this.lblNamingPatternDescription.Location = new System.Drawing.Point(191, 116);
         this.lblNamingPatternDescription.Name = "lblNamingPatternDescription";
         this.lblNamingPatternDescription.Size = new System.Drawing.Size(134, 13);
         this.lblNamingPatternDescription.StyleController = this.layoutControl;
         this.lblNamingPatternDescription.TabIndex = 7;
         this.lblNamingPatternDescription.Text = "lblNamingPatternDescription";
         // 
         // lblIdividualIdsDescription
         // 
         this.lblIdividualIdsDescription.Location = new System.Drawing.Point(191, 64);
         this.lblIdividualIdsDescription.Name = "lblIdividualIdsDescription";
         this.lblIdividualIdsDescription.Size = new System.Drawing.Size(118, 13);
         this.lblIdividualIdsDescription.StyleController = this.layoutControl;
         this.lblIdividualIdsDescription.TabIndex = 6;
         this.lblIdividualIdsDescription.Text = "lblIdividualIdsDescription";
         // 
         // tbNamingPattern
         // 
         this.tbNamingPattern.Location = new System.Drawing.Point(191, 92);
         this.tbNamingPattern.Name = "tbNamingPattern";
         this.tbNamingPattern.Size = new System.Drawing.Size(345, 20);
         this.tbNamingPattern.StyleController = this.layoutControl;
         this.tbNamingPattern.TabIndex = 5;
         // 
         // tbIndividualIdsExpression
         // 
         this.tbIndividualIdsExpression.Location = new System.Drawing.Point(191, 40);
         this.tbIndividualIdsExpression.Name = "tbIndividualIdsExpression";
         this.tbIndividualIdsExpression.Size = new System.Drawing.Size(345, 20);
         this.tbIndividualIdsExpression.StyleController = this.layoutControl;
         this.tbIndividualIdsExpression.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemIndividualIds,
            this.layoutItemNamingPatternDescription,
            this.layoutItemNamingPattern,
            this.layoutItemIndividualIdsDescription,
            this.emptySpaceItem2,
            this.emptySpaceItem1,
            this.layoutItemPopulationDescription,
            this.emptySpaceItem4,
            this.layoutItemOutput});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Size = new System.Drawing.Size(548, 470);
         this.layoutControlGroup.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 119);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(424, 10);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemIndividualIds
         // 
         this.layoutItemIndividualIds.Control = this.tbIndividualIdsExpression;
         this.layoutItemIndividualIds.CustomizationFormText = "layoutItemIndividualIds";
         this.layoutItemIndividualIds.Location = new System.Drawing.Point(0, 28);
         this.layoutItemIndividualIds.Name = "layoutItemIndividualIds";
         this.layoutItemIndividualIds.Size = new System.Drawing.Size(528, 24);
         this.layoutItemIndividualIds.TextSize = new System.Drawing.Size(176, 13);
         // 
         // layoutItemNamingPatternDescription
         // 
         this.layoutItemNamingPatternDescription.Control = this.lblNamingPatternDescription;
         this.layoutItemNamingPatternDescription.CustomizationFormText = "layoutItemNamingPatternDescription";
         this.layoutItemNamingPatternDescription.Location = new System.Drawing.Point(0, 104);
         this.layoutItemNamingPatternDescription.Name = "layoutItemNamingPatternDescription";
         this.layoutItemNamingPatternDescription.Size = new System.Drawing.Size(528, 17);
         this.layoutItemNamingPatternDescription.TextSize = new System.Drawing.Size(176, 13);
         // 
         // layoutItemNamingPattern
         // 
         this.layoutItemNamingPattern.Control = this.tbNamingPattern;
         this.layoutItemNamingPattern.Location = new System.Drawing.Point(0, 80);
         this.layoutItemNamingPattern.Name = "layoutItemNamingPattern";
         this.layoutItemNamingPattern.Size = new System.Drawing.Size(528, 24);
         this.layoutItemNamingPattern.TextSize = new System.Drawing.Size(176, 13);
         // 
         // layoutItemIndividualIdsDescription
         // 
         this.layoutItemIndividualIdsDescription.Control = this.lblIdividualIdsDescription;
         this.layoutItemIndividualIdsDescription.Location = new System.Drawing.Point(0, 52);
         this.layoutItemIndividualIdsDescription.Name = "layoutItemIndividualIdsDescription";
         this.layoutItemIndividualIdsDescription.Size = new System.Drawing.Size(528, 17);
         this.layoutItemIndividualIdsDescription.TextSize = new System.Drawing.Size(176, 13);
         // 
         // emptySpaceItem2
         // 
         this.emptySpaceItem2.AllowHotTrack = false;
         this.emptySpaceItem2.Location = new System.Drawing.Point(0, 68);
         this.emptySpaceItem2.Name = "emptySpaceItem2";
         this.emptySpaceItem2.Size = new System.Drawing.Size(424, 10);
         this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
         // 
         // lblPopulationDescription
         // 
         this.lblPopulationDescription.Location = new System.Drawing.Point(191, 12);
         this.lblPopulationDescription.Name = "lblPopulationDescription";
         this.lblPopulationDescription.Size = new System.Drawing.Size(113, 13);
         this.lblPopulationDescription.StyleController = this.layoutControl;
         this.lblPopulationDescription.TabIndex = 9;
         this.lblPopulationDescription.Text = "lblPopulationDescription";
         // 
         // layoutItemPopulationDescription
         // 
         this.layoutItemPopulationDescription.Control = this.lblPopulationDescription;
         this.layoutItemPopulationDescription.Location = new System.Drawing.Point(0, 0);
         this.layoutItemPopulationDescription.Name = "layoutItemPopulationDescription";
         this.layoutItemPopulationDescription.Size = new System.Drawing.Size(528, 17);
         this.layoutItemPopulationDescription.TextSize = new System.Drawing.Size(176, 13);
         // 
         // emptySpaceItem4
         // 
         this.emptySpaceItem4.AllowHotTrack = false;
         this.emptySpaceItem4.Location = new System.Drawing.Point(0, 17);
         this.emptySpaceItem4.Name = "emptySpaceItem4";
         this.emptySpaceItem4.Size = new System.Drawing.Size(424, 10);
         this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
         // 
         // tbOutput
         // 
         this.tbOutput.Location = new System.Drawing.Point(191, 144);
         this.tbOutput.Name = "tbOutput";
         this.tbOutput.Size = new System.Drawing.Size(345, 314);
         this.tbOutput.StyleController = this.layoutControl;
         this.tbOutput.TabIndex = 10;
         // 
         // layoutItemOutput
         // 
         this.layoutItemOutput.Control = this.tbOutput;
         this.layoutItemOutput.Location = new System.Drawing.Point(0, 132);
         this.layoutItemOutput.Name = "layoutItemOutput";
         this.layoutItemOutput.Size = new System.Drawing.Size(528, 318);
         this.layoutItemOutput.TextSize = new System.Drawing.Size(176, 13);
         // 
         // ExtractIndividualsFromPopulationView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Caption = "ExctractIndividualsFromPopulationView";
         this.ClientSize = new System.Drawing.Size(548, 516);
         this.Controls.Add(this.layoutControl);
         this.Name = "ExtractIndividualsFromPopulationView";
         this.Text = "ExctractIndividualsFromPopulationView";
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
         ((System.ComponentModel.ISupportInitialize)(this.tbNamingPattern.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbIndividualIdsExpression.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualIds)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNamingPatternDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNamingPattern)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemIndividualIdsDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemPopulationDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.tbOutput.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemOutput)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.LabelControl lblNamingPatternDescription;
      private DevExpress.XtraEditors.LabelControl lblIdividualIdsDescription;
      private DevExpress.XtraEditors.TextEdit tbNamingPattern;
      private DevExpress.XtraEditors.TextEdit tbIndividualIdsExpression;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
      private DevExpress.XtraEditors.LabelControl lblPopulationDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividualIds;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNamingPatternDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNamingPattern;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemIndividualIdsDescription;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemPopulationDescription;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
      private DevExpress.XtraEditors.MemoEdit tbOutput;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemOutput;
   }
}