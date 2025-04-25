namespace DVLA.WinDataMigration
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            BtnDownloadData = new Button();
            txtSqlQuery = new TextBox();
            label7 = new Label();
            label3 = new Label();
            txtSourcePassword = new TextBox();
            label2 = new Label();
            txtSourceUserID = new TextBox();
            label1 = new Label();
            txtSourceIP = new TextBox();
            groupBox2 = new GroupBox();
            label4 = new Label();
            txtDestinationPassword = new TextBox();
            label5 = new Label();
            txtDestinationUserID = new TextBox();
            label6 = new Label();
            txtDestinationIP = new TextBox();
            BtnPush = new Button();
            BtnCancel = new Button();
            lblMessage = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(BtnDownloadData);
            groupBox1.Controls.Add(txtSqlQuery);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtSourcePassword);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtSourceUserID);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtSourceIP);
            groupBox1.Location = new Point(27, 26);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(645, 147);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Source";
            // 
            // BtnDownloadData
            // 
            BtnDownloadData.Location = new Point(218, 118);
            BtnDownloadData.Name = "BtnDownloadData";
            BtnDownloadData.Size = new Size(244, 23);
            BtnDownloadData.TabIndex = 8;
            BtnDownloadData.Text = "Download Data";
            BtnDownloadData.UseVisualStyleBackColor = true;
            BtnDownloadData.Click += BtnDownloadData_Click;
            // 
            // txtSqlQuery
            // 
            txtSqlQuery.Location = new Point(100, 67);
            txtSqlQuery.Multiline = true;
            txtSqlQuery.Name = "txtSqlQuery";
            txtSqlQuery.Size = new Size(524, 42);
            txtSqlQuery.TabIndex = 7;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(31, 70);
            label7.Name = "label7";
            label7.Size = new Size(63, 15);
            label7.TabIndex = 6;
            label7.Text = "SQL Query";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(446, 33);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 5;
            label3.Text = "Password";
            label3.Click += label3_Click;
            // 
            // txtSourcePassword
            // 
            txtSourcePassword.Location = new Point(511, 30);
            txtSourcePassword.Name = "txtSourcePassword";
            txtSourcePassword.Size = new Size(113, 23);
            txtSourcePassword.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(281, 33);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 3;
            label2.Text = "UserID";
            // 
            // txtSourceUserID
            // 
            txtSourceUserID.Location = new Point(327, 30);
            txtSourceUserID.Name = "txtSourceUserID";
            txtSourceUserID.Size = new Size(102, 23);
            txtSourceUserID.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 33);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 1;
            label1.Text = "IP/Data Source";
            // 
            // txtSourceIP
            // 
            txtSourceIP.Location = new Point(101, 30);
            txtSourceIP.Name = "txtSourceIP";
            txtSourceIP.Size = new Size(160, 23);
            txtSourceIP.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(txtDestinationPassword);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(txtDestinationUserID);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(txtDestinationIP);
            groupBox2.Location = new Point(27, 199);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(645, 76);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Destination";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(446, 33);
            label4.Name = "label4";
            label4.Size = new Size(57, 15);
            label4.TabIndex = 5;
            label4.Text = "Password";
            // 
            // txtDestinationPassword
            // 
            txtDestinationPassword.Location = new Point(511, 33);
            txtDestinationPassword.Name = "txtDestinationPassword";
            txtDestinationPassword.Size = new Size(113, 23);
            txtDestinationPassword.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(281, 33);
            label5.Name = "label5";
            label5.Size = new Size(41, 15);
            label5.TabIndex = 3;
            label5.Text = "UserID";
            // 
            // txtDestinationUserID
            // 
            txtDestinationUserID.Location = new Point(327, 30);
            txtDestinationUserID.Name = "txtDestinationUserID";
            txtDestinationUserID.Size = new Size(102, 23);
            txtDestinationUserID.TabIndex = 2;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(10, 33);
            label6.Name = "label6";
            label6.Size = new Size(85, 15);
            label6.TabIndex = 1;
            label6.Text = "IP/Data Source";
            // 
            // txtDestinationIP
            // 
            txtDestinationIP.Location = new Point(101, 30);
            txtDestinationIP.Name = "txtDestinationIP";
            txtDestinationIP.Size = new Size(160, 23);
            txtDestinationIP.TabIndex = 0;
            // 
            // BtnPush
            // 
            BtnPush.Location = new Point(597, 291);
            BtnPush.Name = "BtnPush";
            BtnPush.Size = new Size(75, 23);
            BtnPush.TabIndex = 2;
            BtnPush.Text = "Push Data";
            BtnPush.UseVisualStyleBackColor = true;
            BtnPush.Click += BtnPush_Click;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(516, 291);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(75, 23);
            BtnCancel.TabIndex = 3;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(144, 180);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 15);
            lblMessage.TabIndex = 4;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(697, 340);
            Controls.Add(lblMessage);
            Controls.Add(BtnCancel);
            Controls.Add(BtnPush);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private Label label3;
        private TextBox txtSourcePassword;
        private Label label2;
        private TextBox txtSourceUserID;
        private Label label1;
        private TextBox txtSourceIP;
        private TextBox txtSqlQuery;
        private Label label7;
        private GroupBox groupBox2;
        private Label label4;
        private TextBox txtDestinationPassword;
        private Label label5;
        private TextBox txtDestinationUserID;
        private Label label6;
        private TextBox txtDestinationIP;
        private Button BtnPush;
        private Button BtnCancel;
        private Button BtnDownloadData;
        private Label lblMessage;
    }
}
