using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EasyAppointment
{
    /// <summary>
    /// Interaction logic for RegisterPatientDialog.xaml
    /// </summary>
    public partial class RegisterPatientDialog : Window
    {
        Patient PatientToEdit;
        WebCam webcam;
        byte[] Photo = null;
        public RegisterPatientDialog(Window owner, Patient patientToEdit = null)
        {
            Owner = owner;
            InitializeComponent();
            Title = "New Patient";
            
            PatientToEdit = patientToEdit;
            if (PatientToEdit != null)
            {
                Title = "Update Patient";
                btRegisterPatient.Content = "Save changes";
                tbFirstName.Text = PatientToEdit.FirstName;
                tbLastName.Text = PatientToEdit.LastName;
                tbMedicalInsurance.Text = PatientToEdit.MedInsurance;
                rbFemale.IsChecked = (PatientToEdit.Gender == "Female");
                tbDOB.Text = PatientToEdit.DateOfBirth.ToShortDateString();
                tbAddress.Text = PatientToEdit.PatientAddress;
                tbCity.Text = PatientToEdit.City;
                tbPostalCode.Text = PatientToEdit.PostalCode;
                cboProvince.Text = PatientToEdit.Province;
                tbTelephone.Text = PatientToEdit.Telephone;
                tbMedicalCondition.Text = PatientToEdit.MedCondition;

                if (Globals.Db.ShowPatientPhoto(PatientToEdit.Id) != null)
                {
                    imgCapture.Source = Globals.Db.ShowPatientPhoto(PatientToEdit.Id);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
        }

        private void BtRegisterPatient_Click(object sender, RoutedEventArgs e)
        {
            string firstName = tbFirstName.Text;
            if (firstName.Length < 1 || firstName.Length > 50)
            {
                MessageBox.Show("First name should be between 1 and 50 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string lastName = tbLastName.Text;
            if (lastName.Length < 1 || lastName.Length > 50)
            {
                MessageBox.Show("Last name should be between 1 and 50 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string medInsurance = tbMedicalInsurance.Text;
            if (medInsurance.Length < 10)
            {
                MessageBox.Show("Medical insurance should be 10 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string gender = (bool)rbMale.IsChecked ? "Male" : "Female";
            
            DateTime dob = new DateTime();
            try
            {
                dob = DateTime.ParseExact(tbDOB.Text, "d", null);
            }
            catch (FormatException)
            {
                MessageBox.Show("Date should be in yyyy-mm-dd format.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string patientAddress = tbAddress.Text;
            if (patientAddress.Length < 1 || patientAddress.Length > 30)
            {
                MessageBox.Show("Address should be between 1 and 30 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string city = tbCity.Text;
            if (city.Length < 1 || city.Length > 20)
            {
                MessageBox.Show("City should be between 1 and 20 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //FIXME: Postal code add regex check
            string postalCode = tbPostalCode.Text;
            string province = cboProvince.Text;
            if (province == "")
            {
                MessageBox.Show("Please select province.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //TODO: masked textbox for telephone  !Regex.IsMatch(telephone, @"^[0-9]{3}[-][0-9]{3}[-][0-9]{4}$")
            string telephone = tbTelephone.Text;
            if ( telephone.Length < 10)
            {
                MessageBox.Show("Telephone number format should be 10 characters minimum.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string medCondition = tbMedicalCondition.Text;
            if (medCondition.Length < 1 || medCondition.Length > 100)
            {
                MessageBox.Show("Medical condition should be between 1 and 100 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            DateTime dateModified = DateTime.Now;
            if (Photo == null)
            {
                MessageBox.Show("Please take a photo and save it.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
                
            }
           
            byte[] photo = Photo;
            
            if (PatientToEdit == null)
            {
                Patient newPatient = new Patient() {MedInsurance = medInsurance, FirstName = firstName, LastName = lastName, Gender = gender, DateOfBirth = dob, PatientAddress = patientAddress, City = city, PostalCode = postalCode, Province = province, Telephone = telephone, MedCondition = medCondition, DateModified = dateModified, Photo = photo };

                try
                {
                    Globals.Db.AddPatient(newPatient);

                    DialogResult = true;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\nPlease fill in all the fields." + ex.Message,
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                PatientToEdit.FirstName = firstName;
                PatientToEdit.LastName = lastName;
                PatientToEdit.MedInsurance = medInsurance;
                PatientToEdit.Gender = gender;
                PatientToEdit.DateOfBirth = dob;
                PatientToEdit.PatientAddress = patientAddress;
                PatientToEdit.City = city;
                PatientToEdit.PostalCode = postalCode;
                PatientToEdit.Province = province;
                PatientToEdit.Telephone = telephone;
                PatientToEdit.MedCondition = medCondition;
                PatientToEdit.Photo = photo;
                try
                {
                    Globals.Db.UpdatePatient(PatientToEdit);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\nPlease fill in all the fields." + ex.Message,
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);

                }

            }
            DialogResult = true;
        }

        private void BntStart_Click(object sender, RoutedEventArgs e)
        {
            webcam.Start();
        }

        private void BntStop_Click(object sender, RoutedEventArgs e)
        {
            webcam.Stop();
        }

        private void BntContinue_Click(object sender, RoutedEventArgs e)
        {
            webcam.Continue();
        }

        private void BntCapture_Click(object sender, RoutedEventArgs e)
        {
            imgCapture.Source = imgVideo.Source;
        }

        private void BntSaveImage_Click(object sender, RoutedEventArgs e)
        {
            Photo = Helper.SaveImageCapture((BitmapSource)imgCapture.Source);
            if (Photo != null && Photo.Length > 0)
            {
                MessageBox.Show("Photo was successfully saved.",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            webcam.Stop();
        }

        
    }
}
