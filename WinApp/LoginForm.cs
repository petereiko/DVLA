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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            txtEmail.Text = "peterayebhere@gmail.com";
            txtPassword.Text = "password";
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (txtEmail.Text.Trim().Length == 0)
            {
                MessageBox.Show("Email is required", "Validation Error", MessageBoxButtons.OK);
                txtEmail.Focus();
                return;
            }
            if (txtPassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("Email is required", "Validation Error", MessageBoxButtons.OK);
                txtPassword.Focus();
                return;
            }
            BtnLogin.Enabled = false;
            BtnLogin.Text = "Please wait...";
            MessageResponse result = await AccountService.Login(new Models.LoginModel
            {
                Email = txtEmail.Text,
                Password = txtPassword.Text,
            });
            if (!result.Success)
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK);
                return;
            }
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
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
    }
}
