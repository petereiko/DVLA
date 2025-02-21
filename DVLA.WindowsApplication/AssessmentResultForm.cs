using AForge.Video;
using AForge.Video.DirectShow;
using DVLA.WindowsApplication.Business;
using DVLA.WindowsApplication.Data;
using DVLA.WindowsApplication.Enums;
using DVLA.WindowsApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLA.WindowsApplication
{
    public partial class AssessmentResultForm : Form
    {
        private FilterInfoCollection _videoDevices;  // Stores video devices (cameras)
        private VideoCaptureDevice _videoSource;     // Represents the video capture device
        private Bitmap _currentFrame;                // Current frame from the camera

        private string _imagePath;

        private CreateVisualAssessmentResultDependencyModel _visualAssessmentResultDependency;

        private UserViewModel _user;
        private long _id;

        public AssessmentResultForm(UserViewModel user)
        {
            InitializeComponent();
            BtnTransmit.Enabled = false;
            pictureBoxVideoFeed.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxShapShot.SizeMode = PictureBoxSizeMode.Zoom;
            if (_visualAssessmentResultDependency == null)
                LoadData(_id = 0);
            _user = user;
        }

        private VisualAssessmentResult _visualAssessmentResult;
        public AssessmentResultForm(UserViewModel user, long id)
        {
            InitializeComponent();
            BtnTransmit.Enabled = false;
            _id = id;
            pictureBoxVideoFeed.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxShapShot.SizeMode = PictureBoxSizeMode.Zoom;
            if (_visualAssessmentResultDependency == null)
                LoadData(id);
            _user = user;
        }

        private async void LoadVisualAssessmentData(long id)
        {
            _visualAssessmentResult = await VisualAssessmentService.GetAsync(id);
            try
            {
                _imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", _visualAssessmentResult.PassportImageUrl);
                pictureBoxShapShot.Image = new Bitmap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", _visualAssessmentResult.PassportImageUrl));
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex);
            }
            comboBoxServiceType.SelectedValue = (int)_visualAssessmentResult.ResultServiceType.Value;
            if (comboBoxServiceType.SelectedIndex == 1)
            {
                comboBoxLearnerDriversLicense.SelectedValue = (int)_visualAssessmentResult.LearnerDriversLicence.Value;
            }
            txtSurname.Text= _visualAssessmentResult.Surname;
            txtFirstName.Text= _visualAssessmentResult.FirstName;
            txtOthername.Text = _visualAssessmentResult.OtherName;
            dateTimePickerDOB.Value = _visualAssessmentResult.DOB.GetValueOrDefault();
            txtAddress.Text = _visualAssessmentResult.PostalAddress;
            txtTin.Text = _visualAssessmentResult.TaxIdentificationNumber;
            txtEmail.Text = _visualAssessmentResult.Email;
            txtContact.Text = _visualAssessmentResult.ContactNumber;
            comboBoxUnaidedOD.Text = _visualAssessmentResult.Unaided_OD;
            comboBoxUnaidedOS.Text = _visualAssessmentResult.Unaided_OS;
            comboBoxBCVOD.Text = _visualAssessmentResult.BCV_OD;
            comboBoxBCVOS.Text = _visualAssessmentResult.BCV_OS;
            comboBoxBCVOU.Text = _visualAssessmentResult.BCV_OU;
            comboBoxHXBCVOD.Text=_visualAssessmentResult.HX_BCV_OD;
            comboBoxHXBCVOS.Text = _visualAssessmentResult.HX_BCV_OS;
            comboBoxSingleImageBCVOU.Text = _visualAssessmentResult.SingleImage_BCV_OU;
            comboBoxGlareTestBCVOU.Text = _visualAssessmentResult.GlareTest_BCV_OU;
            comboBoxColorVisionBCVOU.Text = _visualAssessmentResult.ColourVision_BCV_OU;
            txtRemark.Text = _visualAssessmentResult.PathologicalRemarks;
            comboBoxResultConclusion.Text = _visualAssessmentResult.ResultConclusion;
            comboBoxPassOrFail.Text = _visualAssessmentResult.PassOrFail.ToString();
            BtnSave.Enabled = !_visualAssessmentResult.IsSubmitted;
            BtnSubmit.Enabled = BtnSave.Enabled;
            BtnTransmit.Enabled = !_visualAssessmentResult.IsTransmitted;
        }

        


        private async void LoadData(long id)
        {
            _visualAssessmentResultDependency = await AccountService.GetVisualAssessmentDependencies();

            if (id > 0)
                LoadVisualAssessmentData(id);

            //Load All Combox Boxes
            //Load Service Types:
            
            LoadComboBox<IdNameModel<int>>(comboBoxServiceType, _visualAssessmentResultDependency.ResultServiceTypes, "--Select Service Type--");
            LoadComboBox<IdNameModel<int>>(comboBoxLearnerDriversLicense, _visualAssessmentResultDependency.LearnerDriversLicenceType, "--Select Type--");

            var comboBoxUnaidedODList = _visualAssessmentResultDependency.VisualAcuity;
            LoadComboBox<IdNameModel<int>>(comboBoxUnaidedOD, comboBoxUnaidedODList, "--Select Type--");

            var comboBoxUnaidedOSList = _visualAssessmentResultDependency.VisualAcuity;
            LoadComboBox<IdNameModel<int>>(comboBoxUnaidedOS, comboBoxUnaidedOSList, "--Select Type--");//comboBoxBCVOD

            var comboBoxBCVODList = _visualAssessmentResultDependency.VisualAcuity;
            LoadComboBox<IdNameModel<int>>(comboBoxBCVOD, comboBoxBCVODList, "--Select Type--");

            var comboBoxBCVOSList= _visualAssessmentResultDependency.VisualAcuity;
            LoadComboBox<IdNameModel<int>>(comboBoxBCVOS, comboBoxBCVOSList, "--Select Type--");

            var comboBoxBCVOUList = _visualAssessmentResultDependency.VisualAcuity;
            LoadComboBox<IdNameModel<int>>(comboBoxBCVOU, comboBoxBCVOUList, "--Select Type--");

            var comboBoxHXBCVODList= _visualAssessmentResultDependency.VisualFieldScores;
            LoadComboBox<IdNameModel<int>>(comboBoxHXBCVOD, comboBoxHXBCVODList, "--Select Type--");

            var comboBoxHXBCVOSList = _visualAssessmentResultDependency.VisualFieldScores;
            LoadComboBox<IdNameModel<int>>(comboBoxHXBCVOS, comboBoxHXBCVOSList, "--Select Type--");

            var comboBoxSingleImageBCVOUList = _visualAssessmentResultDependency.SingleImage;
            LoadComboBox<IdNameModel<int>>(comboBoxSingleImageBCVOU, comboBoxSingleImageBCVOUList, "--Select Type--");

            var comboBoxGlareTestBCVOUList = _visualAssessmentResultDependency.SingleImage;
            LoadComboBox<IdNameModel<int>>(comboBoxGlareTestBCVOU, comboBoxGlareTestBCVOUList, "--Select Type--");

            var comboBoxColorVisionBCVOUist = _visualAssessmentResultDependency.ColourVisionScores;
            LoadComboBox<IdNameModel<int>>(comboBoxColorVisionBCVOU, comboBoxColorVisionBCVOUist, "--Select Type--");

            var comboBoxResultConclusionList = _visualAssessmentResultDependency.ResultConclusions;
            LoadComboBox<IdNameModel<string>>(comboBoxResultConclusion, comboBoxResultConclusionList, "--Select Type--");

            var comboBoxPassOrFailList = _visualAssessmentResultDependency.PassOrFail;
            LoadComboBox<IdNameModel<int>>(comboBoxPassOrFail, comboBoxPassOrFailList, "--Select Type--");

            MessageBox.Show("Dependencies loaded successfully");
        }

        


        public void LoadComboBox<T>(ComboBox comboBox, List<T> entries, string defaultText) where T:new()
        {
            List<T> list = new List<T>();

            if (typeof(T).GetProperty("Id") != null && typeof(T).GetProperty("Name") != null)
            {
                try
                {
                    dynamic item = new T();
                    item.Id = 0;
                    item.Name = defaultText;
                    list.Insert(0, item);
                }
                catch (Exception)
                {

                }
                
            }

            list.AddRange(entries);
            comboBox.DataSource = list;
            comboBox.DisplayMember = "Name";
            comboBox.SelectedText = "Name";
            comboBox.ValueMember = "Id";
        }

        private void AssessmentResultForm_Load(object sender, EventArgs e)
        {
            // Get available video devices
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_videoDevices.Count == 0)
            {
                MessageBox.Show("No video devices found.");
                return;
            }

            // Initialize the video capture device with the first available device
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);

            // Set a high resolution (if available)
            if (_videoSource.VideoCapabilities.Length > 0)
            {
                // Choose the highest resolution available
                var bestResolution = _videoSource.VideoCapabilities.OrderByDescending(cap => cap.FrameSize.Width * cap.FrameSize.Height).First();
                _videoSource.VideoResolution = bestResolution;
            }

            _videoSource.NewFrame += VideoSource_NewFrame;

            // Start the video capture
            _videoSource.Start();
        }

        // Capture the current frame from the video feed
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Set the current frame from the camera feed
            _currentFrame = (Bitmap)eventArgs.Frame.Clone();

            // Display the video feed in PictureBox
            pictureBoxVideoFeed.Image = _currentFrame;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.WaitForStop();
            }
            //AssessmentResultList listForm = new AssessmentResultList(_user);
            //listForm.ShowDialog();
        }

        private void BtnCapture_Click(object sender, EventArgs e)
        {
            if (_currentFrame != null)
            {
                // Save the current frame as an image
                string imageDirtectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");// "captured_image.jpg";
                if (!Directory.Exists(imageDirtectory)) Directory.CreateDirectory(imageDirtectory);

                string imagePath = Path.Combine(imageDirtectory, DateTime.Now.Ticks.ToString() + ".jpg");
                _currentFrame.Save(imagePath);

                MessageBox.Show("Image captured and saved to " + imagePath);
                _imagePath = imagePath;

                pictureBoxShapShot.Image = _currentFrame;
            }
            else
            {
                MessageBox.Show("No video feed available.");
            }
        }

        private void comboBoxResultConclusion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboBoxPassOrFail.SelectedIndex = comboBoxResultConclusion.SelectedIndex == 2 ? 0 : 1;
            int selectedIndex = comboBoxResultConclusion.SelectedIndex;
            if (comboBoxPassOrFail.Items.Count > 2)
            {
                if (selectedIndex == 1 || selectedIndex == 0)
                {
                    comboBoxPassOrFail.SelectedIndex = 1;
                }
                else
                {
                    comboBoxPassOrFail.SelectedIndex = 2;
                }
            }
        }

        private void comboBoxPassOrFail_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBoxPassOrFail.SelectedIndex;
        }

        private MessageResponse SubmitValidation()
        {
            MessageResponse result = new MessageResponse();
            if (comboBoxServiceType.SelectedIndex == 0)
            {
                result.Message = "Please select a Service Type";
                return result;
            }
            if (comboBoxServiceType.SelectedIndex == 1 && comboBoxLearnerDriversLicense.SelectedIndex == 0)
            {
                result.Message = "Please select Driver's License Type";
                return result;
            }
            if (string.IsNullOrEmpty(txtSurname.Text.Trim()))
            {
                result.Message = "Please enter Surname";
                return result;
            }
            if (string.IsNullOrEmpty(txtFirstName.Text.Trim()))
            {
                result.Message = "Please enter First Name";
                return result;
            }
            if (string.IsNullOrEmpty(txtOthername.Text.Trim()))
            {
                result.Message = "Please enter Othername";
                return result;
            }
            if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                result.Message = "Please enter Address";
                return result;
            }
            if (string.IsNullOrEmpty(txtContact.Text.Trim()))
            {
                result.Message = "Please enter Contact";
                return result;
            }
            if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                result.Message = "Please enter Address";
                return result;
            }
            if (string.IsNullOrEmpty(txtTin.Text.Trim()))
            {
                result.Message = "Please enter TIN";
                return result;
            }
            if (comboBoxUnaidedOD.SelectedIndex == 0)
            {
                result.Message = "Select Unaided OD";
                return result;
            }
            if (comboBoxUnaidedOS.SelectedIndex == 0)
            {
                result.Message = "Select Unaided OS";
                return result;
            }
            if (comboBoxBCVOD.SelectedIndex == 0)
            {
                result.Message = "Select BCV OD";
                return result;
            }
            if (comboBoxBCVOS.SelectedIndex == 0)
            {
                result.Message = "Select BCV OS";
                return result;
            }
            if (comboBoxBCVOU.SelectedIndex == 0)
            {
                result.Message = "Select BCV OU";
                return result;
            }
            if (string.IsNullOrEmpty(txtRemark.Text.Trim()))
            {
                result.Message = "Enter remark";
                return result;
            }
            if (comboBoxResultConclusion.SelectedIndex == 0)
            {
                result.Message = "Select Result Conclusion";
                return result;
            }

            result.Success = true;
            return result;
        }

        private MessageResponse SaveValidation()
        {
            MessageResponse result = new MessageResponse();
            if (comboBoxServiceType.SelectedIndex == 0)
            {
                result.Message = "Please select a Service Type";
                return result;
            }
            if (comboBoxServiceType.SelectedIndex == 1 && comboBoxLearnerDriversLicense.SelectedIndex == 0)
            {
                result.Message = "Please select Driver's License Type";
                return result;
            }
            if (string.IsNullOrEmpty(txtSurname.Text.Trim()))
            {
                result.Message = "Please enter Surname";
                return result;
            }
            if (string.IsNullOrEmpty(txtFirstName.Text.Trim()))
            {
                result.Message = "Please enter First Name";
                return result;
            }
            if (string.IsNullOrEmpty(txtContact.Text.Trim()))
            {
                result.Message = "Please enter Contact";
                return result;
            }
            if (string.IsNullOrEmpty(txtAddress.Text.Trim()))
            {
                result.Message = "Please enter Address";
                return result;
            }

            result.Success = true;
            return result;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            var validation = SaveValidation();
            if (!validation.Success)
            {
                MessageBox.Show(validation.Message, "Validation Error", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(_imagePath))
            {
                MessageBox.Show("Kindly take a Capture before procedding", "Validation Error", MessageBoxButtons.OK);
                return;
            }

            //AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory
            string resultConclusion = comboBoxResultConclusion.SelectedValue.ToString();

            string resultConclusionValue = comboBoxResultConclusion.SelectedValue.ToString();

            int HX_BCV_OUValue = (comboBoxHXBCVOD.SelectedIndex == 0 ? 0 : Convert.ToInt32(comboBoxHXBCVOD.SelectedValue))
                + (comboBoxHXBCVOS.SelectedIndex == 0 ? 0 : Convert.ToInt32(comboBoxHXBCVOS.SelectedValue));
            int? passResult = null;
            if (comboBoxPassOrFail.SelectedIndex > 0) passResult = Convert.ToInt32(comboBoxPassOrFail.SelectedValue);

            BtnSave.Enabled = false;
            MessageResponse<long> result = null;
            VisualAssessmentResult visualAssessmentResult = new VisualAssessmentResult
            {
                AccessType = comboBoxServiceType.SelectedIndex == 1 ? 1 : 2,
                PassOrFail = (resultConclusion == "Fit to drive" || resultConclusion == "Fit to drive with glasses") ? PassOrFail.Pass : PassOrFail.Fail,
                PassResult = passResult,
                Surname = txtSurname.Text.ToUpper(),
                //DriversLicence = model.DriversLicence,
                //DVLAReferenceNo = model.DVLAReferenceNo,
                FirstName = txtFirstName.Text.ToUpper(),
                OtherName = txtOthername.Text.Trim().ToUpper(),
                DOB = dateTimePickerDOB.Value,
                PostalAddress = txtAddress.Text.Trim(),
                ContactNumber = txtContact.Text,
                TaxIdentificationNumber = txtTin.Text,
                Email = txtEmail.Text,
                Unaided_OD = comboBoxUnaidedOD.SelectedIndex == 0 ? "" : comboBoxUnaidedOD.Text,
                Unaided_OS = comboBoxUnaidedOS.SelectedIndex == 0 ? "" : comboBoxUnaidedOS.Text,
                //Unaided_OU = comboBoxUnaidedOU.SelectedIndex == 0 ? "" : comboBoxUnaidedOS.SelectedText,
                BCV_OD = comboBoxBCVOD.SelectedIndex == 0 ? "" : comboBoxBCVOD.Text,
                BCV_OS = comboBoxBCVOS.SelectedIndex == 0 ? "" : comboBoxBCVOS.Text,
                BCV_OU = comboBoxBCVOU.SelectedIndex == 0 ? "" : comboBoxBCVOU.Text,
                HX_BCV_OD = comboBoxHXBCVOD.SelectedIndex == 0 ? "" : comboBoxHXBCVOD.Text,
                HX_BCV_OS = comboBoxHXBCVOS.SelectedIndex == 0 ? "" : comboBoxHXBCVOS.Text,
                HX_BCV_OU = HX_BCV_OUValue.ToString(),
                SingleImage_BCV_OU = comboBoxSingleImageBCVOU.SelectedIndex == 0 ? "" : comboBoxSingleImageBCVOU.Text,
                //HX_BCV_OU = HX_BCV_OUValue.ToString(),
                //GlareTest_BCV_OD = model.GlareTest_BCV_OD,
                //GlareTest_BCV_OS = model.GlareTest_BCV_OS,
                GlareTest_BCV_OU = comboBoxGlareTestBCVOU.SelectedIndex == 0 ? "" : comboBoxGlareTestBCVOU.Text,

                ColourVision_BCV_OU = comboBoxColorVisionBCVOU.SelectedIndex == 0 ? "" : comboBoxColorVisionBCVOU.Text,
                //ContrastSensitivity_BCV = model.ContrastSensitivity_BCV,
                PathologicalRemarks = txtRemark.Text.Trim(),
                ResultConclusion = resultConclusionValue,
                ResultServiceType = Convert.ToInt32(comboBoxServiceType.SelectedValue),
                LearnerDriversLicence = Convert.ToInt32(comboBoxLearnerDriversLicense.SelectedValue), //learnerDriversLicence,
                OptometristFirmId = _user.OptometristFirmId.GetValueOrDefault(),
                ReferenceNumber = null,
                CreatedBy = _user.Id,
                IsActive = true,
                IsDeleted = false,
                TestDate = DateTime.UtcNow,
                PassportImageUrl = Path.GetFileName(_imagePath),
                Status = Status.InProgress,
                IsSynchronized = false,
                TestType = 0,
                OldDVLAReferenceNo = null,
                FormNumber = Guid.NewGuid().ToString(),
                IsTransmitted = false
            };
            if (_id == 0)
            {
                result = await VisualAssessmentService.CreateAsync(visualAssessmentResult);
            }
            else
            {
                visualAssessmentResult.Id = _id;
                result = await VisualAssessmentService.UpdateAsync(visualAssessmentResult);
            }
            if (result.Success)
            {
                MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK);
                BtnSave.Enabled = true;
                BtnSave.Text = "Retry";
            }
            LoadVisualAssessmentData(result.Result);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnSubmit_Click(object sender, EventArgs e)
        {
            var validation = SubmitValidation();
            if (!validation.Success)
            {
                MessageBox.Show(validation.Message, "Validation Error", MessageBoxButtons.OK);
                return;
            }

            if (string.IsNullOrEmpty(_imagePath))
            {
                MessageBox.Show("Kindly take a Capture before procedding", "Validation Error", MessageBoxButtons.OK);
                return;
            }

            //AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory
            string resultConclusion = comboBoxResultConclusion.SelectedValue.ToString();

            string resultConclusionValue = comboBoxResultConclusion.SelectedValue.ToString();

            int HX_BCV_OUValue = (comboBoxHXBCVOD.SelectedIndex == 0 ? 0 : Convert.ToInt32(comboBoxHXBCVOD.SelectedValue))
                + (comboBoxHXBCVOS.SelectedIndex == 0 ? 0 : Convert.ToInt32(comboBoxHXBCVOS.SelectedValue));
            int? passResult = null;
            if (comboBoxPassOrFail.SelectedIndex > 0) passResult = Convert.ToInt32(comboBoxPassOrFail.SelectedValue);

            BtnSave.Enabled = false;
            MessageResponse<long> result = null;
            VisualAssessmentResult visualAssessmentResult = new VisualAssessmentResult
            {
                AccessType = comboBoxServiceType.SelectedIndex == 1 ? 1 : 2,
                PassOrFail = (resultConclusion == "Fit to drive" || resultConclusion == "Fit to drive with glasses") ? PassOrFail.Pass : PassOrFail.Fail,
                PassResult = passResult,
                Surname = txtSurname.Text.ToUpper(),
                //DriversLicence = model.DriversLicence,
                //DVLAReferenceNo = model.DVLAReferenceNo,
                FirstName = txtFirstName.Text.ToUpper(),
                OtherName = txtOthername.Text.Trim().ToUpper(),
                DOB = dateTimePickerDOB.Value,
                PostalAddress = txtAddress.Text.Trim(),
                ContactNumber = txtContact.Text,
                TaxIdentificationNumber = txtTin.Text,
                Email = txtEmail.Text,
                Unaided_OD = comboBoxUnaidedOD.SelectedIndex == 0 ? "" : comboBoxUnaidedOD.Text,
                Unaided_OS = comboBoxUnaidedOS.SelectedIndex == 0 ? "" : comboBoxUnaidedOS.Text,
                //Unaided_OU = comboBoxUnaidedOU.SelectedIndex == 0 ? "" : comboBoxUnaidedOS.SelectedText,
                BCV_OD = comboBoxBCVOD.SelectedIndex == 0 ? "" : comboBoxBCVOD.Text,
                BCV_OS = comboBoxBCVOS.SelectedIndex == 0 ? "" : comboBoxBCVOS.Text,
                BCV_OU = comboBoxBCVOU.SelectedIndex == 0 ? "" : comboBoxBCVOU.Text,
                HX_BCV_OD = comboBoxHXBCVOD.SelectedIndex == 0 ? "" : comboBoxHXBCVOD.Text,
                HX_BCV_OS = comboBoxHXBCVOS.SelectedIndex == 0 ? "" : comboBoxHXBCVOS.Text,
                HX_BCV_OU = HX_BCV_OUValue.ToString(),
                SingleImage_BCV_OU = comboBoxSingleImageBCVOU.SelectedIndex == 0 ? "" : comboBoxSingleImageBCVOU.Text,
                //HX_BCV_OU = HX_BCV_OUValue.ToString(),
                //GlareTest_BCV_OD = model.GlareTest_BCV_OD,
                //GlareTest_BCV_OS = model.GlareTest_BCV_OS,
                GlareTest_BCV_OU = comboBoxGlareTestBCVOU.SelectedIndex == 0 ? "" : comboBoxGlareTestBCVOU.Text,

                ColourVision_BCV_OU = comboBoxColorVisionBCVOU.SelectedIndex == 0 ? "" : comboBoxColorVisionBCVOU.Text,
                //ContrastSensitivity_BCV = model.ContrastSensitivity_BCV,
                PathologicalRemarks = txtRemark.Text.Trim(),
                ResultConclusion = resultConclusionValue,
                ResultServiceType = Convert.ToInt32(comboBoxServiceType.SelectedValue),
                LearnerDriversLicence = Convert.ToInt32(comboBoxLearnerDriversLicense.SelectedValue), //learnerDriversLicence,
                OptometristFirmId = _user.OptometristFirmId.GetValueOrDefault(),
                ReferenceNumber = null,
                CreatedBy = _user.Id,
                IsActive = true,
                IsDeleted = false,
                TestDate = DateTime.UtcNow,
                PassportImageUrl = Path.GetFileName(_imagePath),
                Status = Status.InProgress,
                IsSynchronized = false,
                TestType = 0,
                OldDVLAReferenceNo = null,
                FormNumber = Guid.NewGuid().ToString(),
                IsSubmitted = true
            };
            if (_id == 0)
            {
                result = await VisualAssessmentService.CreateAsync(visualAssessmentResult);
            }
            else
            {
                visualAssessmentResult.Id = _id;
                result = await VisualAssessmentService.UpdateAsync(visualAssessmentResult);
            }
            if (result.Success)
            {
                MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK);
                BtnSave.Enabled = true;
                BtnSave.Text = "Retry";
            }
            _id = result.Result;
            LoadVisualAssessmentData(result.Result);

        }

        private async void BtnTransmit_Click(object sender, EventArgs e)
        {
            MessageResponse result = await VisualAssessmentService.Transmit(_id);
            if (result.Success)
            {
                MessageBox.Show(result.Message, "Success", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK);
            }
            LoadVisualAssessmentData(_id);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AssessmentResultList list = new AssessmentResultList(_user);
            this.Close();
            list.Show();
            

        }
    }
}
