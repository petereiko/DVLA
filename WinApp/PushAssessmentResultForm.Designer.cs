namespace WinApp
{
    partial class PushAssessmentResultForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PushAssessmentResultForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnTransmit = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblTotalApplicationCount = new System.Windows.Forms.Label();
            this.lblApplicationsForTransmissionCount = new System.Windows.Forms.Label();
            this.lblConclusion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblConclusion);
            this.groupBox1.Controls.Add(this.lblApplicationsForTransmissionCount);
            this.groupBox1.Controls.Add(this.lblTotalApplicationCount);
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.BtnTransmit);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.DarkBlue;
            this.groupBox1.Location = new System.Drawing.Point(32, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(700, 289);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Push Assessment Form";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(169, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Total Applications:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(32, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(301, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Applications ready for Transmission:";
            // 
            // BtnTransmit
            // 
            this.BtnTransmit.BackColor = System.Drawing.Color.DarkBlue;
            this.BtnTransmit.ForeColor = System.Drawing.Color.White;
            this.BtnTransmit.Location = new System.Drawing.Point(109, 156);
            this.BtnTransmit.Name = "BtnTransmit";
            this.BtnTransmit.Size = new System.Drawing.Size(218, 34);
            this.BtnTransmit.TabIndex = 2;
            this.BtnTransmit.Text = "Transmit Applications";
            this.BtnTransmit.UseVisualStyleBackColor = false;
            this.BtnTransmit.Click += new System.EventHandler(this.BtnTransmit_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(360, 161);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(315, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // lblTotalApplicationCount
            // 
            this.lblTotalApplicationCount.AutoSize = true;
            this.lblTotalApplicationCount.Location = new System.Drawing.Point(390, 65);
            this.lblTotalApplicationCount.Name = "lblTotalApplicationCount";
            this.lblTotalApplicationCount.Size = new System.Drawing.Size(55, 23);
            this.lblTotalApplicationCount.TabIndex = 4;
            this.lblTotalApplicationCount.Text = "label3";
            // 
            // lblApplicationsForTransmissionCount
            // 
            this.lblApplicationsForTransmissionCount.AutoSize = true;
            this.lblApplicationsForTransmissionCount.Location = new System.Drawing.Point(390, 107);
            this.lblApplicationsForTransmissionCount.Name = "lblApplicationsForTransmissionCount";
            this.lblApplicationsForTransmissionCount.Size = new System.Drawing.Size(55, 23);
            this.lblApplicationsForTransmissionCount.TabIndex = 5;
            this.lblApplicationsForTransmissionCount.Text = "label3";
            // 
            // lblConclusion
            // 
            this.lblConclusion.AutoSize = true;
            this.lblConclusion.Location = new System.Drawing.Point(390, 213);
            this.lblConclusion.Name = "lblConclusion";
            this.lblConclusion.Size = new System.Drawing.Size(55, 23);
            this.lblConclusion.TabIndex = 6;
            this.lblConclusion.Text = "label3";
            // 
            // PushAssessmentResultForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 340);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PushAssessmentResultForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Driver\'s Sight";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblConclusion;
        private System.Windows.Forms.Label lblApplicationsForTransmissionCount;
        private System.Windows.Forms.Label lblTotalApplicationCount;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button BtnTransmit;
    }
}