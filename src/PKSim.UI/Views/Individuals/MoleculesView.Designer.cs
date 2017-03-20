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
         this.treeView = new UxImageTreeView();
         this.groupExpressions = new DevExpress.XtraEditors.GroupControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         this.splitContainerControl.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.groupExpressions)).BeginInit();
         this.SuspendLayout();
         // 
         // splitContainerControl
         // 
         this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
         this.splitContainerControl.Name = "splitContainerControl";
         this.splitContainerControl.Panel1.Controls.Add(this.treeView);
         this.splitContainerControl.Panel1.Text = "PanelEnzyme";
         this.splitContainerControl.Panel2.Controls.Add(this.groupExpressions);
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
         // 
         // groupExpressions
         // 
         this.groupExpressions.Dock = System.Windows.Forms.DockStyle.Fill;
         this.groupExpressions.Location = new System.Drawing.Point(0, 0);
         this.groupExpressions.Name = "groupExpressions";
         this.groupExpressions.Size = new System.Drawing.Size(546, 486);
         this.groupExpressions.TabIndex = 0;
         this.groupExpressions.Text = "groupExpressions";
         // 
         // barDockControlTop
         // 
         // IndividualProteinsView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainerControl);
         this.Name = "MoleculesView";
         this.Size = new System.Drawing.Size(735, 486);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.treeView)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.groupExpressions)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      protected DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      private UxImageTreeView treeView;
      private DevExpress.XtraEditors.GroupControl groupExpressions;


   }
}


