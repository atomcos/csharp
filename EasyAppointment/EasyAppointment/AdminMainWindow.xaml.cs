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
    /// Interaction logic for AdminMainWindow.xaml
    /// </summary>
    public partial class AdminMainWindow : Window
    {
        
        List<Appointment> appointmentsList = new List<Appointment>();
        public AdminMainWindow()
        {
            InitializeComponent();
            lvViewAppointments.ItemsSource = appointmentsList;
            try
            {
                Globals.Db = new Database();
                // 2 - load data in ListView
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
                
                List<Appointment> list = Globals.Db.GetAllAppointments();

                appointmentsList.Clear();
                foreach (Appointment a in list)
                {
                    appointmentsList.Add(a);
                }
                lvViewAppointments.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        private void AddPatient_MenuClick(object sender, RoutedEventArgs e)
        {
            RegisterPatientDialog registerPatient = new RegisterPatientDialog(this);
            
            if (registerPatient.ShowDialog() == true)
            {
                ReloadAppointmentsList();
            }
        }

        private void AddDoctor_MenuClick(object sender, RoutedEventArgs e)
        {

            RegisterDoctorDialog registerDoctor = new RegisterDoctorDialog(this);
            
            if (registerDoctor.ShowDialog() == true)
            {
                ReloadAppointmentsList();
            }
        }

        private void ViewAppointments_MenuClick(object sender, RoutedEventArgs e)
        {
            lvViewAppointments.ItemsSource = appointmentsList;
            ReloadAppointmentsList();
            
        }

        private void ViewPatients_MenuClick(object sender, RoutedEventArgs e)
        {
            ShowPatientsWindow patientListWindow = new ShowPatientsWindow(this);
            patientListWindow.ShowDialog();
        }

        private void ViewDoctors_MenuClick(object sender, RoutedEventArgs e)
        {
            ShowDoctorsWindow doctorListWindow = new ShowDoctorsWindow(this);
            doctorListWindow.ShowDialog();
        }

        private void DeleteAppointment_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            Appointment appToDelete = lvViewAppointments.SelectedItem as Appointment;
            if (appToDelete == null) return;
            //
            MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to delete:\n" +
                appToDelete, "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            //
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    Globals.Db.DeleteAppointment(appToDelete.Id);
                    ReloadAppointmentsList();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AddAppointment_MenuClick(object sender, RoutedEventArgs e)
        {
            BookAppointment bookAppDialog = new BookAppointment(this);
          if  (bookAppDialog.ShowDialog() == true)
            {
                ReloadAppointmentsList();
            }
        }

        private void DoctorSchedule_MenuClick(object sender, RoutedEventArgs e)
        {
            ShowDoctorSchedule doctorSchedule = new ShowDoctorSchedule(this);
            
            if (doctorSchedule.ShowDialog() == true)
            {
                ReloadAppointmentsList();
            }
        }
    }
}
