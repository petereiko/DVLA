using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Models;
using WinApp.Services;

namespace WinApp
{
    public partial class PushAssessmentResultForm : Form
    {
        public PushAssessmentResultForm()
        {
            InitializeComponent();
            lblTotalApplicationCount.Text = "";
            lblApplicationsForTransmissionCount.Text = "";
            LoadFormDepencies();
        }

        private async void LoadFormDepencies()
        {
            lblConclusion.Text = "Please wait...";
            Tuple<int, int> result = await VisualAssessmentService.LoadDependencies();
            lblTotalApplicationCount.Text = result.Item1.ToString();
            lblApplicationsForTransmissionCount.Text= result.Item2.ToString();
            lblConclusion.Text = "Application ready for transmittion";
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

        public async Task<MessageResponse> DoWork()
        {
            var result = await  VisualAssessmentService.TransmitBulk();
            UpdateProgressBar(progressBar1, 100);
            return result;
        }

        private async void BtnTransmit_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Please note that you require internet access for this operation! Do you want to proceed?", "Notice", MessageBoxButtons.YesNo);

            if(result == DialogResult.Yes)
            {
                var retVal = await DoWork();
                if (retVal.Success)
                {
                    lblConclusion.ForeColor = Color.Green;
                    lblConclusion.Text = retVal.Message;

                    Tuple<int, int> res = await VisualAssessmentService.LoadDependencies();
                    lblTotalApplicationCount.Text = res.Item1.ToString();
                    lblApplicationsForTransmissionCount.Text = res.Item2.ToString();
                }
                else
                {
                    lblConclusion.ForeColor = Color.Red;
                    lblConclusion.Text = retVal.Message;
                }
            }

            
        }
    }
}
