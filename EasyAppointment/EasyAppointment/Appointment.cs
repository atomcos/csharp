using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAppointment
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string AppointmentDate { get; set; }
        public int AppointmentTime { get; set; }
        public string AppointmentReason { get; set; }

        public override string ToString()
        {
            return string.Format("Appointment #{0} {1} at {2}:00", Id, AppointmentDate, AppointmentTime);
        }

    }
    public class AvailableTime
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public override string ToString()
        {
            return string.Format("{0}");
        }
    }
}
