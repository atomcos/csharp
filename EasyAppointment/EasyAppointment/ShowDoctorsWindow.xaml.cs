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
    /// Interaction logic for ShowDoctorsWindow.xaml
    /// </summary>
    public partial class ShowDoctorsWindow : Window
    {
        List<Doctor> doctorsList = new List<Doctor>();
        public ShowDoctorsWindow(Window owner)
        {
            Owner = owner;
            InitializeComponent();
            lvDoctorsList.ItemsSource = doctorsList;
            try
            {
                
                ReloadDoctorsList();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Fatal error: unable to connect to database\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
                Close(); // close the main window, terminate the program
            }
        }

        private void ReloadDoctorsList()
        {
            try
            {

                List<Doctor> list = Globals.Db.GetAllAvailableDoctors();

                doctorsList.Clear();
                foreach (Doctor d in list)
                {
                    doctorsList.Add(d);
                }
                lvDoctorsList.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditDoctor_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            Doctor docToUpdate = lvDoctorsList.SelectedItem as Doctor;
            if (docToUpdate == null) return;

            RegisterDoctorDialog docUpdateDialog = new RegisterDoctorDialog(this, docToUpdate);
            if (docUpdateDialog.ShowDialog() == true)
            {
                ReloadDoctorsList();
            }
        }

        private void DeleteDoctor_ContextMenuClick(object sender, RoutedEventArgs e)
        {
            Doctor docToDelete = lvDoctorsList.SelectedItem as Doctor;
            if (docToDelete == null) return;
            //
            MessageBoxResult result = MessageBox.Show(this, "Are you sure you want to delete:\n" +
                docToDelete + "\nAll data related to this doctor will be lost.", "Confirm delete", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            //
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    Globals.Db.DeleteDoctor(docToDelete.Id);
                    ReloadDoctorsList();
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

