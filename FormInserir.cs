using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace P01_SAD
{
    public partial class FormInserir : Form
    {
        private DataGridView dataGridViewContactos;
        private TextBox txtNIF;
        private TextBox txtNome;
        private TextBox txtEmail;
        private TextBox txtCP;
        private Button btnInserir;

        public FormInserir()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.dataGridViewContactos = new DataGridView();
            this.txtNIF = new TextBox();
            this.txtNome = new TextBox();
            this.txtEmail = new TextBox();
            this.txtCP = new TextBox();
            this.btnInserir = new Button();

            this.SuspendLayout();

            this.dataGridViewContactos.Location = new System.Drawing.Point(12, 150);
            this.dataGridViewContactos.Size = new System.Drawing.Size(400, 200);
            this.dataGridViewContactos.Columns.Add("Telefone", "Telefone");

            this.txtNIF.Location = new System.Drawing.Point(12, 12);
            this.txtNIF.Size = new System.Drawing.Size(200, 20);
            this.txtNIF.PlaceholderText = "NIF";

            this.txtNome.Location = new System.Drawing.Point(12, 40);
            this.txtNome.Size = new System.Drawing.Size(200, 20);
            this.txtNome.PlaceholderText = "Nome";

            this.txtEmail.Location = new System.Drawing.Point(12, 70);
            this.txtEmail.Size = new System.Drawing.Size(200, 20);
            this.txtEmail.PlaceholderText = "Email";

            this.txtCP.Location = new System.Drawing.Point(12, 100);
            this.txtCP.Size = new System.Drawing.Size(200, 20);
            this.txtCP.PlaceholderText = "CP (Código Postal Sem Hífen)";

            this.btnInserir.Location = new System.Drawing.Point(12, 360);
            this.btnInserir.Size = new System.Drawing.Size(100, 30);
            this.btnInserir.Text = "Inserir";
            this.btnInserir.Click += new EventHandler(this.BtnInserir_Click);

            this.ClientSize = new System.Drawing.Size(450, 420);
            this.Controls.Add(this.dataGridViewContactos);
            this.Controls.Add(this.txtNIF);
            this.Controls.Add(this.txtNome);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtCP);
            this.Controls.Add(this.btnInserir);
            this.Text = "Inserir Cliente e Contactos";

            this.ResumeLayout(false);
        }

        private void BtnInserir_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=LEGION;Database=P01-SAD;Trusted_Connection=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string queryCliente = "INSERT INTO cliente (NIF, Nome, cp) VALUES (@NIF, @Nome, @CP);";
                    SqlCommand cmdCliente = new SqlCommand(queryCliente, connection);
                    cmdCliente.Parameters.AddWithValue("@NIF", txtNIF.Text);
                    cmdCliente.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmdCliente.Parameters.AddWithValue("@CP", txtCP.Text);
                    cmdCliente.ExecuteNonQuery();

                    string queryEmail = "INSERT INTO contacto (numero_c, tipo_contacto_id, NIF) VALUES (@Email, 2, @NIF);";
                    SqlCommand cmdEmail = new SqlCommand(queryEmail, connection);
                    cmdEmail.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmdEmail.Parameters.AddWithValue("@NIF", txtNIF.Text);
                    cmdEmail.ExecuteNonQuery();

                    foreach (DataGridViewRow row in dataGridViewContactos.Rows)
                    {
                        if (row.Cells[0].Value != null)
                        {
                            string queryContacto = "INSERT INTO contacto (numero_c, tipo_contacto_id, NIF) VALUES (@Telefone, 1, @NIF);";
                            SqlCommand cmdContacto = new SqlCommand(queryContacto, connection);
                            cmdContacto.Parameters.AddWithValue("@Telefone", row.Cells[0].Value.ToString());
                            cmdContacto.Parameters.AddWithValue("@NIF", txtNIF.Text);
                            cmdContacto.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Cliente e contactos inseridos com sucesso!");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}