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

namespace DVLA.WindowsApplication
{
    public partial class AssessmentResultList : Form
    {
        private UserViewModel _user;
        public AssessmentResultList(UserViewModel user)
        {
            InitializeComponent();
            _user = user;
            LoadAssessmentResults();
        }

        private async void LoadAssessmentResults()
        {
            // Set up the DataGridView
            dataGridViewAssessmentResultList.AutoGenerateColumns = false;

            // Define and add the columns you want
            DataGridViewTextBoxColumn IdColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id", // Property in the data source
                HeaderText = "Id",
                Name = "Id"
            };
            dataGridViewAssessmentResultList.Columns.Add(IdColumn);
            //IdColumn.Visible = false;

            DataGridViewTextBoxColumn FirstNameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FirstName", // Property in the data source
                HeaderText = "FirstName"
            };
            dataGridViewAssessmentResultList.Columns.Add(FirstNameColumn);

            DataGridViewTextBoxColumn SurnameColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Surname",
                HeaderText = "Surname"
            };
            dataGridViewAssessmentResultList.Columns.Add(SurnameColumn);

            DataGridViewTextBoxColumn OptometristFirmColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OptometristFirmName",
                HeaderText = "Optometrist Firm"
            };
            dataGridViewAssessmentResultList.Columns.Add(OptometristFirmColumn);

            DataGridViewTextBoxColumn ContactColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ContactNumber",
                HeaderText = "Contact"
            };
            dataGridViewAssessmentResultList.Columns.Add(ContactColumn);

            DataGridViewTextBoxColumn TransmittedColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IsTransmitted",
                HeaderText = "Transmitted"
            };
            dataGridViewAssessmentResultList.Columns.Add(TransmittedColumn);

            DataGridViewTextBoxColumn ResultConclusionColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ResultConclusion",
                HeaderText = "Result Conclusion"
            };
            dataGridViewAssessmentResultList.Columns.Add(ResultConclusionColumn);

            DataGridViewTextBoxColumn AccessTypeColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AccessType",
                HeaderText = "Access Type"
            };
            dataGridViewAssessmentResultList.Columns.Add(AccessTypeColumn);

            DataGridViewTextBoxColumn TINColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TIN",
                HeaderText = "TIN"
            };
            dataGridViewAssessmentResultList.Columns.Add(TINColumn);

            // Bind the data
            dataGridViewAssessmentResultList.DataSource = await VisualAssessmentService.GetAllAsync(_user.Id);
            
        }

        private void BtnCreateNewAssessmentResultLink_Click(object sender, EventArgs e)
        {
            AssessmentResultForm form = new AssessmentResultForm(_user);
            form.ShowDialog();
            this.Close();
        }

        private void dataGridViewAssessmentResultList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the ID from the row
                var id = Convert.ToInt32(dataGridViewAssessmentResultList.Rows[e.RowIndex].Cells["Id"].Value);

                // Open the dialog form and pass the ID
                using (var assessmentForm = new AssessmentResultForm(_user, id))
                {
                    assessmentForm.ShowDialog();
                }
            }
            this.Close();
        }
    }
}
