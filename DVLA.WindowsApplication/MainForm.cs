using AForge.Video;
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

namespace DVLA.WindowsApplication
{
    public partial class MainForm : Form
    {
        private readonly UserViewModel _user;
        public MainForm(UserViewModel user)
        {
            InitializeComponent();
            _user = user;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
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

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssessmentResultForm form = new AssessmentResultForm(_user);
            form.ShowDialog();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssessmentResultList list = new AssessmentResultList(_user);
            list.ShowDialog();
        }
    }
}
