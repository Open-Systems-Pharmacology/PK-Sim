namespace PKSim.UI.Views.Parameters
{
   partial class FavoriteParametersView
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
         this.buttonMoveDown = new DevExpress.XtraEditors.SimpleButton();
         this.buttonMoveUp = new DevExpress.XtraEditors.SimpleButton();
         this.panelParameters = new DevExpress.XtraEditors.PanelControl();
         this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
         this.layoutItemParameters = new DevExpress.XtraLayout.LayoutControlItem();
         this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
         this.layoutItemButtonMoveUp = new DevExpress.XtraLayout.LayoutControlItem();
         this.layoutItemButtonMoveDown = new DevExpress.XtraLayout.LayoutControlItem();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
         this.layoutControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonMoveUp)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonMoveDown)).BeginInit();
         this.SuspendLayout();
         // 
         // layoutControl
         // 
         this.layoutControl.AllowCustomization = false;
         this.layoutControl.Controls.Add(this.buttonMoveDown);
         this.layoutControl.Controls.Add(this.buttonMoveUp);
         this.layoutControl.Controls.Add(this.panelParameters);
         this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.layoutControl.Location = new System.Drawing.Point(0, 0);
         this.layoutControl.Name = "layoutControl";
         this.layoutControl.Root = this.layoutControlGroup;
         this.layoutControl.Size = new System.Drawing.Size(518, 360);
         this.layoutControl.TabIndex = 0;
         this.layoutControl.Text = "layoutControl1";
         // 
         // buttonMoveDown
         // 
         this.buttonMoveDown.Location = new System.Drawing.Point(422, 28);
         this.buttonMoveDown.Name = "buttonMoveDown";
         this.buttonMoveDown.Size = new System.Drawing.Size(94, 22);
         this.buttonMoveDown.StyleController = this.layoutControl;
         this.buttonMoveDown.TabIndex = 6;
         this.buttonMoveDown.Text = "buttonMoveDown";
         // 
         // buttonMoveUp
         // 
         this.buttonMoveUp.Location = new System.Drawing.Point(422, 2);
         this.buttonMoveUp.Name = "buttonMoveUp";
         this.buttonMoveUp.Size = new System.Drawing.Size(94, 22);
         this.buttonMoveUp.StyleController = this.layoutControl;
         this.buttonMoveUp.TabIndex = 5;
         this.buttonMoveUp.Text = "buttonMoveUp";
         // 
         // panelParameters
         // 
         this.panelParameters.Location = new System.Drawing.Point(0, 0);
         this.panelParameters.Name = "panelParameters";
         this.panelParameters.Size = new System.Drawing.Size(420, 360);
         this.panelParameters.TabIndex = 4;
         // 
         // layoutControlGroup
         // 
         this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
         this.layoutControlGroup.GroupBordersVisible = false;
         this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutItemParameters,
            this.emptySpaceItem,
            this.layoutItemButtonMoveUp,
            this.layoutItemButtonMoveDown});
         this.layoutControlGroup.Name = "layoutControlGroup";
         this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutControlGroup.Size = new System.Drawing.Size(518, 360);
         this.layoutControlGroup.TextVisible = false;
         // 
         // layoutItemParameters
         // 
         this.layoutItemParameters.Control = this.panelParameters;
         this.layoutItemParameters.Location = new System.Drawing.Point(0, 0);
         this.layoutItemParameters.Name = "layoutItemParameters";
         this.layoutItemParameters.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
         this.layoutItemParameters.Size = new System.Drawing.Size(420, 360);
         this.layoutItemParameters.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemParameters.TextVisible = false;
         // 
         // emptySpaceItem
         // 
         this.emptySpaceItem.AllowHotTrack = false;
         this.emptySpaceItem.Location = new System.Drawing.Point(420, 52);
         this.emptySpaceItem.Name = "emptySpaceItem";
         this.emptySpaceItem.Size = new System.Drawing.Size(98, 308);
         this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
         // 
         // layoutItemButtonMoveUp
         // 
         this.layoutItemButtonMoveUp.Control = this.buttonMoveUp;
         this.layoutItemButtonMoveUp.Location = new System.Drawing.Point(420, 0);
         this.layoutItemButtonMoveUp.Name = "layoutItemButtonMoveUp";
         this.layoutItemButtonMoveUp.Size = new System.Drawing.Size(98, 26);
         this.layoutItemButtonMoveUp.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonMoveUp.TextVisible = false;
         // 
         // layoutItemButtonMoveDown
         // 
         this.layoutItemButtonMoveDown.Control = this.buttonMoveDown;
         this.layoutItemButtonMoveDown.Location = new System.Drawing.Point(420, 26);
         this.layoutItemButtonMoveDown.Name = "layoutItemButtonMoveDown";
         this.layoutItemButtonMoveDown.Size = new System.Drawing.Size(98, 26);
         this.layoutItemButtonMoveDown.TextSize = new System.Drawing.Size(0, 0);
         this.layoutItemButtonMoveDown.TextVisible = false;
         // 
         // FavoriteParametersView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.layoutControl);
         this.Name = "FavoriteParametersView";
         this.Size = new System.Drawing.Size(518, 360);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
         this.layoutControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.panelParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemParameters)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonMoveUp)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonMoveDown)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private OSPSuite.UI.Controls.UxLayoutControl layoutControl;
      private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup;
      private DevExpress.XtraEditors.SimpleButton buttonMoveDown;
      private DevExpress.XtraEditors.SimpleButton buttonMoveUp;
      private DevExpress.XtraEditors.PanelControl panelParameters;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemParameters;
      private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonMoveUp;
      private DevExpress.XtraLayout.LayoutControlItem layoutItemButtonMoveDown;
   }
}
