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
using WinApp.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinApp
{
    public partial class SystemSettingForm : Form
    {
        public SystemSettingForm()
        {
            InitializeComponent();
        }

        private async Task PerformSystemCleanupTask()
        {
            // Simulate work
            await AdminService.DeleteAllEntities();
            UpdateProgressBar(progressBarSystemCleanup, 100);
            UpdateLabelStatus(lblSystemCleanupTask, "Completed", Color.Green);
        }

        private async Task PerformUserTask()
        {
            // Simulate work
            await AdminService.SynchUsers();
            UpdateProgressBar(progressBarUserJob, 100);
            UpdateLabelStatus(lblUserStatus, "Completed", Color.Green);
        }
        private async Task PerformLocationTask()
        {
            // Simulate work
            var result = await AdminService.SyncLocations();
            UpdateProgressBar(progressBarLocationJob, 100);
            UpdateLabelStatus(lblLocationStatus, result.Success ? "Completed" : "Failed", result.Success ? Color.Green : Color.Red);
        }


        private async Task PerformClinicalDependenciesTask()
        {
            // Simulate work
            var result = await AdminService.SyncClinicals();
            UpdateProgressBar(progressBarClinicalDependenciesJob, result.Success ? 100 : 5);
            UpdateLabelStatus(lblClinicalDependenciesStatus, result.Success ? "Completed" : "Failed", result.Success ? Color.Green : Color.Red);
        }

        private async Task PerformOptometristFirmTask()
        {
            // Simulate work
            var result = await AdminService.SyncOptometristFirms();
            UpdateProgressBar(progressBarOptometristFirms, result.Success ? 100 : 5);
            UpdateLabelStatus(lblOptometristFirmStatus, result.Success ? "Completed" : "Failed", result.Success ? Color.Green : Color.Red);
        }

        private void UpdateProgressBar(System.Windows.Forms.ProgressBar progressBar, int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() =>
                {
                    progressBar.Value = value;
                }));
            }
            else
            {
                progressBar.Value = value;
            }
        }

        private void UpdateLabelStatus(Label label, string text, Color color)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() =>
                {
                    label.Text = text;
                    label.ForeColor = color;
                }));
            }
            else
            {
                label.Text = text;
                label.ForeColor = color;
            }
        }

        private async void BtnDownloadSystemSettings_Click(object sender, EventArgs e)
        {
            BtnDownloadSystemSettings.Text = "Please wait...";
            BtnDownloadSystemSettings.Enabled = false;
            
            var result = MessageBox.Show("Please note that you require internet access for this operation! Do you want to proceed?", "Notice", MessageBoxButtons.YesNo);

            await PerformSystemCleanupTask();

            await PerformUserTask();

            await PerformLocationTask();

            // Start all tasks
            //var task1 = Task.Run(() => PerformUserTask());
            //var task3 = Task.Run(() => PerformLocationTask());

            // Wait for all tasks to complete
            //await Task.WhenAll(task1, task3);

            await PerformClinicalDependenciesTask();
            await PerformOptometristFirmTask();

            // Perform action after all tasks are completed
            lblFinalStatus.Text = "All system settings downloaded successfully!";
            lblFinalStatus.ForeColor = Color.Green;

            BtnDownloadSystemSettings.Enabled = true;
            BtnDownloadSystemSettings.Text = "Download System Settings";
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            
            //List<Form> formsToClose = new List<Form>();

            //// Iterate through the open forms and add them to the list
            //foreach (Form form in Application.OpenForms)
            //{
            //    if (form != this) // Optionally exclude the main form
            //    {
            //        formsToClose.Add(form);
            //    }
            //}

            //// Now close all forms in the list
            //foreach (Form form in formsToClose)
            //{
            //    form.Close();
            //}
            
            //this.Close();
        }

    }
}
