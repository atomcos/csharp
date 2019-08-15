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
    /// Interaction logic for DoctorMainWindow.xaml
    /// </summary>
    public partial class DoctorMainWindow : Window
    {
        
        static string scheduleDate;
        List<Appointment> appointmentsList = new List<Appointment>();
        public DoctorMainWindow()
        {
            InitializeComponent();
            try
            {
                Doctor DocLoggedIn = Globals.Db.GetLoggedInDoctorData(Globals.CurrentUser.DoctorId);
                lbFullName.Content = string.Format("{0} {1}", DocLoggedIn.FirstName, DocLoggedIn.LastName);
                lbDocotorType.Content = DocLoggedIn.DoctorType;
                lbOffice.Content = DocLoggedIn.Office;
                lbTelephone.Content = DocLoggedIn.Telephone;
                lbSpecialty.Content = DocLoggedIn.Specialty;

                imgPhoto.Source = Globals.Db.ShowDoctorPhoto(Globals.CurrentUser.DoctorId);
                dpSchedule.SelectedDate = DateTime.Today; 
                scheduleDate = dpSchedule.SelectedDate.Value.ToString("yyyy-MM-dd");

                lvAppointments.ItemsSource = appointmentsList;
            
                
                ReloadAppointmentsList();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Fatal error: unable to connect to database\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // close the main window, terminate the program
            }


        }

        private void ReloadAppointmentsList()
        {
            try
            {

                List<Appointment> list = Globals.Db.GetAllAppointsByDoctorDate(Globals.CurrentUser.DoctorId, scheduleDate);

                appointmentsList.Clear();
                foreach (Appointment a in list)
                {
                    appointmentsList.Add(a);
                }
                lvAppointments.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DpSchedule_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            scheduleDate = dpSchedule.SelectedDate.Value.ToString("yyyy-MM-dd");
            ReloadAppointmentsList();
        }

        private void BtAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            Appointment app = lvAppointments.SelectedValue as Appointment;
            if (app == null)
            {
                MessageBox.Show("You must select an appointment.", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            (new CreatePrescription(this, app)).ShowDialog();
        }
    }
}
