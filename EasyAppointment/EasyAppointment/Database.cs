using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EasyAppointment
{

    partial class Database
    {
        const string DbConnectionString = @"Server=tcp:ipd16easyapp.database.windows.net,1433;Initial Catalog=EasyAppointmentDB;Persist Security Info=False;User ID=sqladmin;Password=123ABC123abc;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        private SqlConnection conn;

        public Database()
        {
            conn = new SqlConnection(DbConnectionString);
            conn.Open();
        }
        public EnumUserType GetLoginUserInfo(string uname, string upasswd)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT * FROM Users WHERE UserName = @UName", conn);
            cmdSelect.Parameters.AddWithValue("UName", uname);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                if (reader.Read())
                {
                    try
                    {
                        string pwd = reader["Password"].ToString();
                        if (pwd != upasswd)
                        {
                            MessageBox.Show("User Name or Password mismatch.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return EnumUserType.Unknown;
                        }
                        int doctorid = 0;
                        if (reader["DoctorId"].ToString() != "")
                            doctorid = (int)reader["DoctorId"];
                        string lastlg = reader["LastLoginDate"].ToString();

                        Globals.CurrentUser = new User() { UserName = uname, LastLoginDate = lastlg };
                        if (doctorid != 0)
                        {
                            Globals.CurrentUser.DoctorId = doctorid;
                            Globals.CurrentUser.UserType = EnumUserType.Doctor;
                            return EnumUserType.Doctor;
                        }
                        return EnumUserType.Admin;
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
                else
                {
                    MessageBox.Show("User Name or Password mismatch.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return EnumUserType.Unknown;
                }
            }
            return EnumUserType.Unknown;
        }

        public int AddDoctor(Doctor doctor)
        {

            SqlCommand cmdInsert = new SqlCommand("INSERT INTO Doctors (FirstName, LastName, Gender, DoctorType, Specialty, Office, Telephone, IsAvailable, Photo) OUTPUT INSERTED.ID VALUES (@FirstName, @LastName, @Gender, @DoctorType, @Specialty, @Office, @Telephone, @IsAvailable, @Photo)", conn);
            cmdInsert.Parameters.AddWithValue("FirstName", doctor.FirstName);
            cmdInsert.Parameters.AddWithValue("LastName", doctor.LastName);
            cmdInsert.Parameters.AddWithValue("Gender", doctor.Gender);
            cmdInsert.Parameters.AddWithValue("DoctorType", doctor.DoctorType);
            cmdInsert.Parameters.AddWithValue("Specialty", doctor.Specialty);
            cmdInsert.Parameters.AddWithValue("Office", doctor.Office);
            cmdInsert.Parameters.AddWithValue("Telephone", doctor.Telephone);
            cmdInsert.Parameters.AddWithValue("IsAvailable", doctor.isAvailable);
            cmdInsert.Parameters.AddWithValue("Photo", doctor.Photo);

            int insertId = (int)cmdInsert.ExecuteScalar();
            doctor.Id = insertId; // we may choose to do it
            return insertId;
            //cmdInsert.ExecuteNonQuery();
        }

        public void AddUser(string uname, string passw, int doctorId, DateTime registerDate)
        {
            SqlCommand cmdInsert = new SqlCommand("INSERT INTO Users (UserName, Password, DoctorId, LastLoginDate) OUTPUT INSERTED.ID VALUES (@UserName, @Password, @DoctorId, @LastLoginDate)", conn);
            cmdInsert.Parameters.AddWithValue("UserName", uname);
            cmdInsert.Parameters.AddWithValue("Password", passw);
            cmdInsert.Parameters.AddWithValue("DoctorId", doctorId);
            cmdInsert.Parameters.AddWithValue("LastLoginDate", registerDate);
            int insertId = (int)cmdInsert.ExecuteScalar();
        }
        public void AddPatient(Patient patient)
        {

            SqlCommand cmdInsert = new SqlCommand("INSERT INTO Patients (MedicalInsuranceNumber, FirstName, LastName, Gender, DOB, PatientAddress, City, PostalCode, Province, Phone, MedicalCondition, DateModified, Photo) OUTPUT INSERTED.ID VALUES (@MedicalInsuranceNumber, @FirstName, @LastName, @Gender, @DOB, @PatientAddress, @City, @PostalCode, @Province, @Phone, @MedicalCondition, @DateModified, @Photo)", conn);
            cmdInsert.Parameters.AddWithValue("MedicalInsuranceNumber", patient.MedInsurance);
            cmdInsert.Parameters.AddWithValue("FirstName", patient.FirstName);
            cmdInsert.Parameters.AddWithValue("LastName", patient.LastName);
            cmdInsert.Parameters.AddWithValue("Gender", patient.Gender);
            cmdInsert.Parameters.AddWithValue("DOB", patient.DateOfBirth);
            cmdInsert.Parameters.AddWithValue("PatientAddress", patient.PatientAddress);
            cmdInsert.Parameters.AddWithValue("City", patient.City);
            cmdInsert.Parameters.AddWithValue("PostalCode", patient.PostalCode);
            cmdInsert.Parameters.AddWithValue("Province", patient.Province);
            cmdInsert.Parameters.AddWithValue("Phone", patient.Telephone);
            cmdInsert.Parameters.AddWithValue("MedicalCondition", patient.MedCondition);
            cmdInsert.Parameters.AddWithValue("DateModified", patient.DateModified);
            cmdInsert.Parameters.AddWithValue("Photo", patient.Photo);
            cmdInsert.ExecuteNonQuery();
        }

        public void AddDoctorAvailability(DoctorAvailability workDay)
        {
            SqlCommand cmdInsert = new SqlCommand("INSERT INTO DoctorAvailability (DoctorId, AvailableDate, StartHour, EndHour) VALUES (@DoctorId, @AvailableDate, @StartHour, @EndHour)", conn);
            cmdInsert.Parameters.AddWithValue("DoctorId", workDay.DoctorId);
            cmdInsert.Parameters.AddWithValue("AvailableDate", workDay.AvailableDate);
            cmdInsert.Parameters.AddWithValue("StartHour", workDay.StartHour);
            cmdInsert.Parameters.AddWithValue("EndHour", workDay.EndHour);

            cmdInsert.ExecuteNonQuery();
        }

        public List<Appointment> GetAllAppointments()
        {
            List<Appointment> appList = new List<Appointment>();
            SqlCommand cmdSelect = new SqlCommand("SELECT A.ID as 'ID', D.FirstName + ' ' + D.LastName as 'Doctor', P.FirstName + ' ' + P.LastName as 'Patient', A.AppointmentDate as 'Date', A.AppointmentTime as 'Time', A.AppointmentReason as 'Reason' FROM Appointments as A INNER JOIN Patients as P ON A.PatientId = P.ID INNER JOIN Doctors as D ON A.DoctorId = D.ID", conn);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        int id = (int)reader[0];
                        string doctor = (string)reader[1];
                        string patient = (string)reader[2];
                        //DateTime appDate = (DateTime)reader[3];
                        string appDate = Convert.ToDateTime(reader[3]).ToString("yyyy-MM-dd");

                        int appTime = (int)reader["Time"];
                        string reason = (string)reader[5];

                        appList.Add(new Appointment() { Id = id, DoctorName = doctor, PatientName = patient, AppointmentDate = appDate, AppointmentTime = appTime, AppointmentReason = reason });
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return appList;


        }

        public List<Patient> GetAllPatientsList()
        {
            List<Patient> list = new List<Patient>();
            SqlCommand cmdSelect = new SqlCommand("SELECT ID, FirstName, LastName, Gender, DOB,MedicalInsuranceNumber, PatientAddress, City, PostalCode, Province, Phone, MedicalCondition  FROM Patients", conn);

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
                        DateTime dob = (DateTime)reader["DOB"];
                        string medinsurance = (string)reader["MedicalInsuranceNumber"];
                        string patientaddr = (string)reader["PatientAddress"];
                        string city = (string)reader["City"];
                        string postalcode = (string)reader["PostalCode"];
                        string province = (string)reader["Province"];
                        string phone = (string)reader["Phone"];
                        string medicalcondition = (string)reader["MedicalCondition"];

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

                            MedCondition = medicalcondition,

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



        public bool DeleteAppointment(int id)
        {
            SqlCommand cmdDelete = new SqlCommand("DELETE FROM Appointments WHERE Id=@Id", conn);
            cmdDelete.Parameters.AddWithValue("Id", id);
            int rowsAffected = cmdDelete.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool DeleteDoctor(int id)
        {
            SqlCommand cmdDeleteFromUsers = new SqlCommand("DELETE FROM Users WHERE DoctorId=@Id", conn);
            cmdDeleteFromUsers.Parameters.AddWithValue("Id", id);
            int rowsAffected1 = cmdDeleteFromUsers.ExecuteNonQuery();


            SqlCommand cmdDeleteFromAppointments = new SqlCommand("DELETE FROM Appointments WHERE DoctorId=@Id", conn);
            cmdDeleteFromAppointments.Parameters.AddWithValue("Id", id);
            int rowsAffected2 = cmdDeleteFromUsers.ExecuteNonQuery();

            SqlCommand cmdDeleteFromDocAv = new SqlCommand("DELETE FROM DoctorAvailability WHERE DoctorId=@Id", conn);
            cmdDeleteFromDocAv.Parameters.AddWithValue("Id", id);
            int rowsAffected3 = cmdDeleteFromUsers.ExecuteNonQuery();

            SqlCommand cmdDeleteFromDoctors = new SqlCommand("DELETE FROM Doctors WHERE Id=@Id", conn);
            cmdDeleteFromDoctors.Parameters.AddWithValue("Id", id);
            int rowsAffected4 = cmdDeleteFromDoctors.ExecuteNonQuery();

            return (rowsAffected1 > 0 && rowsAffected2 > 0 && rowsAffected3 > 0 && rowsAffected4 > 0);
        }

        public void DeletePatient(int id)
        {
            List<Appointment> itemsToDelete = new List<Appointment>();
            SqlCommand cmdSelect = new SqlCommand("SELECT a.ID, p.ID FROM Appointments as a LEFT OUTER JOIN Patients as p on a.PatientID = p.ID WHERE a.PatientID = @Id", conn);
            cmdSelect.Parameters.AddWithValue("Id", id);

            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {

                    int appId = (int)reader[0];
                    int docId = (int)reader[1];

                    itemsToDelete.Add(new Appointment()
                    {
                        Id = appId,
                        DoctorId = docId
                    });
                }

            }

            foreach (Appointment a in itemsToDelete)
            {
                SqlCommand cmdDeletePrescription = new SqlCommand("DELETE FROM Prescriptions WHERE AppointmentId=@Id", conn);
                cmdDeletePrescription.Parameters.AddWithValue("Id", a.Id);
                int affectedRows1 = cmdDeletePrescription.ExecuteNonQuery();
            }

            SqlCommand cmdDeleteFromAppointments = new SqlCommand("DELETE FROM Appointments WHERE PatientId=@Id", conn);
            cmdDeleteFromAppointments.Parameters.AddWithValue("Id", id);
            int affectedRows2 = cmdDeleteFromAppointments.ExecuteNonQuery();

            SqlCommand cmdDeleteFromPatients = new SqlCommand("DELETE FROM Patients WHERE Id=@Id", conn);
            cmdDeleteFromPatients.Parameters.AddWithValue("Id", id);
            int affectedRows3 = cmdDeleteFromPatients.ExecuteNonQuery();

        }


        public BitmapImage ShowDoctorPhoto(int id)
        {

            BitmapImage myBitmapImage = new BitmapImage();
            SqlCommand cmdSelect = new SqlCommand("SELECT Photo FROM Doctors WHERE [Id] = @Id", conn);
            cmdSelect.Parameters.AddWithValue("Id", id);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {

                while (reader.Read())
                {
                    try
                    {
                        byte[] buffer = (byte[])reader[0];

                        MemoryStream strmImg = new MemoryStream(buffer);

                        myBitmapImage.BeginInit();
                        myBitmapImage.StreamSource = strmImg;
                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();
                    }
                    catch (Exception ex)
                    {
                        if (ex is InvalidCastException) { }
                        else if (ex is System.NotSupportedException)
                        {
                            myBitmapImage = null;
                        }

                    }
                }
                return myBitmapImage;
            }
        }
        public BitmapImage ShowPatientPhoto(int id)
        {

            BitmapImage myBitmapImage = new BitmapImage();
            SqlCommand cmdSelect = new SqlCommand("SELECT Photo FROM Patients WHERE [Id] = @Id", conn);
            cmdSelect.Parameters.AddWithValue("Id", id);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {

                while (reader.Read())
                {
                    try
                    {
                        byte[] buffer = (byte[])reader[0];

                        MemoryStream strmImg = new MemoryStream(buffer);

                        myBitmapImage.BeginInit();
                        myBitmapImage.StreamSource = strmImg;
                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();
                    }
                    catch (Exception ex)
                    {
                        if (ex is InvalidCastException) { }
                        else if (ex is System.NotSupportedException)
                        {
                            myBitmapImage = null;
                        }

                    }

                }
                return myBitmapImage;
            }
        }
        public bool UpdateDoctor(Doctor doc)
        {
            SqlCommand cmdUpdate = new SqlCommand("UPDATE Doctors SET FirstName=@FirstName, LastName = @LastName, Gender = @Gender, DoctorType=@DoctorType, Specialty = @Specialty, Office = @Office, Telephone = @Telephone, IsAvailable = @IsAvailable, Photo = @Photo WHERE Id=@Id", conn);
            cmdUpdate.Parameters.AddWithValue("Id", doc.Id);
            cmdUpdate.Parameters.AddWithValue("FirstName", doc.FirstName);
            cmdUpdate.Parameters.AddWithValue("LastName", doc.LastName);
            cmdUpdate.Parameters.AddWithValue("Gender", doc.Gender);
            cmdUpdate.Parameters.AddWithValue("DoctorType", doc.DoctorType);
            cmdUpdate.Parameters.AddWithValue("Specialty", doc.Specialty);
            cmdUpdate.Parameters.AddWithValue("Office", doc.Office);
            cmdUpdate.Parameters.AddWithValue("Telephone", doc.Telephone);
            cmdUpdate.Parameters.AddWithValue("IsAvailable", doc.isAvailable);
            cmdUpdate.Parameters.AddWithValue("Photo", doc.Photo);
            int rowsAffected = cmdUpdate.ExecuteNonQuery();
            // Maybe I would prefer to throw SqlException in case row was not found?
            // Problem: if row exists but was updated with the same values then
            // affected rows is still 0, so we'd have to execute select to find record first.
            return rowsAffected > 0;
        }

        public bool UpdatePatient(Patient patient)
        {
            SqlCommand cmdUpdate = new SqlCommand("UPDATE Patients SET MedicalInsuranceNumber = @MedicalInsuranceNumber, FirstName=@FirstName, LastName = @LastName, Gender = @Gender, DOB = @DOB, PatientAddress = @PatientAddress, City = @City, PostalCode = @PostalCode, Province = @Province, Phone = @Phone, MedicalCondition = @MedicalCondition, DateModified = @DateModified, Photo = @Photo WHERE Id=@Id", conn);
            cmdUpdate.Parameters.AddWithValue("Id", patient.Id);
            cmdUpdate.Parameters.AddWithValue("MedicalInsuranceNumber", patient.MedInsurance);
            cmdUpdate.Parameters.AddWithValue("FirstName", patient.FirstName);
            cmdUpdate.Parameters.AddWithValue("LastName", patient.LastName);
            cmdUpdate.Parameters.AddWithValue("Gender", patient.Gender);
            cmdUpdate.Parameters.AddWithValue("DOB", patient.DateOfBirth);
            cmdUpdate.Parameters.AddWithValue("PatientAddress", patient.PatientAddress);
            cmdUpdate.Parameters.AddWithValue("City", patient.City);
            cmdUpdate.Parameters.AddWithValue("PostalCode", patient.PostalCode);
            cmdUpdate.Parameters.AddWithValue("Province", patient.Province);
            cmdUpdate.Parameters.AddWithValue("Phone", patient.Telephone);
            cmdUpdate.Parameters.AddWithValue("MedicalCondition", patient.MedCondition);
            cmdUpdate.Parameters.AddWithValue("DateModified", DateTime.Now);

            cmdUpdate.Parameters.AddWithValue("Photo", patient.Photo);
            int rowsAffected = cmdUpdate.ExecuteNonQuery();
            // Maybe I would prefer to throw SqlException in case row was not found?
            // Problem: if row exists but was updated with the same values then
            // affected rows is still 0, so we'd have to execute select to find record first.
            return rowsAffected > 0;
        }


        public Doctor GetLoggedInDoctorData(int id)
        {
            Doctor docLoggedIn = new Doctor();
            SqlCommand cmdSelect = new SqlCommand("SELECT FirstName, LastName, DoctorType, Specialty, Office, Telephone " +
                "FROM Doctors WHERE ID = @id", conn);
            cmdSelect.Parameters.AddWithValue("id", id);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {

                        string firstname = (string)reader["FirstName"];
                        string lastname = (string)reader["LastName"];
                        string docType = (string)reader["DoctorType"];

                        string specialty = (string)reader["Specialty"];
                        string office = (string)reader["Office"];
                        string telephone = (string)reader["Telephone"];

                        docLoggedIn.FirstName = firstname;
                        docLoggedIn.LastName = lastname;
                        docLoggedIn.DoctorType = docType;
                        docLoggedIn.Specialty = specialty;
                        docLoggedIn.Office = office;
                        docLoggedIn.Telephone = telephone;

                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            return docLoggedIn;
        }

        public List<Appointment> GetDoctorAppointments(int id, DateTime appDate)
        {
            List<Appointment> appList = new List<Appointment>();
            SqlCommand cmdSelect = new SqlCommand("SELECT P.FirstName + ' ' + P.LastName as 'Patient', A.AppointmentTime as 'Time', A.AppointmentReason as 'Reason' FROM Appointments as A INNER JOIN Patients as P ON A.PatientId = P.ID INNER JOIN Doctors as D ON A.DoctorId = D.ID WHERE DoctorId = @DoctorId AND AppointmentDate = @AppointmentDate ", conn);
            cmdSelect.Parameters.AddWithValue("DoctorId", id);
            cmdSelect.Parameters.AddWithValue("AppointmentDate", appDate);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {

                        string patient = (string)reader["Patient"];


                        int appTime = (int)reader["Time"];
                        string reason = (string)reader["Reason"];

                        appList.Add(new Appointment() { PatientName = patient, AppointmentTime = appTime, AppointmentReason = reason });
                    }
                    catch (InvalidCastException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return appList;


        }

        public DoctorAvailability GetDoctorScheduleByDoctorIdAndDate(int id, string date)
        {
            DoctorAvailability daySchedule = new DoctorAvailability();
            SqlCommand cmdSelect = new SqlCommand("SELECT StartHour, EndHour FROM DoctorAvailability WHERE DoctorId = @DocId AND convert(varchar(10), AvailableDate, 120) = @AppDate", conn);
            cmdSelect.Parameters.AddWithValue("DocId", id);
            cmdSelect.Parameters.AddWithValue("AppDate", date);
            using (SqlDataReader reader = cmdSelect.ExecuteReader())
            {
                while (reader.Read())
                {

                    int sHour = (int)reader["StartHour"];
                    int eHour = (int)reader["EndHour"];

                    daySchedule.StartHour = sHour;
                    daySchedule.EndHour = eHour;
                }
            }
            return daySchedule;
        }
        public int CheckIfScheduleIsSet(int id, string date)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT COUNT(1) FROM DoctorAvailability WHERE DoctorId = @DocId AND convert(varchar(10), AvailableDate, 120) = @AppDate", conn);
            cmdSelect.Parameters.AddWithValue("DocId", id);
            cmdSelect.Parameters.AddWithValue("AppDate", date);
            int count = (int)cmdSelect.ExecuteScalar();
            return count;
        }

        public bool DeleteScheduleForSpecificDoctorAndDate(int id, string date)
        {
            SqlCommand cmdDeleteSchedule = new SqlCommand("DELETE FROM DoctorAvailability WHERE DoctorId = @DocId AND convert(varchar(10), AvailableDate, 120) = @AppDate", conn);
            cmdDeleteSchedule.Parameters.AddWithValue("DocId", id);
            cmdDeleteSchedule.Parameters.AddWithValue("AppDate", date);
            int affectedRows = cmdDeleteSchedule.ExecuteNonQuery();
            return affectedRows > 0;
        }
    }
}
