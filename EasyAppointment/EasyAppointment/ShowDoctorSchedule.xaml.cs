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
    /// Interaction logic for ShowDoctorSchedule.xaml
    /// </summary>
    public partial class ShowDoctorSchedule : Window
    {
        List<Doctor> doctorsList = new List<Doctor>();
        Doctor docToSetSchedule = new Doctor();
        public ShowDoctorSchedule(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            lvDoctors.ItemsSource = doctorsList;

            ReloadDoctorsList();

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
                lvDoctors.Items.Refresh();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                    "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            docToSetSchedule = lvDoctors.SelectedItem as Doctor;
            if (docToSetSchedule == null)
            {
                MessageBox.Show("Please select a doctor from the list",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            string startTimeStr = cboStartTime.Text;
            if (startTimeStr == null)
            {
                MessageBox.Show("Please select start time from the list",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            List<string> listStrLineStartTime = startTimeStr.Split(':').ToList<string>();
            int startTime = int.Parse(listStrLineStartTime[0]);

            string endTimeStr = cboEndTime.Text;
            if (endTimeStr == null)
            {
                MessageBox.Show("Please select end time from the list",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            List<string> listStrLineEndTime = endTimeStr.Split(':').ToList<string>();
            int endTime = int.Parse(listStrLineEndTime[0]);

            //List<DateTime> selectedDates = new List<DateTime>();

            var selectedDates = clSheduleSet.SelectedDates;

            if (selectedDates.Count == 0)
            {
                MessageBox.Show("Please select dates from the calendar.",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            foreach (var sDate in selectedDates)
            {
                try
                {
                    DoctorAvailability workDay = new DoctorAvailability() { DoctorId = docToSetSchedule.Id, AvailableDate = sDate.ToString("yyyy-MM-dd"), StartHour = startTime, EndHour = endTime };
                    Globals.Db.AddDoctorAvailability(workDay);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                        "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            MessageBox.Show("Schedule saved.",
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void ClSheduleSet_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DoctorAvailability scheduleForSpecificDate = new DoctorAvailability();
            //check if doctor selected from list
            if (lvDoctors.SelectedItem != null)
            {
                try
                {
                    docToSetSchedule = lvDoctors.SelectedItem as Doctor;
                    //check if schedule is set for selected date for the selected doctor
                    if (Globals.Db.CheckIfScheduleIsSet(docToSetSchedule.Id, clSheduleSet.SelectedDate.Value.ToString("yyyy-MM-dd")) > 0)

                    {

                        scheduleForSpecificDate = Globals.Db.GetDoctorScheduleByDoctorIdAndDate(docToSetSchedule.Id, clSheduleSet.SelectedDate.Value.ToString("yyyy-MM-dd"));
                        lbWorkingHours.Content = scheduleForSpecificDate;
                    }

                    else
                    { lbWorkingHours.Content = "not set"; }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                        "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a doctor from the list.",
                        "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
            DoctorAvailability scheduleForSpecificDate = new DoctorAvailability();
            //check if doctor selected from list
            if (lvDoctors.SelectedItem != null)
            {
                try
                {
                    docToSetSchedule = lvDoctors.SelectedItem as Doctor;
                    var selectedDates = clSheduleSet.SelectedDates;

                    if (selectedDates.Count != 1)
                    {
                        MessageBox.Show("Please select one date from the calendar in oreder to delete doctor's schedule.",
                            "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                    //check if schedule is set for selected date for the selected doctor
                    if (Globals.Db.CheckIfScheduleIsSet(docToSetSchedule.Id, clSheduleSet.SelectedDate.Value.ToString("yyyy-MM-dd")) > 0)

                    {
                        Globals.Db.DeleteScheduleForSpecificDoctorAndDate(docToSetSchedule.Id, clSheduleSet.SelectedDate.Value.ToString("yyyy-MM-dd"));
                        MessageBox.Show("Schedule deleted.",
                        "EasyAppointment Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        lbWorkingHours.Content = "not set";
                    }
                    else
                    {
                        MessageBox.Show("Schedule is not set for this date to be deleted.",
                        "EasyAppointment Database", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error executing SQL query:\n" + ex.Message,
                        "EasyAppointemtn Database", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select a doctor from the list",
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
        }
    }
}

        