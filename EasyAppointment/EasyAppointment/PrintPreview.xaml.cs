using System;
using System.Collections.Generic;
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
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        public PrintPreview(Window owner, Appointment app, Patient p, Prescription pp)
        {
           
            InitializeComponent();
            Owner = owner;
            lblFirstName.Content = p.FirstName;
            lblLastName.Content = p.LastName;
            lblDoB.Content = p.DateOfBirth;
            lblAddr.Content = p.PatientAddress;
            lblPrescription.Content = pp.PrescriptionDetails;
            List<Doctor> dl = Globals.Db.GetAllAvailableDoctors(app.DoctorId);
            if (dl.Count > 0)
            {
                Doctor d = dl[0] as Doctor;
                lblDoctorFullName.Content = string.Format("{0} {1}", d.FirstName, d.LastName);
            }
            lblPrescriptionDate.Content = pp.PrescriptionDate;
        }

        private void MnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dlg = new PrintDialog();

            Window currentMainWindow = Application.Current.MainWindow;

            Application.Current.MainWindow = this;

            if ((bool)dlg.ShowDialog().GetValueOrDefault())
            {
                Application.Current.MainWindow = currentMainWindow; // do it early enough if the 'if' is entered
                dlg.PrintVisual(this, "Certificate");
            }
        }
    }
}
