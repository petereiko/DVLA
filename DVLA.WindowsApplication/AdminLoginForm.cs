using DVLA.WindowsApplication.Business;
using DVLA.WindowsApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DVLA.WindowsApplication
{
    public partial class AdminLoginForm : Form
    {
        private BackgroundWorker _backgroundWorker;
        public AdminLoginForm()
        {
            InitializeComponent();

            _backgroundWorker = new BackgroundWorker();

            // Set properties for BackgroundWorker
            _backgroundWorker.WorkerReportsProgress = true;  // Enable progress reporting
            _backgroundWorker.WorkerSupportsCancellation = true;  // Allow cancellation

            

            // Wire up events
            _backgroundWorker.DoWork += DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            progressBarLogin.Visible = false;

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

                MessageResponse result = AdminService.Login(txtEmail.Text.Trim(), txtPassword.Text.Trim()).GetAwaiter().GetResult();

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
                        MainAdminForm mainForm = new MainAdminForm();
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
            progressBarLogin.Value = e.ProgressPercentage;  // Update the progress bar
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (!_backgroundWorker.IsBusy)
            {
                //BtnLogin.Text = "Please wait...";
                //BtnLogin.Enabled = false;
                progressBarLogin.Visible = true;
                progressBarLogin.Value = 0;  // Reset progress bar
                _backgroundWorker.RunWorkerAsync();  // Start the background task
            }
        }
    }
}
