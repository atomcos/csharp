using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAppointment
{
    public class Prescription
    {
        public int Id { get; set; }
        public DateTime PrescriptionDate { get; set; }
     //   public int PatientId { get; set; }
        public string PrescriptionDetails { get; set; }
        public int AppointmentId { get; set; }
    }
}
