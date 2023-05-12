namespace PKSim.UI.Views.DiseaseStates
{
   partial class DiseaseStateSelectionView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.lblDescription = new DevExpress.XtraEditors.LabelControl();
         this.cbDiseaseState = new OSPSuite.UI.Controls.UxComboBoxEdit();
         this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemDiseaseState = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
         this.uxDiseaseParameter = new PKSim.UI.Views.Parameters.UxParameterDTOEdit();
         this.layoutItemDiseaseParameter = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbDiseaseState.Properties)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseParameter)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.uxDiseaseParameter);
         this.layoutControl.Controls.Add(this.lblDescription);
         this.layoutControl.Controls.Add(this.cbDiseaseState);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.Root;
         this.layoutControl.Size = new System.Drawing.Size(688, 96);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "uxLayoutControl1";
         // 
         // lblDescription
         // 
         this.lblDescription.Location = new System.Drawing.Point(12, 61);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.StyleController = this.layoutControl;
         this.lblDescription.TabIndex = 5;
         this.lblDescription.Text = "lblDescription";
         // 
         // cbDiseaseState
         // 
         this.cbDiseaseState.Location = new System.Drawing.Point(163, 12);
         this.cbDiseaseState.Name = "cbDiseaseState";
         this.cbDiseaseState.Properties.AllowMouseWheel = false;
         this.cbDiseaseState.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
         this.cbDiseaseState.Size = new System.Drawing.Size(513, 20);
         this.cbDiseaseState.StyleController = this.layoutControl;
         this.cbDiseaseState.TabIndex = 4;
         // 
         // Root
         // 
         this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.Root.GroupBordersVisible = false;
         this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemDiseaseState,
            this.layoutItemDescription,
            this.layoutItemDiseaseParameter,
            this.emptySpaceItem1});
         this.Root.Name = "Root";
         this.Root.Size = new System.Drawing.Size(688, 96);
         this.Root.TextVisible = false;
         // 
         // layoutItemDiseaseState
         // 
         this.layoutItemDiseaseState.Control = this.cbDiseaseState;
         this.layoutItemDiseaseState.Location = new System.Drawing.Point(0, 0);
         this.layoutItemDiseaseState.Name = "layoutItemDiseaseState";
         this.layoutItemDiseaseState.Size = new System.Drawing.Size(668, 24);
         this.layoutItemDiseaseState.TextSize = new System.Drawing.Size(139, 13);
         // 
         // layoutItemDescription
         // 
         this.layoutItemDescription.Control = this.lblDescription;
         this.layoutItemDescription.Location = new System.Drawing.Point(0, 49);
         this.layoutItemDescription.Name = "layoutItemDescription";
         this.layoutItemDescription.Size = new System.Drawing.Size(668, 17);
         this.layoutItemDescription.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemDescription.TextVisible = false;
         // 
         // emptySpaceItem1
         // 
         this.emptySpaceItem1.AllowHotTrack = false;
         this.emptySpaceItem1.Location = new System.Drawing.Point(0, 66);
         this.emptySpaceItem1.Name = "emptySpaceItem1";
         this.emptySpaceItem1.Size = new System.Drawing.Size(668, 10);
         this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
         // 
         // uxDiseaseParameter
         // 
         this.uxDiseaseParameter.Caption = "";
         this.uxDiseaseParameter.Location = new System.Drawing.Point(163, 36);
         this.uxDiseaseParameter.Margin = new System.Windows.Forms.Padding(6);
         this.uxDiseaseParameter.MinimumSize = new System.Drawing.Size(0, 21);
         this.uxDiseaseParameter.Name = "uxDiseaseParameter";
         this.uxDiseaseParameter.Size = new System.Drawing.Size(513, 21);
         this.uxDiseaseParameter.TabIndex = 6;
         this.uxDiseaseParameter.ToolTip = "";
         // 
         // layoutItemDiseaseParameter
         // 
         this.layoutItemDiseaseParameter.Control = this.uxDiseaseParameter;
         this.layoutItemDiseaseParameter.Location = new System.Drawing.Point(0, 24);
         this.layoutItemDiseaseParameter.MaxSize = new System.Drawing.Size(0, 25);
         this.layoutItemDiseaseParameter.MinSize = new System.Drawing.Size(260, 25);
         this.layoutItemDiseaseParameter.Name = "layoutItemDiseaseParameter";
         this.layoutItemDiseaseParameter.Size = new System.Drawing.Size(668, 25);
         this.layoutItemDiseaseParameter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemDiseaseParameter.TextSize = new System.Drawing.Size(139, 13);
         // 
         // DiseaseStateSelectionView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "DiseaseStateSelectionView";
         this.Size = new System.Drawing.Size(688, 96);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         this.layoutControl.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.cbDiseaseState.Properties)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseState)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDescription)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemDiseaseParameter)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup Root;
      private OSPSuite.UI.Controls.UxComboBoxEdit cbDiseaseState;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseState;
      private DevExpress.XtraEditors.LabelControl lblDescription;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDescription;
      private Parameters.UxParameterDTOEdit uxDiseaseParameter;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemDiseaseParameter;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
   }
}
