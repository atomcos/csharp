using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAppointment
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MedInsurance { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PatientAddress { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Telephone { get; set; }
        public string MedCondition { get; set; }
        public DateTime DateModified { get; set; }
        public byte[] Photo { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", LastName, FirstName);
        }
    }
}
