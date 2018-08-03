using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO.Compression;
using System.Net.Mail;

namespace DailyRefreshCacheLinks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("Close Me");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/International/UserDetailsServices.svc/CreateCacheFiles/US");
                WebResponse response = request.GetResponse();
                response.Close();

                HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/Europe/UserDetailsServices.svc/CreateCacheFiles/EUR");
                WebResponse response1 = request1.GetResponse();
                response1.Close();

                HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/aircraft/UserDetailsServices.svc/CreateCacheFiles/AIR");
                WebResponse response2 = request2.GetResponse();
                response2.Close();


                HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create(@"http://80.80.229.30/AutoUpload/UserDetailsServices.svc/CacheFileAirportAirlineList/US");
                WebResponse response3 = request3.GetResponse();
                response3.Close();
            

                //HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(@"http://www.google.com");
                //WebResponse response2 = request2.GetResponse();
                //response2.Close();


            }
            catch
            {

            }
            finally
            {
                Application.Exit();
            }
          
        }
    }
}
