using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;

namespace EasyAppointment
{
    public class Doctor// : User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DoctorType { get; set; }
        public string Specialty { get; set; }
        public string Office { get; set; }
        public string Telephone { get; set; }
        public bool isAvailable { get; set; }
        public byte[] Photo { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", LastName, FirstName, Specialty);
        }

        //public override string GetFullName()
       // {
          //  return string.Format("{0} {1}", LastName, FirstName);
       // }
    }
}
