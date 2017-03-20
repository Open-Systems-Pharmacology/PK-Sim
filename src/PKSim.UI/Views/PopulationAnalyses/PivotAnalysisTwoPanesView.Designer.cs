namespace PKSim.UI.Views.PopulationAnalyses
{
       partial class PivotAnalysisTwoPanesView
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
         this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
         this.splitContainerControl.SuspendLayout();
         this.SuspendLayout();
         // 
         // splitContainerControl
         // 
         this.splitContainerControl.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
         this.splitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerControl.Location = new System.Drawing.Point(0, 0);
         this.splitContainerControl.Name = "splitContainerControl";
         this.splitContainerControl.Panel1.Text = "Panel1";
         this.splitContainerControl.Panel2.Text = "Panel2";
         this.splitContainerControl.Size = new System.Drawing.Size(452, 374);
         this.splitContainerControl.SplitterPosition = 281;
         this.splitContainerControl.TabIndex = 0;
         this.splitContainerControl.Text = "splitContainerControl1";
         // 
         // PivotAnalysisTwoPanesView
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.splitContainerControl);
         this.Name = "PivotAnalysisTwoPanesView";
         this.Size = new System.Drawing.Size(452, 374);
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
         this.splitContainerControl.ResumeLayout(false);
         this.ResumeLayout(false);

         }

         #endregion

         private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
      }
   }

