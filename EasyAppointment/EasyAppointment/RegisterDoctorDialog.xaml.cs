using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace EasyAppointment
{
    /// <summary>
    /// Interaction logic for RegisterPatientDialog.xaml
    /// </summary>
    public partial class RegisterDoctorDialog : Window
    {
        Doctor DocToEdit;
        WebCam webcam;
        byte[] Photo = null;
        public RegisterDoctorDialog(Window owner, Doctor docToEdit = null)
        {
            
            InitializeComponent();
            Owner = owner;
            Title = "New Doctor";
            
            DocToEdit = docToEdit;
            if (DocToEdit != null)
            {
                Title = "Update Doctor";
                btRegisterDoctor.Content = "Save changes";
                tbFirstName.Text = DocToEdit.FirstName;
                tbLastName.Text = DocToEdit.LastName;
                rbFemale.IsChecked = (DocToEdit.Gender == "Female");
                cboType.Text = DocToEdit.DoctorType;
                cboSpecialty.Text = DocToEdit.Specialty;
                tbOffice.Text = DocToEdit.Office;
                tbTelephone.Text = DocToEdit.Telephone;
                tbUserName.IsEnabled = false;
                tbPassword.IsEnabled = false;
                if (Globals.Db.ShowDoctorPhoto(DocToEdit.Id) != null)
                {
                    imgCapture.Source = Globals.Db.ShowDoctorPhoto(DocToEdit.Id);
                }
            }
        }

        private void BtRegisterDoctor_Click(object sender, RoutedEventArgs e)
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
            string gender = (bool)rbMale.IsChecked ? "Male" : "Female";
            string doctorType = cboType.Text;
            if (doctorType == "")
            {
                MessageBox.Show("Please select Type of a doctor.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string specialty = cboSpecialty.Text;
            if (specialty == "")
            {
                MessageBox.Show("Please select Specialty of a doctor.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string office = tbOffice.Text;
            if (office.Length < 1 || office.Length > 10)
            {
                MessageBox.Show("Office should be between 1 and 50 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            string telephone = tbTelephone.Text;
            if (!Regex.IsMatch(telephone, @"^[0-9]{3}[-][0-9]{3}[-][0-9]{4}$") || telephone.Length < 10)
            {
                MessageBox.Show("Telephone number format should be 000-000-0000.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            bool availibility = true;
            byte[] photo = Photo;

            if (DocToEdit == null)
            {

                string userName = tbUserName.Text;
                if (userName.Length < 1 || userName.Length > 50)
                {
                    MessageBox.Show("User name should be between 1 and 50 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string password = tbPassword.Text;
                if (password.Length < 1 || password.Length > 50)
                {
                    MessageBox.Show("Password should be between 1 and 50 characters long.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                Doctor newDoctor = new Doctor() { FirstName = firstName, LastName = lastName, Gender = gender, DoctorType = doctorType, Specialty = specialty, Office = office, Telephone = telephone, isAvailable = availibility, Photo = photo };

                try
                {
                    int docId = Globals.Db.AddDoctor(newDoctor);
                    Globals.Db.AddUser(userName, password, docId, DateTime.Now);

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\nPlease fill in all the fields." + ex.Message,
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            else
            {
                DocToEdit.FirstName = firstName;
                DocToEdit.LastName = lastName;
                DocToEdit.Gender = gender;
                DocToEdit.DoctorType = doctorType;
                DocToEdit.Specialty = specialty;
                DocToEdit.Office = office;
                DocToEdit.Telephone = telephone;
                DocToEdit.isAvailable = true;
                DocToEdit.Photo = photo;
                try
                {
                    Globals.Db.UpdateDoctor(DocToEdit);
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
            if (Photo != null)
            {
                MessageBox.Show("Photo was successfully saved.",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            webcam.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref imgVideo);
        }
    }
}
