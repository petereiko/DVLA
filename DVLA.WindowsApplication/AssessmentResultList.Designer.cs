namespace DVLA.WindowsApplication
{
    partial class AssessmentResultList
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssessmentResultList));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridViewAssessmentResultList = new System.Windows.Forms.DataGridView();
            this.BtnCreateNewAssessmentResultLink = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssessmentResultList)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridViewAssessmentResultList);
            this.groupBox1.Controls.Add(this.BtnCreateNewAssessmentResultLink);
            this.groupBox1.ForeColor = System.Drawing.Color.DarkBlue;
            this.groupBox1.Location = new System.Drawing.Point(28, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1050, 478);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assessment Result List";
            // 
            // dataGridViewAssessmentResultList
            // 
            this.dataGridViewAssessmentResultList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAssessmentResultList.Location = new System.Drawing.Point(25, 103);
            this.dataGridViewAssessmentResultList.Name = "dataGridViewAssessmentResultList";
            this.dataGridViewAssessmentResultList.RowHeadersWidth = 51;
            this.dataGridViewAssessmentResultList.RowTemplate.Height = 24;
            this.dataGridViewAssessmentResultList.Size = new System.Drawing.Size(1000, 338);
            this.dataGridViewAssessmentResultList.TabIndex = 1;
            this.dataGridViewAssessmentResultList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewAssessmentResultList_CellClick);
            // 
            // BtnCreateNewAssessmentResultLink
            // 
            this.BtnCreateNewAssessmentResultLink.Location = new System.Drawing.Point(25, 41);
            this.BtnCreateNewAssessmentResultLink.Name = "BtnCreateNewAssessmentResultLink";
            this.BtnCreateNewAssessmentResultLink.Size = new System.Drawing.Size(189, 27);
            this.BtnCreateNewAssessmentResultLink.TabIndex = 0;
            this.BtnCreateNewAssessmentResultLink.Text = "Create New Assessment Result";
            this.BtnCreateNewAssessmentResultLink.UseVisualStyleBackColor = true;
            this.BtnCreateNewAssessmentResultLink.Click += new System.EventHandler(this.BtnCreateNewAssessmentResultLink_Click);
            // 
            // AssessmentResultList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1112, 532);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AssessmentResultList";
            this.Text = "Assessment Results";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAssessmentResultList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dataGridViewAssessmentResultList;
        private System.Windows.Forms.Button BtnCreateNewAssessmentResultLink;
    }
}