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
using Xceed.Wpf.Toolkit;


namespace EasyAppointment
{
    /// <summary>
    /// Interaction logic for BookAppointment.xaml
    /// </summary>
    public partial class BookAppointment : Window
    {
        private List<Doctor> DoctorList = new List<Doctor>();
        private List<Patient> PatientList = new List<Patient>();
        private List<AvailableTime> AvailableTimeList = new List<AvailableTime>();
        private int selecteddocid;
        private string selecteddocdate = DateTime.Now.ToString("yyyy-MM-dd");
        private int selecteddoctime;

        public BookAppointment(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            lvDoctors.ItemsSource = DoctorList;
            //lvAvailableTimes.ItemsSource = AvailableTimeList;
            ReloadAvailableDoctorList();
            //ReloadAvailableTimeList();
            lvAvailableTimes.ItemsSource = AvailableTimeList;
            
        }

        private List<AvailableTime> GetDoctorAvailableTimes(List<Appointment> appointmentlist, AvailableTime worktime)
        {
            List<AvailableTime> availList = new List<AvailableTime>();

           // if (appointmentlist == null || worktime == null)
           //     return null;
            for (int i = worktime.StartTime; i < worktime.EndTime; i ++)
            {
                bool isBooked = false;
                foreach (Appointment a in appointmentlist)
                {
                    if (a.AppointmentTime == i)
                        isBooked = true;
                }
                if (!isBooked)
                    availList.Add(new AvailableTime() { StartTime = i, EndTime = i + 1 });
            }
            return availList;
        }
        private void ReloadAvailableTimeList()
        {
            try
            {

                List<Appointment> list = Globals.Db.GetAllAppointsByDoctorDate(selecteddocid, selecteddocdate);

                AvailableTime docWorkHours = Globals.Db.GetDoctorAvailability(selecteddocid, selecteddocdate);
                if (docWorkHours == null)
                {
                    System.Windows.MessageBox.Show("Doctor schedule hasn't been set for date: " + selecteddocdate);

                    AvailableTimeList.Clear();
                }
                else
                {
                    AvailableTimeList = GetDoctorAvailableTimes(list, docWorkHours);
                }
                //if (list.Count > 0)
                
                
                lvAvailableTimes.ItemsSource = AvailableTimeList;
                lvAvailableTimes.Items.Refresh();
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void ReloadAvailableDoctorList()
        {
            try
            {

                List<Doctor> list = Globals.Db.GetAllAvailableDoctors();

                DoctorList.Clear();
                foreach (Doctor d in list)
                {
                    DoctorList.Add(d);
                }
                lvDoctors.Items.Refresh();
            }
            catch (SqlException ex)
            {
                System.Windows.MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void Wizard_Next(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            if ((sender as Wizard).CurrentPage.Name == "Page1")
            {
                if ((lvDoctors.SelectedValue as Doctor) == null)
                {
                    System.Windows.MessageBox.Show("Please select doctor", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                    e.Cancel = true;
                    return;
                }

                selecteddocid = (lvDoctors.SelectedValue as Doctor).Id;
                ReloadAvailableTimeList();
            }
            else if ((sender as Wizard).CurrentPage.Name == "Page2")
            {
                AvailableTime att = lvAvailableTimes.SelectedValue as AvailableTime;
                if (att == null)
                {
                    System.Windows.MessageBox.Show("Please select available time", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                    e.Cancel = true;
                    return;
                }
                if (!cldAppointmentDate.SelectedDate.HasValue)
                {
                    System.Windows.MessageBox.Show("Please select date", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                    e.Cancel = true;
                    return;
                }
                selecteddocdate = cldAppointmentDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                selecteddoctime = att.StartTime;
            }
           
        }

        private void Wizard_Finish(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            Patient pt = lvPatients.SelectedValue as Patient;
            if (pt == null)
            {
                System.Windows.MessageBox.Show("Please select patient", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                e.Cancel = true;
                return;
            }
            if (tbAppointmentReason.Text == "")
            {
                System.Windows.MessageBox.Show("Please input appointment reason", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                e.Cancel = true;
                return;
            }
            Globals.Db.AddAppointment(selecteddocid, pt.Id, tbAppointmentReason.Text, selecteddocdate, selecteddoctime);
        }

        private void TbPatientName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tbPatientName.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                if (tbPatientName.Text == "")
                {
                    System.Windows.MessageBox.Show("Please input patient name", "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                lvPatients.ItemsSource = Globals.Db.GetAllPatients(tbPatientName.Text);
                
            }
        }

        private void CldAppointmentDate_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cldAppointmentDate.SelectedDate.HasValue)
            {
                selecteddocdate = cldAppointmentDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                ReloadAvailableTimeList();
            }
                
        }

        private void TbDoctorName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbDoctorName.Text == "")
            {
                lvDoctors.ItemsSource = DoctorList;
                return;
            }
            List<Doctor> rltDoctorList = DoctorList.Where(x => (x.FirstName.ToLower().Contains(tbDoctorName.Text.ToLower()) || x.LastName.ToLower().Contains(tbDoctorName.Text.ToLower()))).ToList();
            lvDoctors.ItemsSource = rltDoctorList;
        }

        private void CkbUseMDocName_Checked(object sender, RoutedEventArgs e)
        {
            Task<string>.Run(async () => { VoiceToText.VoiceToTextOnce(this.tbDoctorName, this.ckbUseMDocName); });
        }

        private void CkbUseMPatName_Checked(object sender, RoutedEventArgs e)
        {
            Task<string>.Run(async () => { VoiceToText.VoiceToTextOnce(this.tbPatientName, this.ckbUseMPatName); });
        }
    }
}