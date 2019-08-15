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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasyAppointment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Globals.Db = new Database();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Fatal error: unable to connect to database\n" + ex.Message, "Easy Appointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                Close();
            }           
        }

        private void BtLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbLoginName.Text == "")
            {
                MessageBox.Show("Login Name must not be empty.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            if (tbLoginPwd.Password == "")
            {
                MessageBox.Show("Login password must not be empty.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            try
            {
                EnumUserType eu = Globals.Db.GetLoginUserInfo(tbLoginName.Text, tbLoginPwd.Password);
                if (eu == EnumUserType.Admin)
                {
                    (new AdminMainWindow()).Show();


                    Close();
                }
                    
                else if (eu == EnumUserType.Doctor)
                {

                    (new DoctorMainWindow()).Show();
                    Close();
                }
                
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error in fetch login user info.", "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
