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
            AtualizarDados();
        }

        private void AtualizarDados()
        {
            string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";
            string query = "SELECT * FROM cliente";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message);
            }
        }

        private void btnAbrirInserir_Click(object sender, EventArgs e)
        {
            FormInserir formInserir = new FormInserir();
            formInserir.ShowDialog();
            AtualizarDados();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string nif = dataGridView1.SelectedRows[0].Cells["NIF"].Value.ToString();
                string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";
                string query = "DELETE FROM cliente WHERE NIF = @NIF";

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.AddWithValue("@NIF", nif);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cliente eliminado com sucesso!");
                        AtualizarDados();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao eliminar cliente: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecione um cliente para eliminar.");
            }
        }
    }
}