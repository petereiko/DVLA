using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void systemSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create and show an MDI child form
            SystemSettingForm systemSettingForm = new SystemSettingForm();
            systemSettingForm.MdiParent = this; // Set the parent form

            systemSettingForm.FormClosing += ChildForm_FormClosing;
            systemSettingForm.Show();
        }

        private void ChildForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Custom logic when the user clicks the close button
            var result = MessageBox.Show(
                "Are you sure you want to close this form?",
                "Confirm Close",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Prevent the form from closing
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                List<Form> formsToClose = new List<Form>();

                // Iterate through the open forms and add them to the list
                foreach (Form form in Application.OpenForms)
                {
                    if (form != this) // Optionally exclude the main form
                    {
                        formsToClose.Add(form);
                    }
                }

                // Now close all forms in the list
                foreach (Form form in formsToClose)
                {
                    form.Close();
                }
            }
            catch (Exception)
            {

            }
            

        }

        private void pushAssessmentResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create and show an MDI child form
            PushAssessmentResultForm pushAssessmentResultForm = new PushAssessmentResultForm();
            pushAssessmentResultForm.MdiParent = this; // Set the parent form

            pushAssessmentResultForm.FormClosing += ChildForm_FormClosing;
            pushAssessmentResultForm.Show();
        }
    }
}
