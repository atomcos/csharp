using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAppointment
{
    public class DoctorAvailability
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string AvailableDate { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public override string ToString()
        {
            return string.Format("{0}:00 - {1}:00", StartHour, EndHour);
        }
    }
}
