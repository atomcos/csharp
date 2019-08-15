using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyAppointment
{
    partial class Database
    {
        public List<Doctor> GetAllAvailableDoctors(int doctorId = -1)
        {
            List<Doctor> list = new List<Doctor>();
            SqlCommand cmdSelect;
            if (doctorId  == -1)
                cmdSelect = new SqlCommand("SELECT * FROM Doctors WHERE IsAvailable = 1 ORDER BY Id", conn);
            else
            {
                cmdSelect = new SqlCommand("SELECT * FROM Doctors WHERE Id = @DoctorId ORDER BY Id", conn);
                cmdSelect.Parameters.AddWithValue("DoctorId", doctorId);
            }
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        int id = (int)reader["Id"];
                        string firstname = (string)reader["FirstName"];
                        string lastname = (string)reader["LastName"];
                        string gender = (string)reader["Gender"];
                        string doctortype = (string)reader["DoctorType"];
                        string specialty = (string)reader["Specialty"];
                        string office = (string)reader["Office"];
                        string tel = (string)reader["Telephone"];
                        //int isavailable = (((bool)reader["isAvailable"]) == true) ? 1 : 0;
                        int isavailable = (byte)reader["IsAvailable"];
                        byte[] photo = (reader["Photo"] as byte[]) ?? null;
                        list.Add(new Doctor()
                        {
                            Id = id,
                            FirstName = firstname,
                            LastName = lastname,
                            Gender = gender,
                            DoctorType = doctortype,
                            Specialty = specialty,
                            Office = office,
                            Telephone = tel,
                            isAvailable = (isavailable == 1),
                            Photo = photo
                        });
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            return list;
        }
        public List<Patient> GetAllPatients(string pname = "")
        {
            List<Patient> list = new List<Patient>();
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Patients WHERE LOWER(FirstName) LIKE @PName OR LOWER(LastName) LIKE @PName OR LOWER(MedicalInsuranceNumber) LIKE @PName ORDER BY Id", conn);
            cmdSelect.Parameters.AddWithValue("PName", pname.ToLower());
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        int id = (int)reader["Id"];
                        string firstname = (string)reader["FirstName"];
                        string medinsurance = (string)reader["MedicalInsuranceNumber"];
                        string lastname = (string)reader["LastName"];
                        string gender = (string)reader["Gender"];
                        string patientaddr = (string)reader["PatientAddress"];
                        DateTime dob = (DateTime)reader["DOB"];
                        string city = (string)reader["City"];
                        string postalcode = (string)reader["PostalCode"];
                        string province = (string)reader["Province"];
                        string phone = (string)reader["Phone"];
                        string medicalcondition = (string)reader["MedicalCondition"];
                        DateTime dateofmodify = (DateTime)reader["DateModified"];
                        byte[] photo = (byte[])reader["Photo"];
                        list.Add(new Patient()
                        {
                            Id = id,
                            FirstName = firstname,
                            LastName = lastname,
                            Gender = gender,
                            MedInsurance = medinsurance,
                            PatientAddress = patientaddr,
                            DateOfBirth = dob,
                            City = city,
                            PostalCode = postalcode,
                            Province = province,
                            Photo = photo,
                            MedCondition = medicalcondition,
                            DateModified = dateofmodify,
                            Telephone = phone
                        });
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            return list;
        }
        public List<Appointment> GetAllAppointsByDoctorDate(int docId, string date)//date format: yyyy-mm-dd
        {
            List<Appointment> list = new List<Appointment>();
            //SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Appointments WHERE DoctorId = @DocId AND convert(varchar(10), AppointmentDate, 120) = @AppDate ORDER BY Id ", conn);
            string queryStr = @"SELECT a.Id AS Id, a.DoctorId AS DoctorId, a.PatientId AS PatientId, a.AppointmentTime AS AppointmentTime, 
                                a.AppointmentDate AS AppointmentDate, a.AppointmentReason AS AppointmentReason,
                                d.FirstName AS DoctFirstName, d.LastName AS DoctLastName, p.FirstName AS PatFirstName, p.LastName AS PatLastName
                                FROM Appointments AS a
                                INNER JOIN Doctors AS d
                                    ON a.DoctorId = d.Id
                                INNER JOIN Patients AS p
                                    ON a.PatientId = p.Id
                                WHERE DoctorId = @DocId AND convert(varchar(10), AppointmentDate, 120) = @AppDate ORDER BY Id ";
            SqlCommand cmdSelect = new SqlCommand(queryStr, conn);
            cmdSelect.Parameters.AddWithValue("DocId", docId);
            cmdSelect.Parameters.AddWithValue("AppDate", date);

            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        int id = (int)reader["Id"];
                        int docid = (int)reader["DoctorId"];
                        int patientid = (int)reader["PatientId"];
                        int appdatetime = (int)reader["AppointmentTime"];
                        string appdatedate = Convert.ToDateTime(reader["AppointmentDate"]).ToString("yyyy-MM-dd");
                        string appreason = (string)reader["AppointmentReason"];
                        string doctname = string.Format("{0} {1}", (string)reader["DoctFirstName"], (string)reader["DoctLastName"]);
                        string patname = string.Format("{0} {1}", (string)reader["PatFirstName"], (string)reader["PatLastName"]);
                        list.Add(new Appointment() { Id = id, DoctorId = docid, PatientId = patientid, DoctorName = doctname, PatientName = patname, AppointmentDate = appdatedate, AppointmentTime = appdatetime, AppointmentReason = appreason });
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
            }
            }
            return list;
        }

        public AvailableTime GetDoctorAvailability(int docId, string date)//date format: yyyymmdd
        {
            //List<AvailableTime> list = new List<AvailableTime>();
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM DoctorAvailability WHERE DoctorId = @DocId AND convert(varchar(10), AvailableDate, 120) = @AppDate ORDER BY Id ", conn);
            cmdSelect.Parameters.AddWithValue("DocId", docId);
            cmdSelect.Parameters.AddWithValue("AppDate", date);
            AvailableTime at = null;

            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                if (reader.Read())
                {
                    try
                    {
                        //int id = (int)reader["Id"];
                        //int docid = (int)reader["DoctorId"];
                        int starthour = (int)reader["StartHour"];
                        int endhour = (int)reader["EndHour"];
                        //string availabledate = (string)reader["AvailableDate"];

                        //list.Add(new AvailableTime() { StartTime = starthour, EndTime = endhour  });
                        at = new AvailableTime() { StartTime = starthour, EndTime = endhour };
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return at;
        }
        public void AddAppointment(int docorid, int patientid, string appreason, string appdate, int apptime)
        {
            SqlCommand cmdInsert = new SqlCommand("INSERT INTO Appointments (DoctorId, PatientId, AppointmentReason, AppointmentDate, AppointmentTime) OUTPUT INSERTED.ID VALUES (@DocId, @PatId, @AppReason, @AppDate, @AppTime)", conn);
            cmdInsert.Parameters.AddWithValue("DocId", docorid);
            cmdInsert.Parameters.AddWithValue("PatId", patientid);
            cmdInsert.Parameters.AddWithValue("AppReason", appreason);
            cmdInsert.Parameters.AddWithValue("AppDate", appdate);
            cmdInsert.Parameters.AddWithValue("AppTime", apptime);
            int insertId = (int)cmdInsert.ExecuteScalar();
        }
        public List<Prescription> GetAllPrescription(int patientId)
        {
            string querystr = @"SELECT p.Id AS Id, 
                                p.PrescriptionDetails as PrescriptionDetails, 
                                p.PrescriptionDate AS PrescriptionDate,
                                p.AppointmentId AS AppointmentId
                                FROM Prescriptions AS p
                                INNER JOIN Appointments AS a
                                    ON a.Id = p.AppointmentId
                                    WHERE a.PatientId = @PId ORDER BY Id ";
            SqlCommand cmdSelect = new SqlCommand(querystr, conn);
            cmdSelect.Parameters.AddWithValue("PId", patientId);
            
            List<Prescription> plist = new List<Prescription>();

            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        int presid = (int)reader["Id"];
                        DateTime presdate = (DateTime)reader["PrescriptionDate"];
                        //int patientid = (int)reader["PatientId"];
                        string presdetails = (string)reader["PrescriptionDetails"];
                        int appid = (int)reader["AppointmentId"];
                        //string availabledate = (string)reader["AvailableDate"];

                        //list.Add(new AvailableTime() { StartTime = starthour, EndTime = endhour  });
                        plist.Add( new Prescription() { Id = presid, PrescriptionDate = presdate, /*PatientId = patientid,*/ PrescriptionDetails = presdetails, AppointmentId = appid });
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return plist;
        }
        public Patient GetPatient(int patientId)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Patients WHERE Id = @PId", conn);
            cmdSelect.Parameters.AddWithValue("PId", patientId);

            Patient p = new Patient();

            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                if (reader.Read())
                {
                    try
                    {

                        p.Id  = (int)reader["Id"];
                        p.FirstName = (string)reader["FirstName"];
                        p.LastName = (string)reader["LastName"];
                        p.Gender = (string)reader["Gender"];
                        p.DateOfBirth = (DateTime)reader["DOB"];
                        p.MedInsurance = (string)reader["MedicalInsuranceNumber"];
                        p.PatientAddress = (string)reader["PatientAddress"];
                        p.City = (string)reader["City"];
                        p.PostalCode = (string)reader["PostalCode"];
                        p.Province = (string)reader["Province"];
                        p.Telephone = (string)reader["Phone"];
                        p.MedCondition = (string)reader["MedicalCondition"];

                        
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return p;
        }
        public int AddPrescription(Prescription p)
        {
            SqlCommand cmdInsert = new SqlCommand("INSERT INTO Prescriptions (PrescriptionDate, PrescriptionDetails, AppointmentId) OUTPUT INSERTED.ID VALUES (@PDate, @PDetail, @AppId)", conn);
            cmdInsert.Parameters.AddWithValue("PDate", p.PrescriptionDate);
            //cmdInsert.Parameters.AddWithValue("PatId", p.PatientId);
            cmdInsert.Parameters.AddWithValue("PDetail", p.PrescriptionDetails);
            cmdInsert.Parameters.AddWithValue("AppId", p.AppointmentId);
           
            return (int)cmdInsert.ExecuteScalar();
        }
    }
}
