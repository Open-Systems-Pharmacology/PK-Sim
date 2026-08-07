namespace PKSim.UI.Views.Core
{
   partial class MultiplePanelView
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.layoutControl = new OSPSuite.UI.Controls.UxLayoutControl();
         this.panelNote = new OSPSuite.UI.Controls.UxHintPanel();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemNote = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNote)).BeginInit();
         this.SuspendLayout();
         //
         // layoutControl
         //
         this.layoutControl.Controls.Add(this.panelNote);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(676, 134, 250, 350);
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(471, 285);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         //
         // panelNote
         //
         this.panelNote.Location = new System.Drawing.Point(2, 2);
         this.panelNote.MaximumSize = new System.Drawing.Size(1000000, 40);
         this.panelNote.MaxLines = 3;
         this.panelNote.MinimumSize = new System.Drawing.Size(200, 40);
         this.panelNote.Name = "panelNote";
         this.panelNote.NoteText = "";
         this.panelNote.Size = new System.Drawing.Size(467, 40);
         this.panelNote.TabIndex = 1;
         //
         // layoutControlGroup
         //
         this.layoutControlGroup.CustomizationFormText = "layoutControlGroup1";
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemNote});
         this.layoutControlGroup.Location = new System.Drawing.Point(0, 0);
         this.layoutControlGroup.Name = "Root";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(471, 285);
         this.layoutControlGroup.Text = "Root";
         this.layoutControlGroup.TextVisible = false;
         //
         // layoutItemNote
         //
         this.layoutItemNote.Control = this.panelNote;
         this.layoutItemNote.CustomizationFormText = "layoutItemNote";
         this.layoutItemNote.Location = new System.Drawing.Point(0, 0);
         this.layoutItemNote.Name = "layoutItemNote";
         this.layoutItemNote.Size = new System.Drawing.Size(471, 44);
         this.layoutItemNote.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
         this.layoutItemNote.MaxSize = new System.Drawing.Size(0, 44);
         this.layoutItemNote.MinSize = new System.Drawing.Size(1, 44);
         this.layoutItemNote.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemNote.TextVisible = false;
         this.layoutItemNote.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
         //
         // MultiplePanelView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
         this.Controls.Add(this.layoutControl);
         this.Name = "MultiplePanelView";
         this.Size = new System.Drawing.Size(471, 285);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemNote)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private OSPSuite.UI.Controls.UxHintPanel panelNote;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemNote;
   }
}
