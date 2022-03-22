using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Individuals
{
   partial class MoleculesView
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
         treeView.NodeClick -= nodeClick;
         treeView.NodeDoubleClick -= nodeDoubleClicked;
         treeView.SelectedNodeChanged -= nodeSelected;
         treeView.Dispose();
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         this.treeView = new OSPSuite.UI.Controls.UxImageTreeView();
         this.lblLinkedExpressionProfile = new DevExpress.XtraEditors.LabelControl();
         this.panelExpression = new DevExpress.XtraEditors.PanelControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel1)).BeginInit();
         this.splitContainerControl.Panel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel2)).BeginInit();
         this.splitContainerControl.Panel2.SuspendLayout();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).BeginInit();
         this.SuspendLayout();
         // 
         // splitContainerControl
         // 
         this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
         this.splitContainerControl.Name = "splitContainerControl";
         // 
         // splitContainerControl.Panel1
         // 
         this.splitContainerControl.Panel1.Controls.Add(this.treeView);
         this.splitContainerControl.Panel1.Text = "PanelEnzyme";
         // 
         // splitContainerControl.Panel2
         // 
         this.splitContainerControl.Panel2.Controls.Add(this.panelExpression);
         this.splitContainerControl.Panel2.Controls.Add(this.lblLinkedExpressionProfile);
         this.splitContainerControl.Panel2.Text = "PanelContent";
         this.splitContainerControl.Size = new System.Drawing.Size(735, 486);
         this.splitContainerControl.SplitterPosition = 184;
         this.splitContainerControl.TabIndex = 3;
         this.splitContainerControl.Text = "splitContainerControl1";
         // 
         // treeView
         // 
         this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.treeView.IsLatched = false;
         this.treeView.Location = new System.Drawing.Point(0, 0);
         this.treeView.Name = "treeView";
         this.treeView.OptionsBehavior.Editable = false;
         this.treeView.OptionsView.ShowColumns = false;
         this.treeView.OptionsView.ShowHorzLines = false;
         this.treeView.OptionsView.ShowIndicator = false;
         this.treeView.OptionsView.ShowVertLines = false;
         this.treeView.Size = new System.Drawing.Size(184, 486);
         this.treeView.TabIndex = 8;
         this.treeView.ToolTipForNode = null;
         this.treeView.UseLazyLoading = false;
         // 
         // lblLinkedExpressionProfile
         // 
         this.lblLinkedExpressionProfile.Dock = System.Windows.Forms.DockStyle.Top;
         this.lblLinkedExpressionProfile.Location = new System.Drawing.Point(0, 0);
         this.lblLinkedExpressionProfile.Name = "lblLinkedExpressionProfile";
         this.lblLinkedExpressionProfile.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
         this.lblLinkedExpressionProfile.Size = new System.Drawing.Size(122, 19);
         this.lblLinkedExpressionProfile.TabIndex = 4;
         this.lblLinkedExpressionProfile.Text = "lblLinkedExpressionProfile";
         // 
         // panelExpression
         // 
         this.panelExpression.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panelExpression.Location = new System.Drawing.Point(0, 19);
         this.panelExpression.Name = "panelExpression";
         this.panelExpression.Size = new System.Drawing.Size(541, 467);
         this.panelExpression.TabIndex = 5;
         // 
         // MoleculesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainerControl);
         this.Name = "MoleculesView";
         this.Size = new System.Drawing.Size(735, 486);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel1)).EndInit();
         this.splitContainerControl.Panel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl.Panel2)).EndInit();
         this.splitContainerControl.Panel2.ResumeLayout(false);
         this.splitContainerControl.Panel2.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.panelExpression)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      protected DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private UxImageTreeView treeView;
      private DevExpress.XtraEditors.LabelControl lblLinkedExpressionProfile;
      private DevExpress.XtraEditors.PanelControl panelExpression;
   }
}


