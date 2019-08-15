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
    /// Interaction logic for ShowPatientsWindow.xaml
    /// </summary>
    public partial class ShowPatientsWindow : Window
    {
        List<Patient> patientsList = new List<Patient>();
        public ShowPatientsWindow(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            lvPatientsList.ItemsSource = patientsList;
            try
            {
                Globals.Db = new Database();
                // 2 - load data in ListView
                ReloadPatientsList();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Fatal error: unable to connect to database\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // close the main window, terminate the program
            }
        }

        private void ReloadPatientsList()
        {
            try
            {

                List<Patient> list = Globals.Db.GetAllPatientsList();

                patientsList.Clear();
                foreach (Patient a in list)
                {
                    patientsList.Add(a);
                }
                lvPatientsList.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPatient_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            Patient patientToUpdate = lvPatientsList.SelectedItem as Patient;
            if (patientToUpdate == null) return;

            RegisterPatientDialog patientUpdateDialog = new RegisterPatientDialog(this, patientToUpdate);
            if (patientUpdateDialog.ShowDialog() == true)
            {
                ReloadPatientsList();
            }
        }

        private void DeletePatient_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            Patient patientToDelete = lvPatientsList.SelectedItem as Patient;
            if (patientToDelete == null) return;
            //
            MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to delete:\n" +
                patientToDelete+ "\nAll data related to this patient will be lost.", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            //
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    Globals.Db.DeletePatient(patientToDelete.Id);
                    ReloadPatientsList();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

