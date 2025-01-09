using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace P01_SAD
{
    public partial class FormEditar : Form
    {
        private string nif;

        private TextBox txtNome;
        private TextBox txtCP;
        private TextBox txtEmail;
        private DataGridView dataGridViewTelefones;
        private Button btnSalvar;

        public FormEditar(string nif)
        {
            this.nif = nif;
            InitializeComponent();
            CarregarDadosCliente();
        }

        private void InitializeComponent()
        {
            this.txtNome = new TextBox();
            this.txtCP = new TextBox();
            this.txtEmail = new TextBox();
            this.dataGridViewTelefones = new DataGridView();
            this.btnSalvar = new Button();

            this.SuspendLayout();

            this.txtNome.Location = new System.Drawing.Point(12, 12);
            this.txtNome.Size = new System.Drawing.Size(200, 20);
            this.txtNome.PlaceholderText = "Nome";

            this.txtCP.Location = new System.Drawing.Point(12, 40);
            this.txtCP.Size = new System.Drawing.Size(200, 20);
            this.txtCP.PlaceholderText = "CP";

            this.txtEmail.Location = new System.Drawing.Point(12, 70);
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            this.txtEmail.PlaceholderText = "Email";

            this.dataGridViewTelefones.Location = new System.Drawing.Point(12, 100);
            this.dataGridViewTelefones.Size = new System.Drawing.Size(300, 150);
            this.dataGridViewTelefones.Columns.Add("Telefone", "Telefone");

            this.btnSalvar.Location = new System.Drawing.Point(12, 260);
            this.btnSalvar.Size = new System.Drawing.Size(100, 30);
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Click += new EventHandler(this.BtnSalvar_Click);

            this.ClientSize = new System.Drawing.Size(350, 300);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.txtCP);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.dataGridViewTelefones);
            this.Controls.Add(this.btnSalvar);
            this.Text = "Editar Cliente";

            this.ResumeLayout(false);
        }

        private void CarregarDadosCliente()
        {
            string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";
            string queryCliente = "SELECT Nome, cp FROM cliente WHERE NIF = @NIF";
            string queryEmail = "SELECT numero_c FROM contacto WHERE NIF = @NIF AND tipo_contacto_id = 2";
            string queryTelefones = "SELECT numero_c FROM contacto WHERE NIF = @NIF AND tipo_contacto_id = 1";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand cmdCliente = new SqlCommand(queryCliente, connection);
                    cmdCliente.Parameters.AddWithValue("@NIF", nif);
                    SqlDataReader readerCliente = cmdCliente.ExecuteReader();
                    if (readerCliente.Read())
                    {
                        txtNome.Text = readerCliente["Nome"].ToString();
                        txtCP.Text = readerCliente["cp"].ToString();
                    }
                    readerCliente.Close();

                    SqlCommand cmdEmail = new SqlCommand(queryEmail, connection);
                    cmdEmail.Parameters.AddWithValue("@NIF", nif);
                    txtEmail.Text = cmdEmail.ExecuteScalar()?.ToString() ?? "";

                    SqlCommand cmdTelefones = new SqlCommand(queryTelefones, connection);
                    cmdTelefones.Parameters.AddWithValue("@NIF", nif);
                    SqlDataReader readerTelefones = cmdTelefones.ExecuteReader();
                    while (readerTelefones.Read())
                    {
                        dataGridViewTelefones.Rows.Add(readerTelefones["numero_c"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados do cliente: " + ex.Message);
            }
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateCliente = "UPDATE cliente SET Nome = @Nome, cp = @CP WHERE NIF = @NIF";
                    SqlCommand cmdUpdateCliente = new SqlCommand(updateCliente, connection);
                    cmdUpdateCliente.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmdUpdateCliente.Parameters.AddWithValue("@CP", txtCP.Text);
                    cmdUpdateCliente.Parameters.AddWithValue("@NIF", nif);
                    cmdUpdateCliente.ExecuteNonQuery();

                    string updateEmail = @"
                        IF EXISTS (SELECT 1 FROM contacto WHERE NIF = @NIF AND tipo_contacto_id = 2)
                            UPDATE contacto SET numero_c = @Email WHERE NIF = @NIF AND tipo_contacto_id = 2
                        ELSE
                            INSERT INTO contacto (numero_c, tipo_contacto_id, NIF) VALUES (@Email, 2, @NIF);";
                    SqlCommand cmdUpdateEmail = new SqlCommand(updateEmail, connection);
                    cmdUpdateEmail.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmdUpdateEmail.Parameters.AddWithValue("@NIF", nif);
                    cmdUpdateEmail.ExecuteNonQuery();

                    string deleteTelefones = "DELETE FROM contacto WHERE NIF = @NIF AND tipo_contacto_id = 1";
                    SqlCommand cmdDeleteTelefones = new SqlCommand(deleteTelefones, connection);
                    cmdDeleteTelefones.Parameters.AddWithValue("@NIF", nif);
                    cmdDeleteTelefones.ExecuteNonQuery();

                    foreach (DataGridViewRow row in dataGridViewTelefones.Rows)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            string insertTelefone = "INSERT INTO contacto (numero_c, tipo_contacto_id, NIF) VALUES (@Telefone, 1, @NIF)";
                            SqlCommand cmdInsertTelefone = new SqlCommand(insertTelefone, connection);
                            cmdInsertTelefone.Parameters.AddWithValue("@Telefone", row.Cells[0].Value.ToString());
                            cmdInsertTelefone.Parameters.AddWithValue("@NIF", nif);
                            cmdInsertTelefone.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Dados do cliente atualizados com sucesso!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar dados do cliente: " + ex.Message);
            }
        }
    }
}