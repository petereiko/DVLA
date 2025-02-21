using DVLA.WindowsApplication.Business;
using DVLA.WindowsApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DVLA.WindowsApplication
{
    public partial class LoginForm : Form
    {
        private BackgroundWorker _backgroundWorker;

        public LoginForm()
        {
            InitializeComponent();
            txtEmail.Text = "peterayebhere@gmail.com";
            txtPassword.Text = "Securityr&d1";
            _backgroundWorker = new BackgroundWorker();

            // Set properties for BackgroundWorker
            _backgroundWorker.WorkerReportsProgress = true;  // Enable progress reporting
            _backgroundWorker.WorkerSupportsCancellation = true;  // Allow cancellation

            // Wire up events
            _backgroundWorker.DoWork += DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            progressBar1.Visible = false;
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Example of a long-running task (simulated with sleep)
                if (_backgroundWorker.CancellationPending) // Check for cancellation
                {
                    e.Cancel = true;
                    return;
                }

                MessageResponse<UserViewModel> result = AccountService.Authenticate(new LoginDto
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtPassword.Text.Trim()
                }).GetAwaiter().GetResult();

                if (!result.Success)
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK);
                    BtnLogin.Enabled = true;
                    BtnLogin.Text = "Login";
                    return;
                }
                _backgroundWorker.ReportProgress(100);

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        // You can safely update UI elements here
                        MessageBox.Show(result.Message);
                        MainForm mainForm = new MainForm(result.Result);
                        mainForm.Show();  // Show the main form

                        this.Hide();      // Hide the login form
                    }));
                }
                else
                {
                    // If already on the UI thread, just proceed
                    MessageBox.Show("Background task completed.");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                //BtnLogin.Enabled = true;
                //BtnLogin.Text = "Login";
            }
            

        }

        // This event runs when the background task completes
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was canceled.");
            }
            //}
            //else
            //{
            //    MessageBox.Show("Operation completed successfully.");
            //}
        }


        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;  // Update the progress bar
        }


        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (!_backgroundWorker.IsBusy)
            {
                //BtnLogin.Text = "Please wait...";
                //BtnLogin.Enabled = false;
                progressBar1.Visible = true;
                progressBar1.Value = 0;  // Reset progress bar
                _backgroundWorker.RunWorkerAsync();  // Start the background task
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();  // Request cancellation
            }
            this.Close();
        }

        private void linkLabelAdminLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AdminLoginForm adminLoginForm = new AdminLoginForm();
            adminLoginForm.ShowDialog();
            this.Close();
        }
    }
}
