using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAppointment
{
    public enum EnumUserType { Admin, Doctor, Unknown }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string LastLoginDate { get; set; }
        public int DoctorId { get; set; }
        private string fullname = "Admin";
        public EnumUserType UserType { get; set; }
        public User()
        {
            UserType = EnumUserType.Admin;
        }
        public virtual string GetFullName()
        {
            return fullname;
        }
    }
}
