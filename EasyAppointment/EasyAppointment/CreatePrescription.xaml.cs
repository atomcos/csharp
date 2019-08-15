using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace EasyAppointment
{
    /// <summary>
    /// Interaction logic for CreatePrescription.xaml
    /// </summary>
    public class Dic
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public partial class CreatePrescription : Window
    {
        private List<Prescription> PrescriptionList = new List<Prescription>();
        private List<Dic> PatientDetail = new List<Dic>();
        private Appointment appointment;
        private Prescription currentprescription;
        private Patient currentpatient;

        private void LoadPrescriptions(int pid)
        {
            try
            {
                List<Prescription> pl = Globals.Db.GetAllPrescription(pid);
                PrescriptionList.Clear();
                foreach (Prescription p in pl)
                    PrescriptionList.Add(p);
                lvPreviousPrescriptions.Items.Refresh();
                
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Loading Data error: " + ex.Message, "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void LoadPatientDetails(int pid)
        {
            try
            {
                currentpatient = Globals.Db.GetPatient(pid);
                PatientDetail.Add(new Dic() { key = "Medical Insurance Number", value = currentpatient.MedInsurance });
                PatientDetail.Add(new Dic() { key = "First Name", value = currentpatient.FirstName });
                PatientDetail.Add(new Dic() { key = "Last Name", value = currentpatient.LastName });
                PatientDetail.Add(new Dic() { key = "Gender", value = currentpatient.Gender });
                PatientDetail.Add(new Dic() { key = "Date of birth", value = currentpatient.DateOfBirth.ToString("yyyy-MM-dd") });
                PatientDetail.Add(new Dic() { key = "Phone", value = currentpatient.Telephone });
                PatientDetail.Add(new Dic() { key = "Medical Condition", value = currentpatient.MedCondition });
                lvPatientInfo.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Loading Patient Data error: " + ex.Message, "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public CreatePrescription(Window owner, Appointment app)
        {
            appointment = app;
            InitializeComponent();
            Owner = owner;
            lvPreviousPrescriptions.ItemsSource = PrescriptionList;
            lvPatientInfo.ItemsSource = PatientDetail;
            LoadPatientDetails(app.PatientId);
            LoadPrescriptions(app.PatientId);
        }

        private void BtAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            if (tbPrescriptionDetails.Text == "")
            {
                MessageBox.Show("Prescription Details must not empty", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (appointment == null)
            {
                MessageBox.Show("Appointment Details Error", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                currentprescription = new Prescription() { PrescriptionDate = DateTime.Now, AppointmentId = appointment.Id, /*PatientId = appointment.PatientId,*/ PrescriptionDetails = tbPrescriptionDetails.Text };
                if (Globals.Db.AddPrescription(currentprescription) > 0)
                    MessageBox.Show("Adding prescription successfully.", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadPrescriptions(appointment.PatientId);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Create Prescription Error" + ex.Message, "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void LvPreviousPrescriptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Prescription p = lvPreviousPrescriptions.SelectedValue as Prescription;
            if (p == null)
                return;
            tbPrescriptionDetails.Text = p.PrescriptionDetails;
        }




        
        public void BtPreview_Click(object sender, RoutedEventArgs e)
        {
            if (tbPrescriptionDetails.Text == "")
            {
                MessageBox.Show("Prescription Details must not empty", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Prescription pp = lvPreviousPrescriptions.SelectedValue as Prescription;
            if (pp == null || pp.PrescriptionDetails != tbPrescriptionDetails.Text)
                pp = new Prescription() { PrescriptionDetails = tbPrescriptionDetails.Text, PrescriptionDate = DateTime.Now };
            (new PrintPreview(this, appointment, currentpatient, pp)).ShowDialog();
        }
    }
}
