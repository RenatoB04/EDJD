using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace P01_SAD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                MessageBox.Show("Connection successful!");
            }
        }
    }
}
